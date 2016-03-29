namespace DataMigrator.Container.DirectoryContainer.Header
{
    using System.Collections.Generic;
    using System.IO;
    using Base;
    using FileContainer.Header;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class DirectoryHeader : DirectoryHeaderBase<DirectoryHeader, FileHeader>
    {
        protected DirectoryHeader()
        {
        }

        public DirectoryHeader(long contentOffset, long nextHeaderLength) : base(contentOffset, nextHeaderLength)
        {
        }

        protected override void HandleFile(FileInfo fileInfo, ref long offsetAkk, IList<string> filter = null)
        {
            var fileHeader = new FileHeader(offsetAkk, 0);
            fileHeader.AssociateWith(fileInfo, filter);
            Files.Add(fileHeader);

            ContentLength += fileHeader.ContentLength;
            offsetAkk += fileHeader.ContentLength;
        }

        protected override void HandleSubDirectory(DirectoryInfo directoryInfo,
                                                   ref long offsetAkk,
                                                   IList<string> filter = null)
        {
            if (directoryInfo.Attributes.HasFlag(FileAttributes.ReparsePoint)) Logger.Trace("Skipping reparse point at: '{0}'.", directoryInfo.FullName);

            else
            {
                var directoryHeader = new DirectoryHeader(offsetAkk, 0);
                directoryHeader.InitializeRecursivelyFrom(directoryInfo, RelativePath, filter);
                SubDirectories.Add(directoryHeader);

                ContentLength += directoryHeader.ContentLength;
                offsetAkk += directoryHeader.ContentLength;
            }
        }
    }
}
