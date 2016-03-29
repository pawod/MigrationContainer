namespace DataMigrator.Serialization.Partitioning.DirectoryContainer
{
	using System.Collections.Generic;
	using System.IO;
	using Base;
	using Container.Base.Header;
	using Container.FileContainer.Header;
	using Container.NtfsDirectoryContainer;
	using Enumerator.ContentMapping;

	public class NtfsDirectoryContainerPartitioner : DirectoryContainerPartitionerBase<NtfsDirectoryHeader, NtfsFileHeader>
	{
		protected override IEnumerator<IHeaderSourceMapping<IContentHeader>> GetMappingEnumerator(DirectoryInfo rootDirectory,
																								  NtfsDirectoryHeader rootDirectoryHeader,
																								  IList<string> filter = null)
		{
			return new NtfsDirectoryHeaderMappingEnumerator(rootDirectory, rootDirectoryHeader, filter);
		}

		protected override IPartitionInfo GetStreamInfo(IHeaderSourceMapping<IContentHeader> headerSourceMapping,
														long streamPosition,
														long toBeWritten)
		{
			var sourceName = (headerSourceMapping.ContentHeader is AlternateStreamHeader)
				? headerSourceMapping.SourceName + headerSourceMapping.ContentHeader.OriginalName
				: headerSourceMapping.SourceName;
			return new PartitionInfo(sourceName, streamPosition, toBeWritten);
		}
	}
}
