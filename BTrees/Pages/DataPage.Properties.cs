using BTrees.Types;
using System.Collections.Immutable;

namespace BTrees.Pages
{
    internal readonly partial struct DataPage<TKey, TValue>
        : IComparable<DataPage<TKey, TValue>>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        private readonly ImmutableArray<KeyValuesTuple> tuples;
        private readonly int searchHigh;
        private readonly TKey? minKey;

        public bool IsEmpty { get; }
        public int Length { get; }
        public int Size { get; }
        public TKey MinKey => this.IsEmpty || this.minKey is null
            ? throw new InvalidOperationException("Empty DataPage doesn't have MinKey")
            : this.minKey;
    }
}
