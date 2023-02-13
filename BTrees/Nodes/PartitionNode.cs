//using BTrees.Pages;

//namespace BTrees.Nodes
//{
//    internal sealed class PartitionNode<TKey, TValue>
//        : Node<TKey, TValue>
//        where TKey : IComparable<TKey>
//    {
//        public static INode<TKey, TValue> Create(
//            int size,
//            INode<TKey, TValue> leftNode,
//            INode<TKey, TValue> rightNode)
//        {
//            return new PartitionNode<TKey, TValue>(size, leftNode, rightNode);
//        }

//        private PartitionNode(
//            int size,
//            INode<TKey, TValue> leftNode,
//            INode<TKey, TValue> rightNode)
//        {
//            this.page = PartitionPage<TKey, INode<TKey, TValue>>
//                .Create(size, rightNode.MinKey, leftNode, rightNode);
//        }

//        private readonly object gate = new();
//        private PartitionPage<TKey, INode<TKey, TValue>> page;

//        public override int Count => this.page.Count;
//        public override bool IsEmpty => this.page.IsEmpty;
//        public override bool IsFull => this.page.IsFull;
//        public override bool IsOverflow => this.page.IsOverflow;
//        public override bool IsUnderflow => this.page.IsUnderflow;
//        public override TKey MinKey => this.page.MinKey;
//        public override TKey MaxKey => this.page.MaxKey;
//        public override int Size => this.page.Size;

//        public override void Delete(TKey key)
//        {
//            // todo: switch to TryEnter and throw on fail
//            Monitor.Enter(this.gate);
//            try
//            {
//                var (subtree, index) = this
//                    .page
//                    .SelectSubtree(key);

//                if (!subtree.IsUnderflow)
//                {
//                    Monitor.Exit(this.gate);
//                }

//                subtree.Delete(key);

//                // todo: handle underflow
//            }
//            finally
//            {
//                if (Monitor.IsEntered(this.gate))
//                {
//                    Monitor.Exit(this.gate);
//                }
//            }
//        }

//        public override void Insert(TKey key, TValue value)
//        {
//            // todo: switch to TryEnter and throw on fail
//            Monitor.Enter(this.gate);
//            try
//            {
//                var (subtree, index) = this
//                    .page
//                    .SelectSubtree(key);

//                if (!subtree.IsFull)
//                {
//                    Monitor.Exit(this.gate);
//                }

//                subtree.Insert(key, value);

//                if (subtree.IsOverflow)
//                {
//                    var (left, right, pivotKey) = subtree.Split();

//                    this.page = this.page
//                        .Update(index, left)
//                        .Insert(pivotKey, right);
//                }

//                // todo: handle overflow
//            }
//            finally
//            {
//                if (Monitor.IsEntered(this.gate))
//                {
//                    Monitor.Exit(this.gate);
//                }
//            }
//        }

//        public override INode<TKey, TValue> Merge(INode<TKey, TValue> node)
//        {
//            throw new NotImplementedException();
//        }

//        public override (INode<TKey, TValue> left, INode<TKey, TValue> right, TKey pivotKey) Split()
//        {
//            throw new NotImplementedException();
//        }

//        public override void Update(TKey key, TValue value)
//        {
//            // todo: switch to TryEnter and throw on fail
//            Monitor.Enter(this.gate);
//            try
//            {
//                var subtree = this.SelectSubtree(key);
//                if (!subtree.IsFull)
//                {
//                    Monitor.Exit(this.gate);
//                }

//                subtree.Update(key, value);
//            }
//            finally
//            {
//                if (Monitor.IsEntered(this.gate))
//                {
//                    Monitor.Exit(this.gate);
//                }
//            }
//        }

//        public override bool ContainsKey(TKey key)
//        {
//            return this
//                .SelectSubtree(key)
//                .ContainsKey(key);
//        }

//        public override bool TryRead(TKey key, out TValue? value)
//        {
//            return this
//                .SelectSubtree(key)
//                .TryRead(key, out value);
//        }
//    }
//}
