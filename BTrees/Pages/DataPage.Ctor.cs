using BTrees.Types;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace BTrees.Pages
{
    // todo: DataPage: basing full/overflow/underflow on data size was misguided
    // todo: DataPage: better to go back to count and let the btree decide the value of K based on the size of the data types
    // todo: DataPage: this means you can't store strings in the btree, but this is okay as the actual data to be stored in the btree will be page id values
    // todo: DataPage: review ImmutableInterlocked to see if node and page can be reunified into a single class
    // todo: DataPage: the reason I say this is the main reason for the decoupling was to allow node to lock on immutable datapage changes, but we can just lock the tuples array with ImmutableInterlocked instead

    internal readonly partial struct DataPage<TKey, TValue>
        : IComparable<DataPage<TKey, TValue>>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        public static DataPage<TKey, TValue> Empty { get; } = new DataPage<TKey, TValue>(ImmutableArray<KeyValuesTuple>.Empty);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DataPage(ImmutableArray<KeyValuesTuple> tuples)
        {
            this.tuples = tuples;
            this.Size = tuples.Sum(t => t.Size);
            this.IsEmpty = tuples.IsEmpty;
            this.Length = tuples.Length;
            this.searchHigh = tuples.Length - 1;
            this.minKey = tuples.IsEmpty ? default : tuples[0].Key;
        }
    }
}
