namespace DataMigrator.Factory.Parameters
{
	using System.Collections.Generic;
	using System.IO;
	using Base.Parameters;
	using DataMigrator.Container.FileContainer;
	using DataMigrator.Container.FileContainer.Header;
	using Serialization.Partitioning.FileContainer;

	public class NtfsFileSerializationParametersFactory :
		SerializationParametersFactory<NtfsFileContainerInfo, NtfsFileHeader, FileInfo>
	{
		public NtfsFileSerializationParametersFactory() : base(new NtfsFileContainerPartitioner())
		{
		}

		protected override NtfsFileHeader CreateContentHeader(FileInfo sourceInfo,
															  long nextHeaderLength,
															  IList<string> filter = null)
		{
			var fileHeader = new NtfsFileHeader(0, nextHeaderLength);
			fileHeader.AssociateWith(sourceInfo, filter);
			return fileHeader;
		}
	}
}
