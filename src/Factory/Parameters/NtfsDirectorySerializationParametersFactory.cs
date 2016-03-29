namespace DataMigrator.Factory.Parameters
{
	using System.Collections.Generic;
	using System.IO;
	using Base.Parameters;
	using DataMigrator.Container.NtfsDirectoryContainer;
	using Serialization.Partitioning.DirectoryContainer;

	public class NtfsDirectorySerializationParametersFactory :
		SerializationParametersFactory<NtfsDirectoryContainerInfo, NtfsDirectoryHeader, DirectoryInfo>
	{
		public NtfsDirectorySerializationParametersFactory() : base(new NtfsDirectoryContainerPartitioner())
		{
		}

		protected override NtfsDirectoryHeader CreateContentHeader(DirectoryInfo sourceInfo,
																   long nextHeaderLength,
																   IList<string> filter = null)
		{
			var directoryHeader = new NtfsDirectoryHeader(0, nextHeaderLength);
			directoryHeader.InitializeFrom(sourceInfo, ContainerConfiguration.Filter);
			return directoryHeader;
		}
	}
}
