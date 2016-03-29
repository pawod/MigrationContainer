namespace DataMigratorTest.Container.DirectoryContainer.Base
{
	using System.IO;
	using Configuration;
	using Container.Base;
	using DataMigrator.Container.DirectoryContainer;
	using DataMigrator.Container.DirectoryContainer.Base;
	using DataMigrator.Container.FileContainer.Header;
	using NUnit.Framework;
	using Validation.Base;

    [TestFixture("complex filled")]
    [TestFixture("complex huge")]
    [TestFixture("plain dir")]
    [TestFixture("two levels")]
    public abstract class DirectoryContainerTestBase<TContainer, TDirectoryHeader, TFileHeader> :
        ContainerTestBase<TContainer, TDirectoryHeader, DirectoryInfo>
        where TContainer : DirectoryContainerInfoBase<TContainer, TDirectoryHeader, TFileHeader>
        where TDirectoryHeader : DirectoryHeaderBase<TDirectoryHeader, TFileHeader>
        where TFileHeader : FileHeader
    {
        protected readonly DirectoryInfo SourceDir;

        protected DirectoryContainerTestBase(string dirName,
                                             IContainerValidator<TContainer, TDirectoryHeader, DirectoryInfo> validator)
            : base(validator)
        {
            SourceDir = new DirectoryInfo(Path.Combine(TestConfiguration.Instance.SourceDirs.FullName, dirName));
        }

        [Test]
        public override void ContentHeaderTest()
        {
            Validator.ValidateContentHeader(MainPart, SourceDir);
        }

        [Test]
        public override void ImportTest()
        {
            Validator.ValidateImport(Imported, SourceDir);
        }

        [Test]
        public override void MigrationContainerTest()
        {
            Validator.ValidateMigrationContainer(MainPart, SourceDir);
        }

        [Test]
        public override void StartHeaderTest()
        {
            Validator.ValidateStartHeader(MainPart, SourceDir);
        }
    }
}
