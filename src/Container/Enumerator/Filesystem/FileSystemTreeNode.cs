using System.Collections.Generic;
using System.Linq;
using Pawod.MigrationContainer.Container.Enumerator.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Container.Enumerator.Filesystem
{
    public class FileSystemTreeNode<TDirectory, TFile> : IterableTreeNode<TDirectory, TFile>
        where TDirectory : IDirectory
        where TFile : IFile
    {
        public FileSystemTreeNode(TDirectory node, IEnumerable<string> filter) : base(node, filter)
        {
        }

        protected override IList<TDirectory> GetChildNodes(TDirectory node)
        {
            return node.GetDirectories().Cast<TDirectory>().ToList();
        }

        protected override IList<TFile> GetLeaves(TDirectory node)
        {
            return node.GetFiles().Cast<TFile>().ToList();
        }

        protected override bool PassesFilter(TDirectory node)
        {
            return Filter == null || !Filter.Contains(node.Name);
        }

        protected override bool PassesFilter(TFile leaf)
        {
            return Filter == null || !Filter.Contains(leaf.Name);
        }
    }
}