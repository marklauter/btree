namespace BTrees.Types
{
    public readonly record struct DbDateTime(DateTime Value)
        : IDbType<DateTime>
        , IEquatable<DbDateTime>
    {
        public int Size => sizeof(long);

        public GiraffeDbType Type => GiraffeDbType.DateTime;

        public int CompareTo(IDbType<DateTime>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbDateTime other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbDateTime left, DbDateTime right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbDateTime left, DbDateTime right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbDateTime left, DbDateTime right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbDateTime left, DbDateTime right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
