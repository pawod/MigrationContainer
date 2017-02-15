using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Serialization.Partitioning;

namespace Pawod.MigrationContainer.Factory.Parameters
{
    public class NtfsDirectoryContainerParametersFactory :
        SerializationParametersFactory<NtfsDirectoryContainer, NtfsDirectoryHeader, NtfsDirectory, NtfsFile>
    {
        private static NtfsDirectoryContainerParametersFactory _instance;
        public static NtfsDirectoryContainerParametersFactory Instance => _instance ?? (_instance = new NtfsDirectoryContainerParametersFactory());
        protected override IFileContainerPartitioner<NtfsDirectoryHeader, NtfsDirectory> Partitioner => new NtfsDirectoryContainerPartitioner();

        private NtfsDirectoryContainerParametersFactory()
        {
        }

        protected override NtfsDirectoryHeader CreateContentHeader(NtfsDirectory source, long nextHeaderLength)
        {
            var dirHeader = new NtfsDirectoryHeader(source, 0, nextHeaderLength);
            dirHeader.MapDirectoryContent(source, Config.Filter);
            return dirHeader;
        }
    }
}