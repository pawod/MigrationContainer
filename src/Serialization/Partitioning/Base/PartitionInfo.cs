namespace DataMigrator.Serialization.Partitioning.Base
{
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
    public class PartitionInfo : IPartitionInfo
    {
        public long Length { get; private set; }
        public string Name { get; private set; }
        public long StartPosition { get; private set; }

        public PartitionInfo(string name, long startPosition, long length)
        {
            Name = name;
            Length = length;
            StartPosition = startPosition;
        }
    }
}
