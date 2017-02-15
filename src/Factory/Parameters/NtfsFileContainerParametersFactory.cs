using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Serialization.Partitioning;

namespace Pawod.MigrationContainer.Factory.Parameters
{
    public class NtfsFileContainerParametersFactory : SerializationParametersFactory<NtfsFileContainer, NtfsFileHeader, NtfsFile, NtfsFile>
    {
        private static NtfsFileContainerParametersFactory _instance;
        public static NtfsFileContainerParametersFactory Instance => _instance ?? (_instance = new NtfsFileContainerParametersFactory());
        protected override IFileContainerPartitioner<NtfsFileHeader, NtfsFile> Partitioner => new NtfsFileContainerPartitioner();

        private NtfsFileContainerParametersFactory()
        {
        }

        protected override NtfsFileHeader CreateContentHeader(NtfsFile source, long nextHeaderLength)
        {
            return new NtfsFileHeader(source, 0, nextHeaderLength);
        }
    }
}