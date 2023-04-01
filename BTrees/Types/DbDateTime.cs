using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbDateTime(DateTime Value)
        : IDbType<DateTime>
        , IComparable<DbDateTime>
        , IEquatable<DbDateTime>
    {
        public const int Size = sizeof(long);

        int ISizeable.ByteSize => Size;

        public const DbType Type = DbType.DateTime;

        DbType IDbType.Type => Type;

        public int CompareTo(DbDateTime? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<DateTime>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbDateTime? other)
        {
            return other is not null && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbDateTime(DateTime value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DateTime(DbDateTime value)
        {
            return value.Value;
        }
    }
}
