using System.IO;

namespace Pawod.MigrationContainer.Container.Header.Base
{
    public interface IProtoHeader : IHeader
    {
        /// <summary>
        ///     Serializes this header to a protobuf message.
        /// </summary>
        /// <returns>
        ///     A MemoryStream containing the serialized protobuf message of this header.
        /// </returns>
        MemoryStream Serialize();
    }
}