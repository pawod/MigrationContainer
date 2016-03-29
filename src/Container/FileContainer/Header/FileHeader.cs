namespace DataMigrator.Container.FileContainer.Header
{
	using System.Collections.Generic;
	using System.IO;
    using Base.Header;
    using ProtoBuf;

    /// <summary>
    ///     Contains a normalized model of the information, which required to reproduce a
    ///     file from a MigrationContainer.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(1, typeof(NtfsFileHeader))]
    public class FileHeader : FilesystemHeader<FileInfo>
    {
        public FileHeader(long contentOffset, long nextHeaderLength) : base(contentOffset, nextHeaderLength)
        {
        }

        protected FileHeader()
        {
        }

        public override void AssociateWith(FileInfo fileInfo, IList<string> filter = null)
        {
            base.AssociateWith(fileInfo, filter);
            ContentLength = fileInfo.Length;
        }
    }
}
