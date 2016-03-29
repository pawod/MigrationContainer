namespace DataMigrator.Serialization.Parameters.FileContainer
{
    using System.Collections.Generic;
    using System.IO;
    using Container.FileContainer;
    using Container.FileContainer.Header;
    using Partitioning.Base;

    public class FileContainerSerializationParameters :
        FileContainerSerializationParametersBase<FileContainerInfo, FileHeader>
    {
        public FileContainerSerializationParameters(FileInfo sourceInfo,
                                                    DirectoryInfo targetDir,
                                                    IPartitioningScheme partitioningScheme,
                                                    FileHeader fileHeader,
                                                    long allHeadersLength,
                                                    long maxFileSize,
                                                    string formatExtension,
                                                    IList<MemoryStream> appHeaderStreams = null)
            : base(
                sourceInfo,
                targetDir,
                partitioningScheme,
                fileHeader,
                allHeadersLength,
                maxFileSize,
                formatExtension,
                appHeaderStreams)
        {
        }
    }
}
