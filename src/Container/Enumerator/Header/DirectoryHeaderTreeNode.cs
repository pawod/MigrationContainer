using System.Collections.Generic;
using System.Linq;
using Pawod.MigrationContainer.Container.Enumerator.Base;
using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Header
{
    public class DirectoryHeaderTreeNode<TDirectoryHeader, TFileHeader> : IterableTreeNode<TDirectoryHeader, TFileHeader>
        where TDirectoryHeader : class, IDirectoryHeader<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFileHeader
    {
        protected DirectoryHeaderTreeNode(TDirectoryHeader node, IEnumerable<string> filter) : base(node, filter)
        {
        }


        protected override IList<TDirectoryHeader> GetChildNodes(TDirectoryHeader node)
        {
            return node.SubdirHeaders;
        }

        protected override IList<TFileHeader> GetLeaves(TDirectoryHeader node)
        {
            return node.FileHeaders;
        }

        protected override bool PassesFilter(TDirectoryHeader node)
        {
            if (Filter == null) return true;
            return !Filter.Contains(node.OriginalName);
        }

        protected override bool PassesFilter(TFileHeader leaf)
        {
            if (Filter == null) return true;
            return !Filter.Contains(leaf.OriginalName);
        }
    }
}