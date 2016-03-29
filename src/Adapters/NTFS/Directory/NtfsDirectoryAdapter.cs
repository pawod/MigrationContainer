namespace DataMigrator.Adapters.NTFS.Directory
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Base.Directory;
    using CodeFluent.Runtime.BinaryServices;
    using Container.Base.Body;
    using Container.Base.Header;
    using Container.FileContainer.Header;
    using Container.NtfsDirectoryContainer;
    using File;
    using FileSystem;

    public class NtfsDirectoryAdapter :
        DirectoryAdapterBase<NtfsDirectoryContainerInfo, NtfsDirectoryHeader, NtfsFileHeader>,
        INtfsDirectoryAdapter
    {
        public NtfsDirectoryAdapter() : base(new NtfsFileAdapter())
        {
        }

        public void ImportAlternateStream(IContainerBody body,
                                          AlternateStreamHeader alternateStreamHeader,
                                          string streamTargetPath)
        {
            var path = streamTargetPath + alternateStreamHeader.OriginalName;
            using (var targetStream = NtfsAlternateStream.Open(path, FileAccess.Write, FileMode.Create, FileShare.None)) body.Extract(alternateStreamHeader, targetStream);
        }

        public void ImportJunction(JunctionHeader junctionHeader, string targetPath)
        {
            var importedLink = Path.Combine(targetPath, junctionHeader.Name);
            string importedTarget;
            if (junctionHeader.IsRelativeTarget)
            {
                var seperator = junctionHeader.Target.Split(Path.DirectorySeparatorChar).First();
                var importRootParent = Regex.Split(targetPath, Regex.Escape(seperator)).First();
                importedTarget = Path.Combine(importRootParent, junctionHeader.Target);
            }
            else importedTarget = junctionHeader.Target;

            using (var hFile = new JunctionPoint(importedLink, importedTarget).CreateGetFileHandle()) Win32File.SetFileTime(hFile, junctionHeader.TimeStamps);
        }

        protected override string GetImportKey(NtfsDirectoryHeader directoryHeader, string locationKey)
        {
            return Path.Combine(locationKey, directoryHeader.RelativePath);
        }

        protected override void RestoreDirectory(NtfsDirectoryHeader fileHeader, IContainerBody body, string importKey)
        {
            Logger.Trace("Importing Directory: '{0}' to: '{1}'",
                Path.GetFileName(importKey),
                Path.GetDirectoryName(importKey));
            Directory.CreateDirectory(importKey);
        }

        protected override void RestoreProperties(Stack<Tuple<NtfsDirectoryHeader, string>> dirStack,
                                                  IContainerBody body)
        {
            // restore junctions & alternate streams
            foreach (var tuple in dirStack)
            {
                var junctions = tuple.Item1.Junctions;
                var alternateStreams = tuple.Item1.AlternateStreams;

                if (alternateStreams != null)
                {
                    foreach (var alternateStream in alternateStreams)
                    {
                        ImportAlternateStream(body, alternateStream, tuple.Item2);
                    }
                }

                if (junctions != null)
                {
                    foreach (var junctionHeader in junctions)
                    {
                        ImportJunction(junctionHeader, tuple.Item2);
                    }
                }
            }

            // restore timestamps and attribute flags
            while (dirStack.Count > 0)
            {
                var tuple = dirStack.Pop();
                using (var win32File = new Win32File(tuple.Item2, FileAttributes.Normal, FileMode.Open, true))
                {
                    win32File.SetAttributes(tuple.Item1.AttributeFlags);
                    win32File.SetFileTime(tuple.Item1.TimeStamps);
                }
            }
        }
    }
}
