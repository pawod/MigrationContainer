namespace DataMigrator.Container.Base
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;
	using Body;
	using Configuration;
	using Exception;
	using Header;
	using NLog;

	/// <summary>
	///     The base class of migration containers. It provides access to the serialized
	///     container's headers and its content.
	/// </summary>
	public abstract class MigrationContainerInfo<TChild, TContentHeader, TFsInfo>
		where TChild : MigrationContainerInfo<TChild, TContentHeader, TFsInfo>
		where TContentHeader : class, IContentHeader
		where TFsInfo : FileSystemInfo
	{
		protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		protected ContainerConfiguration Configuration
		{
			get { return ContainerConfiguration.Instance<TChild, TContentHeader, TFsInfo>(); }
		}

		/// <summary>
		///     Gets the FileInfo for the current container file.
		/// </summary>
		public readonly FileInfo FileInfo;

		private IContainerBody _body;
		private TContentHeader _contentHeader;
		private StartHeader _startHeader;

		/// <summary>
		///     Gets the container's body, which provides access to the stored content.
		/// </summary>
		/// <returns>A disposable ContainerBody instance.</returns>
		public IContainerBody Body
		{
			get
			{
				if (_body == null)
				{
					var allParts = GetAllParts();
					if (StartHeader.Parts != allParts.Count) throw new MissingPartException(StartHeader.Parts, allParts.Count);
					_body = CreateBodyInstance(allParts);
				}
				return _body;
			}
		}

		/// <summary>
		///     The stream position, where the body of this container starts.
		/// </summary>
		public abstract long BodyPosition { get; }

		/// <summary>
		///     Gets the ContentHeader from the main part of this container.
		/// </summary>
		public TContentHeader ContentHeader
		{
			get
			{
				if (_contentHeader != null) return _contentHeader;
				var mainPart = IsMainPart()? this : FindMainPart();
				if (mainPart == null) throw new MainPartNotFoundException(FileInfo.DirectoryName);
				return _contentHeader = ProtoHeader.Get<TContentHeader>(mainPart.FileInfo, mainPart.StartHeader);
			}
		}

		/// <summary>
		///     Gets an instance of the parent directory.
		/// </summary>
		public DirectoryInfo Directory
		{
			get { return FileInfo.Directory; }
		}

		/// <summary>
		///     Gets the StartHeader of this container.
		/// </summary>
		public StartHeader StartHeader
		{
			get { return _startHeader ?? (_startHeader = StartHeader.Extract(FileInfo)); }
		}

		protected MigrationContainerInfo(FileInfo fileInfo)
		{
			FileInfo = fileInfo;
		}

		/// <summary>
		///     Searches for the main part of a parted MigrationContainer.
		/// </summary>
		/// <param name="searchOption">The option to be used when searching.</param>
		/// <param name="ignoreFileExtension">
		///     If set to false, only files with a matching
		///     file extension will be considered.
		/// </param>
		/// <returns>The main part if found, else null.</returns>
		public TChild FindMainPart(SearchOption searchOption = SearchOption.TopDirectoryOnly, bool ignoreFileExtension = false)
		{
			if (IsMainPart()) return (TChild)this;
			var pattern = ignoreFileExtension
				? "*"
				: string.Format("*{0}", MigrationContainer.GetContainerExtension<TChild, TContentHeader, TFsInfo>());
			var containers =
				Directory.GetFiles(pattern, searchOption)
						 .Where(file => file.IsMigrationContainer())
						 .Select(file => ((TChild)Activator.CreateInstance(typeof(TChild), file)))
						 .ToArray();
			return !containers.Any()
				? null
				: containers.FirstOrDefault(c => c.IsMainPart() && c.StartHeader.ContainerId == StartHeader.ContainerId);
		}

		/// <summary>
		///     Gets all secondary MigrationContainer files, that belong together.
		/// </summary>
		/// <param name="searchOption">The option to be used when searching.</param>
		/// <param name="ignoreFileExtension">
		///     If set to false, only files with a matching
		///     file extension will be considered.
		/// </param>
		/// <returns>
		///     All parts that have the same container ID as the current container, excluding
		///     the main part.
		/// </returns>
		public IList<TChild> FindRelatedParts(SearchOption searchOption = SearchOption.TopDirectoryOnly,
											  bool ignoreFileExtension = false)
		{
			if (!FileInfo.IsMigrationContainer()) throw new InvalidContainerException();

			var pattern = ignoreFileExtension
				? "*"
				: string.Format("*{0}.part*", MigrationContainer.GetContainerExtension<TChild, TContentHeader, TFsInfo>());
			var relatedParts =
				Directory.GetFiles(pattern, searchOption)
						 .Where(file => file.IsMigrationContainer())
						 .Select(file => ((TChild)Activator.CreateInstance(typeof(TChild), file)))
						 .ToList();

			return !relatedParts.Any()
				? relatedParts
				: relatedParts.Where(c => c.StartHeader.ContainerId == StartHeader.ContainerId).ToList();
		}

		/// <summary>
		///     Gets all parts of the MigrationContainer including the current part.
		/// </summary>
		/// <param name="searchOption">The option to be used when searching.</param>
		/// <returns>A sorted list of all found container files.</returns>
		public List<FileInfo> GetAllParts(SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			var allParts = new List<FileInfo>();
			var mainPart = FindMainPart(searchOption).FileInfo;
			if (mainPart != null) allParts.Add(mainPart);
			var relatedParts = FindRelatedParts(searchOption);
			if (relatedParts != null) allParts.AddRange(relatedParts.Select(c => c.FileInfo).OrderBy(p => p.Name));
			return allParts;
		}

		/// <summary>
		///     Gets a value indicating whether this file is the last part of a parted
		///     container.
		/// </summary>
		/// <returns>
		///     true if this file is the last of n parts; else false.
		/// </returns>
		public bool IsLastPart()
		{
			return StartHeader.PartNumber == StartHeader.Parts;
		}

		/// <summary>
		///     Gets a value indicating whether this file is the main part of a parted
		///     container.
		/// </summary>
		/// <returns>
		///     true if this file is the first of n parts; else false.
		/// </returns>
		public bool IsMainPart()
		{
			return StartHeader.PartNumber == 0;
		}

		/// <summary>
		///     Gets a value indicating whether this file is only a partition of a full
		///     migration container.
		/// </summary>
		/// <returns>
		///     true if this file is just one of several parts, which represent the whole
		///     container, else false.
		/// </returns>
		public bool IsParted()
		{
			return StartHeader.Parts > 1;
		}

		/// <summary>
		///     Validates the migration container against its checksum.
		/// </summary>
		public bool IsValid()
		{
			using (var md5 = MD5.Create())
			using (var fileStream = FileInfo.OpenRead())
			{
				// skip magic numbers and checksum
				fileStream.Position = StartHeader.MagicNumbers.Count + StartHeader.MD5_LENGTH;

				var hash = md5.ComputeHash(fileStream);
				return StartHeader.Md5Hash.SequenceEqual(hash);
			}
		}

		protected virtual IContainerBody CreateBodyInstance(List<FileInfo> allParts)
		{
			return new ContainerBody(BodyPosition, allParts, Configuration.ContentBufferSize);
		}
	}
}
