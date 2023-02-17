using BTrees.Pages;

namespace BTrees.Nodes
{
    //todo: logging
    //https://www.stevejgordon.co.uk/high-performance-logging-in-net-core
    //https://github.com/aspnet/Logging/blob/a024648829c60/samples/SampleApp/LoggerExtensions.cs

    internal sealed class DataNode<TKey, TValue>
        : INode<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public static DataNode<TKey, TValue> Empty(int size)
        {
            return new DataNode<TKey, TValue>(size);
        }

        private DataNode(int size)
        {
            this.page = DataPage<TKey, TValue>.Empty(size);
        }

        private DataNode(DataPage<TKey, TValue> page)
        {
            this.page = page;
        }

        private readonly object gate = new();
        private DataPage<TKey, TValue> page;

        public int Count => this.page.Count;
        public bool IsEmpty => this.page.IsEmpty;
        public bool IsFull => this.page.IsFull;
        public bool IsOverflow => this.page.IsOverflow;
        public bool IsUnderflow => this.page.IsUnderflow;
        public int Size => this.page.Size;
        public TKey MinKey => this.page.MinKey;
        public TKey MaxKey => this.page.MaxKey;

        #region structure
        public INode<TKey, TValue> Fork()
        {
            return new DataNode<TKey, TValue>(this.page);
        }

        public INode<TKey, TValue> Merge(INode<TKey, TValue> node)
        {
            return node is null
                ? throw new ArgumentNullException(nameof(node))
                : node is DataNode<TKey, TValue> dataNode
                    ? (INode<TKey, TValue>)new DataNode<TKey, TValue>(this.page.Merge(dataNode.page))
                    : throw new InvalidOperationException($"{nameof(node)} was wrong type: {node.GetType().Name}. Expected {nameof(DataNode<TKey, TValue>)}");
        }

        public (INode<TKey, TValue> left, INode<TKey, TValue> right, TKey pivotKey) Split()
        {
            var (leftPage, rightPage, pivotKey) = this.page.Split();
            return (
                new DataNode<TKey, TValue>(leftPage),
                new DataNode<TKey, TValue>(rightPage),
                pivotKey);
        }
        #endregion

        #region reads
        public int BinarySearch(TKey key)
        {
            return this.page.BinarySearch(key);
        }

        public bool ContainsKey(TKey key)
        {
            return this.page.ContainsKey(key);
        }

        public bool TryRead(TKey key, out TValue? value)
        {
            return this.page.TryRead(key, out value);
        }
        #endregion

        #region writes
        public bool TryDelete(TKey key)
        {
            lock (this.gate)
            {
                var deleted = this.page.TryDelete(key, out var page);
                if (deleted)
                {
                    this.page = page;
                }

                return deleted;
            }
        }

        public bool TryInsert(TKey key, TValue value)
        {
            lock (this.gate)
            {
                var inserted = this.page.TryInsert(key, value, out var page);
                if (inserted)
                {
                    this.page = page;
                }

                return inserted;
            }
        }

        public bool TryUpdate(TKey key, TValue value)
        {
            lock (this.gate)
            {
                var updated = this.page.TryUpdate(key, value, out var page);
                if (updated)
                {
                    this.page = page;
                }

                return updated;
            }
        }
        #endregion
    }
}
