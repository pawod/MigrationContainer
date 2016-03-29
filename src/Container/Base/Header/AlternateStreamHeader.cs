namespace DataMigrator.Container.Base.Header
{
	using ProtoBuf;

	// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class AlternateStreamHeader : ProtoHeader, IContentHeader
    {
        public long ContentLength { get; private set; }
        public long ContentOffset { get; private set; }
        public string OriginalName { get; set; }

        protected AlternateStreamHeader()
        {
        }

        public AlternateStreamHeader(string originalName, long contentLength, long contentOffset)
        {
            ContentLength = contentLength;
            ContentOffset = contentOffset;
            OriginalName = originalName;
        }
    }
}
