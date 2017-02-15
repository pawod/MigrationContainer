using System.Collections.Generic;
using System.Linq;
using Pawod.MigrationContainer.Container.Enumerator.Filesystem;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Filesystem.NTFS;

namespace Pawod.MigrationContainer.Container.Enumerator.Mapping
{
    public class NtfsMappingEnumerator : DirectoryMappingEnumerator<NtfsDirectoryHeader, NtfsFileHeader, NtfsTreeNode, NtfsFile, NtfsDirectory>
    {
        private Queue<AlternateStreamHeader> _alternateStreams;
        private bool _collectedAlternateStreams;

        public NtfsMappingEnumerator(NtfsDirectory rootDirectory, NtfsDirectoryHeader rootDirectoryHeader, IList<string> filter = null)
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
                if (!DirHeaderEnumerator.Current.IsLeaf()) continue;
                CurrentFilesystemMapping = new FilesystemMapping<IFileHeader>(CurrentSourceName, CurrentHeader);
                return true;
            }
        }

        private bool CurrentHasAlternateStreams()
        {
            var alternateStreams = DirHeaderEnumerator.Current.IsNode()
                                       ? DirHeaderEnumerator.Current.Node.AlternateStreamHeaders
                                       : DirHeaderEnumerator.Current.Leaf.AlternateStreamHeaders;
            return alternateStreams != null && alternateStreams.Any();
        }

        private bool MoveNextAlternateStream()
        {
            if (!_collectedAlternateStreams && CurrentHasAlternateStreams())
            {
                var altStreams = DirHeaderEnumerator.Current.IsNode()
                                     ? ((NtfsDirectoryHeader) Current.Header).AlternateStreamHeaders
                                     : ((NtfsFileHeader) Current.Header).AlternateStreamHeaders;
                _alternateStreams = new Queue<AlternateStreamHeader>(altStreams);
                _collectedAlternateStreams = true;
            }
            if (_alternateStreams.Count > 0)
            {
                CurrentFilesystemMapping = new FilesystemMapping<IFileHeader>(CurrentSourceName, _alternateStreams.Dequeue());
                return true;
            }
            return false;
        }
    }
}