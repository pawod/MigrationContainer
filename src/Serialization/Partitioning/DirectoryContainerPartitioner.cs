using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Enumerator.Mapping;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public abstract class DirectoryContainerPartitioner<TDirectoryHeader, TFileHeader, TDirectory> :
        IDirectoryContainerPartitioner<TDirectoryHeader, TDirectory>
        where TDirectoryHeader : class, IDirectoryHeader<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFileHeader
        where TDirectory : IDirectory
    {
        public virtual IPartitioningScheme GetPartitioningScheme(TDirectory directory,
                                                                 TDirectoryHeader fileHeader,
                                                                 long mainPartBodyLength,
                                                                 long bodyLength,
                                                                 IList<string> filter = null)
        {
            var scheme = new PartitioningScheme();
            var mappingEnumerator = GetMappingEnumerator(directory, fileHeader, filter);

            var part = 0;
            var streamPosition = 0L;
            var remainingContentLength = 0L;
            while (remainingContentLength > 0 || mappingEnumerator.MoveNext())
            {
                var remainingPartitionSpace = part == 0 ? mainPartBodyLength : bodyLength;
                if (remainingContentLength == 0) remainingContentLength = mappingEnumerator.Current.Header.ContentLength;

                while (remainingPartitionSpace > 0)
                {
                    if (remainingContentLength == 0)
                    {
                        if (!mappingEnumerator.MoveNext()) return scheme;
                        remainingContentLength = mappingEnumerator.Current.Header.ContentLength;
                        streamPosition = 0L;
                        continue;
                    }
                    var toBeWritten = remainingContentLength > remainingPartitionSpace ? remainingPartitionSpace : remainingContentLength;

                    scheme.AddPartitionInfo(part, CreatePartitionInfo(mappingEnumerator.Current, streamPosition, toBeWritten));

                    streamPosition += toBeWritten;
                    remainingPartitionSpace -= toBeWritten;
                    remainingContentLength -= toBeWritten;
                }
                part++;
            }
            return scheme;
        }

        protected abstract IEnumerator<IFilesystemMapping<IFileHeader>> GetMappingEnumerator(TDirectory rootDirectory,
                                                                                             TDirectoryHeader rootDirectoryHeader,
                                                                                             IList<string> filter = null);

        protected abstract IPartitionInfo CreatePartitionInfo(IFilesystemMapping<IFileHeader> filesystemMapping, long streamPosition, long toBeWritten);
    }
}