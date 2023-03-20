namespace BTrees.Types
{
    public readonly record struct DbInt16(short Value)
        : IDbType<short>
        , IEquatable<DbInt16>
    {
        public int Size => sizeof(short);

        public GiraffeDbType Type => GiraffeDbType.Int16;

        public int CompareTo(IDbType<short>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbInt16 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbInt16 left, DbInt16 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbInt16 left, DbInt16 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbInt16 left, DbInt16 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbInt16 left, DbInt16 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
