namespace DataMigrator.Enumerator.ContentMapping
{
    using Container.Base.Header;

    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

    /// <summary>
    ///     Associates a directory or file to its representing ContentHeader.
    /// </summary>
    public class HeaderSourceMapping<TContentHeader> : IHeaderSourceMapping<TContentHeader>
        where TContentHeader : IContentHeader
    {
        public TContentHeader ContentHeader { get; private set; }
        public string SourceName { get; private set; }

        public HeaderSourceMapping(string sourceName, TContentHeader contentHeader)
        {
            ContentHeader = contentHeader;
            SourceName = sourceName;
        }
    }
}
