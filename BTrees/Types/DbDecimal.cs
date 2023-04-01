using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbDecimal(decimal Value)
        : IDbType<decimal>
        , IComparable<DbDecimal>
        , IEquatable<DbDecimal>
    {
        public const int Size = sizeof(decimal);

        int ISizeable.Size => Size;

        public const DbType Type = DbType.Decimal;

        DbType IDbType.Type => Type;

        public int CompareTo(DbDecimal? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<decimal>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbDecimal? other)
        {
            return other is not null && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
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
        public static implicit operator DbDecimal(decimal value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator decimal(DbDecimal value)
        {
            return value.Value;
        }
    }
}
