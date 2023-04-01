using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbDouble(double Value)
        : IDbType<double>
        , IComparable<DbDouble>
        , IEquatable<DbDouble>
    {
        public const int Size = sizeof(double);

        int ISizeable.ByteSize => Size;

        public const DbType Type = DbType.Double;

        DbType IDbType.Type => Type;

        public int CompareTo(DbDouble? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<double>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbDouble? other)
        {
            return other is not null && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbDouble left, DbDouble right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbDouble left, DbDouble right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbDouble left, DbDouble right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbDouble left, DbDouble right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbDouble(double value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(DbDouble value)
        {
            return value.Value;
        }
    }
}
