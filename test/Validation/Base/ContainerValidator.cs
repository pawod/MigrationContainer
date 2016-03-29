namespace DataMigratorTest.Validation.Base
{
    using System;
    using System.IO;
    using DataMigrator.Container.Base;
    using DataMigrator.Container.Base.Header;
    using DataMigrator.Helper;
    using FluentAssertions;

    public abstract class ContainerValidator<TContainer, TContentHeader, TFsInfo> :
        IContainerValidator<TContainer, TContentHeader, TFsInfo>
        where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
		where TContentHeader : class, IFilesystemHeader<TFsInfo>
        where TFsInfo : FileSystemInfo
    {
        public virtual void ValidateContentHeader(TContainer container, TFsInfo sourceInfo)
        {
            container.Should().NotBeNull();

            container.ContentHeader.TimeStamps.Should().NotBeNull().And.NotBeEmpty();
            container.ContentHeader.NextHeaderLength.Should().Be(0);
            container.ContentHeader.OriginalName.Should().Be(sourceInfo.Name);
        }

        public virtual void ValidateFile(FileInfo imported, FileInfo sourceFile)
        {
            // properties
            imported.Exists.Should().BeTrue();
            imported.IsMigrationContainer().Should().BeFalse();
            imported.Name.Should().Be(sourceFile.Name);
            imported.Extension.Should().Be(sourceFile.Extension);
            imported.Length.Should().Be(sourceFile.Length);
            imported.Attributes.Should().Be(sourceFile.Attributes);

            // timestamps

            // verify content
            using (var importedStream = imported.OpenRead()) using (var sourceStream = sourceFile.OpenRead()) importedStream.StreamEquals(sourceStream).Should().BeTrue();
        }

        public abstract void ValidateImport(TFsInfo imported, TFsInfo sourceInfo);

        public virtual void ValidateMigrationContainer(TContainer container, TFsInfo sourceInfo)
        {
            container.FileInfo.IsMigrationContainer().Should().BeTrue();
            container.IsValid().Should().BeTrue();

            if (container.IsMainPart()) container.StartHeader.PartNumber.Should().Be(0);
            else container.StartHeader.PartNumber.Should().BeGreaterThan(0);
            if (container.IsParted()) container.StartHeader.Parts.Should().BeGreaterThan(1);
            else container.StartHeader.Parts.Should().Be(1);
            if (container.IsMainPart())
            {
                var formatExtension = MigrationContainer.GetContainerExtension<TContainer, TContentHeader, TFsInfo>();
                container.FileInfo.Extension.Should().Be(formatExtension);
                var pattern = sourceInfo.Name + formatExtension;
                container.FileInfo.Name.Should().Be(pattern);
            }
            else container.FileInfo.Extension.Should().Contain(".part");
        }

        public virtual void ValidateStartHeader(TContainer container, TFsInfo sourceInfo)
        {
            try
            {
                StartHeader.Extract(container.FileInfo);
            }
            catch (Exception ex)
            {
                ex.GetType().Should().Be(typeof(InvalidDataException));
            }
            container.StartHeader.Md5Hash.Should().NotBeNullOrEmpty();
            container.StartHeader.Md5String.Should().NotBeNullOrEmpty();
            container.StartHeader.ContainerId.Should().NotBe(new Guid());
        }
    }
}
