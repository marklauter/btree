using System.Collections.Immutable;

namespace BTrees.Nodes
{
    internal interface INode<TKey, TValue>
        where TKey : struct, IComparable<TKey>
        where TValue : IComparable<TValue>
    {
        public readonly record struct SplitResult(
            INode<TKey, TValue> LeftNode,
            INode<TKey, TValue> RightNode,
            TKey PivotKey)
        {
        }

        int Length { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }
        bool IsOverflow { get; }
        bool IsUnderflow { get; }
        int Size { get; }
        TKey PivotKey { get; }

        INode<TKey, TValue>? Parent { get; }
        INode<TKey, TValue>? LeftSibling { get; }
        INode<TKey, TValue>? RightSibling { get; }

        bool ContainsKey(TKey key);

        INode<TKey, TValue> Fork();
        INode<TKey, TValue> Merge(INode<TKey, TValue> node);
        SplitResult Split();

        void Delete(TKey key);
        void Delete(TKey key, TValue value);
        void Insert(TKey key, TValue value);
        ImmutableArray<TValue> Read(TKey key);
    }
}
