namespace DataMigrator.Container.Base.Header
{
    // ReSharper disable EmptyConstructor
    using ProtoBuf;

    /// <summary>
    ///     The base class for all MigrationContainer headers.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(2, typeof(ProtoHeader))]
    public abstract class Header : IHeader
    {
        public long NextHeaderLength { get; protected set; }

        protected Header()
        {
        }

        public bool IsLastHeader()
        {
            return NextHeaderLength == 0;
        }
    }
}
