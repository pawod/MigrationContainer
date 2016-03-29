namespace DataMigrator.Serialization.Partitioning.Base
{
    public interface IPartitionInfo
    {
        long Length { get; }
        string Name { get; }
        long StartPosition { get; }
    }
}
