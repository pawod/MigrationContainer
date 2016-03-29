namespace DataMigrator.Factory.Base.Parameters
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Configuration;
	using DataMigrator.Container.Base;
	using DataMigrator.Container.Base.Header;
	using Exception;
	using Serialization.Parameters.Base;
	using Serialization.Partitioning.Base;

	public abstract class SerializationParametersFactory<TContainer, TContentHeader, TFsInfo>
		where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
		where TContentHeader : class, IFilesystemHeader<TFsInfo>
		where TFsInfo : FileSystemInfo
	{
		protected readonly ContainerConfiguration ContainerConfiguration;
		private readonly IContainerPartitioner<TContentHeader, TFsInfo> _partitioner;

		protected SerializationParametersFactory(IContainerPartitioner<TContentHeader, TFsInfo> partitioner)
		{
			ContainerConfiguration = ContainerConfiguration.Instance<TContainer, TContentHeader, TFsInfo>();
			_partitioner = partitioner;
		}

		public ISerializationParameters<TContentHeader, TFsInfo> Create(TFsInfo sourceInfo,
																		DirectoryInfo targetDir,
																		params MemoryStream[] appHeaderStreams)
		{
			var contentHeader = CreateContentHeader(sourceInfo,
				appHeaderStreams != null && appHeaderStreams.Any()? appHeaderStreams.First().Length : 0L,
				ContainerConfiguration.Filter);
			var contentHeaderStream = contentHeader.Serialize();

			var appHeadersLength = (appHeaderStreams == null)
				? 0L
				: (appHeaderStreams.Any())? appHeaderStreams.Sum(h => h.Length) : 0L;

			var allHeadersLength = StartHeader.Length + contentHeaderStream.Length + appHeadersLength;
			var contentSpace = ContainerConfiguration.MaxFileSize - StartHeader.Length;
			var mainPartContentSpace = ContainerConfiguration.MaxFileSize - allHeadersLength;
			if (mainPartContentSpace < 0 || allHeadersLength > ContainerConfiguration.MaxFileSize) throw new ContainerTooSmallException(ContainerConfiguration.MaxFileSize, allHeadersLength);
			var partitioningScheme = _partitioner.GetPartitioningScheme(sourceInfo,
				contentHeader,
				mainPartContentSpace,
				contentSpace,
				ContainerConfiguration.Filter);

			return new SerializationParameters<TContainer, TContentHeader, TFsInfo>(sourceInfo,
				targetDir,
				partitioningScheme,
				contentHeader,
				allHeadersLength,
				ContainerConfiguration.MaxFileSize,
				MigrationContainer.GetContainerExtension<TContainer, TContentHeader, TFsInfo>(),
				appHeaderStreams);
		}

		protected abstract TContentHeader CreateContentHeader(TFsInfo sourceInfo,
															  long nextHeaderLength,
															  IList<string> filter = null);
	}
}
