﻿using BTrees.Pages;

namespace BTrees.Nodes
{
    internal sealed class PartitionNode<TKey, TValue>
        : INode<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public static INode<TKey, TValue> Create(
            int size,
            INode<TKey, TValue> leftNode,
            INode<TKey, TValue> rightNode)
        {
            return new PartitionNode<TKey, TValue>(size, leftNode, rightNode);
        }

        private PartitionNode(
            int size,
            INode<TKey, TValue> leftNode,
            INode<TKey, TValue> rightNode)
        {
            this.page = PartitionPage<TKey, INode<TKey, TValue>>
                .Create(size, rightNode.MinKey, leftNode, rightNode);
        }

        private PartitionNode(PartitionPage<TKey, INode<TKey, TValue>> page)
        {
            this.page = page ?? throw new ArgumentNullException(nameof(page));
        }

        private readonly object gate = new();
        private PartitionPage<TKey, INode<TKey, TValue>> page;

        public int Count => this.page.Count;
        public bool IsEmpty => this.page.IsEmpty;
        public bool IsFull => this.page.IsFull;
        public bool IsOverflow => this.page.IsOverflow;
        public bool IsUnderflow => this.page.IsUnderflow;
        public TKey MinKey => this.page.MinKey;
        public TKey MaxKey => this.page.MaxKey;
        public int Size => this.page.Size;

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

        public INode<TKey, TValue> Merge(INode<TKey, TValue> node)
        {
            return node is null
                ? throw new ArgumentNullException(nameof(node))
                : node is PartitionNode<TKey, TValue> partitionNode
                    ? (INode<TKey, TValue>)new PartitionNode<TKey, TValue>(this.page.Merge(partitionNode.page))
                    : throw new InvalidOperationException($"{nameof(node)} was wrong type: {node.GetType().Name}. Expected {nameof(PartitionNode<TKey, TValue>)}");
        }

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

        public void Delete(TKey key)
        {
            this.TryLock();
            try
            {
                var (subtree, index) = this
                    .page
                    .SelectSubtree(key);

                if (subtree.Count > 1)
                {
                    Monitor.Exit(this.gate);
                }

                subtree.Delete(key);

                if (subtree.IsEmpty)
                {
                    this.page = this.page
                        .RemoveSubtree(index);
                }
            }
            finally
            {
                this.TryUnlock();
            }
        }

        public void Insert(TKey key, TValue value)
        {
            this.TryLock();
            try
            {
                var (subtree, index) = this
                    .page
                    .SelectSubtree(key);

                if (!subtree.IsFull)
                {
                    Monitor.Exit(this.gate);
                }

                subtree.Insert(key, value);

                if (subtree.IsOverflow)
                {
                    this.page = this
                        .SplitSubtree(index, subtree);
                }
            }
            finally
            {
                this.TryUnlock();
            }
        }

        private PartitionPage<TKey, INode<TKey, TValue>> SplitSubtree(
            int subtreeIndex,
            INode<TKey, TValue> subtree)
        {
            var (leftNode, rightNode, pivotKey) = subtree
                .Split();

            return this.page.WriteSplit(
                subtreeIndex,
                leftNode,
                rightNode,
                pivotKey);
        }

        public void Update(TKey key, TValue value)
        {
            var subtree = this.page.Read(key);
            subtree.Update(key, value);
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

        public int BinarySearch(TKey key)
        {
            return this.page.BinarySearch(key);
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
    }
}
