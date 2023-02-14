using System.Collections.Immutable;

namespace BTrees.Pages
{
    internal sealed class PartitionPage<TKey, TNode>
        where TKey : IComparable<TKey>
    {
        private readonly int halfSize;
        private readonly ImmutableArray<TKey> keys;
        private readonly ImmutableArray<TNode> subtrees;

        public static PartitionPage<TKey, TNode> Create(
            int size,
            TKey pivotKey,
            TNode left,
            TNode right)
        {
            return new PartitionPage<TKey, TNode>(size, pivotKey, left, right);
        }

        private PartitionPage(
            int size,
            TKey pivotKey,
            TNode left,
            TNode right)
            : this(
                  size,
                  ImmutableArray.Create(pivotKey),
                  ImmutableArray.Create(left, right))
        {
        }

        private PartitionPage(
            int size,
            ImmutableArray<TKey> keys,
            ImmutableArray<TNode> values)
        {
            this.Size = size;
            this.halfSize = size / 2;
            this.keys = keys;
            this.subtrees = values;
        }

        public int Count => this.keys.Length;
        public bool IsEmpty => this.Count == 0;
        public bool IsFull => this.Count == this.Size;
        public bool IsOverflow => this.Count > this.Size;
        public bool IsUnderflow => this.Count < this.halfSize;
        public TKey MinKey => this.Count != 0 ? this.keys[0] : throw new InvalidOperationException($"{nameof(this.MinKey)} is undefined when Count == 0");
        public TKey MaxKey => this.Count != 0 ? this.keys[^1] : throw new InvalidOperationException($"{nameof(this.MaxKey)} is undefined when Count == 0");
        public int Size { get; }

        public int BinarySearch(TKey key)
        {
            return ImmutableArray.BinarySearch(this.keys, key);
        }

        //private PartitionPage<TKey, TValue> MergeSubtree(int subTreeIndex, IPage<TKey, TValue> page)
        //{
        //    var keys = this.keys;
        //    var pages = this.values;

        //    if (subTreeIndex > 0)
        //    {
        //        page = this.values[subTreeIndex - 1]
        //            .Merge(page);
        //        keys = keys
        //            .RemoveAt(subTreeIndex - 1);
        //        pages = pages
        //            .SetItem(subTreeIndex - 1, (TValue)page)
        //            .RemoveAt(subTreeIndex);
        //        --subTreeIndex;
        //    }
        //    else if (subTreeIndex < this.Count)
        //    {
        //        page = page
        //            .Merge((IPage<TKey, TValue>)this.values[subTreeIndex + 1]);
        //        keys = keys
        //            .RemoveAt(subTreeIndex);
        //        pages = pages
        //            .SetItem(subTreeIndex, (TValue)page)
        //            .RemoveAt(subTreeIndex + 1);
        //    }

        //    var pivotPage = new PartitionPage<TKey, TValue>(
        //        this.Size,
        //        keys,
        //        pages);

        //    if (page.IsOverflow)
        //    {
        //        pivotPage = pivotPage.SplitSubtree(subTreeIndex, page);
        //    }

        //    return pivotPage;
        //}

        public PartitionPage<TKey, TNode> Delete(TKey key)
        {
            throw new NotImplementedException();
            //var index = this.SubtreeIndex(key);
            //var subtree = this
            //    .values[index]
            //    .Delete(key);

            //return subtree.IsUnderflow
            //    ? this.MergeSubtree(index, subtree)
            //    : new PartitionPage<TKey, TValue>(
            //        this.Size,
            //        this.keys,
            //        this.values.SetItem(index, subtree));
        }

        public PartitionPage<TKey, TNode> InsertSplitPages(
            int index,
            TNode leftNode,
            TNode rightNode,
            TKey pivotKey)
        {
            return new PartitionPage<TKey, TNode>(
                this.Size,
                this.keys
                    .SetItem(index, pivotKey),
                this.subtrees
                    .SetItem(index, leftNode)
                    .Insert(index + 1, rightNode));
        }

        public PartitionPage<TKey, TNode> Merge(PartitionPage<TKey, TNode> page)
        {
            throw new NotImplementedException();
            //if (page is null)
            //{
            //    throw new ArgumentNullException(nameof(page));
            //}

            //if (page is PartitionPage<TKey, TValue> pivotPage)
            //{
            //    if (this.CompareTo(page) <= 0)
            //    {
            //        var pages = this.values.AddRange(pivotPage.values);
            //        var keys = pages
            //            .Skip(1)
            //            .Take(pages.Length - 1)
            //            .Select(page => page.MinKey)
            //            .ToImmutableArray();

            //        return new PartitionPage<TKey, TValue>(
            //            this.Size,
            //            keys,
            //            pages);
            //    }

            //    return page.Merge(this);
            //}

            //throw new InvalidOperationException($"{nameof(page)} was wrong type: {page.GetType().Name}. Expected {nameof(PartitionPage<TKey, TValue>)}");
        }

        public (PartitionPage<TKey, TNode> leftPage, PartitionPage<TKey, TNode> rightPage, TKey pivotKey) Split()
        {
            var middle = this.Count / 2;
            var rightPageStart = middle + 1;

            return (
                new PartitionPage<TKey, TNode>(
                    this.Size,
                    this.keys[..middle],
                    this.subtrees[..rightPageStart]),
                new PartitionPage<TKey, TNode>(
                    this.Size,
                    this.keys[rightPageStart..this.Count],
                    this.subtrees[rightPageStart..(this.Count + 1)]),
                this.keys[middle]);
        }

        private int SubtreeIndex(TKey key)
        {
            var index = this.keys.BinarySearch(key);
            return index < 0
                ? ~index
                : index + 1;
        }

        public (TNode value, int index) SelectSubtree(TKey key)
        {
            var index = this.SubtreeIndex(key);
            return (this.subtrees[index], index);
        }

        public TNode Read(TKey key)
        {
            var index = this.SubtreeIndex(key);
            return this.subtrees[index];
        }

        public PartitionPage<TKey, TNode> Insert(TKey key, TNode value)
        {
            var index = this.SubtreeIndex(key);
            return new PartitionPage<TKey, TNode>(
                    this.Size,
                    this.keys.Insert(index, key),
                    this.subtrees.Insert(index, value));
        }

        public PartitionPage<TKey, TNode> Update(int index, TNode value)
        {
            return new PartitionPage<TKey, TNode>(
                this.Size,
                this.keys,
                this.subtrees.SetItem(index, value));
        }
    }
}
