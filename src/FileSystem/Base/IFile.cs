using System;
using System.Collections.Generic;
using System.IO;

namespace Pawod.MigrationContainer.Filesystem.Base
{
    /// <summary>
    ///     Provides access to a file on a file system.
    /// </summary>
    public interface IFile
    {
        Dictionary<string, object> Attributes { get; }
        DateTime DateCreatedUtc { get; }
        DateTime DateLastModifiedUtc { get; }

        bool Exists { get; }

        string Extension { get; }

        string FullPath { get; }
        long Length { get; }
        string Name { get; }
        IDirectory Parent { get; }

        /// <summary>
        ///     Determines wether the specified file is a migration container.
        /// </summary>
        /// <returns>True if the specified file is considered to be a migration container.</returns>
        bool IsMigrationContainer();

        /// <summary>
        ///     Opens a stream in the specified mode.
        /// </summary>
        /// <param name="mode">The mode to be used to open the stream.</param>
        /// <param name="access">The access rights for the stream.</param>
        /// <returns>A steam opened in the specified mode.</returns>
        Stream Open(FileMode mode, FileAccess access);

        /// <summary>
        ///     Opens a readonly stream.
        /// </summary>
        /// <returns>An opened stream.</returns>
        Stream OpenRead();

        /// <summary>
        ///     Updates the state of this instance.
        /// </summary>
        void Refresh();
    }
}