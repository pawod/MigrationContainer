namespace DataMigrator.Enumerator.Base
{
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

    /// <summary>
    ///     Represents the tree element you get after one iteration of a TreeEnumerator.
    ///     It either holds a Leaf or a Node.
    /// </summary>
    /// <typeparam name="TNode">The type of nodes contained in the tree.</typeparam>
    /// <typeparam name="TLeaf">The type of leves contained in the tree.</typeparam>
    public class TreeElement<TNode, TLeaf> : ITreeElement<TNode, TLeaf>
    {
        public TLeaf Leaf { get; private set; }
        public TNode Node { get; private set; }

        public TreeElement(TNode node)
        {
            Node = node;
        }

        public TreeElement(TLeaf leaf)
        {
            Leaf = leaf;
        }

        public bool IsLeaf()
        {
            return Leaf != null;
        }

        public bool IsNode()
        {
            return Node != null;
        }
    }
}
