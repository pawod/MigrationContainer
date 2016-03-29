namespace DataMigrator.Factory.Container.File
{
    using System.IO;
    using Base.Container;
    using DataMigrator.Container.FileContainer;
    using DataMigrator.Container.FileContainer.Header;
    using Serialization.Base;

    public class FileContainerFactory : MigrationContainerFactory<FileContainerInfo, FileHeader, FileInfo>
    {
        protected override IContainerSerializer<FileHeader, FileInfo> GetSerializer()
        {
            return new ContainerSerializer<FileHeader, FileInfo>(ContainerConfiguration.ContentBufferSize);
        }
    }
}
