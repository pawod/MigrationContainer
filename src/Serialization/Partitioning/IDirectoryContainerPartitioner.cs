using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public interface IDirectoryContainerPartitioner<in THeader, in TDirectory> : IFileContainerPartitioner<THeader, TDirectory>
        where THeader : IFileHeader
        where TDirectory : IDirectory
    {
        new IPartitioningScheme GetPartitioningScheme(TDirectory directory, THeader fileHeader, long mainPartBodyLength, long bodyLength, IList<string> filter = null);
    }
}