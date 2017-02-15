using System.IO;
using Alphaleonis.Win32.Filesystem;
using NLog;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Body;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Exception;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Filesystem.NTFS;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Pawod.MigrationContainer.Adapter
{
    /// <summary>
    ///     Provides methods for the import of NTFS file containers into an NTFS file system.
    /// </summary>
    public class NtfsFileAdapter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public NtfsFile Import(NtfsFileContainer container, NtfsDirectory targetDir)
        {
            if (container.ContentHeader == null) throw new InvalidContainerException("FileHeader is null.");
            return ImportFile(container.ContentHeader, container.Body, targetDir);
        }

        /// <summary>
        ///     Imports an alternate stream, based on the header, that describes it.
        /// </summary>
        /// <param name="body">The MigrationContainer's body.</param>
        /// <param name="alternateStreamHeader">
        ///     The header, which describes the alternate
        ///     stream to be imported.
        /// </param>
        /// <param name="targetFile">The file to append the stream to.</param>
        public void ImportAlternateStream(IContainerBody body, AlternateStreamHeader alternateStreamHeader, NtfsFile targetFile)
        {
            var path = $"{targetFile.FullPath}:{alternateStreamHeader.OriginalName}:$DATA";
            using (
                var targetStream = File.Open(path,
                                             FileMode.Create,
                                             FileAccess.Write,
                                             FileShare.None,
                                             ExtendedFileAttributes.BackupSemantics,
                                             PathFormat.LongFullPath)) {
                body.Extract(alternateStreamHeader, targetStream);
            }
        }

        /// <summary>
        ///     Imports a specific file from the MigrationContainer's content to the target
        ///     environment.
        /// </summary>
        /// <param name="header">The FileHeader, which describes the contained file.</param>
        /// <param name="body">The MigrationContainer's body.</param>
        /// <param name="targetDir">The directory where to import the file.</param>
        /// <returns>The imported file.</returns>
        public NtfsFile ImportFile(NtfsFileHeader header, IContainerBody body, IDirectory targetDir)
        {
            try
            {
                var file = new NtfsFile(Path.Combine(targetDir.FullPath, header.OriginalName));
                Logger.Trace($"Importing file with name: '{header.OriginalName}' to location: '{targetDir}'");

                using (var win32File = new Win32File(file.FullPath, (FileAttributes) header.AttributeFlags))
                using (var targetStream = new FileStream(win32File.SaveFileHandle, FileAccess.Write))
                {
                    body.Extract(header, targetStream);
                    if (header.AlternateStreamHeaders != null)
                    {
                        foreach (var alternateStream in header.AlternateStreamHeaders) { ImportAlternateStream(body, alternateStream, file); }
                    }
                    win32File.SetFileTime(header.TimeCreatedUtc, header.TimeModifiedUtc);
                }

                return file;
            }
            catch (System.Exception ex)
            {
                var containerException = new ImportFailedException(header, ex);
                Logger.Error(containerException);
                throw containerException;
            }
        }
    }
}