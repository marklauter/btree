using BTrees.Pages;
using BTrees.Types;

namespace BTrees.Nodes
{

    internal sealed partial class DataNode<TKey, TValue> where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        private readonly record struct PageAndSibling(DataPage<TKey, TValue> Page, INode<TKey, TValue>? RightSibling)
        {
        }
    }
}
