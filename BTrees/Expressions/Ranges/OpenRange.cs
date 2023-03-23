namespace BTrees.Expressions.Ranges
{
    public sealed record OpenRange<TKey>(TKey Key, BooleanOperator Operator)
        where TKey : struct, IComparable<TKey>
    {
        public static OpenRange<TKey> Equals(TKey key)
        {
            return new OpenRange<TKey>(key, BooleanOperator.Equal);
        }

        public static OpenRange<TKey> IsGreaterThan(TKey key)
        {
            return new OpenRange<TKey>(key, BooleanOperator.GreaterThan);
        }

        public static OpenRange<TKey> IsGreaterThanOrEqual(TKey key)
        {
            return new OpenRange<TKey>(key, BooleanOperator.GreaterThanOrEqual);
        }

        public static OpenRange<TKey> IsLessThan(TKey key)
        {
            return new OpenRange<TKey>(key, BooleanOperator.LessThan);
        }

        public static OpenRange<TKey> IsLessThanOrEqual(TKey key)
        {
            return new OpenRange<TKey>(key, BooleanOperator.LessThanOrEqual);
        }
    }
}
