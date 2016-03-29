namespace DataMigrator.Container.FileContainer
{
    using System.IO;
    using Base;
    using Header;

    [ContainerMetaDescription(".fc")]
    public class FileContainerInfo : FileContainerInfoBase<FileContainerInfo, FileHeader>
    {
        public FileContainerInfo(FileInfo fileInfo) : base(fileInfo)
        {
        }
    }
}
