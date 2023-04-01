using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbByte(byte Value)
        : IDbType<byte>
        , IComparable<DbByte>
        , IEquatable<DbByte>
    {
        public const int Size = sizeof(byte);

        int ISizeable.ByteSize => Size;

        public const DbType Type = DbType.Byte;

        DbType IDbType.Type => Type;

        public int CompareTo(DbByte? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<byte>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbByte? other)
        {
            return other is not null && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbByte left, DbByte right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbByte left, DbByte right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbByte left, DbByte right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbByte left, DbByte right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbByte(byte value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte(DbByte value)
        {
            return value.Value;
        }
    }
}
