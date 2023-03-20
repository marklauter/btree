namespace BTrees.Types
{
    public readonly record struct DbUInt32(uint Value)
        : IDbType<uint>
        , IEquatable<DbUInt32>
    {
        public int Size => sizeof(uint);

        public GiraffeDbType Type => GiraffeDbType.UInt32;

        public int CompareTo(IDbType<uint>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbUInt32 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
