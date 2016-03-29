namespace DataMigrator.Serialization.Partitioning.Base
{
    using System.Collections.Generic;
    using System.Linq;

    public class PartitioningScheme : IPartitioningScheme
    {
        private readonly Dictionary<int, List<IPartitionInfo>> _parts;

        public int NumberOfParts
        {
            get { return (_parts.Count == 0)? 1 : _parts.Last().Key + 1; }
        }

        public PartitioningScheme()
        {
            _parts = new Dictionary<int, List<IPartitionInfo>>();
        }

        public void AddStreamInfo(int partNumber, IPartitionInfo partitionInfo)
        {
            if (!_parts.ContainsKey(partNumber)) _parts.Add(partNumber, new List<IPartitionInfo>());
            _parts[partNumber].Add(partitionInfo);
        }

        public IList<IPartitionInfo> GetStreamInfo(int partNumber)
        {
            return _parts[partNumber];
        }

        public bool MainPartHasOnlyHeaders()
        {
            return !_parts.ContainsKey(0);
        }
    }
}
