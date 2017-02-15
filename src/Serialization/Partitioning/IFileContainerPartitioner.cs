using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public interface IFileContainerPartitioner<in THeader, in TFile>
        where THeader : IFileHeader
        where TFile : IFile
    {
        IPartitioningScheme GetPartitioningScheme(TFile file, THeader fileHeader, long mainPartBodyLength, long bodyLength, IList<string> filter = null);
    }
}