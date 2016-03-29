namespace DataMigrator.Enumerator.Base
{
    public interface ITreeNode<out TNode, out TLeaf>
    {
        TNode CurrentChildNode { get; }
        TLeaf CurrentLeaf { get; }
        TNode Value { get; }
        bool MoveNextLeaf();
        bool MoveNextNode();
        void Reset();
    }
}
