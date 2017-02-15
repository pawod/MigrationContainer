using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Alphaleonis.Win32.Filesystem;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Filesystem.NTFS
{
    public interface INtfsFile : IFile
    {
        /// <summary>
        ///     The sum of all lengths of all alternate data streams appended to this file.
        /// </summary>
        long AlternateDataLength { get; }

        /// <summary>
        ///     Gets all alternate data streams appended to this file.
        /// </summary>
        IEnumerable<AlternateDataStreamInfo> AlternateStreams { get; }

        /// <summary>
        ///     Gets the NTFS Attributes Flag.
        /// </summary>
        FileAttributes AttributesFlag { get; }

        /// <summary>
        ///     Gets the access control information for this file.
        /// </summary>
        /// <returns>A FileSecurity instance.</returns>
        FileSecurity GetAccessControl();

        /// <summary>
        ///     Opens an alternate data stream appended to this file.
        /// </summary>
        /// <param name="streamName">The name of the stream, including the typical seperator character as prefix (':')</param>
        /// <param name="fileAccess">Specifies the type of access to the stream.</param>
        /// <param name="fileMode">Specifies the type of stream to be opened.</param>
        /// <param name="fileShare">Specifies the type of file share.</param>
        /// <returns>A FileStream to the alternate data stream.</returns>
        FileStream OpenAlternateStream(string streamName, FileAccess fileAccess, FileMode fileMode, FileShare fileShare);
    }
}