namespace DataMigrator.Serialization.Parameters.FileContainer
{
    using System.Collections.Generic;
    using System.IO;
    using Base;
    using Container.Base;
    using Container.Base.Header;
    using Partitioning.Base;

    public abstract class FileContainerSerializationParametersBase<TContainer, TFileHeader> :
        SerializationParameters<TContainer, TFileHeader, FileInfo>
        where TContainer : MigrationContainerInfo<TContainer, TFileHeader, FileInfo>
        where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
        protected FileContainerSerializationParametersBase(FileInfo sourceInfo,
                                                           DirectoryInfo targetDir,
                                                           IPartitioningScheme partitioningScheme,
                                                           TFileHeader fileHeader,
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
