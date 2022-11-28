namespace BTrees.Tests
{
    public class BTree<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly int pageSize;
        private int itemCount;
        private readonly IPage<TKey, TValue> root;

        public BTree(int pageSize)
        {
            this.pageSize = pageSize;
            this.root = new LeafPage<TKey, TValue>(pageSize);
        }

        private void Insert(TKey key, TValue value)
        {
            var page = this.root.SelectSubtree(key);
            _ = page.Insert(key, value);
            ++this.itemCount;
        }
    }

    public interface IPage<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        int Count { get; }
        int Size { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }
        IPage<TKey, TValue> SelectSubtree(TKey key);
        (IPage<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value);
    }

    public abstract class Page<TKey, TValue>
        : IPage<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public Page(int size)
        {
            this.Size = size;
            this.Keys = new TKey[size];
        }

        public abstract (IPage<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value);

        public abstract IPage<TKey, TValue> SelectSubtree(TKey key);

        internal TKey[] Keys { get; }
        public int Count { get; protected set; }
        public int Size { get; }
        public bool IsEmpty => this.Count == 0;
        public bool IsFull => this.Count == this.Size;
    }

    public class NodePage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly IPage<TKey, TValue>[] children;

        public NodePage(int size)
            : base(size)
        {
            this.children = new IPage<TKey, TValue>[size + 1];
        }

        public override IPage<TKey, TValue> SelectSubtree(TKey key)
        {
            // todo: binary search is faster than for loop scan
            for (var i = 0; i < this.Keys.Length; i++)
            {
                if (this.Keys[i].CompareTo(key) <= 0)
                {
                    // left page
                    return this.children[i].SelectSubtree(key);
                }
            }

            // right page
            return this.children[this.Count + 1];
        }

        public override (IPage<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }
    }

    public class LeafPage<TKey, TValue>
        : Page<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly TValue[] values;

        public LeafPage(int size)
            : base(size)
        {
            this.values = new TValue[size];
        }

        public override IPage<TKey, TValue> SelectSubtree(TKey key)
        {
            // it's a leaf, so there is no subtree - this is the end of the traversal
            return this;
        }

        public override (IPage<TKey, TValue>? newPage, TKey? newPivotKey) Insert(TKey key, TValue value)
        {
            if (!this.IsFull)
            {
                this.InsertInternal(key, value);
                return (null, default);
            }
            else
            {
                return this.Split(key, value);
            }
        }

        internal void InsertInternal(TKey key, TValue value)
        {
            var index = this.FindInsertionIndex(key);
            if (index != this.Count)
            {
                this.ShiftData(key, value, index);
            }

            this.Keys[index] = key;
            this.values[index] = value;
            ++this.Count;
        }

        internal void ShiftData(TKey key, TValue value, int index)
        {
            for (var i = this.Count - 1; i >= index; --i)
            {
                this.Keys[i + 1] = this.Keys[i];
                this.values[i + 1] = this.values[i];
            }
        }

        internal int FindInsertionIndex(TKey key)
        {
            if (this.IsEmpty)
            {
                return 0;
            }

            var high = this.Count - 1;
            var low = 0;

            // check edge cases first
            if (key.CompareTo(this.Keys[high]) >= 0) // insert at end
            {
                return this.Count;
            }

            if (key.CompareTo(this.Keys[low]) <= 0) // insert at end
            {
                return 0;
            }

            var index = 0;
            while (low < high)
            {
                index = (high + low) / 2;

                var comparison = key.CompareTo(this.Keys[index]);
                if (comparison > 0)
                {
                    low = index + 1;
                    continue;
                }

                if (comparison < 0)
                {
                    high = index;
                    continue;
                }

                break;
            }

            return index;
        }

        private (LeafPage<TKey, TValue> newPage, TKey newPivotKey) Split(TKey key, TValue value)
        {
            var newPage = new LeafPage<TKey, TValue>(this.Size);
            var copyFromIndex = this.Count / 2;
            var j = 0;
            for (var i = copyFromIndex; i < this.Count; ++i)
            {
                newPage.Keys[j] = this.Keys[i];
                newPage.values[j] = this.values[i];
                ++j;
            }

            newPage.Count = this.Count - copyFromIndex;
            this.Count = copyFromIndex;

            if (key.CompareTo(this.Keys[this.Count - 1]) <= 0)
            {
                this.InsertInternal(key, value);
            }
            else
            {
                newPage.InsertInternal(key, value);
            }

            return (newPage, newPage.Keys[0]);
        }
    }

    public class LeafPageTests
    {
        private readonly int pageSize = 10;

        [Fact]
        public void NewPageIsEmpty()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            Assert.True(page.IsEmpty);
        }

        [Fact]
        public void NewPageHasCorrectPageSize()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            Assert.Equal(this.pageSize, page.Size);
        }

        [Fact]
        public void InsertIncrememtsCount()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            _ = page.Insert(1, 1);
            Assert.Equal(1, page.Count);
        }

        [Fact]
        public void SelectSubtreeReturnsSelf()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            var subtree = page.SelectSubtree(1);
            Assert.Equal(page, subtree);
        }

        [Fact]
        public void DivisionTest()
        {
            var i = this.pageSize / 2;
            Assert.Equal(5, i);
            i /= 2;
            Assert.Equal(2, i);

            i /= 2;
            Assert.Equal(1, i);

            i /= 2;
            Assert.Equal(0, i);

            var j = this.pageSize / 2;
            var x = 0;
            while (j > 0)
            {
                j /= 2;
                ++x;
            }

            Assert.Equal(3, x);
        }

        [Fact]
        public void FindInsertionIndexTest()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            _ = page.Insert(1, 1); // 0 goes to index 0
            _ = page.Insert(2, 2); // 3 goes to index 2
            _ = page.Insert(4, 4);
            _ = page.Insert(5, 5);
            _ = page.Insert(6, 6);
            _ = page.Insert(7, 7); // 8 goes to index 6
            _ = page.Insert(9, 9);
            _ = page.Insert(10, 10); // 11 goes to index 8
            var mid = page.FindInsertionIndex(3);
            Assert.Equal(2, mid);

            mid = page.FindInsertionIndex(8);
            Assert.Equal(6, mid);

            mid = page.FindInsertionIndex(4);
            Assert.Equal(2, mid);

            mid = page.FindInsertionIndex(0);
            Assert.Equal(0, mid);

            mid = page.FindInsertionIndex(11);
            Assert.Equal(8, mid);
        }

        [Fact]
        public void SortedInsertTest()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            _ = page.Insert(1, 1); // 0 goes to index 0
            _ = page.Insert(2, 2); // 3 goes to index 2
            _ = page.Insert(4, 4);
            _ = page.Insert(5, 5);
            _ = page.Insert(6, 6);
            _ = page.Insert(7, 7); // 8 goes to index 6
            _ = page.Insert(9, 9);
            _ = page.Insert(10, 10); // 11 goes to index 8

            _ = page.Insert(3, 3);
            var sortedKeys = page.Keys
                .Take(page.Count)
                .Order();
            Assert.Equal(sortedKeys, page.Keys.Take(page.Count));

            _ = page.Insert(8, 8);
            sortedKeys = page.Keys
                .Take(page.Count)
                .Order();
            Assert.Equal(sortedKeys, page.Keys.Take(page.Count));
        }

        [Fact]
        public void SplitTest()
        {
            var page = new LeafPage<int, int>(this.pageSize);
            for (var i = 0; i < page.Size; i++)
            {
                _ = page.Insert(i + 1, i + 1);
            }

            var (newPage, pivot) = page.Insert(4, 4);
            Assert.NotNull(newPage);
            Assert.Equal(6, pivot);
        }
    }
}
