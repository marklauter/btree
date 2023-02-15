namespace BTrees.Nodes
{
    internal interface INode<TKey>
        where TKey : IComparable<TKey>
    {
        TKey MinKey { get; }
        TKey MaxKey { get; }

        int BinarySearch(TKey key);
        bool ContainsKey(TKey key);
    }

    internal interface INode<TKey, TValue>
        : INode<TKey>
        where TKey : IComparable<TKey>
    {
        int Count { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }
        bool IsOverflow { get; }
        bool IsUnderflow { get; }
        int Size { get; }

        INode<TKey, TValue> Merge(INode<TKey, TValue> node);
        (INode<TKey, TValue> left, INode<TKey, TValue> right, TKey pivotKey) Split();
        void Delete(TKey key);
        void Insert(TKey key, TValue value);
        bool TryRead(TKey key, out TValue? value);
        void Update(TKey key, TValue value);
    }
}
