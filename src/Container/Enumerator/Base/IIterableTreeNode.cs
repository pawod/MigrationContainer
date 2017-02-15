using System.Collections.Generic;

namespace Pawod.MigrationContainer.Container.Enumerator.Base
{
    public interface IIterableTreeNode<TNode, TLeaf>
    {
        TNode CurrentChildNode { get; }
        TLeaf CurrentLeaf { get; }
        IList<TLeaf> Leaves { get; }
        IList<TNode> Nodes { get; }
        TNode Value { get; }
        bool MoveNextLeaf();
        bool MoveNextNode();
        void ResetCursor();
    }
}