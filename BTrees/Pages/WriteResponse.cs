namespace BTrees.Pages
{
    internal sealed class SplitResponse<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public SplitResponse(
            Page<TKey, TValue>? leftPage,
            Page<TKey, TValue>? rightPage)
        {
            this.LeftPage = leftPage;
            this.RightPage = rightPage;
        }

        public Page<TKey, TValue>? LeftPage { get; }
        public Page<TKey, TValue>? RightPage { get; }
    }

    internal sealed class WriteResponse<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public WriteResponse(WriteResult result)
            : this(false, result)
        {
        }

        public WriteResponse(
            bool wasSplit,
            WriteResult result)
            : this(wasSplit, result, null, null)
        {
        }

        public WriteResponse(
            SplitResponse<TKey, TValue> splitResponse,
            WriteResult result)
            : this(
                  splitResponse is not null,
                  splitResponse?.LeftPage ?? throw new ArgumentNullException(nameof(splitResponse)),
                  splitResponse?.RightPage ?? throw new ArgumentNullException(nameof(splitResponse)),
                  result)
        {
        }

        internal WriteResponse(
            bool wasSplit,
            WriteResult result,
            Page<TKey, TValue>? newLeftPage,
            Page<TKey, TValue>? newRightPage)
        {
            this.WasSplit = wasSplit;
            this.NewLeftPage = newLeftPage;
            this.NewRightPage = newRightPage;
            this.Result = result;
        }

        public bool WasSplit { get; }
        public WriteResult Result { get; }
        public Page<TKey, TValue>? NewLeftPage { get; }
        public Page<TKey, TValue>? NewRightPage { get; }
    }
}
