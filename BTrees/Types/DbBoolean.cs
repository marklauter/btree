namespace BTrees.Types
{
    public readonly record struct DbBoolean(bool Value)
        : IDbType<bool>
        , IEquatable<DbBoolean>
    {
        public int Size => sizeof(bool);

        public GiraffeDbType Type => GiraffeDbType.Boolean;

        public int CompareTo(IDbType<bool>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbBoolean other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbBoolean left, DbBoolean right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbBoolean left, DbBoolean right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbBoolean left, DbBoolean right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbBoolean left, DbBoolean right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
