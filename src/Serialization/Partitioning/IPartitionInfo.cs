namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public interface IPartitionInfo
    {
        long Length { get; }
        string ContentStreamId { get; }
        long StartPosition { get; }
    }
}