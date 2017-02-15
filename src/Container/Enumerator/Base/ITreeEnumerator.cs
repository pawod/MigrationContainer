using System;
using System.Collections;

namespace Pawod.MigrationContainer.Container.Enumerator.Base
{
    public interface ITreeEnumerator<out TNode, out TLeaf> : IDisposable, IEnumerator
    {
        new ITreeElement<TNode, TLeaf> Current { get; }
    }
}