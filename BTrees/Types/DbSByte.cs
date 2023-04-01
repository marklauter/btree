using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbSByte(sbyte Value)
        : IDbType<sbyte>
        , IComparable<DbSByte>
        , IEquatable<DbSByte>
    {
        public const int Size = sizeof(sbyte);

        int ISizeable.ByteSize => Size;

        public const DbType Type = DbType.SByte;

        DbType IDbType.Type => Type;

        public int CompareTo(DbSByte? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<sbyte>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbSByte? other)
        {
            return other is not null && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbSByte left, DbSByte right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbSByte left, DbSByte right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbSByte left, DbSByte right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbSByte left, DbSByte right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbSByte(sbyte value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator sbyte(DbSByte value)
        {
            return value.Value;
        }
    }
}
