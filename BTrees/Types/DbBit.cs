namespace BTrees.Types
{
    public readonly record struct DbBit(bool Value)
        : IDbType<bool>
        , IEquatable<DbBit>
    {
        public int Size => sizeof(bool);

        public GiraffeDbType Type => GiraffeDbType.Bit;

        public int CompareTo(IDbType<bool>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbBit other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbBit left, DbBit right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbBit left, DbBit right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbBit left, DbBit right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbBit left, DbBit right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
