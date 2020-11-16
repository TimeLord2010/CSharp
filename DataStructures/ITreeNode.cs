using System.Collections.Generic;

namespace DataStructures {

    public interface ITreeNode <T> : ICollection<ITreeNode<T>> {

        ITreeNode<T> Parent { get; }
        T Content { get; }
        IEnumerable<ITreeNode<T>> Children { get; }

    }

}