using System.Collections.Generic;
using System.IO;
using Pawod.MigrationContainer.Filesystem.NTFS;

namespace Pawod.MigrationContainer.Container.Enumerator.Filesystem
{
    public class NtfsTreeNode : FileSystemTreeNode<NtfsDirectory, NtfsFile>
    {
        public NtfsTreeNode(NtfsDirectory node, IEnumerable<string> filter = null) : base(node, filter)
        {
        }

        protected override bool PassesFilter(NtfsDirectory node)
        {
            return base.PassesFilter(node) && !node.AttributesFlag.HasFlag(FileAttributes.ReparsePoint);
        }
    }
}