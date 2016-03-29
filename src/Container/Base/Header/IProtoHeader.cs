namespace DataMigrator.Container.Base.Header
{
    using System.IO;

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
