using System;
using System.Collections;
using System.Collections.Generic;

namespace Pawod.MigrationContainer.Container.Enumerator.Base
{
    /// <summary>
    ///     Allows to iterate the nodes of an tree data structure. Leaves are visited first. Then the nodes are visited in a
    ///     depth first manner.
    /// </summary>
    /// <typeparam name="TNodeWrapper">The type of iterable wrapper for the nodes contained iside the tree.</typeparam>
    /// <typeparam name="TLeaf">The type of leaves the tree holds.</typeparam>
    /// <typeparam name="TNode">The type of nodes contained in the tree.</typeparam>
    public class TreeEnumerator<TNodeWrapper, TNode, TLeaf> : ITreeEnumerator<TNode, TLeaf>
        where TNodeWrapper : IterableTreeNode<TNode, TLeaf>
    {
        private TNodeWrapper _currentTreeNode;
        private readonly IList<string> _filter;
        private readonly TNode _rootNode;
        private readonly Stack<TNodeWrapper> _stack;

        public TreeEnumerator(TNode root, IList<string> filter = null)
        {
            _stack = new Stack<TNodeWrapper>();
            _rootNode = root;
            _filter = filter;
            _currentTreeNode = (TNodeWrapper) Activator.CreateInstance(typeof(TNodeWrapper), root, _filter);
        }

        public ITreeElement<TNode, TLeaf> Current { get; private set; }

        public virtual void Dispose()
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
                    _currentTreeNode = (TNodeWrapper) Activator.CreateInstance(typeof(TNodeWrapper), _currentTreeNode.CurrentChildNode, _filter);
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
            _currentTreeNode = (TNodeWrapper) Activator.CreateInstance(typeof(TNodeWrapper), _rootNode, _filter);
        }

        object IEnumerator.Current => Current;
    }
}