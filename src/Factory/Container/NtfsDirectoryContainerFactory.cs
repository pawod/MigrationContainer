using Pawod.MigrationContainer.Configuration;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Serialization;

namespace Pawod.MigrationContainer.Factory.Container
{
    public class NtfsDirectoryContainerFactory : MigrationContainerFactory<NtfsDirectoryContainer, NtfsDirectoryHeader, NtfsDirectory, NtfsFile>
    {
        private static NtfsDirectoryContainerFactory _instance;
        public static NtfsDirectoryContainerFactory Instance => _instance ?? (_instance = new NtfsDirectoryContainerFactory());

        protected NtfsDirectoryContainerFactory()
        {
        }

        protected override IContainerSerializer<NtfsDirectoryHeader, NtfsDirectory, NtfsFile> GetSerializer()
        {
            return
                new NtfsContainerSerializer<NtfsDirectoryHeader, NtfsDirectory, NtfsFile>(
                    ContainerConfiguration.Instance<NtfsDirectoryContainer>().ContentBufferSize);
        }
    }
}