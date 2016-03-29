namespace DataMigrator.Enumerator.Filesystem
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Base;

	public class FilesystemTreeNode : TreeNode<DirectoryInfo, FileInfo>
	{
		private IList<DirectoryInfo> _childNodes;
		private IList<FileInfo> _leaves;
		private readonly IList<string> _filter;

		public FilesystemTreeNode(DirectoryInfo node, IList<string> filter = null) : base(node)
		{
			_filter = filter;
		}

		protected override IList<FileInfo> Leaves
		{
			get { return _leaves ?? (_leaves = Value.GetFiles().Where(f => !IsBlackListed(f.Name)).ToList()); }
		}

		protected override IList<DirectoryInfo> Nodes
		{
			get
			{
				return _childNodes
					   ?? (_childNodes =
						   Value.GetDirectories()
								.Where(d => !IsBlackListed(d.Name) && !d.Attributes.HasFlag(FileAttributes.ReparsePoint))
								.ToList());
			}
		}

		private bool IsBlackListed(string name)
		{
			return _filter != null && _filter.Contains(name);
		}
	}
}
