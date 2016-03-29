namespace DataMigrator.Factory.Base.Container
{
    using System.IO;
    using DataMigrator.Container.Base;
    using DataMigrator.Container.Base.Header;
    using Serialization.Base;
    using Serialization.Ntfs;

    public abstract class NtfsContainerFactory<TContainer, TContentHeader, TFsInfo> :
        MigrationContainerFactory<TContainer, TContentHeader, TFsInfo>
        where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
        where TContentHeader : class, INtfsFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo
    {
        protected override IContainerSerializer<TContentHeader, TFsInfo> GetSerializer()
        {
            return new NtfsSerializer<TContentHeader, TFsInfo>(ContainerConfiguration.ContentBufferSize);
        }
    }
}
