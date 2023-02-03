using System.Collections.Immutable;
using System.Diagnostics;

namespace BTrees.Pages
{
    [DebuggerDisplay(nameof(LeafPage<TKey, TValue>))]
    internal class LeafPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public static IPage<TKey, TValue> Empty(int size)
        {
            return new LeafPage<TKey, TValue>(size);
        }

        private readonly ImmutableArray<TKey> keys;
        private readonly ImmutableArray<TValue> values;

        //public LeafPage(
        //    int size,
        //    TKey key,
        //    TValue value)
        //    : base(size)
        //{
        //    this.keys = ImmutableArray.Create(key);
        //    this.values = ImmutableArray.Create(value);
        //}

        private LeafPage(int size)
            : base(size)
        {
            this.keys = ImmutableArray<TKey>.Empty;
            this.values = ImmutableArray<TValue>.Empty;
        }

        private LeafPage(
            int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<TValue> values)
            : base(size)
        {
            this.keys = keys;
            this.values = values;
        }

        private LeafPage(
            int size,
            ReadOnlySpan<TKey> keys,
            ReadOnlySpan<TValue> values)
            : base(size)
        {
            this.keys = ImmutableArray.Create(keys);
            this.values = ImmutableArray.Create(values);
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
                    ? (IPage<TKey, TValue>)new LeafPage<TKey, TValue>(
                        this.Size,
                        this.keys.AddRange(leafPage.keys).Sort(),
                        this.values.AddRange(leafPage.values).Sort())
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

    //    [DebuggerDisplay("LeafPage {Count}")]
    //    internal class LeafPage<TKey, TValue>
    //        : Page<TKey, TValue>
    //        where TKey : IComparable<TKey>
    //    {
    //        internal readonly List<TValue> values;

    //        public override int Order => this.Size;

    //        #region CTOR
    //        public LeafPage(int size)
    //            : base(size)
    //        {
    //            this.values = new List<TValue>(size);
    //        }

    //        internal LeafPage(
    //            int size,
    //            Page<TKey, TValue> leftSibling)
    //            : base(
    //                  size,
    //                  leftSibling)
    //        {
    //            this.values = new List<TValue>(size);
    //        }
    //        #endregion

    //        internal override void Merge(Page<TKey, TValue> sourcePage)
    //        {
    //            var startIndex = this.Count;
    //            var endIndex = sourcePage.Count + startIndex;

    //            var keys = new Span<TKey>(this.Keys);
    //            var children = new Span<TValue>(this.values);

    //            var sourceKeys = new Span<TKey>(sourcePage.Keys);
    //            var sourceChildren = new Span<TValue>(((LeafPage<TKey, TValue>)sourcePage).values);

    //            var j = 0;
    //            for (var i = startIndex; i < endIndex; ++i)
    //            {
    //                keys[i] = sourceKeys[j];
    //                children[i] = sourceChildren[j];
    //                ++j;
    //            }

    //            this.Count = endIndex;
    //        }

    //        internal override Page<TKey, TValue> SelectSubtree(TKey key)
    //        {
    //            // this is the end of the traversal
    //            return this;
    //        }

    //        internal override (Page<TKey, TValue> newPage, TKey newPivotKey) Split()
    //        {
    //            var newPage = new LeafPage<TKey, TValue>(this.Size, this);
    //            var newKeys = new Span<TKey>(newPage.Keys);
    //            var newChildren = new Span<TValue?>(newPage.values);

    //            var keys = new Span<TKey>(this.Keys);
    //            var children = new Span<TValue?>(this.values);

    //            var count = this.Count;
    //            var newPivotIndex = count / 2;
    //            var j = 0;
    //            for (var i = newPivotIndex; i < count; ++i)
    //            {
    //                newKeys[j] = keys[i];
    //                newChildren[j] = children[i];
    //                ++j;
    //            }

    //            newPage.PivotKey = newKeys[0];
    //            newPage.Count = count - newPivotIndex;
    //            this.Count = newPivotIndex;

    //            return (newPage, newPage.PivotKey);
    //        }

    //        public override bool TryDelete(TKey key, out (bool merged, TKey? deprecatedPivotKey) mergeInfo)
    //        {
    //            return this.RemoveKey(key, out mergeInfo);
    //        }

    //        public override async Task<WriteResponse<TKey, TValue>> WriteAsync(
    //            TKey key,
    //            TValue value,
    //            CancellationToken cancellationToken)
    //        {
    //            var lockAquired = await this.TryAquireLockAsync(this.LockTimeout, cancellationToken);
    //            try
    //            {
    //                var writeResult = WriteResult.FailedToAquireLock;
    //                if (!lockAquired)
    //                {
    //                    return new WriteResponse<TKey, TValue>(writeResult);
    //                }

    //                if (!this.IsOverflow)
    //                {
    //                    writeResult = this.WriteInternal(key, value);
    //                    return new WriteResponse<TKey, TValue>(false, writeResult);
    //                }

    //                var count = this.Count;
    //                var index = this.IndexOfKey(key);
    //                var keyFound = index >= 0;
    //                var rightOnly = ~index == count;

    //                // split needs to create a new left page as well as a right page for copy on write - can't modify the current data
    //                var (newPage, newPivotKey) = keyFound
    //                    ? (null, default)
    //                    : rightOnly
    //                        ? (new LeafPage<TKey, TValue>(this.Size, this) { PivotKey = key }, key)
    //                        : this.Split();

    //                var destinationPage = keyFound
    //                    ? this
    //                    : rightOnly || key.CompareTo(newPivotKey) >= 0
    //                        ? ((LeafPage<TKey, TValue>?)newPage)
    //                        : this;

    //#pragma warning disable CS8602 // Dereference of a possibly null reference.
    //                writeResult = destinationPage.WriteInternal(key, value);
    //#pragma warning restore CS8602 // Dereference of a possibly null reference.
    //                return new WriteResponse<TKey, TValue>(true, writeResult, null, newPage);
    //            }
    //            finally
    //            {
    //                if (lockAquired)
    //                {
    //                    this.ReleaseLock();
    //                }
    //            }
    //        }

    //        private WriteResult WriteInternal(TKey key, TValue value)
    //        {
    //            if (this.IsEmpty)
    //            {
    //                this.Keys.Add(key);
    //                this.values.Add(value);
    //                return WriteResult.Inserted;
    //            }

    //            var index = this.Keys.BinarySearch(key);
    //            var keyNotFound = index < 0;

    //            index = keyNotFound
    //                ? ~index
    //                : index;

    //            var writeResult = keyNotFound
    //                ? WriteResult.Inserted
    //                : WriteResult.Updated;

    //            // todo: shift in-place will be replaced with page clone that skips a slot as part of copy-on-write updates
    //            if (keyNotFound && index != this.Count)
    //            {
    //                this.Keys.Insert(index, key);
    //                this.values.Insert(index, value);
    //            }
    //            else
    //            {
    //                this.Keys[index] = key;
    //                this.values[index] = value;
    //            }

    //            return writeResult;
    //        }

    //        public override bool TryRead(TKey key, out TValue? value)
    //        {
    //            value = default;
    //            var index = this.Keys.BinarySearch(key);
    //            if (index < 0)
    //            {
    //                return false;
    //            }

    //            value = this.values[index];
    //            return true;
    //        }
    //    }
}
