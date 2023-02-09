using System.Collections.Immutable;
using System.Diagnostics;

namespace BTrees.Pages
{
    // todo: unqiue or not should be implmeneted with an injected strategy

    [DebuggerDisplay(nameof(LeafPage<TKey, TValue>))]
    internal sealed class LeafPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public static IPage<TKey, TValue> Empty(int size)
        {
            return new LeafPage<TKey, TValue>(size);
        }

        public static IPage<TKey, TValue> Create(
            int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<TValue> values)
        {
            return new LeafPage<TKey, TValue>(size, keys, values);
        }

        private readonly ImmutableArray<TKey> keys;
        private readonly ImmutableArray<TValue> values;

        private LeafPage(
            int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<TValue> values)
            : base(size)
        {
            this.keys = keys;
            this.values = values;
        }

        private LeafPage(int size)
            : base(size)
        {
            this.keys = ImmutableArray<TKey>.Empty;
            this.values = ImmutableArray<TValue>.Empty;
        }

        public override int Count => this.keys.Length;
        public override TKey MinKey => this.Count != 0 ? this.keys[0] : throw new InvalidOperationException($"{nameof(this.MinKey)} is undefined when Count == 0");
        public override TKey MaxKey => this.Count != 0 ? this.keys[^1] : throw new InvalidOperationException($"{nameof(this.MaxKey)} is undefined when Count == 0");

        public override int BinarySearch(TKey key)
        {
            return ImmutableArray.BinarySearch(this.keys, key);
        }

        public override bool ContainsKey(TKey key)
        {
            return this.BinarySearch(key) >= 0;
        }

        public override IPage<TKey, TValue> Delete(TKey key)
        {
            var index = this.BinarySearch(key);
            return index < 0
                ? this
                : new LeafPage<TKey, TValue>(
                    this.Size,
                    this.keys.RemoveAt(index),
                    this.values.RemoveAt(index));
        }

        public override IPage<TKey, TValue> Fork()
        {
            return new LeafPage<TKey, TValue>(
                this.Size,
                this.keys,
                this.values);
        }

        public override IPage<TKey, TValue> Insert(TKey key, TValue value)
        {
            var index = this.BinarySearch(key);
            return index < 0
                ? new LeafPage<TKey, TValue>(
                    this.Size,
                    this.keys.Insert(~index, key),
                    this.values.Insert(~index, value))
                : throw new InvalidOperationException($"key {key} already exists");
        }

        public override IPage<TKey, TValue> Merge(IPage<TKey, TValue> page)
        {
            return page is null
                ? throw new ArgumentNullException(nameof(page))
                : page is LeafPage<TKey, TValue> leafPage
                    ? this.CompareTo(page) <= 0
                        ? new LeafPage<TKey, TValue>(
                            this.Size,
                            this.keys.AddRange(leafPage.keys),
                            this.values.AddRange(leafPage.values))
                        : page.Merge(this)
                    : throw new InvalidOperationException($"{nameof(page)} was wrong type: {page.GetType().Name}. Expected {nameof(LeafPage<TKey, TValue>)}");
        }

        public override (IPage<TKey, TValue> leftPage, IPage<TKey, TValue> rightPage, TKey pivotKey) Split()
        {
            var middle = this.Count / 2;

            return (
                new LeafPage<TKey, TValue>(
                    this.Size,
                    this.keys[..middle],
                    this.values[..middle]),
                new LeafPage<TKey, TValue>(
                    this.Size,
                    this.keys[middle..this.Count],
                    this.values[middle..this.Count]),
                this.keys[middle]);
        }

        public override bool TryRead(TKey key, out TValue? value)
        {
            var index = this.BinarySearch(key);
            var found = index >= 0;
            value = found
                ? this.values[index]
                : default;

            return found;
        }

        public override IPage<TKey, TValue> Update(TKey key, TValue value)
        {
            var index = this.BinarySearch(key);
            return index >= 0
                ? new LeafPage<TKey, TValue>(
                    this.Size,
                    this.keys,
                    this.values.SetItem(index, value))
                : throw new KeyNotFoundException($"{key}");
        }
    }
}
