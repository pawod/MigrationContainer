using System;
using System.IO;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Serialization.Partitioning;

namespace Pawod.MigrationContainer.Serialization.Parameters
{
    public interface ISerializationParameters<THeader, TSource, out TExport>
        where THeader : IFileHeader
        where TSource : IFile
        where TExport : IFile
    {
        /// <summary>
        ///     The sum of all header lengths.
        /// </summary>
        long AllHeadersLength { get; set; }

        /// <summary>
        ///     The streams of the serialized application headers.
        /// </summary>
        MemoryStream[] AppHeaderStreams { get; set; }

        /// <summary>
        ///     The container's ID, which identifies all related container files of a
        ///     parted MigrationContainer.
        /// </summary>
        Guid ContainerId { get; set; }

        /// <summary>
        ///     The header, that describes the container's content.
        /// </summary>
        THeader ContentHeader { get; set; }

        /// <summary>
        ///     The stream of the serialized FileHeader.
        /// </summary>
        MemoryStream ContentHeaderStream { get; set; }

        /// <summary>
        ///     The file extension for the container format.
        /// </summary>
        string FormatExtension { get; set; }

        /// <summary>
        ///     The maximum size of a container file.
        /// </summary>
        long MaxContainerFileSize { get; set; }

        /// <summary>
        ///     A scheme, which describes how to split the container's content into several
        ///     container files.
        /// </summary>
        IPartitioningScheme PartitioningScheme { get; set; }

        /// <summary>
        ///     The file/directory to be exported.
        /// </summary>
        TSource Source { get; set; }

        /// <summary>
        ///     The target directory for export.
        /// </summary>
        IDirectory TargetDir { get; set; }

        /// <summary>
        ///     Gets the target file for the serialization of a container part.
        /// </summary>
        /// <param name="partNumber">The number of the part.</param>
        /// <returns>The desired target file.</returns>
        TExport GetTargetFile(int partNumber);
    }
}