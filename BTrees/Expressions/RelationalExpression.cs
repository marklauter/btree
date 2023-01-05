namespace BTrees.Expressions
{
    public class RelationalExpression<TKey>
        : Expression<TKey>
        where TKey : IComparable<TKey>
    {
        public RelationalExpression<TKey> Equals(TKey value)
        {
            return new RelationalExpression<TKey>(value, RelationalOperator.Equal);
        }

        public RelationalExpression<TKey> NotEqual(TKey value)
        {
            return new RelationalExpression<TKey>(value, RelationalOperator.NotEqual);
        }

        public RelationalExpression<TKey> GreaterThan(TKey value)
        {
            return new RelationalExpression<TKey>(value, RelationalOperator.GreaterThan);
        }

        public RelationalExpression<TKey> LessThan(TKey value)
        {
            return new RelationalExpression<TKey>(value, RelationalOperator.LessThan);
        }

        public RelationalExpression<TKey> GreaterThanOrEqualTo(TKey value)
        {
            return new RelationalExpression<TKey>(value, RelationalOperator.GreaterThanOrEqual);
        }

        public RelationalExpression<TKey> LessThanOrEqualTo(TKey value)
        {
            return new RelationalExpression<TKey>(value, RelationalOperator.LessThanOrEqual);
        }

        private RelationalExpression(TKey value, RelationalOperator @operator)
        {
            this.Value = value;
            this.Operator = @operator;
        }

        public TKey Value { get; }
        public RelationalOperator Operator { get; }
    }
}
