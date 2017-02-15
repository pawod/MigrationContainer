using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using NLog;
using Pawod.MigrationContainer.Container;

namespace Pawod.MigrationContainer.Configuration
{
    /// <summary>
    ///     Provides basic parameters required for the (de-)serialization of MigrationContainers.
    /// </summary>
    public class ContainerConfiguration : ConfigurationSection
    {
        private static readonly Dictionary<string, ContainerConfiguration> Configurations;
        private static readonly Logger Logger;


        /// <summary>
        ///     The buffer size to be used, when reading or writing content from the container.
        /// </summary>
        [ConfigurationProperty("ContentBufferSize", DefaultValue = 4096, IsRequired = false)]
        public int ContentBufferSize => (int) this["ContentBufferSize"];

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
                return val != null ? ((CommaDelimitedStringCollection) this["Filter"]).Cast<string>().ToList() : null;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum size (in bytes) of a container file.
        /// </summary>
        [ConfigurationProperty("MaxContainerFileSize", DefaultValue = 100000000L, IsRequired = true)]
        public long MaxContainerFileSize => (long) this["MaxContainerFileSize"];

      

        static ContainerConfiguration()
        {
            Configurations = new Dictionary<string, ContainerConfiguration>();
            Logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        ///     Gets the configuration for a certain type of container format from the app.comfig.
        /// </summary>
        /// <typeparam name="TContainer">The type of MigrationContainer.</typeparam>
        public static ContainerConfiguration Instance<TContainer>() where TContainer : IMigrationContainer
        {
            return GetConfigFromSection(typeof(TContainer).Name);
        }

        /// <summary>
        ///     Gets the configuration for a certain type of container format from the app.comfig.
        /// </summary>
        /// <param name="containerType">
        ///     The type of container format to get the configuration for.
        /// </param>
        /// <returns>A singleton instance of the specified format's configuration.</returns>
        public static ContainerConfiguration Instance(Type containerType)
        {
            return GetConfigFromSection(containerType.Name);
        }

        /// <summary>
        ///     Gets a ContainerConfiguration from the app.config.
        /// </summary>
        /// <param name="sectionName">
        ///     The name of the configuration section.
        /// </param>
        /// <returns>A singleton instance of the specified configuration section.</returns>
        private static ContainerConfiguration GetConfigFromSection(string sectionName)
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
            catch (System.Exception ex)
            {
                Logger.Error(ex, $"Failed to read configuration section: {sectionName}");
                throw ex;
            }
        }
    }
}