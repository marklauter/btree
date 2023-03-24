using BTrees.Types;

namespace BTrees.Pages
{
    internal readonly partial struct DataPage<TKey, TValue> where TKey : IDbType, IComparable<TKey>
        where TValue : IDbType, IComparable<TValue>
    {
        public readonly record struct SplitResult(
            DataPage<TKey, TValue> LeftPage,
            DataPage<TKey, TValue> RightPage,
            TKey PivotKey)
        {
        }
    }
}
