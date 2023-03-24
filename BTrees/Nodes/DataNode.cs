using BTrees.Pages;
using BTrees.Types;
using System.Collections.Immutable;

namespace BTrees.Nodes
{
    //todo: logging
    //https://www.stevejgordon.co.uk/high-performance-logging-in-net-core
    //https://github.com/aspnet/Logging/blob/a024648829c60/samples/SampleApp/LoggerExtensions.cs

    internal sealed class DataNode<TKey, TValue>
        : INode<TKey, TValue>
        where TKey : IDbType, IComparable<TKey>
        where TValue : IDbType, IComparable<TValue>
    {
        private readonly object gate = new();
        private readonly int maxSize;
        private readonly DataPage<TKey, TValue> page = DataPage<TKey, TValue>.Empty;

        private DataNode(int maxSize)
        {
            this.maxSize = maxSize;
        }

        private DataNode(
            int maxSize,
            INode<TKey, TValue> parent)
            : this(maxSize)
        {
            this.Parent = parent;
        }

        private DataNode(
            int maxSize,
            INode<TKey, TValue> parent,
            INode<TKey, TValue>? leftSibling,
            INode<TKey, TValue>? rightSibling,
            DataPage<TKey, TValue> page)
            : this(maxSize, parent)
        {
            this.LeftSibling = leftSibling;
            this.RightSibling = rightSibling;
            this.page = page;
        }

        public int Length => this.page.Length;
        public bool IsEmpty => this.page.IsEmpty;
        public bool IsFull { get; }
        public bool IsOverflow { get; }
        public bool IsUnderflow { get; }
        public int Size { get; private set; }
        public TKey PivotKey { get; }

        public INode<TKey, TValue>? Parent { get; }
        public INode<TKey, TValue>? LeftSibling { get; }
        public INode<TKey, TValue>? RightSibling { get; }

        public INode<TKey, TValue> Fork()
        {
            throw new NotImplementedException();
            //return new DataNode<TKey, TValue>(this.maxSize, this.page);
        }

        public INode<TKey, TValue> Merge(INode<TKey, TValue> node)
        {
            throw new NotImplementedException();
            //return node is DataNode<TKey, TValue> dataNode
            //    ? (INode<TKey, TValue>)new DataNode<TKey, TValue>(this.maxSize, this.page.Merge(dataNode.page))
            //    : throw new InvalidOperationException($"{nameof(node)} was wrong type: {node.GetType().Name}. Expected {nameof(DataNode<TKey, TValue>)}");
        }

        public INode<TKey, TValue>.SplitResult Split()
        {
            throw new NotImplementedException();
            //var split = this.page.Split();
            //return new INode<TKey, TValue>.SplitResult(
            //    new DataNode<TKey, TValue>(this.maxSize, split.LeftPage),
            //    new DataNode<TKey, TValue>(this.maxSize, split.RightPage),
            //    split.PivotKey);
        }

        public void Delete(TKey key)
        {
            throw new NotImplementedException();

            //lock (this.gate)
            //{
            //    this.page = this.page.Remove(key);
            //}
        }

        public void Delete(TKey key, TValue value)
        {
            throw new NotImplementedException();

            //lock (this.gate)
            //{
            //    this.page = this.page.Remove(key, value);
            //}
        }

        public void Insert(TKey key, TValue value)
        {
            throw new NotImplementedException();
            //lock (this.gate)
            //{
            //    this.page = this.page.Insert(key, value);
            //}
        }

        public bool ContainsKey(TKey key)
        {
            return this.page.ContainsKey(key);
        }

        public ImmutableArray<TValue> Read(TKey key)
        {
            return this.page.Read(key);
        }
    }
}
