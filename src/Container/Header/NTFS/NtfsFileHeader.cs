using System.Collections.Generic;
using System.Security.Principal;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.NTFS;
using ProtoBuf;

namespace Pawod.MigrationContainer.Container.Header.NTFS
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class NtfsFileHeader : FileHeader, INtfsFileHeader
    {
        protected NtfsFileHeader()
        {
        }

        public NtfsFileHeader(INtfsFile file, long contentOffset, long nextHeaderLength) : base(file, contentOffset, nextHeaderLength)
        {
            AttributeFlags = (int) file.AttributesFlag;
            Owner = file.GetAccessControl().GetOwner(typeof(SecurityIdentifier)).ToString();

            var adsOffset = file.Length + ContentOffset;
            foreach (var stream in file.AlternateStreams)
            {
                AlternateStreamHeaders.Add(new AlternateStreamHeader(stream.StreamName, stream.Size, adsOffset + AlternateContentLength));
                AlternateContentLength += stream.Size;
            }
        }

        public long AlternateContentLength { get; }
        public IList<AlternateStreamHeader> AlternateStreamHeaders { get; set; } = new List<AlternateStreamHeader>();
        public int AttributeFlags { get; set; }
        public string Owner { get; set; }
    }
}