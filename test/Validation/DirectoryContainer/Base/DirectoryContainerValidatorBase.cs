namespace DataMigratorTest.Validation.DirectoryContainer.Base
{
	using System.IO;
	using DataMigrator.Configuration;
	using DataMigrator.Container.Base.Header;
	using DataMigrator.Container.DirectoryContainer;
	using DataMigrator.Container.DirectoryContainer.Base;
	using DataMigrator.Helper;
	using FluentAssertions;
	using Validation.Base;

	public abstract class DirectoryContainerValidatorBase<TContainer, TDirectoryHeader, TFileHeader> :
		ContainerValidator<TContainer, TDirectoryHeader, DirectoryInfo>
		where TContainer : DirectoryContainerInfoBase<TContainer, TDirectoryHeader, TFileHeader>
		where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
		where TFileHeader : class, IFilesystemHeader<FileInfo>
	{
		protected readonly ContainerConfiguration ContainerConfiguration;

		protected DirectoryContainerValidatorBase()
		{
			ContainerConfiguration = ContainerConfiguration.Instance<TContainer, TDirectoryHeader, DirectoryInfo>();
		}

		public override void ValidateContentHeader(TContainer container, DirectoryInfo sourceInfo)
		{
			base.ValidateContentHeader(container, sourceInfo);
			var dirLength = sourceInfo.CalculateSize(ContainerConfiguration.Filter);
			container.ContentHeader.ContentLength.Should().Be(dirLength);
		}
		
		public virtual void ValidateDirectory(DirectoryInfo imported, DirectoryInfo sourceDir)
		{
			// properties
			imported.Exists.Should().BeTrue();
			imported.Name.Should().Be(sourceDir.Name);
			imported.Extension.Should().Be(sourceDir.Extension);

			var sourceSize = sourceDir.CalculateSize(ContainerConfiguration.Filter);
			var importedSize = imported.CalculateSize(ContainerConfiguration.Filter);
			importedSize.Should().Be(sourceSize);

			// attributes
			imported.Attributes.Should().Be(sourceDir.Attributes);

			// timestamps
		}

		public override void ValidateImport(DirectoryInfo imported, DirectoryInfo sourceDir)
		{
			ValidateDirectory(imported, sourceDir);

			var sourceEnumerator = sourceDir.GetEnumerator(ContainerConfiguration.Filter);
			var importEnumerator = imported.GetEnumerator(ContainerConfiguration.Filter);

			while (sourceEnumerator.MoveNext())
			{
				importEnumerator.MoveNext().Should().BeTrue();
				importEnumerator.Current.IsLeaf().Should().Be(sourceEnumerator.Current.IsLeaf());
				importEnumerator.Current.IsNode().Should().Be(sourceEnumerator.Current.IsNode());

				if (sourceEnumerator.Current.IsLeaf()) ValidateFile(importEnumerator.Current.Leaf, sourceEnumerator.Current.Leaf);
				else ValidateDirectory(importEnumerator.Current.Node, sourceEnumerator.Current.Node);
			}
		}
	}
}
