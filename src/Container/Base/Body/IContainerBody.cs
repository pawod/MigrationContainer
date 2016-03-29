namespace DataMigrator.Container.Base.Body
{
    using System.IO;
    using Header;
    using Stream;

    public interface IContainerBody
    {
        /// <summary>
        ///     Extracts the content associated with a ContentHeader from the
        ///     MigrationContainer's body.
        /// </summary>
        /// <param name="contentHeader">
        ///     The ContentHeader, which describes the content to be
        ///     extracted.
        /// </param>
        /// <param name="targetStream">The target stream for the content.</param>
        void Extract(IContentHeader contentHeader, Stream targetStream);

        /// <summary>
        ///     Extracts a part of the content associated with a ContentHeader from the
        ///     MigrationContainer's body.
        /// </summary>
        /// <param name="contentHeader">
        ///     The ContentHeader, which describes the content to be
        ///     extracted.
        /// </param>
        /// <param name="targetStream">The target stream for the content.</param>
        /// <param name="startOffset">The relative offset from the content's start index.</param>
        /// <param name="count">The number of bytes to be extracted.</param>
        void Extract(IContentHeader contentHeader, Stream targetStream, long startOffset, long count);

        /// <summary>
        ///     Gets a composite stream of the content associated with a ContentHeader.
        /// </summary>
        /// <param name="contentHeader">
        ///     The ContentHeader, which describes the content to be extracted.
        /// </param>
        /// <returns>A stream, which contains the full content.</returns>
        CompositeStream GetCompositeStream(IContentHeader contentHeader);
    }
}
