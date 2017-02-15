using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Alphaleonis.Win32.Filesystem;
using NLog;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Body;
using Pawod.MigrationContainer.Container.Enumerator.Base;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Exception;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Pawod.MigrationContainer.Adapter
{
    /// <summary>
    ///     Provides methods for the import of NTFS directory containers into an NTFS file system.
    /// </summary>
    public class NtfsDirectoryAdapter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly NtfsFileAdapter _fileAdapter;

        public NtfsDirectoryAdapter()
        {
            _fileAdapter = new NtfsFileAdapter();
        }

        public NtfsDirectory Import(NtfsDirectoryContainer container, NtfsDirectory targetDir)
        {
            if (container.ContentHeader == null) throw new InvalidContainerException("DirectoryHeader is null.");
            return ImportDirectory(container.Body, container.ContentHeader, targetDir);
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
        public void ImportAlternateStream(IContainerBody body, AlternateStreamHeader alternateStreamHeader, IFile targetFile)
        {
            var path =$"{targetFile.FullPath}:{alternateStreamHeader.OriginalName}:$DATA";
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

        public NtfsDirectory ImportDirectory(IContainerBody body,
                                             NtfsDirectoryHeader directoryHeader,
                                             NtfsDirectory targetDir,
                                             string customDirName = null)
        {
            try
            {
                var dirStack = new Stack<Tuple<NtfsDirectoryHeader, NtfsDirectory>>();
                var replaceString = string.Empty;
                foreach (TreeElement<NtfsDirectoryHeader, NtfsFileHeader> header in directoryHeader)
                {
                    if (header.IsNode())
                    {
                        if (!string.IsNullOrWhiteSpace(customDirName))
                        {
                            if (dirStack.Count == 0)
                            {
                                replaceString = header.Node.OriginalName;
                                header.Node.RelativePath = header.Node.RelativePath.Replace(replaceString, customDirName);
                                header.Node.OriginalName = customDirName;
                            }
                            else header.Node.RelativePath = header.Node.RelativePath.Replace(replaceString, customDirName);
                        }

                        var importPath = Path.Combine(targetDir.FullPath, header.Node.RelativePath);
                        var importedDir = new NtfsDirectory(importPath);
                        Logger.Trace($"Importing Directory: '{importedDir.Name}' to: '{importedDir.FullPath}'");
                        Directory.CreateDirectory(importPath);

                        dirStack.Push(new Tuple<NtfsDirectoryHeader, NtfsDirectory>(header.Node, importedDir));
                    }
                    else _fileAdapter.ImportFile(header.Leaf, body, dirStack.Count == 0 ? null : dirStack.Peek().Item2);
                }
                RestoreProperties(dirStack, body);
                return new NtfsDirectory(Path.Combine(targetDir.FullPath, directoryHeader.OriginalName));
            }
            catch (System.Exception ex)
            {
                var containerException = new ImportFailedException(directoryHeader, ex);
                Logger.Error(containerException);
                throw containerException;
            }
        }

        public NtfsFile ImportFile(NtfsFileHeader header, IContainerBody body, IDirectory targetDir)
        {
            return _fileAdapter.ImportFile(header, body, targetDir);
        }

        /// <summary>
        ///     Imports the a JunctionPoint based on the specified JunctionHeader.
        /// </summary>
        /// <param name="junctionHeader">
        ///     The JunctionHeader, that describes the junction to
        ///     be imported.
        /// </param>
        /// <param name="targetDir">The directory, where to create the junction.</param>
        public void ImportJunction(JunctionHeader junctionHeader, NtfsDirectory targetDir)
        {
            var importedLink = Path.Combine(targetDir.FullPath, junctionHeader.Name);
            string importedTarget;
            if (junctionHeader.IsRelativeTarget)
            {
                var seperator = junctionHeader.Target.Split(Path.DirectorySeparatorChar).First();
                var importRootParent = Regex.Split(targetDir.FullPath, Regex.Escape(seperator)).First();
                importedTarget = Path.Combine(importRootParent, junctionHeader.Target);
            }
            else importedTarget = junctionHeader.Target;

            using (var hFile = new JunctionPoint(importedLink, importedTarget).CreateGetFileHandle()) Win32File.SetFileTime(hFile, junctionHeader.TimeCreatedUtc, junctionHeader.TimeModifiedUtc);
        }


        private void RestoreProperties(Stack<Tuple<NtfsDirectoryHeader, NtfsDirectory>> dirStack, IContainerBody body)
        {
            // restore junctions & alternate streams
            foreach (var tuple in dirStack)
            {
                var junctions = tuple.Item1.Junctions;
                var alternateStreams = tuple.Item1.AlternateStreamHeaders;

                if (alternateStreams != null)
                {
                    foreach (var alternateStream in alternateStreams) { ImportAlternateStream(body, alternateStream, tuple.Item2); }
                }

                if (junctions != null)
                {
                    foreach (var junctionHeader in junctions) { ImportJunction(junctionHeader, tuple.Item2); }
                }
            }

            // restore timestamps and attribute flags
            while (dirStack.Count > 0)
            {
                var tuple = dirStack.Pop();
                using (var win32File = new Win32File(tuple.Item2.FullPath, FileAttributes.Normal, FileMode.Open, true))
                {
                    win32File.SetAttributes(tuple.Item1.AttributeFlags);
                    win32File.SetFileTime(tuple.Item1.TimeCreatedUtc, tuple.Item1.TimeModifiedUtc);
                }
            }
        }
    }
}