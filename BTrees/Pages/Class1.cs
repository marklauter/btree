namespace BTrees.Pages
{
    /*
     * i have this idea where every value just gets appended to end of the values array and the key is inserted in order into the key array as a tuple (key, valueIndex)
     */
    //public interface ISortedList<TKey, TValue>
    //    : ISizeable
    //    where TKey : ISizeable, IComparable<TKey>
    //    where TValue : ISizeable, IComparable<TValue>
    //{
    //    int Length { get; }
    //    int Count { get; }
    //    int IndexOf(TKey key);
    //    Span<TValue> Values(TKey key);
    //    void Insert(TKey key, TValue value);

    //    /// <summary>
    //    /// returns only the elements contained in the list, not the whole backing array
    //    /// </summary>
    //    Span<KeyValueTuple<TKey, TValue>> AsSpan();

    //    /// <summary>
    //    /// range into the elements contained in the list, not the whole backing array
    //    /// ex: if start index on general list is 10, then range.Start.Value of 5 will span from 15
    //    /// </summary>
    //    Span<KeyValueTuple<TKey, TValue>> AsSpan(Range range);
    //}

    //public sealed class RightOptimizedSortedList<TKey, TValue>
    //    : ISortedList<TKey, TValue>
    //    where TKey : ISizeable, IComparable<TKey>
    //    where TValue : ISizeable, IComparable<TValue>
    //{
    //    private KeyValueTuple<TKey, TValue>[] items;
    //    private readonly int start;

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public RightOptimizedSortedList()
    //        : this(16)
    //    {
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public RightOptimizedSortedList(int length)
    //    {
    //        this.start = -1;
    //        this.Length = length;
    //        this.Count = 0;
    //        this.items = new KeyValueTuple<TKey, TValue>[length];
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    private RightOptimizedSortedList(RightOptimizedSortedList<TKey, TValue> list)
    //    {
    //        this.start = list.start;
    //        this.Count = list.Count;
    //        this.Length = list.Count;
    //        this.items = this.items.AsSpan().ToArray();
    //    }

    //    public int Length { get; }
    //    public int ByteSize => throw new NotImplementedException(); //this.AsSpan().Sum(kvt => kvt.Size);
    //    public int Count { get; }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public Span<KeyValueTuple<TKey, TValue>> AsSpan()
    //    {
    //        return this.items[this.start..(this.Count - 1)];
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public Span<KeyValueTuple<TKey, TValue>> AsSpan(Range range)
    //    {
    //        var start = this.start + range.Start.Value;
    //        var end = start + range.End.Value - range.Start.Value;
    //        return start < end && end < this.Count
    //            ? this.items[start..end]
    //            : throw new ArgumentOutOfRangeException(nameof(end));
    //    }

    //    public int IndexOf(TKey key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Insert(TKey key, TValue value)
    //    {
    //        this.items = Array.Empty<KeyValueTuple<TKey, TValue>>();
    //        throw new NotImplementedException();
    //    }

    //    public Span<TValue> Values(TKey key)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
