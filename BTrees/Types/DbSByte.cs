namespace BTrees.Types
{
    public readonly record struct DbSByte(sbyte Value)
        : IDbType<sbyte>
        , IEquatable<DbSByte>
    {
        public int Size => sizeof(sbyte);

        public GiraffeDbType Type => GiraffeDbType.SByte;

        public int CompareTo(IDbType<sbyte>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbSByte other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbSByte left, DbSByte right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbSByte left, DbSByte right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbSByte left, DbSByte right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbSByte left, DbSByte right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
