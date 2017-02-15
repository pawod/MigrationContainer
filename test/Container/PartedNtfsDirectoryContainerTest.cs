using NUnit.Framework;

namespace Pawod.MigrationContainer.Test.Container
{
    public class PartedNtfsDirectoryContainerTest : NtfsDirectoryContainerTest
    {
        public PartedNtfsDirectoryContainerTest(string dirName) : base(dirName)
        {
        }

        [Test]
        public override void ContentHeaderTest()
        {
            Validator.ValidatePartedContentHeader(Exported, Source);
        }

        [Test]
        public override void ImportTest()
        {
            Validator.ValidateImport(Imported, Source);
        }

        [Test]
        public override void MigrationContainerTest()
        {
            Validator.ValidatePartedMigrationContainer(Exported, Source);
        }

        [Test]
        public override void StartHeaderTest()
        {
            Validator.ValidatePartedStartHeader(Exported, Source);
        }
    }
}