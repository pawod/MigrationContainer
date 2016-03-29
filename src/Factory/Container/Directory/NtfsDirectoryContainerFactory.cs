namespace DataMigrator.Factory.Container.Directory
{
    using System.IO;
    using Base.Container;
    using DataMigrator.Container.NtfsDirectoryContainer;

    public class NtfsDirectoryContainerFactory :
        NtfsContainerFactory<NtfsDirectoryContainerInfo, NtfsDirectoryHeader, DirectoryInfo>
    {
    }
}
