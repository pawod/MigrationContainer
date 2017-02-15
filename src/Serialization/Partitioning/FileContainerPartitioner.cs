using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Serialization.Partitioning
{
    public abstract class FileContainerPartitioner<THeader, TFile> : IFileContainerPartitioner<THeader, TFile>
        where THeader : IFileHeader
        where TFile : IFile
    {
        public virtual IPartitioningScheme GetPartitioningScheme(TFile file,
                                                                 THeader fileHeader,
                                                                 long mainPartBodyLength,
                                                                 long bodyLength,
                                                                 IList<string> filter = null)
        {
            return GetPartitioningScheme(file.FullPath, fileHeader.ContentLength, mainPartBodyLength, bodyLength);
        }

        protected IPartitioningScheme GetPartitioningScheme(string streamName, long contentLength, long mainPartBodyLength, long bodyLength)
        {
            var scheme = new PartitioningScheme();

            var part = 0;
            var remainingContentLength = contentLength;
            while (remainingContentLength > 0)
            {
                scheme.AddPartitionInfo(part, CreatePartitionInfo(streamName, part, ref remainingContentLength, mainPartBodyLength, bodyLength));
                part++;
            }
            return scheme;
        }

        protected virtual IPartitionInfo CreatePartitionInfo(string contentStreamId,
                                               int part,
                                               ref long remainingContentLength,
                                               long mainPartBodyLength,
                                               long bodyLength,
                                               long bodyOffset = 0,
                                               long? sourcePosition = null)
        {
            var remainingBodyLength = (part == 0 ? mainPartBodyLength : bodyLength) - bodyOffset;
            if (sourcePosition == null) sourcePosition = part == 0 ? 0 : (part - 1)*bodyLength + mainPartBodyLength;
            var length = remainingBodyLength > remainingContentLength ? remainingContentLength : remainingBodyLength;
            remainingContentLength -= length;

            return new PartitionInfo(contentStreamId, sourcePosition.Value, length);
        }
    }
}