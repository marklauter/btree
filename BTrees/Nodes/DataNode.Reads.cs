using BTrees.Types;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace BTrees.Nodes
{
    internal sealed partial class DataNode<TKey, TValue>
        : INode<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return this.Page.Count();
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public INode<TKey, TValue> Fork()
        {
            return new DataNode<TKey, TValue>(Volatile.Read(ref this.pageAndSibling), this.MaxSize);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            return this.Page.ContainsKey(key);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TValue> Read(TKey key)
        {
            return this.Page.Read(key);
        }

        [Pure]
        public IEnumerable<(TKey Key, TValue Value)> Read(TKey leftBoundingKey, TKey rightBoundingKey)
        {
            var pageAndSibling = Volatile.Read(ref this.pageAndSibling);

            var leftBoundingIndex = pageAndSibling
                .Page
                .IndexOf(leftBoundingKey);

            var rightBoundingIndex = pageAndSibling
                .Page
                .IndexOf(rightBoundingKey);

            leftBoundingIndex = leftBoundingIndex >= 0 ? leftBoundingIndex : ~leftBoundingIndex;
            rightBoundingIndex = (rightBoundingIndex >= 0 ? rightBoundingIndex : ~rightBoundingIndex) + 1;

            var values = pageAndSibling
                .Page
                .Read(leftBoundingIndex..rightBoundingIndex);

            // if rightBound key exceeded right edge of the current page then read from the right sibling
            return rightBoundingIndex == pageAndSibling.Page.Length && pageAndSibling.RightSibling is not null
                ? values.Union(pageAndSibling.RightSibling.Read(leftBoundingKey, rightBoundingKey))
                : values;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(INode<TKey, TValue>? other)
        {
            return other is null
                ? 1
                : other is DataNode<TKey, TValue> dataNode
                    ? this.Page.CompareTo(dataNode.Page)
                    : throw new InvalidOperationException($"{nameof(other)} was wrong type: {other.GetType().Name}. Expected {nameof(DataNode<TKey, TValue>)}");
        }
    }
}
