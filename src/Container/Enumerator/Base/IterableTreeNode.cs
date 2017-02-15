using System.Collections.Generic;
using System.Linq;

namespace Pawod.MigrationContainer.Container.Enumerator.Base
{
    public abstract class IterableTreeNode<TNode, TLeaf> : IIterableTreeNode<TNode, TLeaf>
    {
        protected readonly IEnumerable<string> Filter;

        private IList<TNode> _childNodes;
        private int _leafIndex;
        private IList<TLeaf> _leaves;
        private int _nodeIndex;

        protected IterableTreeNode(TNode node, IEnumerable<string> filter)
        {
            Value = node;
            Filter = filter;
        }

        public TNode CurrentChildNode { get; private set; }
        public TLeaf CurrentLeaf { get; private set; }


        public IList<TLeaf> Leaves => _leaves ?? (_leaves = GetLeaves(Value).Where(PassesFilter).ToList());


        public bool MoveNextLeaf()
        {
            if (Leaves == null || !Leaves.Any() || _leafIndex > Leaves.Count - 1) return false;
            CurrentLeaf = Leaves[_leafIndex++];
            return true;
        }

        public bool MoveNextNode()
        {
            if (Nodes == null || !Nodes.Any() || _nodeIndex > Nodes.Count - 1) return false;
            CurrentChildNode = Nodes[_nodeIndex++];
            return true;
        }

        public IList<TNode> Nodes => _childNodes ?? (_childNodes = GetChildNodes(Value).Where(PassesFilter).ToList());


        public void ResetCursor()
        {
            _leafIndex = 0;
            _nodeIndex = 0;
            CurrentLeaf = default(TLeaf);
        }

        public TNode Value { get; }

        protected abstract IList<TNode> GetChildNodes(TNode node);


        protected abstract IList<TLeaf> GetLeaves(TNode node);

        protected abstract bool PassesFilter(TNode node);

        protected abstract bool PassesFilter(TLeaf leaf);
    }
}