namespace BTrees.Nodes
{
    internal interface INode<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        int Count { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }
        bool IsOverflow { get; }
        bool IsUnderflow { get; }
        int Size { get; }
        TKey MinKey { get; }
        TKey MaxKey { get; }

        int BinarySearch(TKey key);
        INode<TKey, TValue> Merge(INode<TKey, TValue> node);
        (INode<TKey, TValue> left, INode<TKey, TValue> right, TKey pivotKey) Split();
        bool ContainsKey(TKey key);
        void Delete(TKey key);
        void Insert(TKey key, TValue value);
        bool TryRead(TKey key, out TValue? value);
        void Update(TKey key, TValue value);
    }
}
