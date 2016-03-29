namespace DataMigrator.Container.FileContainer
{
    using System.IO;
    using Base;
    using Base.Header;

    public abstract class FileContainerInfoBase<TChild, TFileHeader> :
        MigrationContainerInfo<TChild, TFileHeader, FileInfo>
        where TChild : FileContainerInfoBase<TChild, TFileHeader>
        where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
        public override long BodyPosition
        {
            get { return StartHeader.Length + StartHeader.NextHeaderLength; }
        }

        protected FileContainerInfoBase(FileInfo fileInfo) : base(fileInfo)
        {
        }
    }
}
