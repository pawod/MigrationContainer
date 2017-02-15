using System.Collections.Generic;

namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public interface IPartitioningScheme
    {
        int NumberOfParts { get; }
        void AddPartitionInfo(int partNumber, IPartitionInfo partitionInfo);
        IEnumerable<IPartitionInfo> GetPartitionInfo(int partNumber);
        bool MainPartHasOnlyHeaders();
    }
}