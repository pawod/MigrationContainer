namespace DataMigrator.Container.DirectoryContainer
{
    using System.IO;
    using Base;
    using Container.Base;
    using Container.Base.Header;

    public abstract class DirectoryContainerInfoBase<TChild, TDirectoryHeader, TFileHeader> :
        MigrationContainerInfo<TChild, TDirectoryHeader, DirectoryInfo>
        where TChild : DirectoryContainerInfoBase<TChild, TDirectoryHeader, TFileHeader>
        where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
        public override long BodyPosition
        {
            get { return StartHeader.Length + StartHeader.NextHeaderLength; }
        }

        protected DirectoryContainerInfoBase(FileInfo fileInfo) : base(fileInfo)
        {
        }
    }
}
