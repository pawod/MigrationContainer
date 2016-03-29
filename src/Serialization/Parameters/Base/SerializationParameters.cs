namespace DataMigrator.Serialization.Parameters.Base
{
    using Container.Base;
    using Container.Base.Header;
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Partitioning.Base;

    public class SerializationParameters<TContainer, TContentHeader, TFsInfo> :
        ISerializationParameters<TContentHeader, TFsInfo>
        where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
        where TContentHeader : class, IFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo
    {
        public long AllHeadersLength { get; private set; }
        public IList<MemoryStream> AppHeaderStreams { get; private set; }
        public Guid ContainerId { get; private set; }
        public TContentHeader ContentHeader { get; private set; }
        public MemoryStream ContentHeaderStream { get; private set; }
        public string FormatExtension { get; private set; }
        public long MaxFileSize { get; private set; }
        public IPartitioningScheme PartitioningScheme { get; private set; }

        public int RequiredFiles
        {
            get { return PartitioningScheme.NumberOfParts; }
        }

        public TFsInfo SourceInfo { get; private set; }
        public DirectoryInfo TargetDir { get; private set; }

        public SerializationParameters(TFsInfo sourceInfo,
                                       DirectoryInfo targetDir,
                                       IPartitioningScheme partitioningScheme,
                                       TContentHeader contentHeader,
                                       long allHeadersLength,
                                       long maxFileSize,
                                       string formatExtension,
                                       IList<MemoryStream> appHeaderStreams = null)
        {
            ContainerId = Guid.NewGuid();
            TargetDir = targetDir;
            SourceInfo = sourceInfo;

            AppHeaderStreams = appHeaderStreams;

            ContentHeader = contentHeader;
            ContentHeaderStream = ContentHeader.Serialize();

            AllHeadersLength = allHeadersLength;
            MaxFileSize = maxFileSize;
            FormatExtension = formatExtension;

            PartitioningScheme = partitioningScheme;
        }

        public FileInfo GetTargetFile(int partNumber)
        {
            var extension = GetFileExtension(partNumber, PartitioningScheme.NumberOfParts);
            return
                new FileInfo(string.Format("{0}{1}{2}{3}",
                    TargetDir.FullName,
                    Path.DirectorySeparatorChar,
                    SourceInfo.Name,
                    extension));
        }

        private string GetFileExtension(int partNumber, int numberOfFiles)
        {
            var extension = FormatExtension;
            if (partNumber != 0 && numberOfFiles > 1)
            {
                var maxDigits = Math.Ceiling(Math.Log10(numberOfFiles + 1));
                var digits = Math.Ceiling(Math.Log10(partNumber + 1));
                var padding = maxDigits - digits;

                var sb = new StringBuilder();
                for (var i = 0; i < padding; i++)
                {
                    sb.Append("0");
                }

                extension = string.Format("{0}.part{1}{2}", extension, sb, partNumber);
            }
            return extension;
        }
    }
}
