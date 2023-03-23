//using BTrees.Nodes;
//using System.Collections.Immutable;

//namespace BTrees.Pages
//{
//    internal sealed class PartitionPage<TKey, TValue>
//        : IComparable<PartitionPage<TKey, TValue>>
//        where TKey : IComparable<TKey>
//    {
//        private readonly int halfSize;
//        private readonly ImmutableArray<TKey> keys;
//        private readonly ImmutableArray<INode<TKey, TValue>> subtrees;

//        public static PartitionPage<TKey, TValue> Create(
//            int size,
//            TKey pivotKey,
//            INode<TKey, TValue> left,
//            INode<TKey, TValue> right)
//        {
//            return new PartitionPage<TKey, TValue>(size, pivotKey, left, right);
//        }

//        private PartitionPage(
//            int size,
//            TKey pivotKey,
//            INode<TKey, TValue> left,
//            INode<TKey, TValue> right)
//            : this(
//                  size,
//                  ImmutableArray.Create(pivotKey),
//                  ImmutableArray.Create(left, right))
//        {
//        }

//        private PartitionPage(
//            int size,
//            ImmutableArray<TKey> keys,
//            ImmutableArray<INode<TKey, TValue>> subtrees)
//        {
//            this.Size = size;
//            this.halfSize = size >> 1;
//            this.keys = keys;
//            this.subtrees = subtrees;
//        }

//        public int Count => this.keys.Length;
//        public bool IsEmpty => this.Count == 0;
//        public bool IsFull => this.Count == this.Size;
//        public bool IsOverflow => this.Count > this.Size;
//        public bool IsUnderflow => this.Count <= this.halfSize;
//        public TKey MinKey => this.Count != 0 ? this.keys[0] : throw new InvalidOperationException($"{nameof(this.MinKey)} is undefined when Count == 0");
//        public TKey MaxKey => this.Count != 0 ? this.keys[^1] : throw new InvalidOperationException($"{nameof(this.MaxKey)} is undefined when Count == 0");
//        public int Size { get; }

//        public int BinarySearch(TKey key)
//        {
//            return ImmutableArray
//                .BinarySearch(this.keys, key);
//        }

//        //public PartitionPage<TKey, TValue> WriteLeftMerge(
//        //    int index,
//        //    TValue subtree)
//        //{
//        //    var keys = this.keys
//        //        .RemoveAt(index - 1);

//        //    var subtrees = this.subtrees
//        //        .SetItem(index - 1, subtree)
//        //        .RemoveAt(index);

//        //    return new PartitionPage<TKey, TValue>(
//        //        this.Size,
//        //        keys,
//        //        subtrees);
//        //}

//        //public PartitionPage<TKey, TValue> WriteRightMerge(
//        //    int index,
//        //    TValue subtree)
//        //{
//        //    var keys = this.keys
//        //        .RemoveAt(index);

//        //    var subtrees = this.subtrees
//        //        .SetItem(index, subtree)
//        //        .RemoveAt(index + 1);

//        //    return new PartitionPage<TKey, TValue>(
//        //        this.Size,
//        //        keys,
//        //        subtrees);
//        //}

//        //private PartitionPage<TKey, TValue> MergeSubtree(
//        //    int index,
//        //    TValue subtree)
//        //{
//        //    var keys = this.keys;
//        //    var subtrees = this.subtrees;

//        //    if (index > 0)
//        //    {
//        //        //subtree = this.subtrees[index - 1]
//        //        //    .Merge(subtree);
//        //        //keys = keys
//        //        //    .RemoveAt(index - 1);
//        //        //subtrees = subtrees
//        //        //    .SetItem(index - 1, subtree)
//        //        //    .RemoveAt(index);
//        //        //--index;
//        //    }
//        //    else if (index < this.Count)
//        //    {
//        //        //page = page
//        //        //    .Merge((IPage<TKey, TValue>)this.values[index + 1]);
//        //        //keys = keys
//        //        //    .RemoveAt(index);
//        //        //pages = pages
//        //        //    .SetItem(index, (TValue)page)
//        //        //    .RemoveAt(index + 1);
//        //    }

//        //    var pivotPage = new PartitionPage<TKey, TValue>(
//        //        this.Size,
//        //        keys,
//        //        subtrees);

//        //    if (page.IsOverflow)
//        //    {
//        //        pivotPage = pivotPage.SplitSubtree(index, page);
//        //    }

//        //    return pivotPage;
//        //}

//        public PartitionPage<TKey, TValue> RemoveSubtree(int index)
//        {
//            return new PartitionPage<TKey, TValue>(
//                this.Size,
//                index > 0
//                    ? this.keys.RemoveAt(index - 1)
//                    : this.keys,
//                this.subtrees
//                    .RemoveAt(index));
//        }

//        public PartitionPage<TKey, TValue> SplitSubtree(int index, INode<TKey, TValue> subtree)
//        {
//            var (left, right, pivotKey) = subtree.Split();
//            return new PartitionPage<TKey, TValue>(
//                this.Size,
//                this.keys
//                    .Insert(index, pivotKey),
//                this.subtrees
//                    .SetItem(index, left)
//                    .Insert(index + 1, right));
//        }

//        public PartitionPage<TKey, TValue> MergeSubtree(int index, INode<TKey, TValue> subtree)
//        {
//            throw new NotImplementedException();
//        }

//        public PartitionPage<TKey, TValue> Merge(PartitionPage<TKey, TValue> page)
//        {
//            if (page is PartitionPage<TKey, TValue> pivotPage)
//            {
//                if (this.CompareTo(page) <= 0)
//                {
//                    var subtrees = this
//                        .subtrees
//                        .AddRange(pivotPage.subtrees);

//                    var keys = subtrees
//                        .Skip(1)
//                        .Take(subtrees.Length - 1)
//                        .Select(node => node.MinKey)
//                        .ToImmutableArray();

//                    return new PartitionPage<TKey, TValue>(
//                        this.Size,
//                        keys,
//                        subtrees);
//                }

//                return page.Merge(this);
//            }

//            throw new InvalidOperationException($"{nameof(page)} was wrong type: {page.GetType().Name}. Expected {nameof(PartitionPage<TKey, TValue>)}");
//        }

//        public (PartitionPage<TKey, TValue> leftPage, PartitionPage<TKey, TValue> rightPage, TKey pivotKey) Split()
//        {
//            var middle = this.Count >> 1;
//            var rightPageStart = middle + 1;

//            return (
//                new PartitionPage<TKey, TValue>(
//                    this.Size,
//                    this.keys[..middle],
//                    this.subtrees[..rightPageStart]),
//                new PartitionPage<TKey, TValue>(
//                    this.Size,
//                    this.keys[rightPageStart..this.Count],
//                    this.subtrees[rightPageStart..(this.Count + 1)]),
//                this.keys[middle]);
//        }

//        private int SubtreeIndex(TKey key)
//        {
//            var index = this.keys.BinarySearch(key);
//            return index < 0
//                ? ~index
//                : index + 1;
//        }

//        public (INode<TKey, TValue> node, int index) SelectSubtree(TKey key)
//        {
//            var index = this.SubtreeIndex(key);
//            return (this.subtrees[index], index);
//        }

//        public INode<TKey, TValue> Read(TKey key)
//        {
//            var index = this.SubtreeIndex(key);
//            return this.subtrees[index];
//        }

//        public int CompareTo(PartitionPage<TKey, TValue>? other)
//        {
//            return other is null
//                ? -1
//                : this == other
//                    ? 0
//                    : this.MinKey.CompareTo(other.MinKey);
//        }
//    }
//}
