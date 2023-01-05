namespace BTrees.Expressions
{
    public class LogicalExpression<TKey>
        : Expression<TKey>
        where TKey : IComparable<TKey>
    {
        public static LogicalExpression<TKey> And(
            Expression<TKey> leftOperand,
            Expression<TKey> rightOperand)
        {
            return new LogicalExpression<TKey>(leftOperand, rightOperand, LogicalOperator.And);
        }

        public static LogicalExpression<TKey> Or(
            Expression<TKey> leftOperand,
            Expression<TKey> rightOperand)
        {
            return new LogicalExpression<TKey>(leftOperand, rightOperand, LogicalOperator.Or);
        }

        private LogicalExpression(
            Expression<TKey> leftOperand,
            Expression<TKey> rightOperand,
            LogicalOperator @operator)
        {
            this.LeftOperand = leftOperand ?? throw new ArgumentNullException(nameof(leftOperand));
            this.RightOperand = rightOperand ?? throw new ArgumentNullException(nameof(rightOperand));
        }

        public Expression<TKey> LeftOperand { get; }
        public Expression<TKey> RightOperand { get; }
        public LogicalOperator Operator { get; }
    }
}
