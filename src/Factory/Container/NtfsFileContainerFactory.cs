using Pawod.MigrationContainer.Configuration;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Serialization;

namespace Pawod.MigrationContainer.Factory.Container
{
    public class NtfsFileContainerFactory : MigrationContainerFactory<NtfsFileContainer, NtfsFileHeader, NtfsFile, NtfsFile>
    {
        private static NtfsFileContainerFactory _instance;
        public static NtfsFileContainerFactory Instance => _instance ?? (_instance = new NtfsFileContainerFactory());

        protected NtfsFileContainerFactory()
        {
        }

        protected override IContainerSerializer<NtfsFileHeader, NtfsFile, NtfsFile> GetSerializer()
        {
            return
                new NtfsContainerSerializer<NtfsFileHeader, NtfsFile, NtfsFile>(ContainerConfiguration.Instance<NtfsFileContainer>().ContentBufferSize);
        }
    }
}