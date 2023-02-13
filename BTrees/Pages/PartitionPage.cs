//using System.Collections.Immutable;
//using System.Diagnostics;

//namespace BTrees.Pages
//{

//    [DebuggerDisplay(nameof(PartitionPage<TKey, TValue>))]
//    internal sealed class PartitionPage<TKey, TValue>
//        where TKey : IComparable<TKey>
//    {
//        private readonly int halfSize;
//        private readonly ImmutableArray<TKey> keys;
//        private readonly ImmutableArray<TValue> values;

//        public static PartitionPage<TKey, TValue> Create(
//            int size,
//            TKey pivotKey,
//            TValue left,
//            TValue right)
//        {
//            return new PartitionPage<TKey, TValue>(size, pivotKey, left, right);
//        }

//        private PartitionPage(
//            int size,
//            TKey pivotKey,
//            TValue left,
//            TValue right)
//        {
//            this.Size = size;
//            this.keys = ImmutableArray.Create(pivotKey);
//            this.values = ImmutableArray.Create(left, right);
//        }

//        //public static IPage<TKey, TValue> Create(int size,
//        //    IPage<TKey, TValue> leftPage,
//        //    IPage<TKey, TValue> rightPage)
//        //{
//        //    return new PartitionPage<TKey, TValue>(size, leftPage, rightPage);
//        //}

//        //public static IPage<TKey, TValue> Create(int size,
//        //    ImmutableArray<TKey> keys,
//        //    ImmutableArray<IPage<TKey, TValue>> pages)
//        //{
//        //    return new PartitionPage<TKey, TValue>(size, keys, pages);
//        //}

//        private PartitionPage(
//            int size,
//            ImmutableArray<TKey> keys,
//            ImmutableArray<TValue> values)
//        {
//            this.Size = size;
//            this.halfSize = size / 2;
//            this.keys = keys;
//            this.values = values;
//        }

//        public int Count => this.keys.Length;
//        public bool IsEmpty => this.Count == 0;
//        public bool IsFull => this.Count == this.Size;
//        public bool IsOverflow => this.Count > this.Size;
//        public bool IsUnderflow => this.Count < this.halfSize;
//        public TKey MinKey => this.Count != 0 ? this.keys[0] : throw new InvalidOperationException($"{nameof(this.MinKey)} is undefined when Count == 0");
//        public TKey MaxKey => this.Count != 0 ? this.keys[^1] : throw new InvalidOperationException($"{nameof(this.MaxKey)} is undefined when Count == 0");
//        public int Size { get; }

//        public int BinarySearch(TKey key)
//        {
//            return ImmutableArray.BinarySearch(this.keys, key);
//        }

//        public bool ContainsKey(TKey key)
//        {
//            throw new NotImplementedException();
//            //return this
//            //    .SelectSubtree(key)
//            //    .ContainsKey(key);
//        }

//        //private PartitionPage<TKey, TValue> MergeSubtree(int subTreeIndex, IPage<TKey, TValue> page)
//        //{
//        //    var keys = this.keys;
//        //    var pages = this.values;

//        //    if (subTreeIndex > 0)
//        //    {
//        //        page = this.values[subTreeIndex - 1]
//        //            .Merge(page);
//        //        keys = keys
//        //            .RemoveAt(subTreeIndex - 1);
//        //        pages = pages
//        //            .SetItem(subTreeIndex - 1, (TValue)page)
//        //            .RemoveAt(subTreeIndex);
//        //        --subTreeIndex;
//        //    }
//        //    else if (subTreeIndex < this.Count)
//        //    {
//        //        page = page
//        //            .Merge((IPage<TKey, TValue>)this.values[subTreeIndex + 1]);
//        //        keys = keys
//        //            .RemoveAt(subTreeIndex);
//        //        pages = pages
//        //            .SetItem(subTreeIndex, (TValue)page)
//        //            .RemoveAt(subTreeIndex + 1);
//        //    }

//        //    var pivotPage = new PartitionPage<TKey, TValue>(
//        //        this.Size,
//        //        keys,
//        //        pages);

//        //    if (page.IsOverflow)
//        //    {
//        //        pivotPage = pivotPage.SplitSubtree(subTreeIndex, page);
//        //    }

//        //    return pivotPage;
//        //}

//        public PartitionPage<TKey, TValue> Delete(TKey key)
//        {
//            var index = this.SubtreeIndex(key);
//            var subtree = this
//                .values[index]
//                .Delete(key);

//            return subtree.IsUnderflow
//                ? this.MergeSubtree(index, subtree)
//                : new PartitionPage<TKey, TValue>(
//                    this.Size,
//                    this.keys,
//                    this.values.SetItem(index, subtree));
//        }

//        public PartitionPage<TKey, TValue> Fork()
//        {
//            return new PartitionPage<TKey, TValue>(
//                this.Size,
//                this.keys,
//                this.values);
//        }

//        //private PartitionPage<TKey, TValue> SplitSubtree(int subTreeIndex, IPage<TKey, TValue> page)
//        //{
//        //    var (leftPage, rightPage, pivotKey) = page.Split();
//        //    return new PartitionPage<TKey, TValue>(
//        //        this.Size,
//        //        this.keys
//        //            .Insert(subTreeIndex, pivotKey),
//        //        this.values
//        //            .SetItem(subTreeIndex, (TValue)leftPage)
//        //            .Insert(subTreeIndex + 1, (TValue)rightPage));
//        //}

//        public PartitionPage<TKey, TValue> Insert(TKey key, TValue value)
//        {
//            var index = this.SubtreeIndex(key);
//            var subtree = this
//                .values[index]
//                .Insert(key, value);

//            return subtree.IsOverflow
//                ? this.SplitSubtree(index, subtree)
//                : new PartitionPage<TKey, TValue>(
//                    this.Size,
//                    this.keys,
//                    this.values.SetItem(index, subtree));
//        }

//        public PartitionPage<TKey, TValue> Merge(PartitionPage<TKey, TValue> page)
//        {
//            if (page is null)
//            {
//                throw new ArgumentNullException(nameof(page));
//            }

//            if (page is PartitionPage<TKey, TValue> pivotPage)
//            {
//                if (this.CompareTo(page) <= 0)
//                {
//                    var pages = this.values.AddRange(pivotPage.values);
//                    var keys = pages
//                        .Skip(1)
//                        .Take(pages.Length - 1)
//                        .Select(page => page.MinKey)
//                        .ToImmutableArray();

//                    return new PartitionPage<TKey, TValue>(
//                        this.Size,
//                        keys,
//                        pages);
//                }

//                return page.Merge(this);
//            }

//            throw new InvalidOperationException($"{nameof(page)} was wrong type: {page.GetType().Name}. Expected {nameof(PartitionPage<TKey, TValue>)}");
//        }

//        public (PartitionPage<TKey, TValue> leftPage, PartitionPage<TKey, TValue> rightPage, TKey pivotKey) Split()
//        {
//            var middle = this.Count / 2;
//            var rightPageStart = middle + 1;

//            return (
//                new PartitionPage<TKey, TValue>(
//                    this.Size,
//                    this.keys[..middle],
//                    this.values[..rightPageStart]),
//                new PartitionPage<TKey, TValue>(
//                    this.Size,
//                    this.keys[rightPageStart..this.Count],
//                    this.values[rightPageStart..(this.Count + 1)]),
//                this.keys[middle]);
//        }

//        private int SubtreeIndex(TKey key)
//        {
//            var index = this.keys.BinarySearch(key);
//            return index < 0
//                ? ~index
//                : index + 1;
//        }

//        public (TValue value, int index) SelectSubtree(TKey key)
//        {
//            var index = this.SubtreeIndex(key);
//            return (this.values[index], index);
//        }

//        public TValue Read(TKey key)
//        {
//            return this.SelectSubtree(key);
//        }

//        public PartitionPage<TKey, TValue> Update(int index, TValue value)
//        {
//            return new PartitionPage<TKey, TValue>(
//                this.Size,
//                this.keys,
//                this.values.SetItem(index, value));
//        }
//    }
//}
