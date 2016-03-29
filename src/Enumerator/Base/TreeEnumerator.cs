namespace DataMigrator.Enumerator.Base
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///     Allows to iterate the nodes of an tree data structure. Leaves have a higher
    ///     priority than nodes. Nodes are iterated in a depth first manner.
    /// </summary>
    /// <typeparam name="TNode">The type of nodes the tree consists of.</typeparam>
    /// <typeparam name="TLeaf">The type of leaves the tree holds.</typeparam>
    public abstract class TreeEnumerator<TNode, TLeaf> : IEnumerator<ITreeElement<TNode, TLeaf>>
    {
        private readonly TNode _rootNode;
        private readonly Stack<ITreeNode<TNode, TLeaf>> _stack;
        private ITreeNode<TNode, TLeaf> _currentTreeNode;
		private readonly IList<string> _filter;
	    public ITreeElement<TNode, TLeaf> Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        protected TreeEnumerator(ITreeNode<TNode, TLeaf> root, IList<string> filter = null)
        {
            _stack = new Stack<ITreeNode<TNode, TLeaf>>();
            _rootNode = root.Value;
            _currentTreeNode = root;
	        _filter = filter;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            while (true)
            {
                if (Current == null)
                {
                    Current = new TreeElement<TNode, TLeaf>(_currentTreeNode.Value);
                    return true;
                }
                if (_currentTreeNode.MoveNextLeaf())
                {
                    Current = new TreeElement<TNode, TLeaf>(_currentTreeNode.CurrentLeaf);
                    return true;
                }
                if (_currentTreeNode.MoveNextNode())
                {
                    _stack.Push(_currentTreeNode);
                    _currentTreeNode = CreateTreeNode(_currentTreeNode.CurrentChildNode, _filter);
                    Current = new TreeElement<TNode, TLeaf>(_currentTreeNode.Value);
                    return true;
                }
                if (_stack.Count > 0)
                {
                    // return to previous node and continues with next element in row
                    _currentTreeNode = _stack.Pop();
                    continue;
                }
                return false;
            }
        }

        public void Reset()
        {
            _stack.Clear();
            Current = null;
            _currentTreeNode = CreateTreeNode(_rootNode, _filter);
        }

        protected abstract ITreeNode<TNode, TLeaf> CreateTreeNode(TNode node, IList<string> filter = null);
    }
}
