using System.Collections.Immutable;
using System.Diagnostics;

namespace BTrees.Pages
{
    [DebuggerDisplay(nameof(PivotPage<TKey, TValue>))]
    internal class PivotPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly ImmutableArray<TKey> keys;
        private readonly ImmutableArray<IPage<TKey, TValue>> pages;

        public PivotPage(
            int size,
            TKey key,
            IPage<TKey, TValue> leftPage,
            IPage<TKey, TValue> rightPage)
            : base(size)
        {
            this.keys = ImmutableArray.Create(key);
            this.pages = ImmutableArray.Create(leftPage, rightPage);
        }

        private PivotPage(
            int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<IPage<TKey, TValue>> pages)
            : base(size)
        {
            this.keys = keys;
            this.pages = pages;
        }

        private PivotPage(
            int size,
            ReadOnlySpan<TKey> keys,
            ReadOnlySpan<IPage<TKey, TValue>> pages)
            : base(size)
        {
            this.keys = ImmutableArray.Create(keys);
            this.pages = ImmutableArray.Create(pages);
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
            index = index < 0
                ? ~index
                : index + 1;

            var subtree = this.pages[index];
            var page = subtree.Delete(key);

            return new PivotPage<TKey, TValue>(
                this.Size,
                this.keys,
                this.pages.SetItem(index, page));
        }

        public override IPage<TKey, TValue> Fork()
        {
            return new PivotPage<TKey, TValue>(
                this.Size,
                this.keys,
                this.pages);
        }

        public override IPage<TKey, TValue> Insert(TKey key, TValue value)
        {
            var index = this.BinarySearch(key);
            index = index < 0
                ? ~index
                : index + 1;

            var subtree = this.pages[index];
            var page = subtree.Insert(key, value);

            return new PivotPage<TKey, TValue>(
                this.Size,
                this.keys,
                this.pages.SetItem(index, page));
        }

        public override IPage<TKey, TValue> Merge(IPage<TKey, TValue> page)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (page is PivotPage<TKey, TValue> pivotPage)
            {
                var pages = this.pages.AddRange(pivotPage.pages).Sort();
                var keys = ImmutableArray.Create(
                    pages
                        .Take(pages.Length - 1)
                        .Select(p => p.MinKey)
                        .ToArray());

                return new PivotPage<TKey, TValue>(
                        this.Size,
                        keys,
                        pages);
            }

            throw new InvalidOperationException($"{nameof(page)} was wrong type: {page.GetType().Name}. Expected {nameof(PivotPage<TKey, TValue>)}");
        }

        public override (IPage<TKey, TValue> leftPage, IPage<TKey, TValue> rightPage, TKey pivotKey) Split()
        {
            var middle = this.Count / 2;
            var rightPageStart = middle + 1;

            return (
                new PivotPage<TKey, TValue>(
                    this.Size,
                    this.keys[..middle],
                    this.pages[..rightPageStart]),
                new PivotPage<TKey, TValue>(
                    this.Size,
                    this.keys[rightPageStart..this.Count],
                    this.pages[rightPageStart..this.Count]),
                this.keys[middle]);
        }

        public override bool TryRead(TKey key, out TValue? value)
        {
            var subtree = this.SelectSubtree(key);
            return subtree.TryRead(key, out value);
        }

        public override IPage<TKey, TValue> Update(TKey key, TValue value)
        {
            var index = this.BinarySearch(key);
            index = index < 0
                ? ~index
                : index + 1;

            var subtree = this.pages[index];
            var page = subtree.Update(key, value);

            return new PivotPage<TKey, TValue>(
                this.Size,
                this.keys,
                this.pages.SetItem(index, page));
        }

        private IPage<TKey, TValue> SelectSubtree(TKey key)
        {
            var index = this.BinarySearch(key);
            index = index < 0
                ? ~index
                : index + 1;

            return this.pages[index];
        }
    }

    //    [DebuggerDisplay("PivotPage {Count}")]
    //    internal class PivotPage<TKey, TValue>
    //        : Page<TKey, TValue>
    //        where TKey : IComparable<TKey>
    //    {
    //        internal readonly Page<TKey, TValue>[] subtrees;

    //        public override int Order => this.Size + 1;

    //        #region CTOR
    //        public PivotPage(int size)
    //            : base(size)
    //        {
    //            this.subtrees = new Page<TKey, TValue>[size + 1];
    //        }

    //        public PivotPage(
    //            int size,
    //            Page<TKey, TValue> leftPage,
    //            Page<TKey, TValue> rightPage,
    //            TKey pivotKey)
    //            : this(size)
    //        {
    //            this.Keys[0] = pivotKey;
    //            this.subtrees[0] = leftPage;
    //            this.subtrees[1] = rightPage;
    //            this.Count = 1;
    //        }

    //        internal PivotPage(int size, Page<TKey, TValue> leftSibling)
    //            : base(size, leftSibling)
    //        {
    //            this.subtrees = new Page<TKey, TValue>[size + 1];
    //        }
    //        #endregion

    //        private bool TryWriteInternal(TKey key, Page<TKey, TValue> value)
    //        {
    //            var index = this.BinarySearch(key);
    //            var shiftRequired = index < 0;
    //            index = index > 0
    //                    ? index
    //                    : ~index;

    //            shiftRequired = index != this.Count && shiftRequired;
    //            if (shiftRequired)
    //            {
    //                this.ShiftRight(index);
    //            }

    //            this.Keys[index] = key;
    //            this.subtrees[index + 1] = value;
    //            ++this.Count;

    //            return true;
    //        }

    //        protected override void ShiftLeft(int index)
    //        {
    //            for (var i = index; i < this.Count - 1; ++i)
    //            {
    //                this.Keys[i] = this.Keys[i + 1];
    //                this.subtrees[i + 1] = this.subtrees[i + 2];
    //            }
    //        }

    //        protected override void ShiftRight(int index)
    //        {
    //            for (var i = this.Count - 1; i >= index; --i)
    //            {
    //                this.Keys[i + 1] = this.Keys[i];
    //                this.subtrees[i + 2] = this.subtrees[i + 1];
    //            }
    //        }

    //        internal override void Merge(Page<TKey, TValue> sourcePage)
    //        {
    //            var startIndex = this.Count;
    //            var endIndex = sourcePage.Count + startIndex;
    //            var keys = new Span<TKey>(this.Keys);
    //            var children = new Span<Page<TKey, TValue>>(this.subtrees);
    //            var sourceKeys = new Span<TKey>(this.Keys);
    //            var sourceChildren = new Span<Page<TKey, TValue>>(this.subtrees);

    //            var j = 0;
    //            for (var i = startIndex; i < endIndex; ++i)
    //            {
    //                keys[i] = sourceKeys[j];
    //                children[i] = sourceChildren[j];
    //                ++j;
    //            }

    //            children[endIndex] = sourceChildren[j];

    //            this.Count = endIndex;
    //        }

    //        internal override Page<TKey, TValue> SelectSubtree(TKey key)
    //        {
    //            var index = this.BinarySearch(key);
    //            index = index < 0
    //                ? ~index
    //                : index + 1;

    //            return this.subtrees[index];
    //        }

    //        internal override (Page<TKey, TValue> newPage, TKey newPivotKey) Split()
    //        {
    //            var count = this.Count;
    //            var newPivotIndex = count / 2;
    //            var copyFromIndex = newPivotIndex + 1;

    //            var newPageCount = count - copyFromIndex;
    //            var subtreesLength = newPageCount + 1;
    //            var keys = new Span<TKey>(this.Keys, copyFromIndex, newPageCount);
    //            var subtrees = new Span<Page<TKey, TValue>?>(this.subtrees, copyFromIndex, subtreesLength);

    //            var newPage = new PivotPage<TKey, TValue>(this.Size, this);
    //            var newKeys = new Span<TKey>(newPage.Keys, 0, newPageCount);
    //            var newSubtrees = new Span<Page<TKey, TValue>?>(newPage.subtrees, 0, newPageCount + 1);
    //            newPage.Count = newPageCount;
    //            newPage.PivotKey = this.Keys[newPivotIndex];

    //            for (var i = 0; i < newPageCount; ++i)
    //            {
    //                newKeys[i] = keys[i];
    //                newSubtrees[i] = subtrees[i];
    //            }

    //            newSubtrees[newPageCount] = subtrees[newPageCount];

    //            this.Count = newPivotIndex;

    //            return (newPage, newPage.PivotKey);
    //        }

    //        public override bool TryDelete(TKey key, out (bool merged, TKey? deprecatedPivotKey) mergeInfo)
    //        {
    //            mergeInfo.merged = false;
    //            mergeInfo.deprecatedPivotKey = default;

    //            var page = this.SelectSubtree(key);
    //            var deleted = page.TryDelete(key, out var subTreeMergeInfo);

    //            if (deleted && subTreeMergeInfo.merged)
    //            {
    //#pragma warning disable CS8604 // Possible null reference argument.
    //                _ = this.RemoveKey(subTreeMergeInfo.deprecatedPivotKey, out mergeInfo);
    //#pragma warning restore CS8604 // Possible null reference argument.
    //#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    //                this.subtrees[this.Count + 1] = null;
    //#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    //            }

    //            return deleted;
    //        }

    //        public override async Task<WriteResponse<TKey, TValue>> WriteAsync(
    //            TKey key,
    //            TValue value,
    //            CancellationToken cancellationToken)
    //        {
    //            var lockAquired = await this.TryAquireLockAsync(this.LockTimeout, cancellationToken);
    //            try
    //            {
    //                var page = this.SelectSubtree(key);

    //                if (page is null)
    //                {
    //                    throw new KeyNotFoundException($"Subtree not found for key: {key}");
    //                }

    //                if (!page.IsOverflow)
    //                {
    //                    this.ReleaseLock();
    //                    lockAquired = false;
    //                }

    //                var response = page.WriteAsync(key, value, cancellationToken);
    //                if (response.newPage is null || !writeSucceeded)
    //                {
    //                    return writeSucceeded;
    //                }

    //                if (!this.IsOverflow)
    //                {
    //                    writeSucceeded = this.TryWriteInternal(response.newPivotKey, response.newPage);
    //                    response = (null, default, response.result);
    //                    return writeSucceeded;
    //                }

    //                var (newPage, newPivotKey) = this.Split();
    //                var destinationPage = key.CompareTo(newPivotKey) >= 0
    //                    ? (PivotPage<TKey, TValue>)newPage
    //                    : this;

    //                writeSucceeded = destinationPage.TryWriteInternal(response.newPivotKey, response.newPage);
    //                response = (newPage, newPivotKey, response.result);
    //                return writeSucceeded;
    //            }
    //            finally
    //            {
    //                if (lockAquired)
    //                {
    //                    this.ReleaseLock();
    //                }
    //            }
    //        }

    //        public override bool TryRead(TKey key, out TValue? value)
    //        {
    //            var page = this.SelectSubtree(key);
    //            return page.TryRead(key, out value);
    //        }

    //        public override void Dispose()
    //        {
    //            var count = this.Count;
    //            var subtrees = new Span<Page<TKey, TValue>>(this.subtrees, 0, count);
    //            for (var i = 0; i < count; ++i)
    //            {
    //                subtrees[i]?.Dispose();
    //            }

    //            base.Dispose();
    //        }

    //        public override Task<bool> TryWriteAsync(TKey key, TValue value, out (Page<TKey, TValue>? newPage, TKey? newPivotKey, WriteResult result) response, CancellationToken cancellationToken)
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }
}
