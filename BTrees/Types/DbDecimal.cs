using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbDecimal(decimal Value)
        : IDbType<decimal>
        , IComparable<DbDecimal>
        , IEquatable<DbDecimal>
    {
        public int Size => sizeof(decimal);

        public DbType Type => DbType.Decimal;

        public int CompareTo(DbDecimal other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<decimal>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbDecimal other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbDecimal left, DbDecimal right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbDecimal left, DbDecimal right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbDecimal left, DbDecimal right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbDecimal left, DbDecimal right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator DbDecimal(decimal value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator decimal(DbDecimal value)
        {
            return value.Value;
        }
    }
}
