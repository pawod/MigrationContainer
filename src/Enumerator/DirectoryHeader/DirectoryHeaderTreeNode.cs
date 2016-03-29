namespace DataMigrator.Enumerator.DirectoryHeader
{
    using System.Collections.Generic;
    using System.IO;
    using Base;
    using Container.Base.Header;
    using Container.DirectoryContainer.Base;

    public class DirectoryHeaderTreeNode<TDirectoryHeader, TFileHeader> : TreeNode<TDirectoryHeader, TFileHeader>
        where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
        protected override IList<TFileHeader> Leaves
        {
            get { return Value.Files; }
        }

        protected override IList<TDirectoryHeader> Nodes
        {
            get { return Value.SubDirectories; }
        }

        public DirectoryHeaderTreeNode(IDirectoryHeaderBase<TDirectoryHeader, TFileHeader> value)
            : base((TDirectoryHeader)value)
        {
        }
    }
}
