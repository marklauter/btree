using BTrees.Types;
using System.Diagnostics.Contracts;

namespace BTrees.Pages
{
    public readonly partial struct DataPage<TKey, TValue>
        : IComparable<DataPage<TKey, TValue>>
        where TKey : ISizeable, IComparable<TKey>
        where TValue : ISizeable, IComparable<TValue>
    {
        [Pure]
        public DataPage<TKey, TValue> Remove(TKey key)
        {
            var page = this;
            var keyIndex = this.IndexOf(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                page = new DataPage<TKey, TValue>(this.tuples.RemoveAt(keyIndex));
            }

            return page;
        }

        [Pure]
        public DataPage<TKey, TValue> Remove(TKey key, TValue value)
        {
            var page = this;
            var keyIndex = this.IndexOf(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                var tuple = this.tuples[keyIndex];
                var removeResult = tuple.Remove(value);
                if (tuple != removeResult)
                {
                    page = removeResult.IsEmpty
                        ? new DataPage<TKey, TValue>(this.tuples.RemoveAt(keyIndex))
                        : new DataPage<TKey, TValue>(this.tuples.SetItem(keyIndex, removeResult));
                }
            }

            return page;
        }

        [Pure]
        public DataPage<TKey, TValue> Insert(TKey key, TValue value)
        {
            var page = this;
            var keyIndex = this.IndexOf(key);
            var containsKey = keyIndex >= 0;
            if (containsKey)
            {
                var tuple = this.tuples[keyIndex];
                var valueIndex = tuple.IndexOf(value);
                var constainsValue = valueIndex >= 0;
                if (!constainsValue)
                {
                    page = new DataPage<TKey, TValue>(this.tuples.SetItem(keyIndex, tuple.Insert(~valueIndex, value)));
                }
            }
            else
            {
                page = new DataPage<TKey, TValue>(this.tuples.Insert(~keyIndex, new KeyValuesTuple(key, value)));
            }

            return page;
        }
    }
}
