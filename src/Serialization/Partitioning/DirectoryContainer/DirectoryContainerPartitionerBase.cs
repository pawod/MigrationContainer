namespace DataMigrator.Serialization.Partitioning.DirectoryContainer
{
	using System.Collections.Generic;
	using System.IO;
	using Base;
	using Container.Base.Header;
	using Container.DirectoryContainer.Base;
	using Enumerator.ContentMapping;

	public abstract class DirectoryContainerPartitionerBase<TDirectoryHeader, TFileHeader> :
		IContainerPartitioner<TDirectoryHeader, DirectoryInfo>
		where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
		where TFileHeader : class, IFilesystemHeader<FileInfo>
	{
		public virtual IPartitioningScheme GetPartitioningScheme(DirectoryInfo fsInfo,
																 TDirectoryHeader contentHeader,
																 long mainPartBodyLength,
																 long bodyLength,
																 IList<string> filter = null)
		{
			var scheme = new PartitioningScheme();
			var mappingEnumerator = GetMappingEnumerator(fsInfo, contentHeader, filter);

			var part = 0;
			var streamPosition = 0L;
			var remainingContentLength = 0L;
			while (remainingContentLength > 0 || mappingEnumerator.MoveNext())
			{
				var remainingPartitionSpace = (part == 0)? mainPartBodyLength : bodyLength;
				if (remainingContentLength == 0) remainingContentLength = mappingEnumerator.Current.ContentHeader.ContentLength;

				while (remainingPartitionSpace > 0)
				{
					if (remainingContentLength == 0)
					{
						if (!mappingEnumerator.MoveNext()) return scheme;
						remainingContentLength = mappingEnumerator.Current.ContentHeader.ContentLength;
						streamPosition = 0L;
						continue;
					}
					var toBeWritten = (remainingContentLength > remainingPartitionSpace)
						? remainingPartitionSpace
						: remainingContentLength;

					scheme.AddStreamInfo(part, GetStreamInfo(mappingEnumerator.Current, streamPosition, toBeWritten));

					streamPosition += toBeWritten;
					remainingPartitionSpace -= toBeWritten;
					remainingContentLength -= toBeWritten;
				}
				part++;
			}
			return scheme;
		}

		protected abstract IEnumerator<IHeaderSourceMapping<IContentHeader>> GetMappingEnumerator(DirectoryInfo rootDirectory,
																								  TDirectoryHeader rootDirectoryHeader,
																								  IList<string> filter = null);

		protected abstract IPartitionInfo GetStreamInfo(IHeaderSourceMapping<IContentHeader> headerSourceMapping,
														long streamPosition,
														long toBeWritten);
	}
}
