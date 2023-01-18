using System.Diagnostics;

namespace BTrees.Pages
{
    [DebuggerDisplay("{Count}")]
    internal abstract class Page<TKey, TValue>
        : IDisposable
        where TKey : IComparable<TKey>
    {
        private bool disposed;

        internal TKey[] Keys { get; }

        public int Count { get; protected set; }
        public bool IsEmpty => this.Count == 0;
        public bool IsUnderFlow => this.Count < this.Size / 2;
        public bool IsOverflow => this.Count == this.Size;
        public Page<TKey, TValue>? LeftSibling { get; }
        public Page<TKey, TValue>? RightSibling { get; private set; }
        public int Size { get; }
        public TKey MinKey => this.Count != 0 ? this.Keys[0] : throw new InvalidOperationException($"{nameof(this.MinKey)} is undefined when Count == 0");
        public TKey MaxKey => this.Count != 0 ? this.Keys[this.Count - 1] : throw new InvalidOperationException($"{nameof(this.MaxKey)} is undefined when Count == 0");
        public TKey? PivotKey { get; protected set; }
        public abstract int Order { get; }

        private readonly SemaphoreSlim gate = new(1);

        #region CTOR
        public Page(int size)
        {
            this.Size = size;
            this.Keys = new TKey[size];
        }

        protected Page(
            int size,
            Page<TKey, TValue> leftSibling)
            : this(size)
        {
            this.LeftSibling = leftSibling ?? throw new ArgumentNullException(nameof(leftSibling));
            leftSibling.RightSibling = this;
        }
        #endregion

        protected Task<bool> TryAquireLockAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return this.gate.WaitAsync(timeout, cancellationToken);
        }

        protected void ReleaseLock()
        {
            _ = this.gate.Release();
        }

        protected abstract void ShiftLeft(int index);
        protected abstract void ShiftRight(int index);

        internal abstract void Merge(Page<TKey, TValue> sourcePage);
        internal abstract Page<TKey, TValue> SelectSubtree(TKey key);
        internal abstract (Page<TKey, TValue> newPage, TKey newPivotKey) Split();

        internal int IndexOfKey(TKey key)
        {
            var low = 0;
            var high = this.Count - 1;
            var keys = new Span<TKey>(this.Keys, 0, this.Count);

            while (low <= high)
            {
                var middle = (low + high) / 2;

                var comparison = keys[middle].CompareTo(key);

                if (comparison == 0)
                {
                    return middle;
                }

                if (comparison < 0)
                {
                    low = middle + 1;
                    continue;
                }

                high = middle - 1;
                continue;
            }

            return ~low;
        }

        internal bool CanMerge(Page<TKey, TValue>? sourcePage)
        {
            return sourcePage is not null
                && this.Count + sourcePage.Count <= this.Size;
        }

        internal bool RemoveKey(TKey key, out (bool merged, TKey? deprecatedPivotKey) mergeInfo)
        {
            mergeInfo.merged = false;
            mergeInfo.deprecatedPivotKey = default;

            if (this.IsEmpty)
            {
                return false;
            }

            var index = this.IndexOfKey(key);
            if (index < 0)
            {
                return false;
            }

            if (this.Count > 1)
            {
                this.ShiftLeft(index);
            }

            if (this.Count > 0)
            {
                --this.Count;
            }

            if (this.IsEmpty && this.LeftSibling is not null)
            {
                mergeInfo.deprecatedPivotKey = this.PivotKey;
                mergeInfo.merged = true;
                return true;
            }

            if (this.IsUnderFlow)
            {
                var rightSiblingCount = this.RightSibling is null
                    ? this.Size
                    : this.RightSibling.Count;

                // Source is tuple.Item1 and destination is tuple.Item2.
                // Array is ordered by preferred merge candidates based on smallest required mem copy during the merge operation.
                var candidates = rightSiblingCount < this.Count
                    ? new Tuple<Page<TKey, TValue>?, Page<TKey, TValue>?>[]
                    {
                        new (this.RightSibling, this),
                        new (this, this.LeftSibling)
                    }
                    : new Tuple<Page<TKey, TValue>?, Page<TKey, TValue>?>[]
                    {
                        new (this, this.LeftSibling),
                        new (this.RightSibling, this)
                    };

                for (var i = 0; i < 2; ++i)
                {
                    var sourceCandidate = candidates[i].Item1;
                    var destinationCandidate = candidates[i].Item2;

                    if (destinationCandidate is not null
                        && sourceCandidate is not null
                        && destinationCandidate.CanMerge(sourceCandidate))
                    {
                        destinationCandidate.Merge(sourceCandidate);
                        mergeInfo.deprecatedPivotKey = sourceCandidate.PivotKey;
                        destinationCandidate.RightSibling = sourceCandidate.RightSibling;

                        mergeInfo.merged = true;

                        return true;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Merge left on underflow.
        /// </summary>
        /// <returns>True if delete was successful.</returns>
        public abstract bool TryDelete(TKey key, out (bool merged, TKey? deprecatedPivotKey) mergeInfo);

        public abstract bool TryWrite(TKey key, TValue value, out (Page<TKey, TValue>? newPage, TKey? newPivotKey, WriteResult result) response);

        public abstract bool TryRead(TKey key, out TValue? value);

        protected virtual void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        public virtual void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.gate.Dispose();

            this.disposed = true;
        }
    }
}
