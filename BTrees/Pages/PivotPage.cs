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

        public static IPage<TKey, TValue> Create(int size,
            IPage<TKey, TValue> leftPage,
            IPage<TKey, TValue> rightPage)
        {
            return new PivotPage<TKey, TValue>(size, leftPage, rightPage);
        }

        public static IPage<TKey, TValue> Create(int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<IPage<TKey, TValue>> pages)
        {
            return new PivotPage<TKey, TValue>(size, keys, pages);
        }

        private PivotPage(
            int size,
            IPage<TKey, TValue> leftPage,
            IPage<TKey, TValue> rightPage)
            : base(size)
        {
            this.keys = ImmutableArray.Create(rightPage.MinKey);
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

        public override int Count => this.keys.Length;
        public override TKey MinKey => this.Count != 0 ? this.pages[0].MinKey : throw new InvalidOperationException($"{nameof(this.MinKey)} is undefined when Count == 0");
        public override TKey MaxKey => this.Count != 0 ? this.pages[^1].MaxKey : throw new InvalidOperationException($"{nameof(this.MaxKey)} is undefined when Count == 0");

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

        private PivotPage<TKey, TValue> MergeSubtree(int subTreeIndex, IPage<TKey, TValue> page)
        {
            var keys = this.keys;
            var pages = this.pages;

            if (subTreeIndex > 0)
            {
                page = this.pages[subTreeIndex - 1]
                    .Merge(page);
                keys = keys
                    .RemoveAt(subTreeIndex - 1);
                pages = pages
                    .SetItem(subTreeIndex - 1, page)
                    .RemoveAt(subTreeIndex);
                --subTreeIndex;
            }
            else if (subTreeIndex < this.Count)
            {
                page = page
                    .Merge(this.pages[subTreeIndex + 1]);
                keys = keys
                    .RemoveAt(subTreeIndex);
                pages = pages
                    .SetItem(subTreeIndex, page)
                    .RemoveAt(subTreeIndex + 1);
            }

            var pivotPage = new PivotPage<TKey, TValue>(
                this.Size,
                keys,
                pages);

            if (page.IsOverflow)
            {
                pivotPage = pivotPage.SplitSubtree(subTreeIndex, page);
            }

            return pivotPage;
        }

        public override IPage<TKey, TValue> Delete(TKey key)
        {
            var index = this.SubtreeIndex(key);
            var subtree = this
                .pages[index]
                .Delete(key);

            return subtree.IsUnderflow
                ? this.MergeSubtree(index, subtree)
                : new PivotPage<TKey, TValue>(
                    this.Size,
                    this.keys,
                    this.pages.SetItem(index, subtree));
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
            var (leftPage, rightPage, pivotKey) = page.Split();
            return new PivotPage<TKey, TValue>(
                this.Size,
                this.keys
                    .Insert(subTreeIndex, pivotKey),
                this.pages
                    .SetItem(subTreeIndex, leftPage)
                    .Insert(subTreeIndex + 1, rightPage));
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
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (page is PivotPage<TKey, TValue> pivotPage)
            {
                if (this.CompareTo(page) <= 0)
                {
                    var pages = this.pages.AddRange(pivotPage.pages);
                    var keys = pages
                        .Skip(1)
                        .Take(pages.Length - 1)
                        .Select(page => page.MinKey)
                        .ToImmutableArray();

                    return new PivotPage<TKey, TValue>(
                        this.Size,
                        keys,
                        pages);
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
                    this.pages[rightPageStart..(this.Count + 1)]),
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
}
