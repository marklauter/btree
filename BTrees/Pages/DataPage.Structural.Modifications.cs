using BTrees.Types;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    internal readonly partial struct DataPage<TKey, TValue>
        : IComparable<DataPage<TKey, TValue>>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SplitResult Split()
        {
            if (this.tuples.Length < 2)
            {
                throw new InvalidOperationException("Can't split page with less than two elements.");
            }

            var length = this.Length;
            var middle = length >> 1;

            return new SplitResult(
                new DataPage<TKey, TValue>(this.tuples[..middle]),
                new DataPage<TKey, TValue>(this.tuples[middle..length]));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataPage<TKey, TValue> Merge(DataPage<TKey, TValue> otherPage)
        {
            return this.tuples[0].CompareTo(otherPage.tuples[0]) < 0
                ? new DataPage<TKey, TValue>(this.tuples.AddRange(otherPage.tuples).Sort())
                : new DataPage<TKey, TValue>(otherPage.tuples.AddRange(this.tuples).Sort());
        }
    }
}
