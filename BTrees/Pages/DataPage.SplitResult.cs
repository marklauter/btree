using BTrees.Types;

namespace BTrees.Pages
{
    public readonly partial struct DataPage<TKey, TValue>
        : IComparable<DataPage<TKey, TValue>>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        public readonly record struct SplitResult(
            DataPage<TKey, TValue> LeftPage,
            DataPage<TKey, TValue> RightPage)
        {
        }
    }
}
