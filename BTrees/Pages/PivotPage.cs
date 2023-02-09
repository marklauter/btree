using System.Collections.Immutable;
using System.Diagnostics;

namespace BTrees.Pages
{
    [DebuggerDisplay(nameof(PivotPage<TKey, TValue>))]
    internal sealed class PivotPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly ImmutableArray<TKey> keys;
        private readonly ImmutableArray<IPage<TKey, TValue>> pages;

        public PivotPage(
            int size,
            IPage<TKey, TValue> leftPage,
            IPage<TKey, TValue> rightPage)
            : base(size)
        {
            this.keys = ImmutableArray.Create(rightPage.MinKey);
            this.pages = ImmutableArray.Create(leftPage, rightPage);
        }

        public PivotPage(
            int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<IPage<TKey, TValue>> pages)
            : base(size)
        {
            this.keys = keys;
            this.pages = pages;
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
            return this
                .SelectSubtree(key)
                .ContainsKey(key);
        }

        public override IPage<TKey, TValue> Delete(TKey key)
        {
            var index = this.SubtreeIndex(key);
            var page = this
                .pages[index]
                .Delete(key);

            if (page.IsUnderflow)
            {
                // todo: merge with left, right or both siblings?
                throw new NotImplementedException();
            }
            else
            {
                return new PivotPage<TKey, TValue>(
                    this.Size,
                    this.keys,
                    this.pages.SetItem(index, page));
            }
        }

        public override IPage<TKey, TValue> Fork()
        {
            return new PivotPage<TKey, TValue>(
                this.Size,
                this.keys,
                this.pages);
        }

        private PivotPage<TKey, TValue> SplitSubtree(int subTreeIndex, IPage<TKey, TValue> page)
        {
            var splitResult = page.Split();
            return new PivotPage<TKey, TValue>(
                this.Size,
                this.keys
                    .Insert(subTreeIndex, splitResult.pivotKey),
                this.pages
                    .SetItem(subTreeIndex, splitResult.leftPage)
                    .Insert(subTreeIndex + 1, splitResult.rightPage));
        }

        public override IPage<TKey, TValue> Insert(TKey key, TValue value)
        {
            var index = this.SubtreeIndex(key);
            var subtree = this
                .pages[index]
                .Insert(key, value);

            return subtree.IsOverflow
                ? this.SplitSubtree(index, subtree)
                : new PivotPage<TKey, TValue>(
                    this.Size,
                    this.keys,
                    this.pages.SetItem(index, subtree));
        }

        public override IPage<TKey, TValue> Merge(IPage<TKey, TValue> page)
        {
            throw new NotImplementedException();

            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (page is PivotPage<TKey, TValue> pivotPage)
            {
                if (this.CompareTo(page) <= 0)
                {
                    var mergedPages = this.pages.AddRange(pivotPage.pages);
                    var mergedKeys = mergedPages
                        .Take(mergedPages.Length - 1)
                        .Select(page => page.MinKey)
                        .ToImmutableArray();

                    return new PivotPage<TKey, TValue>(
                        this.Size,
                        mergedKeys,
                        mergedPages);
                }

                return page.Merge(this);
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
            return this
                .SelectSubtree(key)
                .TryRead(key, out value);
        }

        public override IPage<TKey, TValue> Update(TKey key, TValue value)
        {
            var index = this.SubtreeIndex(key);
            var page = this
                .pages[index]
                .Update(key, value);

            return new PivotPage<TKey, TValue>(
                this.Size,
                this.keys,
                this.pages.SetItem(index, page));
        }

        private IPage<TKey, TValue> SelectSubtree(TKey key)
        {
            return this.pages[this.SubtreeIndex(key)];
        }

        private int SubtreeIndex(TKey key)
        {
            var index = this.BinarySearch(key);
            return index < 0
                ? ~index
                : index + 1;
        }
    }

    //    [DebuggerDisplay("PivotPage {Count}")]
    //    internal class PivotPage<TKey, TValue>
    //        : Page<TKey, TValue>
    //        where TKey : IComparable<TKey>
    //    {


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
}
