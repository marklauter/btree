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
        private const int DefaultMaxSize = 1024 * 4;

        public static DataNode<TKey, TValue> Empty()
        {
            return new DataNode<TKey, TValue>(
                new PageAndSibling(DataPage<TKey, TValue>.Empty, null),
                DefaultMaxSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataNode(TKey key, TValue value)
            : this(key, value, DefaultMaxSize)
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
            this.pageAndSibling = fields;
        }
    }
}
