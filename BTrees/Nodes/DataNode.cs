using BTrees.Pages;
using BTrees.Types;
using System.Runtime.CompilerServices;

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
        private DataPage<TKey, TValue> page = DataPage<TKey, TValue>.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataNode(TKey key, TValue value)
        {
            this.page = this.page
                .Insert(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DataNode(
            DataPage<TKey, TValue> page,
            INode<TKey, TValue>? rightSibling)
        {
            this.page = page;
            this.RightSibling = rightSibling;
        }
        public static int MaxSize { get; private set; } = 1024 * 4;
        public static int HalfSize { get; private set; } = 1024 * 2;
        public int Length => this.page.Length;
        public bool IsOverflow => this.Size >= MaxSize;
        public bool IsUnderflow => this.Size < HalfSize;
        public int Size => this.page.Size;
        public INode<TKey, TValue>? RightSibling { get; private set; }

        public static void SetMaxSize(int size)
        {
            MaxSize = size;
            HalfSize = MaxSize >> 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return this.page.Count();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public INode<TKey, TValue> Fork()
        {
            return new DataNode<TKey, TValue>(this.page, this.RightSibling);
        }

        public void Merge(INode<TKey, TValue> node)
        {
            lock (this.gate)
            {
                if (node is DataNode<TKey, TValue> dataNode)
                {
                    this.page = this.page.Merge(dataNode.page);
                    this.RightSibling = dataNode.RightSibling;
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(node)} was wrong type: {node.GetType().Name}. Expected {nameof(DataNode<TKey, TValue>)}");
                }
            }
        }

        public INode<TKey, TValue> Split()
        {
            lock (this.gate)
            {
                var splitResult = this.page.Split();
                this.page = splitResult.LeftPage;
                var node = new DataNode<TKey, TValue>(splitResult.RightPage, this.RightSibling);
                this.RightSibling = node;
                return node;
            }
        }

        public void Delete(TKey key)
        {
            lock (this.gate)
            {
                this.page = this.page.Remove(key);
            }
        }

        public void Delete(TKey key, TValue value)
        {
            lock (this.gate)
            {
                this.page = this.page.Remove(key, value);
            }
        }

        public void Insert(TKey key, TValue value)
        {
            lock (this.gate)
            {
                this.page = this.page.Insert(key, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            return this.page.ContainsKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TValue> Read(TKey key)
        {
            return this.page.Read(key);
        }

        public IEnumerable<(TKey, TValue)> Read(TKey leftBoundingKey, TKey rightBoundingKey)
        {
            var page = this.page;
            var sibling = this.RightSibling;
            var leftBoundingIndex = page.IndexOf(leftBoundingKey);
            var rightBoundingIndex = page.IndexOf(rightBoundingKey);
            leftBoundingIndex = leftBoundingIndex >= 0 ? leftBoundingIndex : ~leftBoundingIndex;
            rightBoundingIndex = rightBoundingIndex >= 0 ? rightBoundingIndex : ~rightBoundingIndex;

            var values = page.Read(leftBoundingIndex..rightBoundingIndex);
            return rightBoundingIndex == page.Length && sibling is not null // rightBound key exceeded right edge of the current page
                ? values.Union(sibling.Read(leftBoundingKey, rightBoundingKey))
                : values;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(INode<TKey, TValue>? other)
        {
            return other is null
                ? 1
                : other is DataNode<TKey, TValue> dataNode
                    ? this.page.CompareTo(dataNode.page)
                    : throw new InvalidOperationException($"{nameof(other)} was wrong type: {other.GetType().Name}. Expected {nameof(DataNode<TKey, TValue>)}");
        }
    }
}
