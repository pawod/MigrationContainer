namespace DataMigratorTest.Validation.Base
{
	using System.IO;
	using System.Linq;
	using CodeFluent.Runtime.BinaryServices;
	using Configuration;
	using DataMigrator.Container.Base.Header;
	using DataMigrator.FileSystem;
	using DataMigrator.Helper;
	using FluentAssertions;

	public class NtfsContainerValidator
	{
		public void ValiateAttributeFlags<TFsInfo>(FileSystemInfo sourceInfo, INtfsFilesystemHeader<TFsInfo> contentHeader)
			where TFsInfo : FileSystemInfo
		{
			contentHeader.AttributeFlags.Should().Be((int)sourceInfo.Attributes);
		}

		public void ValidateAlternateStreams(FileSystemInfo imported, FileSystemInfo sourceInfo)
		{
			var alternateSourceStreams = sourceInfo.GetAlternateStreams().ToList();
			var alternateImportStreams = imported.GetAlternateStreams().ToList();

			alternateImportStreams.Count().Should().Be(alternateSourceStreams.Count());
			for (var i = 0; i < alternateImportStreams.Count; i++)
			{
				alternateImportStreams[i].Name.Should().Be(alternateSourceStreams[i].Name);
				alternateImportStreams[i].Size.Should().Be(alternateSourceStreams[i].Size);
				alternateImportStreams[i].StreamType.Should().Be(alternateSourceStreams[i].StreamType);

				using (
					var importedStream = NtfsAlternateStream.Open(imported.FullName + alternateImportStreams[i].Name,
						FileAccess.Read,
						FileMode.Open,
						FileShare.None))
				using (
					var sourceStream = NtfsAlternateStream.Open(sourceInfo.FullName + alternateSourceStreams[i].Name,
						FileAccess.Read,
						FileMode.Open,
						FileShare.None)) importedStream.StreamEquals(sourceStream).Should().BeTrue();
			}
		}

		public void ValidateJunctions(DirectoryInfo imported, DirectoryInfo sourceDir)
		{
			var importJunctions = JunctionPoint.FindJunctions(imported, SearchOption.AllDirectories);
			var sourceJunctions = JunctionPoint.FindJunctions(sourceDir, SearchOption.AllDirectories);

			importJunctions.Count.Should().Be(sourceJunctions.Count);

			for (var i = 0; i < importJunctions.Count; i++)
			{
				var sourceJunction = sourceJunctions[i];
				var importedJunction = importJunctions[i];

				var relativeSourceLink = sourceJunction.Link.FullName.Replace(TestConfiguration.Instance.SourceDirs.FullName,
					string.Empty);
				var relativeImportLink = importedJunction.Link.FullName.Replace(TestConfiguration.Instance.ImportTarget.FullName,
					string.Empty);
				relativeImportLink.Should().Be(relativeSourceLink);

				var relativeSourceTarget = sourceJunction.Target.FullName.Replace(TestConfiguration.Instance.SourceDirs.FullName,
					string.Empty);
				var relativeImportTarget = importedJunction.Target.FullName.Replace(
																				    TestConfiguration.Instance.ImportTarget.FullName,
					string.Empty);
				relativeImportTarget.Should().Be(relativeSourceTarget);

				var sourceLink = new DirectoryInfo(sourceJunction.Link.FullName);
				var importLink = new DirectoryInfo(importedJunction.Link.FullName);

				importLink.CreationTime.Should().Be(sourceLink.CreationTime);
				importLink.CreationTimeUtc.Should().Be(sourceLink.CreationTimeUtc);

				importLink.LastAccessTime.Should().Be(sourceLink.LastAccessTime);
				importLink.LastAccessTimeUtc.Should().Be(sourceLink.LastAccessTimeUtc);

				importLink.LastWriteTime.Should().Be(sourceLink.LastWriteTime);
				importLink.LastWriteTimeUtc.Should().Be(sourceLink.LastWriteTimeUtc);
			}
		}
	}
}
