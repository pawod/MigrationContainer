namespace Pawod.MigrationContainer.Container.Enumerator.Base
{
    public interface ITreeElement<out TNode, out TLeaf>
    {
        TLeaf Leaf { get; }
        TNode Node { get; }
        bool IsLeaf();
        bool IsNode();
    }
}