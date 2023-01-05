namespace BTrees.Expressions
{
    public class SkipExpression<TKey>
        : Expression<TKey>
        where TKey : IComparable<TKey>
    {
        public SkipExpression(int skip)
        {
            if (skip < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(skip));
            }

            this.Skip = skip;
        }

        public int Skip { get; }
    }
}
