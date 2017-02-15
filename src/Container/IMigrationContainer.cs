using System.Collections.Generic;
using System.IO;
using Pawod.MigrationContainer.Container.Body;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Container
{
    public interface IMigrationContainer
    {
        /// <summary>
        ///     Gets the container's body, which provides access to the stored content.
        /// </summary>
        /// <returns>A disposable ContainerBody instance.</returns>
        IContainerBody Body { get; }

        /// <summary>
        ///     The stream position, where the body of this container starts.
        /// </summary>
        long BodyPosition { get; }

        /// <summary>
        ///     Gets the File representing this container file.
        /// </summary>
        IFile File { get; }

        /// <summary>
        ///     Gets the header for the container's content.
        /// </summary>
        IFileHeader ContentHeader { get; }

        /// <summary>
        ///     Gets the Directory containing this container file.
        /// </summary>
        IDirectory Directory { get; }

        /// <summary>
        ///     Gets the StartHeader of this container.
        /// </summary>
        IStartHeader StartHeader { get; }

        /// <summary>
        ///     Searches for the main part of a parted MigrationContainer.
        /// </summary>
        /// <param name="searchOption">The option to be used when searching.</param>
        /// <param name="ignoreFileExtension">
        ///     If set to false, only files with a matching
        ///     file extension will be considered.
        /// </param>
        /// <returns>The main part if found, else null.</returns>
        IMigrationContainer FindMainPart(SearchOption searchOption = SearchOption.TopDirectoryOnly, bool ignoreFileExtension = false);

        /// <summary>
        ///     Gets all secondary MigrationContainer files, that belong together.
        /// </summary>
        /// <param name="searchOption">The option to be used when searching.</param>
        /// <param name="ignoreFileExtension">
        ///     If set to false, only files with a matching
        ///     file extension will be considered.
        /// </param>
        /// <returns>
        ///     All parts that have the same container ID as the current container, excluding
        ///     the main part.
        /// </returns>
        IEnumerable<IMigrationContainer> FindRelatedParts(SearchOption searchOption = SearchOption.TopDirectoryOnly, bool ignoreFileExtension = false);

        /// <summary>
        ///     Gets all parts of the MigrationContainer including the current part.
        /// </summary>
        /// <param name="searchOption">The option to be used when searching.</param>
        /// <returns>A sorted list of all found container files.</returns>
        IEnumerable<IFile> GetAllParts(SearchOption searchOption = SearchOption.TopDirectoryOnly);

        /// <summary>
        ///     Gets a value indicating whether this file is the last part of a parted
        ///     container.
        /// </summary>
        /// <returns>
        ///     true if this file is the last of n parts; else false.
        /// </returns>
        bool IsLastPart();

        /// <summary>
        ///     Gets a value indicating whether this file is the main part of a parted
        ///     container.
        /// </summary>
        /// <returns>
        ///     true if this file is the first of n parts; else false.
        /// </returns>
        bool IsMainPart();

        /// <summary>
        ///     Gets a value indicating whether this file is only a partition of a full
        ///     migration container.
        /// </summary>
        /// <returns>
        ///     true if this file is just one of several parts, which represent the whole
        ///     container, else false.
        /// </returns>
        bool IsParted();

        /// <summary>
        ///     Validates the migration container against its checksum.
        /// </summary>
        bool IsValid();
    }
}