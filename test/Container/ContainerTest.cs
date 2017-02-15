using System.IO;
using NLog;
using NLog.Config;
using NUnit.Framework;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Test.Configuration;
using Pawod.MigrationContainer.Test.Validation;

namespace Pawod.MigrationContainer.Test.Container
{
    public abstract class ContainerTest<TContainer, TSource>
        where TContainer : IMigrationContainer
        where TSource : IFile

    {
        protected readonly IContainerValidator<TContainer, TSource> Validator;

        protected TContainer Exported { get; set; }

        protected IFile Imported { get; set; }

        protected TSource Source { get; set; }

        protected ContainerTest(IContainerValidator<TContainer, TSource> validator)
        {
            Validator = validator;
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(TestConfiguration.Instance.UnitTestDir.FullName, "Nlog.xml"));

            if (!TestConfiguration.Instance.ImportTarget.Exists) TestConfiguration.Instance.ImportTarget.Create();
            if (!TestConfiguration.Instance.ExportTarget.Exists) TestConfiguration.Instance.ExportTarget.Create();
        }

        [TestFixtureTearDown]
        public void ClassCleanup()
        {
            TestConfiguration.Instance.ImportTarget.Refresh();
            foreach (var junction in
                JunctionPoint.FindJunctions(TestConfiguration.Instance.ImportTarget, SearchOption.AllDirectories)) { junction.Delete(); }
            TestConfiguration.Instance.ImportTarget.Refresh();
            if (TestConfiguration.Instance.ImportTarget.Exists)
            {
                foreach (var dirInfo in
                    TestConfiguration.Instance.ImportTarget.GetDirectories("*", SearchOption.TopDirectoryOnly)) { dirInfo.Delete(true); }
                TestConfiguration.Instance.ImportTarget.Refresh();
                foreach (var fileInfo in TestConfiguration.Instance.ImportTarget.GetFiles("*", SearchOption.TopDirectoryOnly)) { fileInfo.Delete(); }
            }

            TestConfiguration.Instance.ExportTarget.Refresh();
            if (TestConfiguration.Instance.ExportTarget.Exists)
            {
                foreach (var dirInfo in
                    TestConfiguration.Instance.ExportTarget.GetDirectories("*", SearchOption.TopDirectoryOnly)) { dirInfo.Delete(true); }
                TestConfiguration.Instance.ExportTarget.Refresh();
                foreach (var fileInfo in TestConfiguration.Instance.ExportTarget.GetFiles("*", SearchOption.TopDirectoryOnly)) { fileInfo.Delete(); }
            }
        }

        [Test]
        public abstract void ContentHeaderTest();

        [Test]
        public abstract void ImportTest();

        [Test]
        public abstract void MigrationContainerTest();

        [Test]
        public abstract void StartHeaderTest();
    }
}