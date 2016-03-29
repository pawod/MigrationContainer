namespace DataMigratorTest.Validation.FileContainer.Base
{
    using System.IO;
    using DataMigrator.Container.Base.Header;
    using DataMigrator.Container.FileContainer;
    using FluentAssertions;
    using Validation.Base;

    public abstract class FileContainerValidatorBase<TContainer, TFileHeader> :
        ContainerValidator<TContainer, TFileHeader, FileInfo>
        where TContainer : FileContainerInfoBase<TContainer, TFileHeader>
		where TFileHeader : class, IFilesystemHeader<FileInfo>
    {
        public override void ValidateContentHeader(TContainer container, FileInfo sourceInfo)
        {
            base.ValidateContentHeader(container, sourceInfo);
            container.ContentHeader.ContentLength.Should().Be(sourceInfo.Length);
            container.ContentHeader.OriginalName.Should().Be(sourceInfo.Name);

            // timestamps
//            container.ContentHeader.TimeStamps["CreationTimeUtc"].Should()
//                                                                 .Be(sourceInfo.CreationTimeUtc.ToFileTimeUtc());
//            container.ContentHeader.TimeStamps["LastAccessTimeUtc"].Should()
//                                                                   .Be(sourceInfo.LastAccessTimeUtc.ToFileTimeUtc());
//
//            container.ContentHeader.TimeStamps["LastWriteTimeUtc"].Should()
//                                                                  .Be(sourceInfo.LastWriteTimeUtc.ToFileTimeUtc());
        }

        public override void ValidateImport(FileInfo imported, FileInfo sourceInfo)
        {
            ValidateFile(imported, sourceInfo);
        }
    }
}
