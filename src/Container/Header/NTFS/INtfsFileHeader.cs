using System.Collections.Generic;
using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Container.Header.NTFS
{
    public interface INtfsFileHeader : IFileHeader
    {
        /// <summary>
        ///     Gets the sum of all length of all appended Alternate Data Streams.
        /// </summary>
        long AlternateContentLength { get; }

        /// <summary>
        ///     The headers of the appended alternate data streams.
        /// </summary>
        IList<AlternateStreamHeader> AlternateStreamHeaders { get; set; }

        /// <summary>
        ///     The content's attributes flag.
        /// </summary>
        int AttributeFlags { get; set; }

        /// <summary>
        ///     The NT account name of the content's owner.
        /// </summary>
        string Owner { get; set; }


        /// <summary>
        ///     The time UNIX timestamp this file has been created (UTC).
        /// </summary>
        long TimeCreatedUtc { get; set; }

        /// <summary>
        ///     The time UNIX timestamp this file has last modified (UTC).
        /// </summary>
        long TimeModifiedUtc { get; set; }
    }
}