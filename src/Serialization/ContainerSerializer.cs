using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Extensions;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Serialization.Parameters;
using Pawod.MigrationContainer.Serialization.Partitioning;

namespace Pawod.MigrationContainer.Serialization
{
    public abstract class ContainerSerializer<THeader, TSource, TExport> : IContainerSerializer<THeader, TSource, TExport>
        where THeader : IFileHeader
        where TSource : IFile
        where TExport : IFile
    {
        private readonly int _contentBufferSize;
        private Stream _sourceStream;
        private string _streamName = string.Empty;
    
        protected ContainerSerializer(int contentBufferSize)
        {
            _contentBufferSize = contentBufferSize;
        }

        public void Dispose()
        {
            _sourceStream?.Dispose();
        }

        public TExport Serialize(ISerializationParameters<THeader, TSource, TExport> parameters, int part)
        {
            var targetFile = parameters.GetTargetFile(part);
            using (var targetStream = targetFile.Open(FileMode.Create, FileAccess.ReadWrite))
            {
                if (part == 0)
                {
                    WriteContentHeader(parameters, targetStream);
                    if (parameters.AppHeaderStreams != null) WriteApplicationHeaders(parameters, targetStream);
                }

                // leave space for the startheader
                if (targetStream.Position == 0) targetStream.Position = StartHeader.Length;

                WriteContent(parameters, targetStream, part);
                WriteStartHeader(parameters, targetStream, part);
            }
            return targetFile;
        }

        protected abstract Stream GetSourceStream(IPartitionInfo partitionInfo);

        private void PrepareSourceStream(IPartitionInfo partitionInfo)
        {
            if (!_streamName.Equals(partitionInfo.ContentStreamId, StringComparison.Ordinal))
            {
                _sourceStream?.Dispose();
                _streamName = partitionInfo.ContentStreamId;
                _sourceStream = GetSourceStream(partitionInfo);
            }
            _sourceStream.Position = partitionInfo.StartPosition;
        }

        private void WriteApplicationHeaders(ISerializationParameters<THeader, TSource, TExport> parameters, Stream targetStream)
        {
            foreach (var appHeader in parameters.AppHeaderStreams)
            {
                appHeader.Position = 0;
                appHeader.CopyTo(targetStream, appHeader.Length, _contentBufferSize);
            }
        }

        private void WriteContent(ISerializationParameters<THeader, TSource, TExport> parameters, Stream targetStream, int partNumber)
        {
            if (parameters.PartitioningScheme.NumberOfParts == 0 || (partNumber == 0 && parameters.PartitioningScheme.MainPartHasOnlyHeaders())) return;
            foreach (var partitionInfo in parameters.PartitioningScheme.GetPartitionInfo(partNumber))
            {
                PrepareSourceStream(partitionInfo);
                _sourceStream.CopyTo(targetStream, partitionInfo.Length, _contentBufferSize);
            }
        }

        private void WriteContentHeader(ISerializationParameters<THeader, TSource, TExport> parameters, Stream targetStream)
        {
            parameters.ContentHeaderStream.CopyTo(targetStream,
                                                  0,
                                                  StartHeader.Length,
                                                  parameters.ContentHeaderStream.Length,
                                                  _contentBufferSize);
        }

        private void WriteStartHeader(ISerializationParameters<THeader, TSource, TExport> serializationParameters, Stream targetStream, int partNumber)
        {
            // move cursor to front and write magic numbers to file
            targetStream.Position = 0;
            targetStream.Write(StartHeader.MagicNumbers.Reverse().ToArray(), 0, StartHeader.GUID_LENGTH);

            // skip hash field and write other fields first
            targetStream.Position = StartHeader.GUID_LENGTH + StartHeader.MD5_LENGTH;

            var id = serializationParameters.ContainerId.ToByteArray();
            id.ToBigEndian();
            targetStream.Write(id, 0, id.Length);

            var part = BitConverter.GetBytes(partNumber);
            part.ToBigEndian();
            targetStream.Write(part, 0, part.Length);

            var parts = BitConverter.GetBytes(serializationParameters.PartitioningScheme.NumberOfParts);
            parts.ToBigEndian();
            targetStream.Write(parts, 0, parts.Length);

            var nextHeaderLength = partNumber == 0
                                       ? BitConverter.GetBytes(serializationParameters.ContentHeaderStream.Length)
                                       : BitConverter.GetBytes(0L);
            nextHeaderLength.ToBigEndian();
            targetStream.Write(nextHeaderLength, 0, nextHeaderLength.Length);

            // compute hash of whole file (excluding the magic number)
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                // set the cursor right to the index after the hash field in order to include all subsequent fields into calculation
                targetStream.Position = StartHeader.GUID_LENGTH + StartHeader.MD5_LENGTH;
                hash = md5.ComputeHash(targetStream);
                hash.ToBigEndian();
            }

            // move cursor back to the index of the hash field and save the hashvalue
            targetStream.Position = StartHeader.GUID_LENGTH;
            targetStream.Write(hash, 0, hash.Length);
        }
    }
}