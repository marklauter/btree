namespace Leaf.Tests
{
    internal class LeafNode<TKey, TValue>
        : Node<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        internal readonly TKey[] keys;
        internal readonly TValue[] values;
        private readonly int size;

        public static LeafNode<TKey, TValue> Empty => new();

        private LeafNode()
        {
            this.keys = Array.Empty<TKey>();
            this.values = Array.Empty<TValue>();
        }

        public LeafNode(int size, TKey key, TValue value)
        {
            this.keys = new TKey[]
            {
                key,
            };

            this.values = new TValue[]
            {
                value,
            };

            this.size = size;
            this.PivotKey = key;
        }

        public LeafNode(
            int size,
            TKey[] keys,
            TValue[] values)
        {
            if (keys.Length != values.Length)
            {
                throw new ArgumentException($"{nameof(keys)}.Length != {nameof(values)}.Length");
            }

            this.keys = keys;
            this.values = values;
            this.size = size;
            this.PivotKey = keys[0];
        }

        public override int Count => this.keys.Length;
        internal override bool IsOverflow => this.Count >= this.size;
        internal override bool IsUnderflow => this.Count < this.size / 2;

        internal override (Node<TKey, TValue> left, Node<TKey, TValue> right) Split()
        {
            var pivotIndex = this.Count / 2;

            var leftKeys = new TKey[pivotIndex];
            Array.Copy(
                this.keys,
                0,
                leftKeys,
                0,
                pivotIndex);
            var leftValues = new TValue[pivotIndex];
            Array.Copy(
                this.values,
                0,
                leftValues,
                0,
                pivotIndex);
            var leftPage = new LeafNode<TKey, TValue>(
                this.size,
                leftKeys,
                leftValues);

            var rightPageSize = this.Count - pivotIndex;
            var rightKeys = new TKey[rightPageSize];
            Array.Copy(
                this.keys,
                pivotIndex,
                rightKeys,
                0,
                rightPageSize);
            var rightValues = new TValue[pivotIndex];
            Array.Copy(
                this.values,
                pivotIndex,
                rightKeys,
                0,
                rightPageSize);
            var rightPage = new LeafNode<TKey, TValue>(
                this.size,
                rightKeys,
                rightValues);

            leftPage.LeftSibling = this.LeftSibling;
            leftPage.RightSibling = rightPage;
            rightPage.LeftSibling = leftPage;
            rightPage.RightSibling = this.RightSibling;

            return (leftPage, rightPage);
        }

        public override bool TryRead(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public override Task<WriteResponse> TryWriteAsync(TKey key, TValue value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal override Node<TKey, TValue> MergeInto(Node<TKey, TValue> right)
        {
            throw new NotImplementedException();
        }

        internal override Node<TKey, TValue> SelectSubtree(TKey key)
        {
            return this;
        }
    }
}
