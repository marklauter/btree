using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbDouble(double Value)
        : IDbType<double>
        , IComparable<DbDouble>
        , IEquatable<DbDouble>
    {
        public int Size => sizeof(double);

        public DbType Type => DbType.Double;

        public int CompareTo(DbDouble other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<double>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbDouble other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
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
