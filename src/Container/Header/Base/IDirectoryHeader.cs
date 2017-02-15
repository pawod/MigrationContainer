using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Enumerator.Header;

namespace Pawod.MigrationContainer.Container.Header.Base
{
    public interface IDirectoryHeader<TDirectoryHeader, TFileHeader> : IFileHeader
        where TDirectoryHeader : class, IDirectoryHeader<TDirectoryHeader, TFileHeader>
        where TFileHeader : class, IFileHeader
    {
        /// <summary>
        ///     The file headers contained on the first level of this directory header.
        /// </summary>
        IList<TFileHeader> FileHeaders { get; set; }

        /// <summary>
        ///     The path of this directory header, relative to the root header.
        /// </summary>
        string RelativePath { get; set; }

        /// <summary>
        ///     The directory headers contained on the first level of this directory header.
        /// </summary>
        IList<TDirectoryHeader> SubdirHeaders { get; set; }

        /// <summary>
        ///     Recursively gets all directory headers contained in this directory header.
        /// </summary>
        /// <returns>A list of directory headers.</returns>
        IList<TDirectoryHeader> GetDirectoryHeaders();

        /// <summary>
        ///     Gets an enumerator, which allows to iterate over all directory- and file headers within this directory header.
        /// </summary>
        /// <returns>An IDirectoryHeaderEnumerator instance.</returns>
        IDirectoryHeaderEnumerator<TDirectoryHeader, TFileHeader> GetEnumerator();

        /// <summary>
        ///     Gets an enumerator, which allows to iterate over all directory- and file headers within this directory header.
        /// </summary>
        /// <param name="filter">The names of directories or files to be skipped during iteration.</param>
        /// <returns>An IDirectoryHeaderEnumerator instance.</returns>
        IDirectoryHeaderEnumerator<TDirectoryHeader, TFileHeader> GetEnumerator(IList<string> filter);

        /// <summary>
        ///     Recursively gets all file headers contained in this directory header.
        /// </summary>
        /// <returns>A list of file headers.</returns>
        IList<TFileHeader> GetFileHeaders();
    }
}