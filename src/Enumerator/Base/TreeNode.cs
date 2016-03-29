namespace DataMigrator.Enumerator.Base
{
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
    using System.Collections.Generic;

    public abstract class TreeNode<TNode, TLeaf> : ITreeNode<TNode, TLeaf>
    {
        private int _leafIndex;
        private int _nodeIndex;
        public TNode CurrentChildNode { get; private set; }
        public TLeaf CurrentLeaf { get; private set; }
        protected abstract IList<TLeaf> Leaves { get; }
        protected abstract IList<TNode> Nodes { get; }
        public TNode Value { get; private set; }

        protected TreeNode(TNode node)
        {
            Value = node;
        }

        public bool MoveNextLeaf()
        {
            if (Leaves == null || Leaves.Count <= 0 || _leafIndex > Leaves.Count - 1) return false;
            CurrentLeaf = Leaves[_leafIndex++];
            return true;
        }

        public bool MoveNextNode()
        {
            if (Nodes == null || Nodes.Count <= 0 || _nodeIndex > Nodes.Count - 1) return false;
            CurrentChildNode = Nodes[_nodeIndex++];
            return true;
        }

        public void Reset()
        {
            _leafIndex = 0;
            _nodeIndex = 0;
            CurrentLeaf = default(TLeaf);
        }
    }
}
