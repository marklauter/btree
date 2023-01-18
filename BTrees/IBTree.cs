using BTrees.Ranges;

namespace BTrees
{
    public interface IBTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        long Count { get; }

        int Height { get; }

        bool TryWrite(TKey key, TValue value);

        bool TryDelete(TKey key);

        bool TryRead(TKey key, out TValue? value);

        IEnumerable<TValue> Read(OpenRange<TKey> range);

        IEnumerable<TValue> Read(ClosedRange<TKey> range);
    }
}
