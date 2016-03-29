namespace DataMigrator.Adapters.Base.Directory
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Container.Base.Body;
	using Container.Base.Header;
	using Container.DirectoryContainer;
	using Container.DirectoryContainer.Base;
	using Exception;
	using File;
	using NLog;

	public abstract class DirectoryAdapterBase<TContainer, TDirectoryHeader, TFileHeader> :
		IContentAdapter<TContainer, TDirectoryHeader, DirectoryInfo>,
		IDirectoryAdapter<TDirectoryHeader, TFileHeader>
		where TContainer : DirectoryContainerInfoBase<TContainer, TDirectoryHeader, TFileHeader>
		where TDirectoryHeader : class, IDirectoryHeaderBase<TDirectoryHeader, TFileHeader>
		where TFileHeader : class, IFilesystemHeader<FileInfo>
	{
		protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		protected readonly IFileAdapter<TFileHeader> FileAdapter;

		protected DirectoryAdapterBase(IFileAdapter<TFileHeader> fileAdapter)
		{
			FileAdapter = fileAdapter;
		}

		public string Import(TContainer container, string locationKey)
		{
			if (container.ContentHeader == null) throw new InvalidContainerException("ContentHeader is null.");
			return ImportDirectory(container.Body, container.ContentHeader, locationKey);
		}

		public string ImportDirectory(IContainerBody body,
									  TDirectoryHeader directoryHeader,
									  string locationKey,
									  string customDirName = null)
		{
			try
			{
				var dirStack = new Stack<Tuple<TDirectoryHeader, string>>();
				var replaceString = string.Empty;
				foreach (var header in directoryHeader)
				{
					if (header.IsNode())
					{
						if (!string.IsNullOrWhiteSpace(customDirName))
						{
							if (dirStack.Count == 0)
							{
								replaceString = header.Node.OriginalName;
								header.Node.RelativePath = header.Node.RelativePath.Replace(replaceString, customDirName);
								header.Node.OriginalName = customDirName;
							}
							else header.Node.RelativePath = header.Node.RelativePath.Replace(replaceString, customDirName);
						}

						var importKey = GetImportKey(header.Node, locationKey);
						Logger.Trace("Importing directory with key: '{0}'. location: '{1}'", header.Node.OriginalName, locationKey);

						RestoreDirectory(header.Node, body, importKey);
						dirStack.Push(new Tuple<TDirectoryHeader, string>(header.Node, importKey));
					}
					else ImportFile(header.Leaf, body, dirStack.Count == 0? string.Empty : dirStack.Peek().Item2);
				}
				RestoreProperties(dirStack, body);
				return GetImportKey(directoryHeader, locationKey);
			}
			catch (Exception ex)
			{
				var containerException = new ImportFailedException(directoryHeader, ex);
				Logger.Error(containerException);
				throw containerException;
			}
		}

		public string ImportFile(TFileHeader fileHeader, IContainerBody body, string locationKey)
		{
			return FileAdapter.ImportFile(fileHeader, body, locationKey);
		}

		/// <summary>
		///     Gets the key of the imported directory.
		/// </summary>
		/// <param name="directoryHeader">The header describing the imported directory.</param>
		/// <param name="locationKey">
		///     The key that uniquely identifies the target location
		///     for the content to be imported.
		/// </param>
		/// <returns>A key, that uniquely identifies the imported directory.</returns>
		protected abstract string GetImportKey(TDirectoryHeader directoryHeader, string locationKey);

		/// <summary>
		///     Restores the directory on the targeted file system.
		/// </summary>
		/// <param name="fileHeader">The FileHeader, which describes the contained file.</param>
		/// <param name="body">The MigrationContainer's body.</param>
		/// <param name="importKey">The key, that uniquely identifies the imported directory.</param>
		protected abstract void RestoreDirectory(TDirectoryHeader fileHeader, IContainerBody body, string importKey);

		/// <summary>
		///     Restores the directory's properties after it has been fully reproduced with
		///     all of its contents. This is where timestamps and other content dependend
		///     meta data are restored.
		/// </summary>
		/// <param name="dirStack">
		///     A stack of all reproduced directories in reverse order of
		///     creation with its associated DirectoryHeader.
		/// </param>
		/// <param name="body">
		///     The MigrationContainer's body, which contains the directory's content.
		/// </param>
		protected abstract void RestoreProperties(Stack<Tuple<TDirectoryHeader, string>> dirStack, IContainerBody body);
	}
}
