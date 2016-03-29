namespace DataMigrator.Adapters.NTFS.File
{
    using System.IO;
    using Base.File;
    using CodeFluent.Runtime.BinaryServices;
    using Container.Base.Body;
    using Container.Base.Header;
    using Container.FileContainer;
    using Container.FileContainer.Header;
    using FileSystem;

    public class NtfsFileAdapter : FileAdapterBase<NtfsFileContainerInfo, NtfsFileHeader>, INtfsAdapter
    {
        public void ImportAlternateStream(IContainerBody body,
                                          AlternateStreamHeader alternateStreamHeader,
                                          string streamTargetPath)
        {
            var path = streamTargetPath + alternateStreamHeader.OriginalName;
            using (var targetStream = NtfsAlternateStream.Open(path, FileAccess.Write, FileMode.Create, FileShare.None)) body.Extract(alternateStreamHeader, targetStream);
        }

        protected override string GetImportKey(NtfsFileHeader fileHeader, string locationKey)
        {
            return Path.Combine(locationKey, fileHeader.OriginalName);
        }

        protected override void RestoreFile(NtfsFileHeader fileHeader, IContainerBody body, string importKey)
        {
            using (var win32File = new Win32File(importKey, (FileAttributes)fileHeader.AttributeFlags))
            using (var targetStream = new FileStream(win32File.SaveFileHandle, FileAccess.Write))
            {
                body.Extract(fileHeader, targetStream);
                if (fileHeader.AlternateStreams != null)
                {
                    foreach (var alternateStream in fileHeader.AlternateStreams)
                    {
                        ImportAlternateStream(body, alternateStream, importKey);
                    }
                }
                win32File.SetFileTime(fileHeader.TimeStamps);
            }
        }
    }
}
