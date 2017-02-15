using System;
using System.Linq;
using Pawod.MigrationContainer.Container;
using Pawod.MigrationContainer.Container.Attribute;

namespace Pawod.MigrationContainer.Extensions
{
    /// <summary>
    ///     A helper class for MigrationContainer's.
    /// </summary>
    public static class MigrationContainerExtensions
    {
        /// <summary>
        ///     Gets the file extension for a certain type of MigrationContainer.
        /// </summary>
        /// <typeparam name="TContainer">The type of MigrationContainer.</typeparam>
        public static string GetFileExtension<TContainer>() where TContainer : IMigrationContainer
        {
            var meta = GetMetaDescription<TContainer>();
            return meta != null ? meta.FileExtension : string.Empty;
        }

        /// <summary>
        ///     Gets the file extension for a certain type of MigrationContainer.
        /// </summary>
        public static string GetFileExtension(this Type type)
        {
            var meta = GetMetaDescription(type);
            return meta != null ? meta.FileExtension : string.Empty;
        }

        /// <summary>
        ///     Gets the meta description, which provides additional information about a specific type of MigrationContainer.
        /// </summary>
        /// <typeparam name="TContainer">The type of MigrationContainer.</typeparam>
        private static ContainerMetaDescriptionAttribute GetMetaDescription<TContainer>() where TContainer : IMigrationContainer

        {
            return
                typeof(TContainer).GetCustomAttributes(typeof(ContainerMetaDescriptionAttribute), true).FirstOrDefault() as
                ContainerMetaDescriptionAttribute;
        }

        /// <summary>
        ///     Gets the meta description, which provides additional information about a specific type of MigrationContainer.
        /// </summary>
        private static ContainerMetaDescriptionAttribute GetMetaDescription(this Type type)

        {
            return
                type.GetCustomAttributes(typeof(ContainerMetaDescriptionAttribute), true).FirstOrDefault() as
                ContainerMetaDescriptionAttribute;
        }
    }
}