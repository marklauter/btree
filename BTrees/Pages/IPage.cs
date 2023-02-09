namespace BTrees.Pages
{
    internal interface IPage<TKey, TValue>
        : IComparable<IPage<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        int Count { get; }
        bool IsEmpty { get; }
        bool IsOverflow { get; }
        bool IsUnderflow { get; }
        int Size { get; }
        TKey MinKey { get; }
        TKey MaxKey { get; }

        IPage<TKey, TValue> Fork();
        IPage<TKey, TValue> Merge(IPage<TKey, TValue> page);
        (IPage<TKey, TValue> leftPage, IPage<TKey, TValue> rightPage, TKey pivotKey) Split();
        int BinarySearch(TKey key);
        bool ContainsKey(TKey key);
        IPage<TKey, TValue> Delete(TKey key);
        IPage<TKey, TValue> Insert(TKey key, TValue value);
        IPage<TKey, TValue> Update(TKey key, TValue value);
        bool TryRead(TKey key, out TValue? value);
    }
}
