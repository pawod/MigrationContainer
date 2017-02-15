using Alphaleonis.Win32.Filesystem;
using NUnit.Framework;
using Pawod.MigrationContainer.Adapter;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Factory.Container;
using Pawod.MigrationContainer.Factory.Parameters;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Test.Configuration;
using Pawod.MigrationContainer.Test.Validation;

namespace Pawod.MigrationContainer.Test.Container
{
    [TestFixture("complex filled")]
    [TestFixture("complex huge")]
    [TestFixture("plain dir")]
    [TestFixture("two levels")]
    [TestFixture("two levels2")]
    [TestFixture("junctions")]
    [TestFixture("ads")]
    [TestFixture("complex ntfs")]
    public abstract class NtfsDirectoryContainerTest : ContainerTest<NtfsDirectoryContainer, NtfsDirectory>
    {
        protected NtfsDirectoryContainerTest(string dirName) : base(new NtfsDirectoryContainerValidator())
        {
            Source = new NtfsDirectory($"{TestConfiguration.Instance.SourceDirs}{Path.DirectorySeparator}{dirName}");
            var paramz = NtfsDirectoryContainerParametersFactory.Instance.Create(Source, new NtfsDirectory(TestConfiguration.Instance.ExportTarget));
            Exported = NtfsDirectoryContainerFactory.Instance.Create(paramz);
            Imported = new NtfsDirectoryAdapter().Import(Exported, new NtfsDirectory(TestConfiguration.Instance.ImportTarget));
        }
    }
}