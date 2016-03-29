namespace DataMigrator.Container.NtfsDirectoryContainer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Principal;
    using System.Text.RegularExpressions;
    using Base.Header;
    using DirectoryContainer.Base;
    using FileContainer.Header;
    using FileSystem;
    using Helper;
    using ProtoBuf;

    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class NtfsDirectoryHeader : DirectoryHeaderBase<NtfsDirectoryHeader, NtfsFileHeader>, INtfsDirectoryHeader
    {
        public IList<AlternateStreamHeader> AlternateStreams { get; private set; }
        public int AttributeFlags { get; private set; }
        public IList<JunctionHeader> Junctions { get; private set; }
        public string Owner { get; private set; }
        public long TotalLength { get; private set; }

        public NtfsDirectoryHeader(long contentOffset, long nextHeaderLength) : base(contentOffset, nextHeaderLength)
        {
            Junctions = new List<JunctionHeader>();
            AlternateStreams = new List<AlternateStreamHeader>();
        }

        protected NtfsDirectoryHeader()
        {
        }

        protected override void HandleCurrentDirectory(DirectoryInfo directoryInfo, ref long offsetAkk)
        {
            AttributeFlags = (int)directoryInfo.Attributes;
            Owner = directoryInfo.GetAccessControl().GetOwner(typeof(SecurityIdentifier)).ToString();
            foreach (var stream in directoryInfo.GetAlternateStreams())
            {
                AlternateStreams.Add(new AlternateStreamHeader(stream.Name, stream.Size, offsetAkk));
                offsetAkk += stream.Size;
                TotalLength += stream.Size;
            }
        }

        protected override void HandleFile(FileInfo fileInfo, ref long offsetAkk, IList<string> filter = null)
        {
            var fileHeader = new NtfsFileHeader(offsetAkk, 0);
            fileHeader.AssociateWith(fileInfo, filter);
            Files.Add(fileHeader);

            ContentLength += fileHeader.ContentLength;
            TotalLength += fileHeader.TotalLength;
            offsetAkk += fileHeader.TotalLength;
        }

        protected override void HandleSubDirectory(DirectoryInfo directoryInfo,
                                                   ref long offsetAkk,
                                                   IList<string> filter = null)
        {
            if (directoryInfo.Attributes.HasFlag(FileAttributes.ReparsePoint)) HandleReparsePoint(directoryInfo);
            else
            {
                var directoryHeader = new NtfsDirectoryHeader(offsetAkk, 0);
                directoryHeader.InitializeRecursivelyFrom(directoryInfo, RelativePath, filter);
                SubDirectories.Add(directoryHeader);

                ContentLength += directoryHeader.ContentLength;
                TotalLength += directoryHeader.TotalLength;
                offsetAkk += directoryHeader.TotalLength;
            }
        }

        private void HandleReparsePoint(DirectoryInfo directoryInfo)
        {
            if (!JunctionPoint.Exists(directoryInfo)) return;
            var originalTarget = JunctionPoint.GetTarget(directoryInfo.FullName);
            var exportRootParent = Regex.Split(directoryInfo.FullName, Regex.Escape(RelativePath)).First();

            var result = Regex.Split(originalTarget, Regex.Escape(exportRootParent));
            var isRelativeTarget = result.Count() > 1;
            var timeStamps = GetTimestamps(directoryInfo);

            Junctions.Add(new JunctionHeader(directoryInfo.Name, result.Last(), isRelativeTarget, timeStamps));
        }
    }
}
