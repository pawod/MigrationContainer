namespace DataMigrator.Enumerator.DirectoryHeader
{
	using System.Collections.Generic;
	using System.IO;
    using Base;
    using Container.Base.Header;
    using Container.DirectoryContainer.Base;

    public class GenericDirectoryHeaderEnumerator<TDirectoryHeader, TFileHeader> :
        TreeEnumerator<TDirectoryHeader, TFileHeader>
        where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
        public GenericDirectoryHeaderEnumerator(ITreeNode<TDirectoryHeader, TFileHeader> root, IList<string> filter = null) : base(root, filter)
        {
        }

        protected override ITreeNode<TDirectoryHeader, TFileHeader> CreateTreeNode(TDirectoryHeader node, IList<string> filter = null)
        {
            return new DirectoryHeaderTreeNode<TDirectoryHeader, TFileHeader>(node);
        }
    }
}
