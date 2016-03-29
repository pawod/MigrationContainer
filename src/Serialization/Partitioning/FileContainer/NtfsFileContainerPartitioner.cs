namespace DataMigrator.Serialization.Partitioning.FileContainer
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Base;
	using Container.FileContainer.Header;
	using Helper;

	public class NtfsFileContainerPartitioner : FileContainerPartitionerBase<NtfsFileHeader>
	{
		public override IPartitioningScheme GetPartitioningScheme(FileInfo fsInfo,
																  NtfsFileHeader contentHeader,
																  long mainPartBodyLength,
																  long bodyLength,
																  IList<string> filter = null)
		{
			var scheme = base.GetPartitioningScheme(fsInfo.FullName, contentHeader.ContentLength, mainPartBodyLength, bodyLength);


			if (contentHeader.AlternateStreams != null && contentHeader.AlternateStreams.Any())
			{
				var part = scheme.NumberOfParts == 0? 0 : scheme.NumberOfParts - 1;
				var bodyOffset = (scheme.NumberOfParts > 1)? scheme.GetStreamInfo(part).Sum(s => s.Length) : 0;
				var remainingPartitionSpace = ((part == 0)? mainPartBodyLength : bodyLength) - bodyOffset;

				foreach (var alternateStream in fsInfo.GetAlternateStreams())
				{
					var sourcePosition = 0L;
					var remainingContentLength = alternateStream.Size;

					while (remainingContentLength > 0)
					{
						var streamInfo = GetStreamInfo(fsInfo.FullName + alternateStream.Name,
							part,
							ref remainingContentLength,
							mainPartBodyLength,
							bodyLength,
							bodyOffset,
							sourcePosition);
						scheme.AddStreamInfo(part, streamInfo);

						sourcePosition += streamInfo.Length;
						remainingPartitionSpace -= streamInfo.Length;
						if (remainingPartitionSpace == 0)
						{
							part++;
							bodyOffset = 0;
							remainingPartitionSpace = ((part == 0)? mainPartBodyLength : bodyLength);
							continue;
						}
						bodyOffset += streamInfo.Length;
					}
				}
			}

			return scheme;
		}
	}
}
