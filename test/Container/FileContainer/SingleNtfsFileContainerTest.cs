namespace DataMigratorTest.Container.FileContainer
{
	using Base;
	using Validation.FileContainer;

	public class SingleNtfsFileContainerTest : NtfsFileContainerTestInit
	{
		public SingleNtfsFileContainerTest(string fileName) : base(fileName, new SingleNtfsFileContainerValidator())
		{
		}
	}
}
