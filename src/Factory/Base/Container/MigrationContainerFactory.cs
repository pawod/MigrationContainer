namespace DataMigrator.Factory.Base.Container
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Configuration;
	using DataMigrator.Container.Base;
	using DataMigrator.Container.Base.Header;
	using NLog;
	using Serialization.Base;
	using Serialization.Parameters.Base;

	public abstract class MigrationContainerFactory<TContainer, TContentHeader, TFsInfo>
		where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
		where TContentHeader : class, IFilesystemHeader<TFsInfo>
		where TFsInfo : FileSystemInfo
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		protected readonly ContainerConfiguration ContainerConfiguration;

		protected MigrationContainerFactory()
		{
			ContainerConfiguration = ContainerConfiguration.Instance<TContainer, TContentHeader, TFsInfo>();
		}

		/// <summary>
		///     Creates a MigrationContainer for the specified source file.
		/// </summary>
		/// <typeparam name="TContainer">
		///     The type of MigrationContainer to be created.
		/// </typeparam>
		/// <typeparam name="TContentHeader">
		///     The type of the ContentHeader, which the MigrationContainer uses to describe
		///     its content.
		/// </typeparam>
		/// <typeparam name="TFsInfo">
		///     The type of the data source.
		/// </typeparam>
		/// <typeparam name="TParams">A SerializationParameters derived type.</typeparam>
		/// <param name="parameters">
		///     The type of ContainerParameters to be used for serialization.
		/// </param>
		/// <returns>
		///     A MigrationContainerInfo instance of the first part of the newly created
		///     MigrationContainer.
		/// </returns>
		public TContainer Create<TParams>(TParams parameters)
			where TParams : ISerializationParameters<TContentHeader, TFsInfo>
		{
			_logger.Trace("Exporting: '{0}' to: '{1}'", parameters.SourceInfo.FullName, parameters.TargetDir);
			_logger.Trace("ContentLength: {0} bytes\r\nMaxFileSize: {1} bytes\r\nParts: {2}",
				parameters.ContentHeader.ContentLength,
				parameters.MaxFileSize,
				parameters.RequiredFiles);

			try
			{
				FileInfo mainPart;
				using (var serializer = GetSerializer())
				{
					mainPart = serializer.Serialize(parameters, 0);
					for (var i = 1; i < parameters.RequiredFiles; i++)
					{
						serializer.Serialize(parameters, i);
					}
				}
				_logger.Trace("Container successfully exported: '{0}'", mainPart.FullName);
				return (TContainer)Activator.CreateInstance(typeof(TContainer), mainPart);
			}
			catch (Exception ex)
			{
				var serializationException = new SerializationException("Failed to serialize MigrationContainer.", ex);
				_logger.Error(serializationException);
				throw serializationException;
			}
		}

		protected abstract IContainerSerializer<TContentHeader, TFsInfo> GetSerializer();
	}
}
