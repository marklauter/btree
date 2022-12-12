namespace BTrees
{
    internal interface IPage<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        int Count { get; }
        int Size { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }
        (IPage<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value);
    }
}
