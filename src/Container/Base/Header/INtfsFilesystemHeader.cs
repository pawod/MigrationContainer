namespace DataMigrator.Container.Base.Header
{
    using System.Collections.Generic;
    using System.IO;
    using FileContainer.Header;

    public interface INtfsFilesystemHeader<in T> : IFilesystemHeader<T>
        where T : FileSystemInfo
    {
        IList<AlternateStreamHeader> AlternateStreams { get; }

        /// <summary>
        ///     The content's attributes flag.
        /// </summary>
        int AttributeFlags { get; }

        /// <summary>|
        ///     The NT account name of the content's owner.
        /// </summary>
        string Owner { get; }

        /// <summary>
        ///     The content's length including appended alternate streams
        /// </summary>
        long TotalLength { get; }
    }
}
