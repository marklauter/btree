//using BTrees.Expressions.Ranges;
//using BTrees.Pages;
//using System.Diagnostics;

//namespace BTrees
//{
//    [DebuggerDisplay("{Count}")]
//    internal class BTree<TKey, TValue>
//        : IBTree<TKey, TValue>
//        , IDisposable
//        where TKey : IComparable<TKey>
//    {
//        private bool disposed;

//        public long Count { get; private set; }

//        public int Height { get; private set; } = 1;

//        private readonly int pageSize;
//        private Page<TKey, TValue> root;

//        public BTree(int pageSize)
//        {
//            this.pageSize = pageSize;
//            this.root = new LeafPage<TKey, TValue>(pageSize);
//        }

//        public bool TryDelete(TKey key)
//        {
//            var deleted = this.root.TryDelete(key, out var mergeInfo);
//            if (deleted)
//            {
//                --this.Count;
//                if (this.root is PivotPage<TKey, TValue> rootPage)
//                {
//                    if (mergeInfo.merged)
//                    {
//#pragma warning disable CS8604 // Possible null reference argument.
//                        _ = rootPage.RemoveKey(mergeInfo.deprecatedPivotKey, out _);
//#pragma warning restore CS8604 // Possible null reference argument.
//                    }

//                    if (rootPage.IsEmpty)
//                    {
//                        var root = this.root;
//                        this.root = rootPage.pages[0];
//                        root.Dispose();
//                        --this.Height;
//                    }
//                }
//            }

//            return deleted;
//        }

//        public bool TryWrite(TKey key, TValue value)
//        {
//            var writeSucceeded = this.root.TryWrite(key, value, out var response);
//            if (response.newPage is not null)
//            {
//#pragma warning disable IDISP003 // Dispose previous before re-assigning
//#pragma warning disable CS8604 // Possible null reference argument.
//                this.root = new PivotPage<TKey, TValue>(
//                    this.pageSize,
//                    this.root,
//                    response.newPage,
//                    response.newPivotKey);
//#pragma warning restore CS8604 // Possible null reference argument.
//#pragma warning restore IDISP003 // Dispose previous before re-assigning

//                ++this.Height;
//            }

//            if (response.result == WriteResult.Inserted)
//            {
//                ++this.Count;
//            }

//            return writeSucceeded;
//        }

//        public bool TryRead(TKey key, out TValue? value)
//        {
//            return this.root.TryRead(key, out value);
//        }

//        // todo: add unit tests
//        public IEnumerable<TValue> Read(OpenRange<TKey> range)
//        {
//            // todo: this can use the leaf page's right sibling
//            throw new NotImplementedException();
//        }

//        // todo: add unit tests
//        public IEnumerable<TValue> Read(ClosedRange<TKey> range)
//        {
//            // todo: this can use the leaf page's right sibling
//            // todo: if we identify the left and right most pages then we can pre-load first, last and all pages between that are involved in the range scan
//            throw new NotImplementedException();
//        }

//        public bool TryUpdate(TKey key, TValue value)
//        {
//            throw new NotImplementedException();
//        }

//        public virtual void Dispose()
//        {
//            if (this.disposed)
//            {
//                return;
//            }

//            this.root.Dispose();

//            this.disposed = true;
//        }

//        protected virtual void ThrowIfDisposed()
//        {
//            if (this.disposed)
//            {
//                throw new ObjectDisposedException(this.GetType().FullName);
//            }
//        }
//    }
//}
