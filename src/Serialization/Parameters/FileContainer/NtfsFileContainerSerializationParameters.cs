namespace DataMigrator.Serialization.Parameters.FileContainer
{
    using System.Collections.Generic;
    using System.IO;
    using Container.FileContainer;
    using Container.FileContainer.Header;
    using Partitioning.Base;

    public class NtfsFileContainerSerializationParameters :
        FileContainerSerializationParametersBase<NtfsFileContainerInfo, NtfsFileHeader>
    {
        public NtfsFileContainerSerializationParameters(FileInfo sourceInfo,
                                                        DirectoryInfo targetDir,
                                                        IPartitioningScheme partitioningScheme,
                                                        NtfsFileHeader directoryHeader,
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
