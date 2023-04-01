using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbTimeSpan(TimeSpan Value)
        : IDbType<TimeSpan>
        , IComparable<DbTimeSpan>
        , IEquatable<DbTimeSpan>
    {
        // timespan is stored as ticks
        public const int Size = sizeof(long);

        int ISizeable.ByteSize => Size;

        public const DbType Type = DbType.TimeSpan;

        DbType IDbType.Type => Type;

        public int CompareTo(DbTimeSpan? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<TimeSpan>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbTimeSpan? other)
        {
            return other is not null && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
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
