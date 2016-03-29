namespace DataMigrator.Enumerator.DirectoryHeader
{
	using System.Collections.Generic;
	using Base;
    using Container.DirectoryContainer.Header;
    using Container.FileContainer.Header;

    public class DirectoryHeaderEnumerator : TreeEnumerator<DirectoryHeader, FileHeader>
    {
        public DirectoryHeaderEnumerator(ITreeNode<DirectoryHeader, FileHeader> root, IList<string> filter = null) : base(root, filter)
        {
        }

        protected override ITreeNode<DirectoryHeader, FileHeader> CreateTreeNode(DirectoryHeader node, IList<string> filter = null)
        {
            return new DirectoryHeaderTreeNode<DirectoryHeader, FileHeader>(node);
        }
    }
}
