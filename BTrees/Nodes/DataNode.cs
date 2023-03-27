using BTrees.Pages;
using BTrees.Types;
using System.Runtime.CompilerServices;

//todo: logging
//https://www.stevejgordon.co.uk/high-performance-logging-in-net-core
//https://github.com/aspnet/Logging/blob/a024648829c60/samples/SampleApp/LoggerExtensions.cs

namespace BTrees.Nodes
{
    internal sealed partial class DataNode<TKey, TValue>
        : INode<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        private PageAndSibling fields = new(DataPage<TKey, TValue>.Empty, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataNode(TKey key, TValue value)
            : this(key, value, 1024 * 4)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataNode(TKey key, TValue value, int maxSize)
            : this(
                  new PageAndSibling(
                      DataPage<TKey, TValue>.Empty.Insert(key, value),
                      null),
                  maxSize)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DataNode(PageAndSibling fields, int maxSize)
        {
            this.MaxSize = maxSize;
            this.HalfSize = this.MaxSize >> 1;

            this.fields = fields;
        }

        public int MaxSize { get; }
        public int HalfSize { get; }
        public int Length => this.fields.Page.Length;
        public bool IsOverflow => this.Size >= this.MaxSize;
        public bool IsUnderflow => this.Size < this.HalfSize;
        public int Size => this.fields.Page.Size;
        public INode<TKey, TValue>? RightSibling => this.fields.RightSibling;
        public bool HasRightSibling => this.fields.RightSibling is not null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return this.fields.Page.Count();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public INode<TKey, TValue> Fork()
        {
            return new DataNode<TKey, TValue>(this.fields, this.MaxSize);
        }

        public void Merge(INode<TKey, TValue> node)
        {
            lock (this)
            {
                if (node is DataNode<TKey, TValue> dataNode)
                {
                    var x = Tuple.Create(1, 2);
                    var fields = new PageAndSibling(
                        this.fields.Page.Merge(dataNode.fields.Page),
                        dataNode.RightSibling);
                    this.fields = fields;
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(node)} was wrong type: {node.GetType().Name}. Expected {nameof(DataNode<TKey, TValue>)}");
                }
            }
        }

        public INode<TKey, TValue> Split()
        {
            lock (this)
            {
                var splitResult = this.fields.Page.Split();

                var node = new DataNode<TKey, TValue>(
                    new PageAndSibling(
                        splitResult.RightPage,
                        this.fields.RightSibling),
                    this.MaxSize);

                this.fields = new PageAndSibling(
                    splitResult.LeftPage,
                    node);

                return node;
            }
        }

        public void Remove(TKey key)
        {
            lock (this)
            {
                this.fields = new PageAndSibling(
                    this.fields.Page.Remove(key),
                    this.fields.RightSibling);
            }
        }

        public void Remove(TKey key, TValue value)
        {
            lock (this)
            {
                this.fields = new PageAndSibling(
                    this.fields.Page.Remove(key, value),
                    this.fields.RightSibling);
            }
        }

        public void Insert(TKey key, TValue value)
        {
            lock (this)
            {
                this.fields = new PageAndSibling(
                    this.fields.Page.Insert(key, value),
                    this.fields.RightSibling);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            return this.fields.Page.ContainsKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TValue> Read(TKey key)
        {
            return this.fields.Page.Read(key);
        }

        public IEnumerable<(TKey Key, TValue Value)> Read(TKey leftBoundingKey, TKey rightBoundingKey)
        {
            var pns = this.fields;

            var leftBoundingIndex = pns.Page.IndexOf(leftBoundingKey);
            var rightBoundingIndex = pns.Page.IndexOf(rightBoundingKey);

            leftBoundingIndex = leftBoundingIndex >= 0 ? leftBoundingIndex : ~leftBoundingIndex;
            rightBoundingIndex = (rightBoundingIndex >= 0 ? rightBoundingIndex : ~rightBoundingIndex) + 1;

            var values = pns.Page.Read(leftBoundingIndex..rightBoundingIndex);
            return rightBoundingIndex == pns.Page.Length && pns.RightSibling is not null // if rightBound key exceeded right edge of the current page then read from the right sibling
                ? values.Union(pns.RightSibling.Read(leftBoundingKey, rightBoundingKey))
                : values;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(INode<TKey, TValue>? other)
        {
            return other is null
                ? 1
                : other is DataNode<TKey, TValue> dataNode
                    ? this.fields.Page.CompareTo(dataNode.fields.Page)
                    : throw new InvalidOperationException($"{nameof(other)} was wrong type: {other.GetType().Name}. Expected {nameof(DataNode<TKey, TValue>)}");
        }
    }
}
