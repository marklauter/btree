namespace BTrees.Types
{
    public readonly record struct DbText(string Value)
        : IDbType<string>
        , IEquatable<DbText>
    {
        public int Size => this.Value.Length;

        public GiraffeDbType Type => GiraffeDbType.Text;

        public int CompareTo(IDbType<string>? other)
        {
            return other is null
                ? -1
                : String.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(DbText other)
        {
            return this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbText left, DbText right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbText left, DbText right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbText left, DbText right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbText left, DbText right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
