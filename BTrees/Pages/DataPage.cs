using System.Collections.Immutable;
using System.Diagnostics;

namespace BTrees.Pages
{
    // todo: unqiue or not should be implmeneted with an injected strategy

    [DebuggerDisplay(nameof(DataPage<TKey, TValue>))]
    internal sealed class DataPage<TKey, TValue>
        : IComparable<DataPage<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        public static DataPage<TKey, TValue> Empty(int size)
        {
            return new DataPage<TKey, TValue>(size);
        }

        public static DataPage<TKey, TValue> Create(
            int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<TValue> values)
        {
            return new DataPage<TKey, TValue>(size, keys, values);
        }

        private readonly int halfSize;
        private readonly ImmutableArray<TKey> keys;
        private readonly ImmutableArray<TValue> values;

        private DataPage(
            int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<TValue> values)
            : this(size)
        {

            this.keys = keys;
            this.values = values;
        }

        private DataPage(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), $"{nameof(size)} must be > 0");
            }

            this.Size = size;
            this.halfSize = size / 2;
            this.keys = ImmutableArray<TKey>.Empty;
            this.values = ImmutableArray<TValue>.Empty;
        }

        public int Count => this.keys.Length;
        public bool IsEmpty => this.Count == 0;
        public bool IsFull => this.Count == this.Size;
        public bool IsOverflow => this.Count > this.Size;
        public bool IsUnderflow => this.Count <= this.halfSize;
        public int Size { get; }
        public TKey MinKey => this.Count != 0 ? this.keys[0] : throw new InvalidOperationException($"{nameof(this.MinKey)} is undefined when Count == 0");
        public TKey MaxKey => this.Count != 0 ? this.keys[^1] : throw new InvalidOperationException($"{nameof(this.MaxKey)} is undefined when Count == 0");

        #region structural
        public DataPage<TKey, TValue> Merge(DataPage<TKey, TValue> page)
        {
            return page is null
                ? throw new ArgumentNullException(nameof(page))
                : page is DataPage<TKey, TValue> leafPage
                    ? this.CompareTo(page) <= 0
                        ? new DataPage<TKey, TValue>(
                            this.Size,
                            this.keys.AddRange(leafPage.keys),
                            this.values.AddRange(leafPage.values))
                        : page.Merge(this)
                    : throw new InvalidOperationException($"{nameof(page)} was wrong type: {page.GetType().Name}. Expected {nameof(DataPage<TKey, TValue>)}");
        }

        public (DataPage<TKey, TValue> leftPage, DataPage<TKey, TValue> rightPage, TKey pivotKey) Split()
        {
            var middle = this.Count / 2;

            return (
                new DataPage<TKey, TValue>(
                    this.Size,
                    this.keys[..middle],
                    this.values[..middle]),
                new DataPage<TKey, TValue>(
                    this.Size,
                    this.keys[middle..this.Count],
                    this.values[middle..this.Count]),
                this.keys[middle]);
        }
        #endregion

        #region reads
        public int BinarySearch(TKey key)
        {
            return ImmutableArray
                .BinarySearch(this.keys, key);
        }

        public bool ContainsKey(TKey key)
        {
            var index = this.BinarySearch(key);
            return index >= 0 && index < this.Count;
        }

        public bool TryRead(TKey key, out TValue? value)
        {
            var index = this.BinarySearch(key);
            var found = index >= 0;
            value = found
                ? this.values[index]
                : default;

            return found;
        }

        public int CompareTo(DataPage<TKey, TValue>? other)
        {
            return other is null
                ? -1
                : this == other
                    ? 0
                    : this.MinKey.CompareTo(other.MinKey);
        }
        #endregion

        #region writes
        public bool TryDelete(TKey key, out DataPage<TKey, TValue> page)
        {
            var index = this.BinarySearch(key);
            var deleted = index >= 0 && index < this.Count;
            page = deleted
                ? new DataPage<TKey, TValue>(
                    this.Size,
                    this.keys.RemoveAt(index),
                    this.values.RemoveAt(index))
                : this;

            return deleted;
        }

        public bool TryInsert(TKey key, TValue value, out DataPage<TKey, TValue> page)
        {
            var index = this.BinarySearch(key);
            var inserted = index < 0;
            page = inserted
                ? new DataPage<TKey, TValue>(
                    this.Size,
                    this.keys.Insert(~index, key),
                    this.values.Insert(~index, value))
                : this;

            return inserted;
        }

        public bool TryUpdate(TKey key, TValue value, out DataPage<TKey, TValue> page)
        {
            var index = this.BinarySearch(key);
            var updated = index >= 0 && index < this.Count;
            page = updated
                ? new DataPage<TKey, TValue>(
                    this.Size,
                    this.keys,
                    this.values.SetItem(index, value))
                : this;

            return updated;
        }
        #endregion
    }
}
