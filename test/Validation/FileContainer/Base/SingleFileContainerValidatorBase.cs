namespace DataMigratorTest.Validation.FileContainer.Base
{
    using System.IO;
    using DataMigrator.Container.Base.Header;
    using DataMigrator.Container.FileContainer;
    using FluentAssertions;

    public abstract class SingleFileContainerValidatorBase<TContainer, TFileHeader> :
        FileContainerValidatorBase<TContainer, TFileHeader>
        where TContainer : FileContainerInfoBase<TContainer, TFileHeader>
		where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
        public override void ValidateStartHeader(TContainer container, FileInfo sourceInfo)
        {
            base.ValidateStartHeader(container, sourceInfo);
            container.StartHeader.PartNumber.Should().Be(0);
            container.StartHeader.Parts.Should().Be(1);
            container.StartHeader.NextHeaderLength.Should().NotBe(0);
            container.StartHeader.IsLastHeader().Should().BeFalse();
        }
    }
}
