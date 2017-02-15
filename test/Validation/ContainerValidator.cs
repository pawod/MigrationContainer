using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Pawod.MigrationContainer.Configuration;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Exception;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Filesystem.NTFS;
using Pawod.MigrationContainer.Test.Configuration;

namespace Pawod.MigrationContainer.Test.Validation
{
    public abstract class ContainerValidator<TContainer, TSource> : IContainerValidator<TContainer, TSource>
        where TContainer : IMigrationContainer
        where TSource : IFile
    {
        protected readonly ContainerConfiguration ContainerConfiguration;
        private IList<TContainer> _relatedParts;

        protected ContainerValidator()
        {
            ContainerConfiguration = ContainerConfiguration.Instance<TContainer>();
        }

        public abstract void ValidateContentHeader(TContainer container, TSource source);

        public abstract void ValidateImport(IFile imported, TSource source);
        public abstract void ValidateMigrationContainer(TContainer container, TSource source);
        public abstract void ValidatePartedContentHeader(TContainer container, TSource source);

        public void ValidatePartedMigrationContainer(TContainer container, TSource source)
        {
            ValidateMigrationContainer(container, source);
            foreach (var partialContainer in GetRelatedParts(container)) { ValidateMigrationContainer(partialContainer, source); }
        }

        public void ValidatePartedStartHeader(TContainer container, TSource source)
        {
            ValidateStartHeader(container, source);
            container.StartHeader.PartNumber.Should().Be(0);
            container.StartHeader.NextHeaderLength.Should().NotBe(0);
            container.StartHeader.IsLastHeader().Should().BeFalse();

            container.StartHeader.Parts.Should().Be(GetRelatedParts(container).Count + 1); // add the excluded main part
            foreach (var partialContainer in GetRelatedParts(container))
            {
                ValidateStartHeader(partialContainer, source);

                var parsed = int.Parse(partialContainer.File.Extension.Replace(".part", string.Empty));

                partialContainer.StartHeader.PartNumber.Should().Be(parsed);
                partialContainer.StartHeader.Parts.Should().Be(container.StartHeader.Parts);
                partialContainer.StartHeader.ContainerId.Should().Be(container.StartHeader.ContainerId);
                partialContainer.StartHeader.NextHeaderLength.Should().Be(0);
                partialContainer.StartHeader.IsLastHeader().Should().BeTrue();
            }
        }

        public virtual void ValidateStartHeader(TContainer container, TSource source)
        {
            try { StartHeader.Extract(new NtfsFile(TestConfiguration.Instance.SourceFiles.GetFiles().First())); }
            catch (System.Exception ex) {
                ex.GetType().Should().Be(typeof(InvalidContainerException));
            }
            container.StartHeader.Md5Hash.Should().NotBeNullOrEmpty();
            container.StartHeader.Md5String.Should().NotBeNullOrEmpty();
            container.StartHeader.ContainerId.Should().NotBe(new Guid());


            if (GetRelatedParts(container).Any())
            {
                container.StartHeader.Parts.Should().Be(GetRelatedParts(container).Count + 1);
            }
            else
            {
                container.StartHeader.PartNumber.Should().Be(0);
                container.StartHeader.NextHeaderLength.Should().NotBe(0);
                container.StartHeader.Parts.Should().Be(1);
            }
        }

        protected IList<TContainer> GetRelatedParts(TContainer container)
        {
            return _relatedParts ?? (_relatedParts = (IList<TContainer>) container.FindRelatedParts());
        }
    }
}