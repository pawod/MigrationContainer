namespace DataMigrator.Container.Base.Header
{
    using System.Collections.Generic;
    using System.IO;

    public interface IFilesystemHeader<in T> : IContentHeader
        where T : FileSystemInfo
    {
        /// <summary>
        ///     The content's timestamp.
        /// </summary>
        IDictionary<string, long> TimeStamps { get; }

	    /// <summary>
	    ///     Associates the header with the specified content. The result is a header,
	    ///     which fully describes the content's properties, meta data and location
	    ///     within the container.
	    /// </summary>
	    /// <param name="fsInfo">The file/directory associate the header with.</param>
	    /// <param name="filter">A list of all subdirectories or files to be excluded.</param>
	    void AssociateWith(T fsInfo, IList<string> filter = null);
    }
}
