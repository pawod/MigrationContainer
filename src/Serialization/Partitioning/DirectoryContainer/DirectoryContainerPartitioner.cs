namespace DataMigrator.Serialization.Partitioning.DirectoryContainer
{
	using System.Collections.Generic;
	using System.IO;
	using Base;
	using Container.Base.Header;
	using Container.DirectoryContainer.Header;
	using Container.FileContainer.Header;
	using Enumerator.ContentMapping;

	public class DirectoryContainerPartitioner : DirectoryContainerPartitionerBase<DirectoryHeader, FileHeader>
	{
		protected override IEnumerator<IHeaderSourceMapping<IContentHeader>> GetMappingEnumerator(DirectoryInfo rootDirectory,
																								  DirectoryHeader rootDirectoryHeader,
																								  IList<string> filter = null)
		{
			return new DirectoryHeaderMappingEnumerator(rootDirectory, rootDirectoryHeader, filter);
		}

		protected override IPartitionInfo GetStreamInfo(IHeaderSourceMapping<IContentHeader> headerSourceMapping,
														long streamPosition,
														long toBeWritten)
		{
			return new PartitionInfo(headerSourceMapping.SourceName, streamPosition, toBeWritten);
		}
	}
}
