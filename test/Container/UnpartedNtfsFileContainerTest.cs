using NUnit.Framework;

namespace Pawod.MigrationContainer.Test.Container
{
    public class UnpartedNtfsFileContainerTest : NtfsFileContainerTest
    {
        public UnpartedNtfsFileContainerTest(string dirName) : base(dirName)
        {
        }

        [Test]
        public override void ContentHeaderTest()
        {
            Validator.ValidateContentHeader(Exported, Source);
        }

        [Test]
        public override void ImportTest()
        {
            Validator.ValidateImport(Imported, Source);
        }

        [Test]
        public override void MigrationContainerTest()
        {
            Validator.ValidateMigrationContainer(Exported, Source);
        }

        [Test]
        public override void StartHeaderTest()
        {
            Validator.ValidateStartHeader(Exported, Source);
        }
    }
}