using BTrees.Types;

namespace BTrees.Nodes
{
    internal sealed partial class DataNode<TKey, TValue>
        : INode<TKey, TValue>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        public void Merge(INode<TKey, TValue> node)
        {
            lock (this)
            {
                if (node is DataNode<TKey, TValue> dataNode)
                {
                    var pageAndSibling = Volatile.Read(ref this.pageAndSibling);
                    Volatile.Write(
                        ref this.pageAndSibling,
                        new PageAndSibling(
                            pageAndSibling.Page.Merge(dataNode.pageAndSibling.Page),
                            dataNode.RightSibling));
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
                var pageAndSibling = Volatile.Read(ref this.pageAndSibling);
                var splitResult = pageAndSibling.Page.Split();

                var node = new DataNode<TKey, TValue>(
                    new PageAndSibling(
                        splitResult.RightPage,
                        pageAndSibling.RightSibling),
                    this.MaxSize);

                Volatile.Write(
                    ref this.pageAndSibling,
                    new PageAndSibling(
                        splitResult.LeftPage,
                        node));

                return node;
            }
        }

        public void Remove(TKey key)
        {
            lock (this)
            {
                var pageAndSibling = Volatile.Read(ref this.pageAndSibling);

                Volatile.Write(
                    ref this.pageAndSibling,
                    new PageAndSibling(
                        pageAndSibling.Page.Remove(key),
                        pageAndSibling.RightSibling));
            }
        }

        public void Remove(TKey key, TValue value)
        {
            lock (this)
            {
                var pageAndSibling = Volatile.Read(ref this.pageAndSibling);

                Volatile.Write(
                    ref this.pageAndSibling,
                    new PageAndSibling(
                        pageAndSibling.Page.Remove(key, value),
                        pageAndSibling.RightSibling));
            }
        }

        public void Insert(TKey key, TValue value)
        {
            lock (this)
            {
                var pageAndSibling = Volatile.Read(ref this.pageAndSibling);

                Volatile.Write(
                    ref this.pageAndSibling,
                    new PageAndSibling(
                        pageAndSibling.Page.Insert(key, value),
                        pageAndSibling.RightSibling));
            }
        }
    }
}
