namespace BTrees.Types
{
    public readonly record struct DbInt32(int Value)
        : IDbType<int>
        , IEquatable<DbInt32>
    {
        public int Size => sizeof(int);

        public GiraffeDbType Type => GiraffeDbType.Int32;

        public int CompareTo(IDbType<int>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbInt32 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbInt32 left, DbInt32 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbInt32 left, DbInt32 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbInt32 left, DbInt32 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbInt32 left, DbInt32 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
