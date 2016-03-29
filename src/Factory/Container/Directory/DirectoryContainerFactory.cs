namespace DataMigrator.Factory.Container.Directory
{
    using System.IO;
    using Base.Container;
    using DataMigrator.Container.DirectoryContainer;
    using DataMigrator.Container.DirectoryContainer.Header;
    using Serialization.Base;

    public class DirectoryContainerFactory :
        MigrationContainerFactory<DirectoryContainerInfo, DirectoryHeader, DirectoryInfo>
    {
        protected override IContainerSerializer<DirectoryHeader, DirectoryInfo> GetSerializer()
        {
            return new ContainerSerializer<DirectoryHeader, DirectoryInfo>(ContainerConfiguration.ContentBufferSize);
        }
    }
}
