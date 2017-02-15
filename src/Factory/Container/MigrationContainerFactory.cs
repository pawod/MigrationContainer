using System;
using System.Runtime.Serialization;
using NLog;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Header.Base;
using Pawod.MigrationContainer.Filesystem.Base;
using Pawod.MigrationContainer.Serialization;
using Pawod.MigrationContainer.Serialization.Parameters;

namespace Pawod.MigrationContainer.Factory.Container
{
    public abstract class MigrationContainerFactory<TContainer, THeader, TSource, TExport>
        where TContainer : IMigrationContainer
        where THeader : class, IFileHeader
        where TSource : IFile
        where TExport : IFile
    {
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Creates a MigrationContainer for the specified source file.
        /// </summary>
        /// <typeparam name="TContainer">
        ///     The type of MigrationContainer to be created.
        /// </typeparam>
        /// <typeparam name="TSource">
        ///     The type of the data source.
        /// </typeparam>
        /// <typeparam name="TExport">
        ///     The type of file representing the exported container.
        /// </typeparam>
        /// <returns>
        ///     A MigrationContainer instance of the first part of the newly created
        ///     MigrationContainer.
        /// </returns>
        public TContainer Create(ISerializationParameters<THeader, TSource, TExport> parameters)
        {
            Logger.Trace($"Exporting: '{parameters.Source.FullPath}' to: '{parameters.TargetDir}'");
            Logger.Trace(
                $"ContentLength: {parameters.ContentHeader.ContentLength} bytes\r\nMaxContainerFileSize: {parameters.MaxContainerFileSize} bytes\r\nParts: {parameters.PartitioningScheme.NumberOfParts}");

            try
            {
                TExport mainPart;
                using (var serializer = GetSerializer())
                {
                    mainPart = serializer.Serialize(parameters, 0);
                    for (var i = 1; i < parameters.PartitioningScheme.NumberOfParts; i++) { serializer.Serialize(parameters, i); }
                }
                Logger.Trace($"Container successfully exported. Main part: '{mainPart.FullPath}'");
                return (TContainer) Activator.CreateInstance(typeof(TContainer), mainPart);
            }
            catch (System.Exception ex)
            {
                var serializationException = new SerializationException("Failed to serialize Container.", ex);
                Logger.Error(serializationException);
                throw serializationException;
            }
        }

        protected abstract IContainerSerializer<THeader, TSource, TExport> GetSerializer();
    }
}