namespace DataMigrator.Container.Base
{
    using System.IO;
    using System.Linq;
    using Header;
    using Helper;

    /// <summary>
    ///     A helper class for MigrationContainer's.
    /// </summary>
    public static class MigrationContainer
    {
        /// <summary>
        ///     Gets the file extension for a certain type of MigrationContainer.
        /// </summary>
        /// <typeparam name="TContainer">The type of MigrationContainer.</typeparam>
        /// <typeparam name="TContentHeader">
        ///     The type of ContentHeader, which describes MigrationContainer's content.
        /// </typeparam>
        /// <typeparam name="TFsInfo">
        ///     The type of FileSystemInfo, which is exported by the specified
        ///     MigrationContainer format.
        /// </typeparam>
        /// <returns>The file extension for the specified MigrationContainer type.</returns>
        public static string GetContainerExtension<TContainer, TContentHeader, TFsInfo>()
            where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
            where TContentHeader : class, IContentHeader where TFsInfo : FileSystemInfo
        {
            var meta = GetMetaDescription<TContainer, TContentHeader, TFsInfo>();
            return meta != null? meta.FileExtension : string.Empty;
        }

        /// <summary>
        ///     Gets the meta description, which provides additional information about a
        ///     specific type of MigrationContainer.
        /// </summary>
        /// <typeparam name="TContainer">Tge type of MigrationContainer.</typeparam>
        /// <typeparam name="TContentHeader">
        ///     The type of ContentHeader, which describes MigrationContainer's content.
        /// </typeparam>
        /// <typeparam name="TFsInfo">
        ///     The type of FileSystemInfo, which is exported by the specified
        ///     MigrationContainer format.
        /// </typeparam>
        /// <returns>The meta description for the specified container type.</returns>
        public static ContainerMetaDescription GetMetaDescription<TContainer, TContentHeader, TFsInfo>()
            where TContainer : MigrationContainerInfo<TContainer, TContentHeader, TFsInfo>
            where TContentHeader : class, IContentHeader where TFsInfo : FileSystemInfo
        {
            return
                typeof(TContainer).GetCustomAttributes(typeof(ContainerMetaDescription), true).FirstOrDefault() as
                    ContainerMetaDescription;
        }

        /// <summary>
        ///     Determines wether the specified file is a migration container.
        /// </summary>
        /// <param name="file">The file to be examined.</param>
        /// <returns>True if the specified file is considered to be a migration container.</returns>
        public static bool IsMigrationContainer(this FileInfo file)
        {
            using (var fileStream = file.OpenRead())
            {
                var magicNumbers = new byte[StartHeader.MagicNumbers.Count];
                fileStream.Read(magicNumbers, 0, StartHeader.MagicNumbers.Count);
                magicNumbers.FromBigEndian();
                return magicNumbers.SequenceEqual(StartHeader.MagicNumbers);
            }
        }
    }
}
