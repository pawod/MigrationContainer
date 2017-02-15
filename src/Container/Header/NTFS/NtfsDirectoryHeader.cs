using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Pawod.MigrationContainer.Container.Enumerator.Header;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Extensions;
using Pawod.MigrationContainer.Filesystem.NTFS;
using ProtoBuf;

namespace Pawod.MigrationContainer.Container.Header.NTFS
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class NtfsDirectoryHeader : DirectoryHeader<NtfsDirectoryHeader, NtfsFileHeader, NtfsFile, NtfsDirectory>,
                                       INtfsDirectoryHeader<NtfsDirectoryHeader, NtfsFileHeader>
    {
        public IList<JunctionHeader> Junctions { get; set; } = new List<JunctionHeader>();

        public NtfsDirectoryHeader(NtfsDirectory directory, long contentOffset, long nextHeaderLength, string parentPath = null)
            : base(directory, contentOffset, nextHeaderLength, parentPath)
        {
            AttributeFlags = (int) directory.AttributesFlag;
            Owner = directory.GetAccessControl().GetOwner(typeof(SecurityIdentifier)).ToString();

            if (directory.IsReparsePoint) HandleReparsePoint(directory);
            foreach (var subDir in directory.GetDirectories())
            {
                if (subDir.IsReparsePoint) HandleReparsePoint(subDir);
            }

            foreach (var stream in directory.AlternateStreams)
            {
                // alternate streams of directories are placed before the actual content
                AlternateStreamHeaders.Add(new AlternateStreamHeader(stream.StreamName, stream.Size, ContentOffset + AlternateContentLength));
                AlternateContentLength += stream.Size;
            }
        }

        protected NtfsDirectoryHeader()
        {
        }

        public long AlternateContentLength { get; }

        public IList<AlternateStreamHeader> AlternateStreamHeaders { get; set; } = new List<AlternateStreamHeader>();
        public int AttributeFlags { get; set; }

        public override IDirectoryHeaderEnumerator<NtfsDirectoryHeader, NtfsFileHeader> GetEnumerator()
        {
            return new DirectoryHeaderEnumerator<NtfsDirHeaderTreeNode, NtfsDirectoryHeader, NtfsFileHeader>(this);
        }

        public override IDirectoryHeaderEnumerator<NtfsDirectoryHeader, NtfsFileHeader> GetEnumerator(IList<string> filter)
        {
            return new DirectoryHeaderEnumerator<NtfsDirHeaderTreeNode, NtfsDirectoryHeader, NtfsFileHeader>(this, filter);
        }

        public string Owner { get; set; }

        protected override NtfsFileHeader CreateFileHeader(NtfsFile file, ref long contentOffset)
        {
            var fileHeader = base.CreateFileHeader(file, ref contentOffset);
            contentOffset += fileHeader.AlternateContentLength;
            return fileHeader;
        }

        protected override NtfsDirectoryHeader CreateSubDirHeader(NtfsDirectory directory, ref long contentOffset, IList<string> nameFilter = null)
        {
            // NextHeaderLength is not relevant to nested ContentHeaders
            var dirHeader = new NtfsDirectoryHeader(directory, contentOffset, 0, RelativePath);
            // consider offset caused by potentially existing ADS before adding further content
            contentOffset += dirHeader.AlternateContentLength;
            dirHeader.MapDirectoryContentRecursive(directory, nameFilter, ref contentOffset);
            return dirHeader;
        }

        protected override bool MatchesMappingConstraints(NtfsDirectory directory, IList<string> nameFilter)
        {
            return base.MatchesMappingConstraints(directory, nameFilter) && !directory.AttributesFlag.HasFlag(FileAttributes.ReparsePoint);
        }

        private void HandleReparsePoint(INtfsDirectory directory)
        {
            if (!directory.IsJunctionPoint) return;
            var originalTarget = directory.GetJunctionTarget();
            var exportRootParent = Regex.Split(directory.FullPath, Regex.Escape(RelativePath)).First();

            var result = Regex.Split(originalTarget, Regex.Escape(exportRootParent));
            var isRelativeTarget = result.Count() > 1;

            var junctionHeader = new JunctionHeader
                                 {
                                     Name = directory.Name,
                                     Target = result.Last(),
                                     IsRelativeTarget = isRelativeTarget,
                                     TimeCreatedUtc = directory.DateCreatedUtc.UtcDateTimeToUnixTimeStamp(),
                                     TimeModifiedUtc = directory.DateLastModifiedUtc.UtcDateTimeToUnixTimeStamp()
                                 };
            Junctions.Add(junctionHeader);
        }
    }
}