using System.IO;
using Pawod.MigrationContainer.Container.Header.Base;

namespace Pawod.MigrationContainer.Container.Body
{
    public interface IContainerBody
    {
        /// <summary>
        ///     Extracts the content associated with a FileHeader from the
        ///     MigrationContainer's body.
        /// </summary>
        /// <param name="fileHeader">
        ///     The FileHeader, which describes the content to be
        ///     extracted.
        /// </param>
        /// <param name="targetStream">The target stream for the content.</param>
        void Extract(IFileHeader fileHeader, Stream targetStream);

        /// <summary>
        ///     Extracts a part of the content associated with a FileHeader from the
        ///     MigrationContainer's body.
        /// </summary>
        /// <param name="fileHeader">
        ///     The FileHeader, which describes the content to be
        ///     extracted.
        /// </param>
        /// <param name="targetStream">The target stream for the content.</param>
        /// <param name="startOffset">The relative offset from the content's start index.</param>
        /// <param name="count">The number of bytes to be extracted.</param>
        void Extract(IFileHeader fileHeader, Stream targetStream, long startOffset, long count);

        /// <summary>
        ///     Gets a composite stream of the content associated with a FileHeader.
        /// </summary>
        /// <param name="fileHeader">
        ///     The FileHeader, which describes the content to be extracted.
        /// </param>
        /// <returns>A stream, which contains the full content.</returns>
        CompositeStream GetCompositeStream(IFileHeader fileHeader);
    }
}