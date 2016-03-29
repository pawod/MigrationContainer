namespace DataMigrator.Serialization.Parameters.DirectoryContainer
{
    using System.Collections.Generic;
    using System.IO;
    using Container.DirectoryContainer;
    using Container.DirectoryContainer.Header;
    using Container.FileContainer.Header;
    using Partitioning.Base;

    public class DirectoryContainerSerializationParameters :
        DirectoryContainerSerializationParametersBase<DirectoryContainerInfo, DirectoryHeader, FileHeader>
    {
        public DirectoryContainerSerializationParameters(DirectoryInfo sourceInfo,
                                                         DirectoryInfo targetDir,
                                                         IPartitioningScheme partitioningScheme,
                                                         DirectoryHeader directoryHeader,
                                                         long allHeadersLength,
                                                         long maxFileSize,
                                                         string formatExtension,
                                                         IList<MemoryStream> appHeaderStreams = null)
            : base(
                sourceInfo,
                targetDir,
                partitioningScheme,
                directoryHeader,
                allHeadersLength,
                maxFileSize,
                formatExtension,
                appHeaderStreams)
        {
        }
    }
}
