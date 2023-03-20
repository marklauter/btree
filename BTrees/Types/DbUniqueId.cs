namespace BTrees.Types
{
    public readonly record struct DbUniqueId(Guid Value)
        : IDbType<Guid>
        , IEquatable<DbUniqueId>
    {
        public int Size => 16;

        public GiraffeDbType Type => GiraffeDbType.UniqueId;

        public int CompareTo(IDbType<Guid>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbUniqueId other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbUniqueId left, DbUniqueId right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUniqueId left, DbUniqueId right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUniqueId left, DbUniqueId right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUniqueId left, DbUniqueId right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
