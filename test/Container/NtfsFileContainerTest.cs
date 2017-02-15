using System.IO;
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
    [TestFixture("(a,b) Baum Definition.jpg")]
    [TestFixture("New Text Document.txt")]
    [TestFixture("PacketDotNet-0.13.0.bin.zip")]
    [TestFixture("README.txt")]
    [TestFixture("VS2005_and_up.proj")]
    [TestFixture("WIN-2015-02-08.zip")]
    [TestFixture("05. Afternoon Soul.mp3")]
    public abstract class NtfsFileContainerTest : ContainerTest<NtfsFileContainer, NtfsFile>
    {
        protected NtfsFileContainerTest(string fileName) : base(new NtfsFileContainerValidator())
        {
            Source = new NtfsFile(Path.Combine(TestConfiguration.Instance.SourceFiles.FullName, fileName));
            var paramz = NtfsFileContainerParametersFactory.Instance.Create(Source, new NtfsDirectory(TestConfiguration.Instance.ExportTarget));
            Exported = NtfsFileContainerFactory.Instance.Create(paramz);
            Imported = new NtfsFileAdapter().Import(Exported, new NtfsDirectory(TestConfiguration.Instance.ImportTarget));
        }
    }
}