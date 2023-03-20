namespace BTrees.Types
{
    public readonly record struct DbInt64(long Value)
        : IDbType<long>
        , IEquatable<DbInt64>
    {
        public int Size => sizeof(long);

        public GiraffeDbType Type => GiraffeDbType.Int64;

        public int CompareTo(IDbType<long>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbInt64 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbInt64 left, DbInt64 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbInt64 left, DbInt64 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbInt64 left, DbInt64 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbInt64 left, DbInt64 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
