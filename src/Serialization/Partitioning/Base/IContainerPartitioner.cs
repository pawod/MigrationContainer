namespace DataMigrator.Serialization.Partitioning.Base
{
	using System.Collections.Generic;
	using System.IO;
	using Container.Base.Header;

	public interface IContainerPartitioner<in TContentHeader, in TFsInfo>
		where TContentHeader : IFilesystemHeader<TFsInfo>
		where TFsInfo : FileSystemInfo
	{
		IPartitioningScheme GetPartitioningScheme(TFsInfo fsInfo,
												  TContentHeader contentHeader,
												  long mainPartBodyLength,
												  long bodyLength,
												  IList<string> filter = null);
	}
}
