using BTrees.Pages;

namespace BTrees.Nodes
{
    internal sealed class PartitionNode<TKey, TValue>
        : INode<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public static INode<TKey, TValue> Create(
            int size,
            INode<TKey, TValue> left,
            INode<TKey, TValue> right)
        {
            return left is null ? throw new ArgumentNullException(nameof(left))
                : right is null ? throw new ArgumentNullException(nameof(right))
                : (INode<TKey, TValue>)new PartitionNode<TKey, TValue>(size, left, right);
        }

        private PartitionNode(
            int size,
            INode<TKey, TValue> left,
            INode<TKey, TValue> right)
        {
            this.page = PartitionPage<TKey, TValue>
                .Create(size, right.MinKey, left, right);
        }

        private PartitionNode(PartitionPage<TKey, TValue> page)
        {
            this.page = page ?? throw new ArgumentNullException(nameof(page));
        }

        private readonly object gate = new();
        private PartitionPage<TKey, TValue> page;

        public int Count => this.page.Count;
        public bool IsEmpty => this.page.IsEmpty;
        public bool IsFull => this.page.IsFull;
        public bool IsOverflow => this.page.IsOverflow;
        public bool IsUnderflow => this.page.IsUnderflow;
        public TKey MinKey => this.page.MinKey;
        public TKey MaxKey => this.page.MaxKey;
        public int Size => this.page.Size;

        #region structure
        private void TryLock()
        {
            if (!Monitor.TryEnter(this.gate, 1000))
            {
                throw new LockFailedException();
            }
        }

        private void TryUnlock()
        {
            if (Monitor.IsEntered(this.gate))
            {
                Monitor.Exit(this.gate);
            }
        }

        public INode<TKey, TValue> Fork()
        {
            return new PartitionNode<TKey, TValue>(this.page);
        }

        public (INode<TKey, TValue> left, INode<TKey, TValue> right, TKey pivotKey) Split()
        {
            var (leftPage, rightPage, pivotKey) = this
                .page
                .Split();

            return (
                new PartitionNode<TKey, TValue>(leftPage),
                new PartitionNode<TKey, TValue>(rightPage),
                pivotKey);
        }

        public INode<TKey, TValue> Merge(INode<TKey, TValue> node)
        {
            return node is null
                ? throw new ArgumentNullException(nameof(node))
                : node is PartitionNode<TKey, TValue> partitionNode
                    ? (INode<TKey, TValue>)new PartitionNode<TKey, TValue>(this.page.Merge(partitionNode.page))
                    : throw new InvalidOperationException($"{nameof(node)} was wrong type: {node.GetType().Name}. Expected {nameof(PartitionNode<TKey, TValue>)}");
        }
        #endregion

        //private (PartitionPage<TKey, INode<TKey, TValue>> page, INode<TKey, TValue> subtree) MergeSubtree(
        //    int index,
        //    INode<TKey, TValue> subtree)
        //{
        //    if (index > 0) // subtree is not far left node
        //    {
        //        // merge from right to left
        //        subtree = this.page
        //            .Subtree(index - 1)
        //            .Merge(subtree);

        //        return (
        //            this.page.HandleLeftMerge(index, subtree),
        //            subtree);
        //    }
        //    else if (index < subtree.Count) // subtree still has at least one key and two values
        //    {
        //        // merge from left to right
        //        subtree = subtree
        //            .Merge(this.page.Subtree(index + 1));

        //        return (
        //            this.page.HandleRightMerge(index, subtree),
        //            subtree);
        //    }
        //    else // all keys are deleted and there is only one value remaining
        //    {
        //        // todo: pull the subtree up a level???
        //        throw new NotImplementedException();
        //    }
        //}

        #region reads
        public int BinarySearch(TKey key)
        {
            return this.page.BinarySearch(key);
        }

        public bool ContainsKey(TKey key)
        {
            return this
                .page
                .Read(key)
                .ContainsKey(key);
        }

        public bool TryRead(TKey key, out TValue? value)
        {
            return this
                .page
                .Read(key)
                .TryRead(key, out value);
        }
        #endregion

        #region writes
        public bool TryInsert(TKey key, TValue value)
        {
            this.TryLock();
            try
            {
                var (subtree, index) = this.page
                    .SelectSubtree(key);

                if (!subtree.IsFull)
                {
                    Monitor.Exit(this.gate);
                }

                var inserted = subtree.TryInsert(key, value);

                if (subtree.IsOverflow)
                {
                    this.page = this.page
                        .SplitSubtree(index, subtree);
                }

                return inserted;
            }
            finally
            {
                this.TryUnlock();
            }
        }

        public bool TryDelete(TKey key)
        {
            this.TryLock();
            try
            {
                var (subtree, index) = this
                    .page
                    .SelectSubtree(key);

                if (!subtree.IsUnderflow)
                {
                    Monitor.Exit(this.gate);
                }

                var deleted = subtree.TryDelete(key);

                if (subtree.IsUnderflow)
                {
                    this.page = this.page
                        .MergeSubtree(index, subtree);
                }

                return deleted;
            }
            finally
            {
                this.TryUnlock();
            }
        }

        public bool TryUpdate(TKey key, TValue value)
        {
            return this.page
                .Read(key)
                .TryUpdate(key, value);
        }
        #endregion
    }
}
