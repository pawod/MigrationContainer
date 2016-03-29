namespace DataMigrator.Enumerator.ContentMapping
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Container.Base.Header;
    using Container.FileContainer.Header;
    using Container.NtfsDirectoryContainer;

    public class NtfsDirectoryHeaderMappingEnumerator : ContentMappingEnumerator<NtfsDirectoryHeader, NtfsFileHeader>
    {
        private Queue<AlternateStreamHeader> _alternateStreams;
        private bool _collectedAlternateStreams;

        public NtfsDirectoryHeaderMappingEnumerator(DirectoryInfo rootDirectory, NtfsDirectoryHeader rootDirectoryHeader, IList<string> filter = null)
            : base(rootDirectory, rootDirectoryHeader, filter)
        {
            _alternateStreams = new Queue<AlternateStreamHeader>();
        }

        public override bool MoveNext()
        {
            if (Current == null && BasicMoveNext()) if (MoveNextAlternateStream()) return true;
            while (true)
            {
                if (MoveNextAlternateStream()) return true;
                var hasNext = BasicMoveNext();
                if (!hasNext) return false;
                _collectedAlternateStreams = false;
                if (!ContentHeaderEnumerator.Current.IsLeaf()) continue;
                CurrentHeaderSourceMapping = new HeaderSourceMapping<IContentHeader>(CurrentSourceName, CurrentHeader);
                return true;
            }
        }

        private bool CurrentHasAlternateStreams()
        {
            var alternateStreams = ContentHeaderEnumerator.Current.IsNode()
                ? ContentHeaderEnumerator.Current.Node.AlternateStreams
                : ContentHeaderEnumerator.Current.Leaf.AlternateStreams;
            return (alternateStreams != null && alternateStreams.Any());
        }

        private bool MoveNextAlternateStream()
        {
            if (!_collectedAlternateStreams && CurrentHasAlternateStreams())
            {
                var altStreams = ContentHeaderEnumerator.Current.IsNode()
                    ? ((NtfsDirectoryHeader)Current.ContentHeader).AlternateStreams
                    : ((NtfsFileHeader)Current.ContentHeader).AlternateStreams;
                _alternateStreams = new Queue<AlternateStreamHeader>(altStreams);
                _collectedAlternateStreams = true;
            }
            if (_alternateStreams.Count > 0)
            {
                CurrentHeaderSourceMapping = new HeaderSourceMapping<IContentHeader>(CurrentSourceName,
                    _alternateStreams.Dequeue());
                return true;
            }
            return false;
        }
    }
}
