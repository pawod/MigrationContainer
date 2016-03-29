namespace DataMigrator.Serialization.Parameters.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Container.Base.Header;
    using Partitioning.Base;

    public interface ISerializationParameters<out TFilesystemHeader, out TFsInfo>
        where TFilesystemHeader : IFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo
    {
        /// <summary>
        ///     The sum of all header lengths.
        /// </summary>
        long AllHeadersLength { get; }

        /// <summary>
        ///     The streams of the serialized application headers.
        /// </summary>
        IList<MemoryStream> AppHeaderStreams { get; }

        /// <summary>
        ///     The container's ID, which identifies all related container files of a
        ///     parted MigrationContainer.
        /// </summary>
        Guid ContainerId { get; }

        /// <summary>
        ///     The header, that describes the container's content.
        /// </summary>
        TFilesystemHeader ContentHeader { get; }

        /// <summary>
        ///     The stream of the serialized ContentHeader.
        /// </summary>
        MemoryStream ContentHeaderStream { get; }

        /// <summary>
        ///     The maximum size of a container file.
        /// </summary>
        long MaxFileSize { get; }

        /// <summary>
        ///     A scheme, which describes how to split the container's content into several
        ///     container files.
        /// </summary>
        IPartitioningScheme PartitioningScheme { get; }

        /// <summary>
        ///     The number of container files required to serialize the container.
        /// </summary>
        int RequiredFiles { get; }

        /// <summary>
        ///     The file/directory to be exported.
        /// </summary>
        TFsInfo SourceInfo { get; }

        /// <summary>
        ///     The target directory for export.
        /// </summary>
        DirectoryInfo TargetDir { get; }

        /// <summary>
        ///     Gets the target file for the serialization of a container part.
        /// </summary>
        /// <param name="partNumber">The number of the part.</param>
        /// <returns>The FileInfo of the desired target file.</returns>
        FileInfo GetTargetFile(int partNumber);
    }
}
