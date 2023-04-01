using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbMoney(decimal Value)
        : IDbType<decimal>
        , IComparable<DbMoney>
        , IEquatable<DbMoney>
    {
        public const int Size = sizeof(decimal);

        int ISizeable.Size => Size;

        public const DbType Type = DbType.Money;

        DbType IDbType.Type => Type;

        public int CompareTo(DbMoney? other)
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

        public bool Equals(DbMoney? other)
        {
            return other is not null && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbMoney(decimal value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator decimal(DbMoney value)
        {
            return value.Value;
        }
    }
}
