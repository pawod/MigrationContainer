namespace DataMigrator.Container.Base.Header
{
    public interface IContentHeader : IProtoHeader
    {
        /// <summary>
        ///     The content's length in bytes.
        /// </summary>
        long ContentLength { get; }

        /// <summary>
        ///     The offset from the container's body starting position (excluding
        ///     partitioning overhead).
        /// </summary>
        long ContentOffset { get; }

        /// <summary>
        ///     The content's original name.
        /// </summary>
        string OriginalName { get; set; }
    }
}
