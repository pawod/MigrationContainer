namespace DataMigratorTest.Container.FileContainer
{
	using Base;
	using Validation.FileContainer;

	public class PartedNtfsFileContainerTest : NtfsFileContainerTestInit
	{
		public PartedNtfsFileContainerTest(string fileName) : base(fileName, new PartedNtfsFileContainerValidator())
		{
		}
	}
}
