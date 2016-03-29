namespace DataMigrator.Serialization.Partitioning.FileContainer
{
	using System.Collections.Generic;
	using System.IO;
	using Base;
	using Container.Base.Header;

	public abstract class FileContainerPartitionerBase<TFileHeader> : IContainerPartitioner<TFileHeader, FileInfo>
		where TFileHeader : IFilesystemHeader<FileInfo>
	{
		public virtual IPartitioningScheme GetPartitioningScheme(FileInfo fsInfo,
																 TFileHeader contentHeader,
																 long mainPartBodyLength,
																 long bodyLength,
																 IList<string> filter = null)
		{
			return GetPartitioningScheme(fsInfo.FullName, contentHeader.ContentLength, mainPartBodyLength, bodyLength);
		}

		protected IPartitioningScheme GetPartitioningScheme(string streamName,
															long contentLength,
															long mainPartBodyLength,
															long bodyLength)
		{
			var scheme = new PartitioningScheme();

			var part = 0;
			var remainingContentLength = contentLength;
			while (remainingContentLength > 0)
			{
				scheme.AddStreamInfo(part,
					GetStreamInfo(streamName, part, ref remainingContentLength, mainPartBodyLength, bodyLength));
				part++;
			}
			return scheme;
		}

		protected IPartitionInfo GetStreamInfo(string contentName,
											   int part,
											   ref long remainingContentLength,
											   long mainPartBodyLength,
											   long bodyLength,
											   long bodyOffset = 0,
											   long? sourcePosition = null)
		{
			var remainingBodyLength = ((part == 0)? mainPartBodyLength : bodyLength) - bodyOffset;
			if (sourcePosition == null) sourcePosition = ((part == 0)? 0 : (part - 1)*bodyLength + mainPartBodyLength);
			var length = (remainingBodyLength > remainingContentLength)? remainingContentLength : remainingBodyLength;
			remainingContentLength -= length;

			return new PartitionInfo(contentName, sourcePosition.Value, length);
		}
	}
}
