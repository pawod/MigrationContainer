using System.IO;
using System.Linq;
using FluentAssertions;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Header.NTFS;
using Pawod.MigrationContainer.Extensions;
using Pawod.MigrationContainer.Filesystem.NTFS;

namespace Pawod.MigrationContainer.Test.Validation
{
    public abstract class NtfsContainerValidator<TContainer, TSource> : ContainerValidator<TContainer, TSource>
        where TContainer : IMigrationContainer
        where TSource : INtfsFile
    {
        public static void ValidateAlternateStreams(INtfsFile imported, INtfsFile source)
        {
            var alternateSourceStreams = source.AlternateStreams.ToList();
            var alternateImportStreams = imported.AlternateStreams.ToList();

            alternateImportStreams.Count().Should().Be(alternateSourceStreams.Count());
            for (var i = 0; i < alternateImportStreams.Count; i++)
            {
                alternateImportStreams[i].StreamName.Should().Be(alternateSourceStreams[i].StreamName);
                Path.GetFileName(alternateImportStreams[i].FullPath).Should().Be(Path.GetFileName(alternateSourceStreams[i].FullPath));
                alternateImportStreams[i].Size.Should().Be(alternateSourceStreams[i].Size);

                using (
                    var importedStream = imported.OpenAlternateStream(alternateImportStreams[i].StreamName,
                                                                      FileAccess.Read,
                                                                      FileMode.Open,
                                                                      FileShare.Read))
                using (
                    var sourceStream = source.OpenAlternateStream(alternateSourceStreams[i].StreamName, FileAccess.Read, FileMode.Open, FileShare.Read)
                ) {
                    importedStream.StreamEquals(sourceStream).Should().BeTrue();
                }
            }
        }

        public override void ValidateContentHeader(TContainer container, TSource source)
        {
            container.Should().NotBeNull();

            container.ContentHeader.ContentLength.Should().Be(source.Length);
            container.ContentHeader.NextHeaderLength.Should().Be(0);
            container.ContentHeader.OriginalName.Should().Be(source.Name);
            ((INtfsFileHeader) container.ContentHeader).AttributeFlags.Should().Be((int) source.AttributesFlag);
        }

        public override void ValidateMigrationContainer(TContainer container, TSource source)
        {
            container.File.IsMigrationContainer().Should().BeTrue();
            container.IsValid().Should().BeTrue();

            if (container.IsMainPart()) container.StartHeader.PartNumber.Should().Be(0);
            else container.StartHeader.PartNumber.Should().BeGreaterThan(0);
            if (container.IsParted()) container.StartHeader.Parts.Should().BeGreaterThan(1);
            else container.StartHeader.Parts.Should().Be(1);
            if (container.IsMainPart())
            {
                var formatExtension = MigrationContainerExtensions.GetFileExtension<TContainer>();
                container.File.Extension.Should().Be(formatExtension);
                var pattern = source.Name + formatExtension;
                container.File.Name.Should().Be(pattern);
            }
            else container.File.Extension.Should().Contain(".part");
        }

        protected void ValidateFile(INtfsFile imported, INtfsFile source)
        {
            imported.Exists.Should().BeTrue();
            imported.IsMigrationContainer().Should().BeFalse();
            imported.Name.Should().Be(source.Name);
            imported.Extension.Should().Be(source.Extension);
            imported.Attributes.Should().Equal(source.Attributes);

            // verify content
            imported.Length.Should().Be(source.Length);
            using (var importedStream = imported.OpenRead()) using (var sourceStream = source.OpenRead()) importedStream.StreamEquals(sourceStream).Should().BeTrue();
            ValidateAlternateStreams(imported, source);
        }
    }
}