using BTrees.Pages;
using BTrees.Types;

namespace BTrees.Nodes
{
    internal sealed partial class DataNode<TKey, TValue> where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        private record PageAndSibling(DataPage<TKey, TValue> Page, INode<TKey, TValue>? RightSibling)
        {
        }

        private PageAndSibling pageAndSibling = new(DataPage<TKey, TValue>.Empty, null);
    }
}
