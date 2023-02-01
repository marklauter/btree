namespace Leaf.Tests
{
    internal abstract class Node<TKey, TValue>
        : IDisposable
        where TKey : IComparable<TKey>
    {
        public abstract int Count { get; }
        internal abstract bool IsOverflow { get; }
        internal abstract bool IsUnderflow { get; }
        internal Node<TKey, TValue>? LeftSibling { get; set; }
        internal Node<TKey, TValue>? RightSibling { get; set; }
        public TKey? PivotKey { get; protected set; }
        private readonly SemaphoreSlim gate = new(1);
        protected TimeSpan LockTimeout { get; } = TimeSpan.FromSeconds(1);

        internal abstract (Node<TKey, TValue> left, Node<TKey, TValue> right) Split();
        internal abstract Node<TKey, TValue> MergeInto(Node<TKey, TValue> deprecated);
        internal abstract Node<TKey, TValue> SelectSubtree(TKey key);

        public abstract Task<WriteResponse> TryWriteAsync(TKey key, TValue value, CancellationToken cancellationToken);
        public abstract bool TryRead(TKey key, out TValue value);
        public abstract Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken);

        public void Dispose()
        {
            ((IDisposable)this.gate).Dispose();
        }

        internal Task<bool> TryAquireLock(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return this.gate.WaitAsync(timeout, cancellationToken);
        }

        internal bool TryReleaseLock()
        {
            return this.gate.Release() > 0;
        }

        internal static async Task<bool> TryLockForSplitAsync(
            Node<TKey, TValue> source,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

#pragma warning disable CS8601 // Possible null reference assignment. - nulls are filtered with linq
            var tasks = new Task<bool>[]
            {
                source.TryAquireLock(timeout, cancellationToken),
                source.LeftSibling?.TryAquireLock(timeout, cancellationToken),
                source.RightSibling?.TryAquireLock(timeout, cancellationToken),
            };
#pragma warning restore CS8601 // Possible null reference assignment.

            var allLocksAquired = (await Task.WhenAll(tasks.Where(t => t is not null)))
                .All(lr => lr);

            if (!allLocksAquired)
            {
                _ = source.TryReleaseLock();
                _ = source.LeftSibling?.TryReleaseLock();
                _ = source.RightSibling?.TryReleaseLock();
            }

            return allLocksAquired;
        }
    }
}
