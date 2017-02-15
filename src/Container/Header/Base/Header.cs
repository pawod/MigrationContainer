using ProtoBuf;

namespace Pawod.MigrationContainer.Container.Header.Base
{
    // ReSharper disable EmptyConstructor

    /// <summary>
    ///     The base class for all MigrationContainer headers.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(2, typeof(ProtoHeader))]
    public abstract class Header : IHeader
    {
        protected Header()
        {
        }

        public bool IsLastHeader()
        {
            return NextHeaderLength == 0;
        }

        public long NextHeaderLength { get; protected set; }
    }
}