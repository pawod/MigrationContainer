namespace DataMigratorTest.Container.DirectoryContainer
{
	using Base;
	using Validation.DirectoryContainer;

	public class PartedNtfsDirectoryContainerTest : NtfsDirectoryContainerTestInit
	{
		public PartedNtfsDirectoryContainerTest(string dirName) : base(dirName, new PartedNtfsDirectoryContainerValidator())
		{
		}
	}
}
