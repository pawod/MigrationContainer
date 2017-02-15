using System;
using System.Collections.ObjectModel;

namespace Pawod.MigrationContainer.Container.Header.Base
{
    public interface IStartHeader : IHeader
    {
        /// <summary>
        ///     The container's ID.
        /// </summary>
        Guid ContainerId { get; }

        /// <summary>
        ///     The MD5 hash of the container file.
        /// </summary>
        ReadOnlyCollection<byte> Md5Hash { get; }

        /// <summary>
        ///     The human readable string representation of the MD5 hash.
        /// </summary>
        string Md5String { get; }

        /// <summary>
        ///     The part number of the current container file.
        /// </summary>
        int PartNumber { get; }

        /// <summary>
        ///     The number of files in which the container is parted.
        /// </summary>
        int Parts { get; }
    }
}