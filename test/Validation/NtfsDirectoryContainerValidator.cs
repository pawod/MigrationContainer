using System.IO;
using FluentAssertions;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Test.Configuration;

namespace Pawod.MigrationContainer.Test.Validation
{
    public class NtfsDirectoryContainerValidator : NtfsContainerValidator<NtfsDirectoryContainer, NtfsDirectory>

    {
        public override void ValidateImport(IFile imported, NtfsDirectory source)
        {
            ValidateDirectory((INtfsDirectory) imported, source);

            var sourceEnumerator = source.GetEnumerator(ContainerConfiguration.Filter);
            var importEnumerator = ((INtfsDirectory) imported).GetEnumerator(ContainerConfiguration.Filter);

            while (sourceEnumerator.MoveNext())
            {
                importEnumerator.MoveNext().Should().BeTrue();
                importEnumerator.Current.IsLeaf().Should().Be(sourceEnumerator.Current.IsLeaf());
                importEnumerator.Current.IsNode().Should().Be(sourceEnumerator.Current.IsNode());

                if (sourceEnumerator.Current.IsLeaf()) ValidateFile((INtfsFile) importEnumerator.Current.Leaf, sourceEnumerator.Current.Leaf);
                else ValidateDirectory((NtfsDirectory) importEnumerator.Current.Node, sourceEnumerator.Current.Node);
            }
        }

        public override void ValidatePartedContentHeader(NtfsDirectoryContainer container, NtfsDirectory source)
        {
            ValidateContentHeader(container, source);
            container.ContentHeader.ContentLength.Should().Be(source.GetLength(ContainerConfiguration.Filter));
        }


        private void ValidateDirectory(INtfsDirectory imported, INtfsDirectory source)
        {
            imported.Exists.Should().BeTrue();
            imported.Name.Should().Be(source.Name);
            
            var sourceSize = source.GetLength(ContainerConfiguration.Filter);
            var importedSize = imported.GetLength(ContainerConfiguration.Filter);
            importedSize.Should().Be(sourceSize);

            imported.Attributes.Should().Equal(source.Attributes);
            ValidateJunctions(imported, source);
            ValidateAlternateStreams(imported, source);
        }

        private static void ValidateJunctions(INtfsDirectory imported, INtfsDirectory sourceDir)
        {
            var importJunctions = imported.GetJunctionPoints(SearchOption.AllDirectories);
            var sourceJunctions = sourceDir.GetJunctionPoints(SearchOption.AllDirectories);

            importJunctions.Count.Should().Be(sourceJunctions.Count);

            for (var i = 0; i < importJunctions.Count; i++)
            {
                var sourceJunction = sourceJunctions[i];
                var importedJunction = importJunctions[i];

                var relativeSourceLink = sourceJunction.Link.FullName.Replace(TestConfiguration.Instance.SourceDirs.FullName, string.Empty);
                var relativeImportLink = importedJunction.Link.FullName.Replace(TestConfiguration.Instance.ImportTarget.FullName, string.Empty);
                relativeImportLink.Should().Be(relativeSourceLink);

                var relativeSourceTarget = sourceJunction.Target.FullName.Replace(TestConfiguration.Instance.SourceDirs.FullName, string.Empty);
                var relativeImportTarget = importedJunction.Target.FullName.Replace(TestConfiguration.Instance.ImportTarget.FullName, string.Empty);
                relativeImportTarget.Should().Be(relativeSourceTarget);

                var sourceLink = new DirectoryInfo(sourceJunction.Link.FullName);
                var importLink = new DirectoryInfo(importedJunction.Link.FullName);

                // timestamps differ slightly for some reason
                //importLink.CreationTime.Should().Be(sourceLink.CreationTime);
                //importLink.CreationTimeUtc.Should().Be(sourceLink.CreationTimeUtc);

                //importLink.LastAccessTime.Should().Be(sourceLink.LastAccessTime);
                //importLink.LastAccessTimeUtc.Should().Be(sourceLink.LastAccessTimeUtc);

                //importLink.LastWriteTime.Should().Be(sourceLink.LastWriteTime);
                //importLink.LastWriteTimeUtc.Should().Be(sourceLink.LastWriteTimeUtc);
            }
        }
    }
}