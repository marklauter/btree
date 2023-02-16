using BTrees.Nodes;
using System.Collections.Immutable;

namespace BTrees.Pages
{
    internal sealed class PartitionPage<TKey, TNode>
        : IComparable<PartitionPage<TKey, TNode>>
        where TKey : IComparable<TKey>
        where TNode : INode<TKey>
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
            ImmutableArray<TNode> subtrees)
        {
            this.Size = size;
            this.halfSize = size / 2;
            this.keys = keys;
            this.subtrees = subtrees;
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

        public TNode ReadSubtree(int index)
        {
            return this.subtrees[index];
        }

        //public PartitionPage<TKey, TNode> WriteLeftMerge(
        //    int index,
        //    TNode subtree)
        //{
        //    var keys = this.keys
        //        .RemoveAt(index - 1);

        //    var subtrees = this.subtrees
        //        .SetItem(index - 1, subtree)
        //        .RemoveAt(index);

        //    return new PartitionPage<TKey, TNode>(
        //        this.Size,
        //        keys,
        //        subtrees);
        //}

        //public PartitionPage<TKey, TNode> WriteRightMerge(
        //    int index,
        //    TNode subtree)
        //{
        //    var keys = this.keys
        //        .RemoveAt(index);

        //    var subtrees = this.subtrees
        //        .SetItem(index, subtree)
        //        .RemoveAt(index + 1);

        //    return new PartitionPage<TKey, TNode>(
        //        this.Size,
        //        keys,
        //        subtrees);
        //}

        //private PartitionPage<TKey, TNode> MergeSubtree(
        //    int index,
        //    TNode subtree)
        //{
        //    var keys = this.keys;
        //    var subtrees = this.subtrees;

        //    if (index > 0)
        //    {
        //        //subtree = this.subtrees[index - 1]
        //        //    .Merge(subtree);
        //        //keys = keys
        //        //    .RemoveAt(index - 1);
        //        //subtrees = subtrees
        //        //    .SetItem(index - 1, subtree)
        //        //    .RemoveAt(index);
        //        //--index;
        //    }
        //    else if (index < this.Count)
        //    {
        //        //page = page
        //        //    .Merge((IPage<TKey, TValue>)this.values[index + 1]);
        //        //keys = keys
        //        //    .RemoveAt(index);
        //        //pages = pages
        //        //    .SetItem(index, (TValue)page)
        //        //    .RemoveAt(index + 1);
        //    }

        //    var pivotPage = new PartitionPage<TKey, TValue>(
        //        this.Size,
        //        keys,
        //        subtrees);

        //    if (page.IsOverflow)
        //    {
        //        pivotPage = pivotPage.SplitSubtree(index, page);
        //    }

        //    return pivotPage;
        //}

        //public PartitionPage<TKey, TNode> Delete(TKey key)
        //{
        //    throw new NotImplementedException();
        //    //var index = this.SubtreeIndex(key);
        //    //var subtree = this
        //    //    .values[index]
        //    //    .Delete(key);

        //    //return subtree.IsUnderflow
        //    //    ? this.MergeSubtree(index, subtree)
        //    //    : new PartitionPage<TKey, TValue>(
        //    //        this.Size,
        //    //        this.keys,
        //    //        this.values.SetItem(index, subtree));
        //}

        public PartitionPage<TKey, TNode> RemoveSubtree(int index)
        {
            return new PartitionPage<TKey, TNode>(
                this.Size,
                index > 0
                    ? this.keys.RemoveAt(index - 1)
                    : this.keys,
                this.subtrees
                    .RemoveAt(index));
        }

        public PartitionPage<TKey, TNode> WriteSplit(
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
            if (page is PartitionPage<TKey, TNode> pivotPage)
            {
                if (this.CompareTo(page) <= 0)
                {
                    var subtrees = this
                        .subtrees
                        .AddRange(pivotPage.subtrees);

                    var keys = subtrees
                        .Skip(1)
                        .Take(subtrees.Length - 1)
                        .Select(node => node.MinKey)
                        .ToImmutableArray();

                    return new PartitionPage<TKey, TNode>(
                        this.Size,
                        keys,
                        subtrees);
                }

                return page.Merge(this);
            }

            throw new InvalidOperationException($"{nameof(page)} was wrong type: {page.GetType().Name}. Expected {nameof(PartitionPage<TKey, TNode>)}");
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

        //public PartitionPage<TKey, TNode> Insert(TKey key, TNode value)
        //{
        //    var index = this.SubtreeIndex(key);
        //    return new PartitionPage<TKey, TNode>(
        //            this.Size,
        //            this.keys.Insert(index, key),
        //            this.subtrees.Insert(index, value));
        //}

        //public PartitionPage<TKey, TNode> Update(int index, TNode value)
        //{
        //    return new PartitionPage<TKey, TNode>(
        //        this.Size,
        //        this.keys,
        //        this.subtrees.SetItem(index, value));
        //}

        public int CompareTo(PartitionPage<TKey, TNode>? other)
        {
            return other is null
                ? -1
                : this == other
                    ? 0
                    : this.MinKey.CompareTo(other.MinKey);
        }
    }
}
