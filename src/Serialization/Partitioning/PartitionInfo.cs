namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public class PartitionInfo : IPartitionInfo
    {
        public PartitionInfo(string contentStreamId, long startPosition, long length)
        {
            ContentStreamId = contentStreamId;
            Length = length;
            StartPosition = startPosition;
        }

        public string ContentStreamId { get; }

        public long Length { get; }
        public long StartPosition { get; }
    }
}