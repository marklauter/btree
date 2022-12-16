namespace BTrees
{
    public interface IBTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        long Count { get; }
        int Depth { get; }
        void Write(TKey key, TValue value);
        bool TryRead(TKey key, out TValue? value);
    }
}
