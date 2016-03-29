namespace DataMigratorTest.Container.FileContainer.Base
{
	using System.IO;
	using Configuration;
	using Container.Base;
	using DataMigrator.Container.FileContainer;
	using DataMigrator.Container.FileContainer.Header;
	using NUnit.Framework;
	using Validation.Base;

	[TestFixture("CSC_1218.JPG")]
	[TestFixture("New Text Document.txt")]
	[TestFixture("README.txt")]
	[TestFixture("WIN-2015-02-08.zip")]
	[TestFixture("05. Afternoon Soul.mp3")]
	public abstract class FileContainerTestBase<TContainer, TFileHeader> :
		ContainerTestBase<TContainer, TFileHeader, FileInfo>
		where TContainer : FileContainerInfoBase<TContainer, TFileHeader>
		where TFileHeader : FileHeader
	{
		protected FileInfo SourceFile { get; private set; }

		protected FileContainerTestBase(string fileName, IContainerValidator<TContainer, TFileHeader, FileInfo> validator)
			: base(validator)
		{
			SourceFile = new FileInfo(Path.Combine(TestConfiguration.Instance.SourceFiles.FullName, fileName));
		}

		[Test]
		public override void ContentHeaderTest()
		{
			Validator.ValidateContentHeader(MainPart, SourceFile);
		}

		[Test]
		public override void ImportTest()
		{
			Validator.ValidateImport(Imported, SourceFile);
		}

		[Test]
		public override void MigrationContainerTest()
		{
			Validator.ValidateMigrationContainer(MainPart, SourceFile);
		}

		[Test]
		public override void StartHeaderTest()
		{
			Validator.ValidateStartHeader(MainPart, SourceFile);
		}
	}
}
