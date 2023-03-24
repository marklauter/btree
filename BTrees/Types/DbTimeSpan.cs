using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbTimeSpan(TimeSpan Value)
        : IDbType<TimeSpan>
        , IComparable<DbTimeSpan>
        , IEquatable<DbTimeSpan>
    {
        // timespan is stored as ticks
        public int Size => sizeof(long);

        public DbType Type => DbType.TimeSpan;

        public int CompareTo(DbTimeSpan other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<TimeSpan>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbTimeSpan other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbTimeSpan left, DbTimeSpan right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbTimeSpan left, DbTimeSpan right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbTimeSpan left, DbTimeSpan right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbTimeSpan left, DbTimeSpan right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbTimeSpan(TimeSpan value)
        {
            return new DbTimeSpan(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator TimeSpan(DbTimeSpan value)
        {
            return value.Value;
        }
    }
}
