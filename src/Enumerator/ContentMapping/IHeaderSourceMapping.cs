namespace DataMigrator.Enumerator.ContentMapping
{
    using Container.Base.Header;

    public interface IHeaderSourceMapping<out TContentHeader>
        where TContentHeader : IContentHeader
    {
        TContentHeader ContentHeader { get; }
        string SourceName { get; }
    }
}
