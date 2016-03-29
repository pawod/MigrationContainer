namespace DataMigrator.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using Helper;
    using Microsoft.Win32.SafeHandles;

	public class Win32File : Win32Api, IDisposable

    {
        public readonly string Path;
        public readonly SafeFileHandle SaveFileHandle;

        /// <summary>
        ///     Creates a new file.
        /// </summary>
        /// <param name="path">The path to the file to be created.</param>
        /// <param name="fileAttributes">The attributes to be set, when CREATING the file.</param>
        /// <param name="directory">Wether it's a directory.</param>
        /// <param name="fileMode">The file mode to be used.</param>
        /// <param name="fileAccess">The EFileAccess flag to be used.</param>
        public Win32File(string path,
                         FileAttributes fileAttributes,
                         FileMode fileMode = FileMode.Create,
                         bool directory = false,
                         EFileAccess fileAccess = EFileAccess.FILE_WRITE_ATTRIBUTES | EFileAccess.FILE_WRITE_DATA)
        {
            Path = path;
            if (directory) fileAttributes = (FileAttributes)EFileAttributes.BACKUP_SEMANTICS;

            SaveFileHandle = CreateFile(path,
                (FileAccess)fileAccess,
                0,
                IntPtr.Zero,
                fileMode,
                fileAttributes,
                IntPtr.Zero);
        }

        public void Dispose()
        {
            if (SaveFileHandle != null) SaveFileHandle.Dispose();
        }

        public void SetAttributes(int fileAttributes)
        {
            SetFileAttributes(Path, fileAttributes); // TODO: set on open fileHandle for better performance
        }

        public void SetFileTime(IDictionary<string, long> timeStamps)
        {
            SetFileTime(SaveFileHandle, timeStamps);
        }

        public static void SetFileTime(SafeFileHandle hFile, IDictionary<string, long> timeStamps)
        {
			var lpCreationTime = timeStamps["CreationTimeUtc"].UnixTimeStampToUtcDateTime().ToFileTimeUtc();
			var lpLastAccessTime = timeStamps["LastAccessTimeUtc"].UnixTimeStampToUtcDateTime().ToFileTimeUtc();
			var lpLastWriteTime = timeStamps["LastWriteTimeUtc"].UnixTimeStampToUtcDateTime().ToFileTimeUtc();

            if (hFile.IsInvalid) throw new Win32Exception(Marshal.GetLastWin32Error());

            var success = SetFileTime(hFile, ref lpCreationTime, ref lpLastAccessTime, ref lpLastWriteTime);
            if (!success)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(string lpFileName,
                                                        [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
                                                        [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
                                                        IntPtr lpSecurityAttributes,
                                                        [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
                                                        [MarshalAs(UnmanagedType.U4)] FileAttributes
                                                            dwFlagsAndAttributes,
                                                        IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        private static extern bool SetFileAttributes(string lpFileName, int dwFileAttributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        private static extern bool SetFileTime(SafeFileHandle hFile,
                                               ref long lpCreationTime,
                                               ref long lpLastAccessTime,
                                               ref long lpLastWriteTime);
    }
}
