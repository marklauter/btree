namespace BTrees.Types
{
    public readonly record struct DbUInt16(ushort Value)
        : IDbType<ushort>
        , IEquatable<DbUInt16>
    {
        public int Size => sizeof(ushort);

        public GiraffeDbType Type => GiraffeDbType.UInt16;

        public int CompareTo(IDbType<ushort>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbUInt16 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbUInt16 left, DbUInt16 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUInt16 left, DbUInt16 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUInt16 left, DbUInt16 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUInt16 left, DbUInt16 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
