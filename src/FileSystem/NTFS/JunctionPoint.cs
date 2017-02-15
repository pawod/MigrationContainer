using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Pawod.MigrationContainer.Filesystem.NTFS
{
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

    /*
     * Credits to jeff.brown from CodeProject: http://www.codeproject.com/Articles/15633/Manipulating-NTFS-Junction-Points-in-NET
     */

    /// <summary>
    ///     Provides access to NTFS junction points in .Net.
    /// </summary>
    public class JunctionPoint : Win32Api
    {
        /// <summary>
        ///     The data present in the reparse point buffer is invalid.
        /// </summary>
        private const int ERROR_INVALID_REPARSE_DATA = 4392;

        /// <summary>
        ///     The file or directory is not a reparse point.
        /// </summary>
        private const int ERROR_NOT_A_REPARSE_POINT = 4390;

        /// <summary>
        ///     The reparse point attribute cannot be set because it conflicts with an
        ///     existing attribute.
        /// </summary>
        private const int ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;

        /// <summary>
        ///     The tag present in the reparse point buffer is invalid.
        /// </summary>
        private const int ERROR_REPARSE_TAG_INVALID = 4393;

        /// <summary>
        ///     There is a mismatch between the tag specified in the request and the tag
        ///     present in the reparse point.
        /// </summary>
        private const int ERROR_REPARSE_TAG_MISMATCH = 4394;

        /// <summary>
        ///     Command to delete the reparse point data base.
        /// </summary>
        private const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

        /// <summary>
        ///     Command to get the reparse point data block.
        /// </summary>
        private const int FSCTL_GET_REPARSE_POINT = 0x000900A8;

        /// <summary>
        ///     Command to set the reparse point data block.
        /// </summary>
        private const int FSCTL_SET_REPARSE_POINT = 0x000900A4;

        /// <summary>
        ///     Reparse point tag used to identify mount points and junction points.
        /// </summary>
        private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

        /// <summary>
        ///     This prefix indicates to NTFS that the path is to be treated as a
        ///     non-interpreted
        ///     path in the virtual file system.
        /// </summary>
        private const string NON_INTERPRETED_PATH_PREFIX = @"\??\";

        public DirectoryInfo Link { get; private set; }
        public DirectoryInfo Target { get; private set; }

        /// <summary>
        ///     Initializes a JunctionPoint instance from an already existing junction.
        /// </summary>
        /// <param name="linkedDir">Path to the directory, that represents the junction</param>
        public JunctionPoint(DirectoryInfo linkedDir)
        {
            Link = linkedDir;
            Target = new DirectoryInfo(Path.GetLongPath(Path.GetFullPath(GetTarget(linkedDir.FullName))));
        }

        /// <summary>
        ///     Initializes a JunctionPoint instance, which is ready to be used for creating
        ///     a junction on the filesystem.
        /// </summary>
        /// <param name="linkedDir">Path to the directory, that represents the junction</param>
        /// <param name="target">Path to the directory, that this junction is refering to .</param>
        public JunctionPoint(string linkedDir, string target)
        {
            Link = new DirectoryInfo(Path.GetLongPath(linkedDir));
            Target = new DirectoryInfo(Path.GetFullPath(target));
        }

        public void Create(bool overwrite = true)
        {
            CreateGetFileHandle(overwrite).Dispose();
        }

        public SafeFileHandle CreateGetFileHandle(bool overwrite = true)
        {
            Link.Refresh();
            if (Link.Exists)
            {
                if (!overwrite) throw new IOException("Directory already exists and overwrite parameter is false.");
            }
            else Link.Create();

            SafeFileHandle handle = null;
            var inBuffer = new IntPtr();

            try
            {
                handle = OpenReparsePoint(Path.GetLongPath(Link.FullName), EFileAccess.FILE_WRITE_ATTRIBUTES);
                var targetDirBytes = Encoding.Unicode.GetBytes(NON_INTERPRETED_PATH_PREFIX + Path.GetLongPath(Target.FullName));
                var reparseDataBuffer = new ReparseDataBuffer();

                reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
                reparseDataBuffer.ReparseDataLength = (ushort) (targetDirBytes.Length + 12);
                reparseDataBuffer.SubstituteNameOffset = 0;
                reparseDataBuffer.SubstituteNameLength = (ushort) targetDirBytes.Length;
                reparseDataBuffer.PrintNameOffset = (ushort) (targetDirBytes.Length + 2);
                reparseDataBuffer.PrintNameLength = 0;
                reparseDataBuffer.PathBuffer = new byte[0x3ff0];
                Array.Copy(targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length);

                var inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                inBuffer = Marshal.AllocHGlobal(inBufferSize);


                Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                int bytesReturned;
                var result = DeviceIoControl(handle.DangerousGetHandle(),
                                             FSCTL_SET_REPARSE_POINT,
                                             inBuffer,
                                             targetDirBytes.Length + 20,
                                             IntPtr.Zero,
                                             0,
                                             out bytesReturned,
                                             IntPtr.Zero);

                if (!result)
                {
                    ThrowLastWin32Error("Unable to create junction point.");
                    handle.Dispose();
                }
            }
            catch (System.Exception ex)
            {
                if (handle != null) handle.Dispose();
                throw ex;
            }
            finally {
                Marshal.FreeHGlobal(inBuffer);
            }

            return handle;
        }

        public void Delete()
        {
            Link.Refresh();
            if (!Link.Exists)
            {
                if (File.Exists(Link.FullName)) throw new IOException("Path is not a junction point.");
                return;
            }

            using (var handle = OpenReparsePoint(Link.FullName, EFileAccess.GENERIC_WRITE))
            {
                var reparseDataBuffer = new ReparseDataBuffer
                                        {
                                            ReparseTag = IO_REPARSE_TAG_MOUNT_POINT,
                                            ReparseDataLength = 0,
                                            PathBuffer = new byte[0x3ff0]
                                        };


                var inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                var inBuffer = Marshal.AllocHGlobal(inBufferSize);
                try
                {
                    Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                    int bytesReturned;
                    var result = DeviceIoControl(handle.DangerousGetHandle(),
                                                 FSCTL_DELETE_REPARSE_POINT,
                                                 inBuffer,
                                                 8,
                                                 IntPtr.Zero,
                                                 0,
                                                 out bytesReturned,
                                                 IntPtr.Zero);

                    if (!result) ThrowLastWin32Error("Unable to delete junction point.");
                }
                finally {
                    Marshal.FreeHGlobal(inBuffer);
                }

                try {
                    Directory.Delete(Path.GetLongPath(Link.FullName));
                }
                catch (IOException ex) {
                    throw new IOException("Unable to delete junction point.", ex);
                }
            }
        }

        public bool Equals(JunctionPoint junction)
        {
            return string.Equals(Link.FullName, junction.Link.FullName, StringComparison.Ordinal) &&
                   string.Equals(Target.FullName, junction.Target.FullName, StringComparison.Ordinal);
        }

        public bool Exists()
        {
            return Exists(Link);
        }

        /// <summary>
        ///     Determines whether the specified DirectoryInfo represents a junction point on
        ///     the filesystem.
        /// </summary>
        /// <param name="directoryInfo">The DirectoryInfo to investigate.</param>
        /// <returns>True if the specified DirectoryInfo represents a junction point</returns>
        public static bool Exists(DirectoryInfo directoryInfo)
        {
            directoryInfo.Refresh();
            if (!directoryInfo.Exists) return false;

            using (var handle = OpenReparsePoint(Path.GetLongPath(directoryInfo.FullName), EFileAccess.GENERIC_READ))
            {
                var target = InternalGetTarget(handle);
                return target != null;
            }
        }

        /// <summary>
        ///     Gets all junctions inside the given directory.
        /// </summary>
        /// <param name="directory">The directory to investigate.</param>
        /// <param name="searchOption">The searchoption to be used.</param>
        /// <returns>A list of all found JunctionPoints.</returns>
        public static IList<JunctionPoint> FindJunctions(DirectoryInfo directory, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var junctions = new List<JunctionPoint>();
            if (Exists(directory)) junctions.Add(new JunctionPoint(directory));

            foreach (var subDir in
                Directory.GetDirectories(directory.FullName, "*", SearchOption.TopDirectoryOnly).Select(s => new DirectoryInfo(s)))
            {
                if (Exists(subDir)) junctions.Add(new JunctionPoint(subDir));
                else junctions.AddRange(FindJunctions(subDir));
            }
            return junctions;
        }

        public static string GetTarget(string junctionPoint)
        {
            using (var handle = OpenReparsePoint(junctionPoint, EFileAccess.GENERIC_READ))
            {
                var target = InternalGetTarget(handle);
                if (target == null) throw new IOException("Path is not a junction point.");

                return target;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(string lpFileName,
                                                EFileAccess dwDesiredAccess,
                                                EFileShare dwShareMode,
                                                IntPtr lpSecurityAttributes,
                                                ECreationDisposition dwCreationDisposition,
                                                EFileAttributes dwFlagsAndAttributes,
                                                IntPtr hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeviceIoControl(IntPtr hDevice,
                                                   uint dwIoControlCode,
                                                   IntPtr InBuffer,
                                                   int nInBufferSize,
                                                   IntPtr OutBuffer,
                                                   int nOutBufferSize,
                                                   out int pBytesReturned,
                                                   IntPtr lpOverlapped);

        private static string InternalGetTarget(SafeFileHandle handle)
        {
            var outBufferSize = Marshal.SizeOf(typeof(ReparseDataBuffer));
            var outBuffer = Marshal.AllocHGlobal(outBufferSize);

            try
            {
                int bytesReturned;
                var result = DeviceIoControl(handle.DangerousGetHandle(),
                                             FSCTL_GET_REPARSE_POINT,
                                             IntPtr.Zero,
                                             0,
                                             outBuffer,
                                             outBufferSize,
                                             out bytesReturned,
                                             IntPtr.Zero);

                if (!result)
                {
                    var error = Marshal.GetLastWin32Error();
                    if (error == ERROR_NOT_A_REPARSE_POINT) return null;

                    ThrowLastWin32Error("Unable to get information about junction point.");
                }

                var reparseDataBuffer = (ReparseDataBuffer) Marshal.PtrToStructure(outBuffer, typeof(ReparseDataBuffer));

                if (reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT) return null;

                var targetDir = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
                                                           reparseDataBuffer.SubstituteNameOffset,
                                                           reparseDataBuffer.SubstituteNameLength);

                if (targetDir.StartsWith(NON_INTERPRETED_PATH_PREFIX)) targetDir = targetDir.Substring(NON_INTERPRETED_PATH_PREFIX.Length);

                return targetDir;
            }
            finally {
                Marshal.FreeHGlobal(outBuffer);
            }
        }

        private static SafeFileHandle OpenReparsePoint(string reparsePoint, EFileAccess accessMode)
        {
            var reparsePointHandle =
                new SafeFileHandle(
                    CreateFile(reparsePoint,
                               accessMode,
                               EFileShare.READ | EFileShare.WRITE | EFileShare.DELETE,
                               IntPtr.Zero,
                               ECreationDisposition.OPEN_EXISTING,
                               EFileAttributes.BACKUP_SEMANTICS | EFileAttributes.OPEN_REPARSE_POINT,
                               IntPtr.Zero),
                    true);

            if (Marshal.GetLastWin32Error() != 0) ThrowLastWin32Error("Unable to open reparse point.");
            return reparsePointHandle;
        }

        private static void ThrowLastWin32Error(string message)
        {
            throw new IOException(message, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ReparseDataBuffer
        {
            /// <summary>
            ///     Reparse point tag. Must be a Microsoft reparse point tag.
            /// </summary>
            public uint ReparseTag;

            /// <summary>
            ///     Size, in bytes, of the data after the Reserved member. This can be calculated
            ///     by:
            ///     (4 * sizeof(ushort)) + SubstituteNameLength + PrintNameLength +
            ///     (namesAreNullTerminated ? 2 * sizeof(char) : 0);
            /// </summary>
            public ushort ReparseDataLength;

            /// <summary>
            ///     Reserved; do not use.
            /// </summary>
            public readonly ushort Reserved;

            /// <summary>
            ///     Offset, in bytes, of the substitute name string in the PathBuffer array.
            /// </summary>
            public ushort SubstituteNameOffset;

            /// <summary>
            ///     Length, in bytes, of the substitute name string. If this string is
            ///     null-terminated,
            ///     SubstituteNameLength does not include space for the null character.
            /// </summary>
            public ushort SubstituteNameLength;

            /// <summary>
            ///     Offset, in bytes, of the print name string in the PathBuffer array.
            /// </summary>
            public ushort PrintNameOffset;

            /// <summary>
            ///     Length, in bytes, of the print name string. If this string is
            ///     null-terminated,
            ///     PrintNameLength does not include space for the null character.
            /// </summary>
            public ushort PrintNameLength;

            /// <summary>
            ///     A buffer containing the unicode-encoded path string. The path string contains
            ///     the substitute name string and print name string.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
            public byte[] PathBuffer;
        }
    }
}