namespace BTrees.Expressions
{
    public class Takexpression<TKey>
        : Expression<TKey>
        where TKey : IComparable<TKey>
    {
        public Takexpression(int take)
        {
            if (take < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(skip));
            }

            this.Take = take;
        }

        public int Take { get; }
    }
}
