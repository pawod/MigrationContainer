namespace DataMigrator.Helper
{
	using System.Collections.Generic;
	using System.IO;
    using Enumerator.Filesystem;

    public static class DirectoryInfoExtensions
    {
        public static long CalculateSize(this DirectoryInfo dir, IList<string> filter = null)
        {
            var sum = 0L;
            var enumerator = dir.GetEnumerator(filter);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsLeaf()) sum += enumerator.Current.Leaf.Length;
            }
            return sum;
        }

        public static FilesystemEnumerator GetEnumerator(this DirectoryInfo directoryInfo, IList<string> filter = null)
        {
            var rootNode = new FilesystemTreeNode(directoryInfo, filter);
            return new FilesystemEnumerator(rootNode, filter);
        }
    }
}
