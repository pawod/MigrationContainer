namespace DataMigrator.Container.DirectoryContainer.Base
{
	using System.Collections.Generic;
	using System.IO;
	using Container.Base.Header;
	using Enumerator.Base;
	using Enumerator.DirectoryHeader;
	using FileContainer.Header;
	using Header;
	using NtfsDirectoryContainer;
	using ProtoBuf;

	[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
	[ProtoInclude(10, typeof(DirectoryHeader))]
	[ProtoInclude(11, typeof(NtfsDirectoryHeader))]
	public abstract class DirectoryHeaderBase<TDirectoryHeader, TFileHeader> : FilesystemHeader<DirectoryInfo>,
																			   IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
		where TDirectoryHeader : DirectoryHeaderBase<TDirectoryHeader, TFileHeader>
		where TFileHeader : FileHeader

	{
		public IList<TFileHeader> Files { get; private set; }
		public string RelativePath { get; set; }
		public IList<TDirectoryHeader> SubDirectories { get; private set; }

		protected DirectoryHeaderBase(long contentOffset, long nextHeaderLength) : base(contentOffset, nextHeaderLength)
		{
		}

		protected DirectoryHeaderBase()
		{
		}

		public override void AssociateWith(DirectoryInfo directoryInfo, IList<string> filter = null)
		{
			InitializeRecursivelyFrom(directoryInfo, string.Empty, filter);
		}

		public IEnumerator<ITreeElement<TDirectoryHeader, TFileHeader>> GetEnumerator()
		{
			var rootNode = new DirectoryHeaderTreeNode<TDirectoryHeader, TFileHeader>(this);
			return new GenericDirectoryHeaderEnumerator<TDirectoryHeader, TFileHeader>(rootNode);
		}

		public IEnumerator<ITreeElement<TDirectoryHeader, TFileHeader>> GetEnumerator(IList<string> filter = null)
		{
			var rootNode = new DirectoryHeaderTreeNode<TDirectoryHeader, TFileHeader>(this);
			return new GenericDirectoryHeaderEnumerator<TDirectoryHeader, TFileHeader>(rootNode, filter);
		}

		public void InitializeFrom(DirectoryInfo directoryInfo, IList<string> filter = null)
		{
			InitializeRecursivelyFrom(directoryInfo, string.Empty, filter);
		}

		protected virtual void HandleCurrentDirectory(DirectoryInfo directoryInfo, ref long offsetAkk)
		{
		}

		protected abstract void HandleFile(FileInfo fileInfo, ref long offsetAkk, IList<string> filter = null);

		protected abstract void HandleSubDirectory(DirectoryInfo directoryInfo,
												   ref long offsetAkk,
												   IList<string> filter = null);

		protected void InitializeRecursivelyFrom(DirectoryInfo directoryInfo, string parentPath, IList<string> filter = null)
		{
			base.AssociateWith(directoryInfo, filter);
			var offsetAkk = ContentOffset;
			RelativePath = Path.Combine(parentPath, OriginalName);
			HandleCurrentDirectory(directoryInfo, ref offsetAkk);

			Files = new List<TFileHeader>();
			foreach (var fileInfo in directoryInfo.GetFiles())
			{
				if (filter != null && filter.Contains(fileInfo.Name)) continue;
				HandleFile(fileInfo, ref offsetAkk, filter);
			}

			SubDirectories = new List<TDirectoryHeader>();
			foreach (var dirInfo in directoryInfo.GetDirectories())
			{
				if (filter != null && filter.Contains(dirInfo.Name)) continue;
				HandleSubDirectory(dirInfo, ref offsetAkk, filter);
			}
		}
	}
}
