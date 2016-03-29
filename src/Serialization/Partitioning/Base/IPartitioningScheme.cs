namespace DataMigrator.Serialization.Partitioning.Base
{
    using System.Collections.Generic;

    public interface IPartitioningScheme
    {
        int NumberOfParts { get; }
        void AddStreamInfo(int partNumber, IPartitionInfo partitionInfo);
        IList<IPartitionInfo> GetStreamInfo(int partNumber);
        bool MainPartHasOnlyHeaders();
    }
}
