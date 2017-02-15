using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Filesystem.Base;

namespace Pawod.MigrationContainer.Test.Validation
{
    public interface IContainerValidator<in TContainer, in TSource>
        where TContainer : IMigrationContainer
        where TSource : IFile
    {
        void ValidateContentHeader(TContainer container, TSource source);
        void ValidateImport(IFile imported, TSource source);
        void ValidateMigrationContainer(TContainer container, TSource source);
        void ValidatePartedContentHeader(TContainer container, TSource source);
        void ValidatePartedMigrationContainer(TContainer container, TSource source);
        void ValidatePartedStartHeader(TContainer container, TSource source);
        void ValidateStartHeader(TContainer container, TSource source);
    }
}