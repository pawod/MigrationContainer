namespace DataMigrator.Serialization.Parameters.DirectoryContainer
{
    using System.Collections.Generic;
    using System.IO;
    using Base;
    using Container.Base;
    using Container.DirectoryContainer.Base;
    using Container.FileContainer.Header;
    using Partitioning.Base;

    public abstract class DirectoryContainerSerializationParametersBase<TContainer, TDirectoryHeader, TFileHeader> :
        SerializationParameters<TContainer, TDirectoryHeader, DirectoryInfo>
        where TContainer : MigrationContainerInfo<TContainer, TDirectoryHeader, DirectoryInfo>
        where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : FileHeader
    {
        protected DirectoryContainerSerializationParametersBase(DirectoryInfo sourceInfo,
                                                                DirectoryInfo targetDir,
                                                                IPartitioningScheme partitioningScheme,
                                                                TDirectoryHeader directoryHeader,
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
