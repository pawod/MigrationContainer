namespace DataMigrator.Enumerator.Filesystem
{
	using System.Collections.Generic;
	using System.IO;
	using Base;

	public class FilesystemEnumerator : TreeEnumerator<DirectoryInfo, FileInfo>
	{
		public FilesystemEnumerator(FilesystemTreeNode root, IList<string> filter = null) : base(root, filter)
		{
		}

		public FilesystemEnumerator(DirectoryInfo root, IList<string> filter = null) : base(new FilesystemTreeNode(root, filter), filter)
		{
		}

		protected override ITreeNode<DirectoryInfo, FileInfo> CreateTreeNode(DirectoryInfo node, IList<string> filter = null)
		{
			return new FilesystemTreeNode(node, filter);
		}
	}
}
