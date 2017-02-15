using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Header.NTFS;

namespace Pawod.MigrationContainer.Container.Enumerator.Header
{
    public class NtfsDirHeaderTreeNode : DirectoryHeaderTreeNode<NtfsDirectoryHeader, NtfsFileHeader>
    {
        public NtfsDirHeaderTreeNode(NtfsDirectoryHeader node, IEnumerable<string> filter) : base(node, filter)
        {
        }
    }
}