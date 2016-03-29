namespace DataMigrator.Container.NtfsDirectoryContainer
{
    using System.IO;
    using Base;
    using DirectoryContainer;
    using FileContainer.Header;

    [ContainerMetaDescription(".xdc")]
    public class NtfsDirectoryContainerInfo :
        DirectoryContainerInfoBase<NtfsDirectoryContainerInfo, NtfsDirectoryHeader, NtfsFileHeader>
    {
        public NtfsDirectoryContainerInfo(FileInfo fileInfo) : base(fileInfo)
        {
        }
    }
}
