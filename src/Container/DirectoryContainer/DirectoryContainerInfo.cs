namespace DataMigrator.Container.DirectoryContainer
{
    using System.IO;
    using Container.Base;
    using FileContainer.Header;
    using Header;

    [ContainerMetaDescription(".dc")]
    public class DirectoryContainerInfo :
        DirectoryContainerInfoBase<DirectoryContainerInfo, DirectoryHeader, FileHeader>
    {
        public DirectoryContainerInfo(FileInfo fileInfo) : base(fileInfo)
        {
        }
    }
}
