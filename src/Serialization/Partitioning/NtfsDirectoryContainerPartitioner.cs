using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Enumerator.Mapping;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;

namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public class NtfsDirectoryContainerPartitioner : DirectoryContainerPartitioner<NtfsDirectoryHeader, NtfsFileHeader, NtfsDirectory>
    {
        protected override IPartitionInfo CreatePartitionInfo(IFilesystemMapping<IFileHeader> filesystemMapping, long streamPosition, long toBeWritten)
        {
            if (!(filesystemMapping.Header is AlternateStreamHeader)) return new PartitionInfo(filesystemMapping.SourceName, streamPosition, toBeWritten);
            var path = $"{filesystemMapping.SourceName}:{filesystemMapping.Header.OriginalName}:$DATA";
            return new PartitionInfo(path, streamPosition, toBeWritten);
        }

        protected override IEnumerator<IFilesystemMapping<IFileHeader>> GetMappingEnumerator(NtfsDirectory rootDirectory,
                                                                                             NtfsDirectoryHeader rootDirectoryHeader,
                                                                                             IList<string> filter = null)
        {
            return new NtfsMappingEnumerator(rootDirectory, rootDirectoryHeader, filter);
        }
    }
}