namespace BTrees.Expressions.Ranges
{
    public sealed class ClosedRange<TKey>
        where TKey : IComparable<TKey>
    {
        public TKey Left { get; }
        public TKey Right { get; }
        public RangeSemantics RangeSemantics { get; }

        public static ClosedRange<TKey> Exclusive(TKey left, TKey right)
        {
            return new ClosedRange<TKey>(left, right, RangeSemantics.Exclusive);
        }

        public static ClosedRange<TKey> LeftInclusive(TKey left, TKey right)
        {
            return new ClosedRange<TKey>(left, right, RangeSemantics.LeftInclusive);
        }

        public static ClosedRange<TKey> RightInclusive(TKey left, TKey right)
        {
            return new ClosedRange<TKey>(left, right, RangeSemantics.RightInclusive);
        }

        public static ClosedRange<TKey> AllInclusive(TKey left, TKey right)
        {
            return new ClosedRange<TKey>(left, right, RangeSemantics.AllInclusive);
        }

        private ClosedRange(TKey left, TKey right, RangeSemantics rangeSemantics)
        {
            if (left.CompareTo(right) >= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(left));
            }

            this.Left = left;
            this.Right = right;
            this.RangeSemantics = rangeSemantics;
        }
    }
}
