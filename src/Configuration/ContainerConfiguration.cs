namespace DataMigrator.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using Container.Base;
	using Container.Base.Header;
	using NLog;

	/// <summary>
	///     Provides all configurable parameters of a MigrationContainer.
	/// </summary>
	public class ContainerConfiguration : ConfigurationSection
	{
		private static readonly Dictionary<string, ContainerConfiguration> Configurations;
		private static readonly Logger Logger;

		/// <summary>
		///     Gets or sets the maximum size (in bytes) of a container file.
		/// </summary>
		[ConfigurationProperty("MaxFileSize", DefaultValue = 100000000L, IsRequired = true)]
		public long MaxFileSize
		{
			get { return (long)this["MaxFileSize"]; }
		}

		/// <summary>
		///     A list of diretories or files to be excluded from serialization.
		/// </summary>
		[TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
		[ConfigurationProperty("Filter", DefaultValue = null, IsRequired = false, IsDefaultCollection = true)]
		public IList<string> Filter
		{
			get
			{
				var val = this["Filter"];
				return val != null? ((CommaDelimitedStringCollection)this["Filter"]).Cast<string>().ToList() : null;
			}
		}

		/// <summary>
		///     The buffer size to be used, when reading or writing content from the
		///     container.
		/// </summary>
		[ConfigurationProperty("ContentBufferSize", DefaultValue = 4096, IsRequired = false)]
		public int ContentBufferSize
		{
			get { return (int)this["ContentBufferSize"]; }
		}

		static ContainerConfiguration()
		{
			Configurations = new Dictionary<string, ContainerConfiguration>();
			Logger = LogManager.GetCurrentClassLogger();
		}

		/// <summary>
		///     Gets the configuration for a certain type of container format from the
		///     app.comfig.
		/// </summary>
		/// <typeparam name="TContainer">The type of MigrationContainer.</typeparam>
		/// <typeparam name="TContentHeader">
		///     The type of ContentHeader used to describe the MigrationContainer's content.
		/// </typeparam>
		/// <typeparam name="TFsInfo">
		///     The type of FileSystemInfo, which is exported by the specified
		///     MigrationContainer format.
		/// </typeparam>
		/// <returns>A singleton instance of the specified format's configuration.</returns>
		public static ContainerConfiguration Instance<TContainer, TContentHeader, TFsInfo>()
			where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
			where TContentHeader : class, IContentHeader where TFsInfo : FileSystemInfo
		{
			return Instance(typeof(TContainer).Name);
		}

		/// <summary>
		///     Gets the configuration for a certain type of container format from the
		///     app.comfig.
		/// </summary>
		/// <param name="containerType">
		///     The type of container format to get the configuration
		///     for.
		/// </param>
		/// <returns>A singleton instance of the specified format's configuration.</returns>
		public static ContainerConfiguration Instance(Type containerType)
		{
			return Instance(containerType.Name);
		}

		/// <summary>
		///     Gets a ContainerConfiguration from the app.config.
		/// </summary>
		/// <param name="sectionName">
		///     The name of the configuration section.
		/// </param>
		/// <returns>A singleton instance of the specified configuration section.</returns>
		public static ContainerConfiguration Instance(string sectionName)
		{
			try
			{
				if (!Configurations.ContainsKey(sectionName))
				{
					var config = ConfigurationManager.GetSection(sectionName) as ContainerConfiguration;
					Configurations.Add(sectionName, config);
				}
				return Configurations[sectionName];
			}
			catch (Exception ex)
			{
				Logger.ErrorException(string.Format("Failed to read configuration section: {0}", sectionName), ex);
				throw ex;
			}
		}
	}
}
