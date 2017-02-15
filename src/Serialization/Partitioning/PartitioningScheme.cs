using System.Collections.Generic;
using System.Linq;

namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public class PartitioningScheme : IPartitioningScheme
    {
        private readonly Dictionary<int, List<IPartitionInfo>> _parts;

        public PartitioningScheme()
        {
            _parts = new Dictionary<int, List<IPartitionInfo>>();
        }

        public void AddPartitionInfo(int partNumber, IPartitionInfo partitionInfo)
        {
            if (!_parts.ContainsKey(partNumber)) _parts.Add(partNumber, new List<IPartitionInfo>());
            _parts[partNumber].Add(partitionInfo);
        }

        public IEnumerable<IPartitionInfo> GetPartitionInfo(int partNumber)
        {
            return _parts[partNumber];
        }

        public bool MainPartHasOnlyHeaders()
        {
            return !_parts.ContainsKey(0);
        }

        public int NumberOfParts => _parts.Count == 0 ? 1 : _parts.Last().Key + 1;
    }
}