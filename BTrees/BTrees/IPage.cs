//namespace BTrees
//{
//    internal interface IPage<TKey, TValue>
//        where TKey : IComparable<TKey>
//    {
//        int Count { get; }
//        int Size { get; }
//        bool IsEmpty { get; }
//        bool IsOverflow { get; }
//        (IPage<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value);
//        TKey[] Keys { get; }
//    }
//}
