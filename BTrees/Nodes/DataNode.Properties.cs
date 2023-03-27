using BTrees.Pages;
using BTrees.Types;

namespace BTrees.Nodes
{
    internal sealed partial class DataNode<TKey, TValue>
        : INode<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        private DataPage<TKey, TValue> Page => Volatile.Read(ref this.pageAndSibling).Page;
        public INode<TKey, TValue>? RightSibling => Volatile.Read(ref this.pageAndSibling).RightSibling;

        public int MaxSize { get; }
        public int HalfSize { get; }
        public int Length => this.Page.Length;
        public int Size => this.Page.Size;
        public bool IsOverflow => this.Size >= this.MaxSize;
        public bool IsUnderflow => this.Size < this.HalfSize;
        public bool HasRightSibling => this.RightSibling is not null;
    }
}
