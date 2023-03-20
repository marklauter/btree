namespace BTrees.Types
{
    public readonly record struct DbMoney(decimal Value)
        : IDbType<decimal>
        , IEquatable<DbMoney>
    {
        public int Size => sizeof(decimal);

        public GiraffeDbType Type => GiraffeDbType.Money;

        public int CompareTo(IDbType<decimal>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbMoney other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
