using System;
using System.Linq;
using Pawod.MigrationContainer.Configuration;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Exception;
using Pawod.MigrationContainer.Extensions;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Serialization.Parameters;
using Pawod.MigrationContainer.Serialization.Partitioning;

namespace Pawod.MigrationContainer.Factory.Parameters
{
    public abstract class SerializationParametersFactory<TContainer, THeader, TSource, TExport> :
            ISerializationParametersFactory<THeader, TSource, TExport>
        where TContainer : IMigrationContainer
        where THeader : class, IFileHeader
        where TSource : IFile
        where TExport : IFile

    {
        protected readonly ContainerConfiguration Config = ContainerConfiguration.Instance<TContainer>();

        protected abstract IFileContainerPartitioner<THeader, TSource> Partitioner { get; }

        public virtual ISerializationParameters<THeader, TSource, TExport> Create(TSource source,
                                                                                  IDirectory targetDir,
                                                                                  params IProtoHeader[] appHeaders)
        {
            var containerId = Guid.NewGuid();

            var appHeaderStreams = (appHeaders != null) && appHeaders.Any() ? appHeaders.Select(h => h.Serialize()).ToArray() : null;
            var appHeadersLength = appHeaderStreams?.Sum(s => s.Length) ?? 0;

            var contentHeader = CreateContentHeader(source, appHeadersLength);
            var contentHeaderStream = contentHeader.Serialize();
            var allHeadersLength = StartHeader.Length + contentHeaderStream.Length + appHeadersLength;

            var mainPartContentSpace = Config.MaxContainerFileSize - allHeadersLength;
            if ((mainPartContentSpace < 0) || (allHeadersLength > Config.MaxContainerFileSize)) throw new ContainerTooSmallException(Config.MaxContainerFileSize, allHeadersLength);
            var contentSpace = Config.MaxContainerFileSize - StartHeader.Length;

            var scheme = Partitioner.GetPartitioningScheme(source, contentHeader, mainPartContentSpace, contentSpace, Config.Filter);

            return new SerializationParameters<THeader, TSource, TExport>
                   {
                       ContainerId = containerId,
                       AllHeadersLength = allHeadersLength,
                       AppHeaderStreams = appHeaderStreams,
                       ContentHeader = contentHeader,
                       ContentHeaderStream = contentHeaderStream,
                       FormatExtension = MigrationContainerExtensions.GetFileExtension<TContainer>(),
                       MaxContainerFileSize = Config.MaxContainerFileSize,
                       PartitioningScheme = scheme,
                       Source = source,
                       TargetDir = targetDir
                   };
        }

        protected abstract THeader CreateContentHeader(TSource source, long nextHeaderLength);
    }
}