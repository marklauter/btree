namespace Leaf.Tests
{
    //https://www.youtube.com/watch?v=yuEbZYKgZas see monitor idea around 30 minutes

    internal class PivotNode<TKey, TValue>
        : Node<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        internal readonly TKey[] keys;
        internal readonly Node<TKey, TValue>[] subtrees;
        private readonly int size;

        public PivotNode(
            int size,
            TKey key,
            Node<TKey, TValue> left,
            Node<TKey, TValue> right)
        {
            this.keys = new TKey[]
            {
                key,
            };

            this.subtrees = new Node<TKey, TValue>[]
            {
                left,
                right,
            };

            this.size = size;
            this.PivotKey = key;
        }

        public PivotNode(
            int size,
            TKey[] keys,
            Node<TKey, TValue>[] subtrees)
        {
            if (keys.Length != subtrees.Length - 1)
            {
                throw new ArgumentException($"{nameof(keys)}.Length != {nameof(subtrees)}.Length");
            }

            this.keys = keys;
            this.subtrees = subtrees;
            this.size = size;
            this.PivotKey = keys[0];
        }

        public override int Count => this.keys.Length;
        internal override bool IsOverflow => this.size == this.Count;
        internal override bool IsUnderflow => this.Count < this.size / 2;

        internal override Node<TKey, TValue> SelectSubtree(TKey key)
        {
            var index = Array.BinarySearch(this.keys, key);
            index = index < 0
                ? ~index
                : index + 1;

            return this.subtrees[index];
        }

        public override Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override bool TryRead(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public override async Task<WriteResponse> TryWriteAsync(TKey key, TValue value, CancellationToken cancellationToken)
        {
            if (!await this.TryAquireLock(this.LockTimeout, cancellationToken))
            {
                return WriteResponse.TimedoutAwaitingLock;
            }

            try
            {
                var subtree = this.SelectSubtree(key);
                if (!subtree.IsOverflow)
                {
                    if (!this.TryReleaseLock())
                    {
                        throw new InvalidOperationException("lock not held");
                    }
                }
                else
                {
                    if (!await TryLockForSplitAsync(subtree, this.LockTimeout, cancellationToken))
                    {
                        return WriteResponse.TimedoutAwaitingLock;
                    }

                    var (left, right) = subtree.Split();
                    subtree = key.CompareTo(right.PivotKey) < 0
                        ? left
                        : right;

                    // todo: update this node in a copy-on-write manner
                }

                var subtreeWriteResponse = await subtree.TryWriteAsync(key, value, cancellationToken);

                // todo: unwind all the locks
                return subtreeWriteResponse;
            }
            finally
            {
                _ = this.TryReleaseLock();
            }
        }

        internal override Node<TKey, TValue> MergeInto(Node<TKey, TValue> deprecated)
        {
            throw new NotImplementedException();
        }

        internal override (Node<TKey, TValue> left, Node<TKey, TValue> right) Split()
        {
            // todo: this is the leaf split - needs to be updated for pivot nodes
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
                this.subtrees,
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
                this.subtrees,
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
    }
}
