namespace DataMigrator.Enumerator.Base
{
    using System.Collections.Generic;
    using System.IO;
    using Container.Base.Header;
    using Container.DirectoryContainer.Base;

    public abstract class DirectoryHeaderTreeNodeBase<TDirectoryHeader, TFileHeader, TContentHeader> :
        TreeNode<TDirectoryHeader, TContentHeader>
        where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : IFilesystemHeader<FileInfo>
        where TContentHeader : IContentHeader
    {
        protected override IList<TDirectoryHeader> Nodes
        {
            get { return Value.SubDirectories; }
        }

        protected DirectoryHeaderTreeNodeBase(TDirectoryHeader node) : base(node)
        {
        }
    }
}
