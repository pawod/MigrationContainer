namespace DataMigrator.Serialization.Base
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using Configuration;
    using Container.Base.Header;
    using Helper;
    using Parameters.Base;
    using Partitioning.Base;

    public class ContainerSerializer<TContentHeader, TFsInfo> : IContainerSerializer<TContentHeader, TFsInfo>
        where TContentHeader : IFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo

    {
        private Stream _sourceStream;
        private string _streamName;
	    private int _contentBufferSize;

        public ContainerSerializer(int contentBufferSize)
        {
	        _contentBufferSize = contentBufferSize;
	        _streamName = string.Empty;
        }

	    public void Dispose()
        {
            if (_sourceStream != null) _sourceStream.Dispose();
        }

        public FileInfo Serialize(ISerializationParameters<TContentHeader, TFsInfo> parameters, int part)
        {
            var targetFile = parameters.GetTargetFile(part);
            using (var targetStream = targetFile.Open(FileMode.Create))
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

        protected virtual Stream OpenSourceStream(string streamName)
        {
            return new FileInfo(streamName).OpenRead();
        }

        private void PrepareSourceStream(IPartitionInfo partitionInfo)
        {
            if (!_streamName.Equals(partitionInfo.Name, StringComparison.Ordinal))
            {
                if (_sourceStream != null) _sourceStream.Dispose();
                _streamName = partitionInfo.Name;
                _sourceStream = OpenSourceStream(_streamName);
            }
            _sourceStream.Position = partitionInfo.StartPosition;
        }

        private static void WriteApplicationHeaders(ISerializationParameters<TContentHeader, TFsInfo> parameters,
                                                    Stream targetStream)
        {
            foreach (var appHeader in parameters.AppHeaderStreams)
            {
                appHeader.Position = 0;
                appHeader.CopyTo(targetStream, appHeader.Length, ProtoConfiguration.Instance.BufferSize);
            }
        }

        private void WriteContent(ISerializationParameters<TContentHeader, TFsInfo> parameters,
                                  Stream targetStream,
                                  int partNumber)
        {
            if (parameters.PartitioningScheme.NumberOfParts == 0
                || (partNumber == 0 && parameters.PartitioningScheme.MainPartHasOnlyHeaders())) return;
            foreach (var partitionInfo in parameters.PartitioningScheme.GetStreamInfo(partNumber))
            {
                PrepareSourceStream(partitionInfo);
                _sourceStream.CopyTo(targetStream,
                    partitionInfo.Length,
					_contentBufferSize);
            }
        }

        private static void WriteContentHeader(ISerializationParameters<TContentHeader, TFsInfo> parameters,
                                               Stream targetStream)
        {
            parameters.ContentHeaderStream.CopyTo(targetStream,
                0,
                StartHeader.Length,
                parameters.ContentHeaderStream.Length,
				ProtoConfiguration.Instance.BufferSize);
        }

        private void WriteStartHeader(ISerializationParameters<TContentHeader, TFsInfo> serializationParameters,
                                      Stream targetStream,
                                      int partNumber)
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

            var parts = BitConverter.GetBytes(serializationParameters.RequiredFiles);
            parts.ToBigEndian();
            targetStream.Write(parts, 0, parts.Length);

            var nextHeaderLength = (partNumber == 0)
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
