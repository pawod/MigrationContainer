namespace DataMigratorTest.Container.DirectoryContainer
{
	using Base;
	using Validation.DirectoryContainer;

	public class SingleNtfsDirectoryContainerTest : NtfsDirectoryContainerTestInit
	{
		public SingleNtfsDirectoryContainerTest(string dirName) : base(dirName, new SingleNtfsDirectoryContainerValidator())
		{
		}
	}
}
