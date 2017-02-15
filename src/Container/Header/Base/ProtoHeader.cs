using System.IO;
using System.Linq;
using NLog;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Exception;
using Pawod.MigrationContainer.Extensions;
using Pawod.MigrationContainer.Filesystem.Base;
using ProtoBuf;

namespace Pawod.MigrationContainer.Container.Header.Base
{
    // ReSharper disable EmptyConstructor

    /// <summary>
    ///     The base class for all headers, that are serialized using protobuf. All
    ///     inheriting classes must be included to the inheritance hierarchy with a tag
    ///     number > 8.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(3, typeof(FileHeader))]
    [ProtoInclude(4, typeof(JunctionHeader))]
    [ProtoInclude(5, typeof(AlternateStreamHeader))]
    public abstract class ProtoHeader : Header, IProtoHeader
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected ProtoHeader()
        {
        }

        /// <summary>
        ///     Serializes this header to a protobuf message.
        /// </summary>
        /// <returns>
        ///     A MemoryStream containing the serialized protobuf message of this header.
        /// </returns>
        public MemoryStream Serialize()
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, this);
            stream.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        ///     Retreives a ProtoHeader from a serialized MigrationContainer.
        /// </summary>
        /// <typeparam name="T">
        ///     The specific type of the ProtoHeader to be retreived.
        /// </typeparam>
        /// <param name="containerFile">
        ///     The MigrationContainer, that contains the desired ProtoHeader.
        /// </param>
        /// <param name="startHeader">
        ///     The StartHeader of the specified MigrationContainer.
        /// </param>
        /// <param name="bufferSize">The size of the deserialization buffer.</param>
        /// <param name="previousHeaders">
        ///     All preceding ProtoHeaders. The order must match the specification of the
        ///     containing MigrationContainer.
        /// </param>
        /// <returns>The deserialized ProtoHeader of the specified type.</returns>
        public static T Get<T>(IFile containerFile, IStartHeader startHeader, int bufferSize, params ProtoHeader[] previousHeaders) where T : IProtoHeader
        {
            try
            {
                using (var fileStream = containerFile.OpenRead())
                using (var protoStream = new MemoryStream())
                {
                    var sourceOffset = previousHeaders.Any() ? StartHeader.Length + startHeader.NextHeaderLength : StartHeader.Length;
                    var headerLength = previousHeaders.Any() ? previousHeaders.First().NextHeaderLength : startHeader.NextHeaderLength;

                    for (var i = 1; i < previousHeaders.Count(); i++)
                    {
                        // first header is always the AssemnlyHeader, whose offset has already been added
                        sourceOffset += previousHeaders[i].NextHeaderLength;
                    }

                    fileStream.CopyTo(protoStream, sourceOffset, 0, headerLength, bufferSize);

                    protoStream.Position = 0;
                    return Serializer.Deserialize<T>(protoStream);
                }
            }
            catch (System.Exception ex)
            {
                var containerException = new ContainerOperationException("Failed to deserialize header.", ex);
                Logger.Error(containerException);
                throw containerException;
            }
        }
    }
}