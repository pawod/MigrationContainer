using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Filesystem.NTFS
{
    public interface INtfsDirectory : IDirectory, INtfsFile
    {
        /// <summary>
        ///     Determines whether this directory is a junction point.
        /// </summary>
        bool IsJunctionPoint { get; }

        /// <summary>
        ///     Determines whether this directory is a reparse point.
        /// </summary>
        bool IsReparsePoint { get; }

        /// <summary>
        ///     Gets the access control information for this directory.
        /// </summary>
        /// <returns>A DirectorySecurity instance.</returns>
        new DirectorySecurity GetAccessControl();

        /// <summary>
        ///     Gets all subdirs which are junction points.
        /// </summary>
        /// <param name="searchOption">The searchoption to be applied.</param>
        /// <returns>A list of JunctionPoints.</returns>
        IList<JunctionPoint> GetJunctionPoints(SearchOption searchOption = SearchOption.TopDirectoryOnly);

        /// <summary>
        ///     Gets the targeted directory if this directory is a junction point.
        /// </summary>
        /// <returns>A path to the targeted directory.</returns>
        string GetJunctionTarget();

        /// <summary>
        ///     Calculates the length of this directory, but ignores files contained in the filter.
        /// </summary>
        /// <param name="filter">The names of files or directories to be ignored.</param>
        /// <param name="includeAlternateDataStreams">Wheter to consider alternate data streams in calculation.</param>
        /// <returns>The size of this directory in bytes.</returns>
        long GetLength(IList<string> filter, bool includeAlternateDataStreams = false);
    }
}