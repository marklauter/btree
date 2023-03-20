namespace BTrees.Types
{
    public readonly record struct DbUInt64(ulong Value)
        : IDbType<ulong>
        , IEquatable<DbUInt64>
    {
        public int Size => sizeof(ulong);

        public GiraffeDbType Type => GiraffeDbType.UInt64;

        public int CompareTo(IDbType<ulong>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbUInt64 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
