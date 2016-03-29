namespace DataMigrator.Enumerator.ContentMapping
{
	using System.Collections.Generic;
	using System.IO;
	using Container.DirectoryContainer.Header;
	using Container.FileContainer.Header;

	public class DirectoryHeaderMappingEnumerator : ContentMappingEnumerator<DirectoryHeader, FileHeader>
	{
		public DirectoryHeaderMappingEnumerator(DirectoryInfo rootDirectory,
												DirectoryHeader rootDirectoryHeader,
												IList<string> filter = null) : base(rootDirectory, rootDirectoryHeader, filter)
		{
		}

		public override bool MoveNext()
		{
			if (!BasicMoveNext()) return false;
			while (ContentHeaderEnumerator.Current.IsNode())
			{
				if (!BasicMoveNext()) return false;
			}
			return true;
		}
	}
}
