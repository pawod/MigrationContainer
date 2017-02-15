using Pawod.MigrationContainer.Container.Header.Base;
using ProtoBuf;

namespace Pawod.MigrationContainer.Container.Header.NTFS
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class AlternateStreamHeader : ProtoHeader, IFileHeader
    {
        protected AlternateStreamHeader()
        {
        }

        public AlternateStreamHeader(string originalName, long contentLength, long contentOffset)
        {
            ContentLength = contentLength;
            ContentOffset = contentOffset;
            OriginalName = originalName;
        }

        public long ContentLength { get; protected set; }
        public long ContentOffset { get; protected set; }

        /// <summary>
        ///     The stream's name.
        /// </summary>
        public string OriginalName { get; set; }
    }
}