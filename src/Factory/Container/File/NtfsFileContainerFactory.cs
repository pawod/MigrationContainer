namespace DataMigrator.Factory.Container.File
{
    using System.IO;
    using Base.Container;
    using DataMigrator.Container.FileContainer;
    using DataMigrator.Container.FileContainer.Header;

    public class NtfsFileContainerFactory : NtfsContainerFactory<NtfsFileContainerInfo, NtfsFileHeader, FileInfo>
    {
    }
}
