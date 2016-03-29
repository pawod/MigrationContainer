namespace DataMigrator.Container.FileContainer.Header
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Principal;
    using Base.Header;
    using Helper;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class NtfsFileHeader : FileHeader, INtfsFilesystemHeader<FileInfo>
    {
        public IList<AlternateStreamHeader> AlternateStreams { get; private set; }
        public int AttributeFlags { get; private set; }
        public string Owner { get; private set; }
        public long TotalLength { get; private set; }

        public NtfsFileHeader(long contentOffset, long nextHeaderLength) : base(contentOffset, nextHeaderLength)
        {
        }

        protected NtfsFileHeader()
        {
        }

        public override void AssociateWith(FileInfo fileInfo, IList<string> filter = null)
        {
            base.AssociateWith(fileInfo, filter);
            AttributeFlags = (int)fileInfo.Attributes;
            Owner = fileInfo.GetAccessControl().GetOwner(typeof(SecurityIdentifier)).ToString();
            AlternateStreams = new List<AlternateStreamHeader>();
            var offsetAkk = 0L;
            foreach (var stream in fileInfo.GetAlternateStreams())
            {
                AlternateStreams.Add(new AlternateStreamHeader(stream.Name,
                    stream.Size,
                    ContentOffset + ContentLength + offsetAkk));
                offsetAkk += stream.Size;
            }
            TotalLength = ContentLength + offsetAkk;
        }
    }
}
