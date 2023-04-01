using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbInt16(short Value)
        : IDbType<short>
        , IComparable<DbInt16>
        , IEquatable<DbInt16>
    {
        public const int Size = sizeof(short);

        int ISizeable.Size => Size;

        public const DbType Type = DbType.Int16;

        DbType IDbType.Type => Type;

        public int CompareTo(DbInt16? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<short>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbInt16? other)
        {
            return other is not null && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbInt16 left, DbInt16 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbInt16 left, DbInt16 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbInt16 left, DbInt16 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbInt16 left, DbInt16 right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbInt16(short value)
        {
            return new DbInt16(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator short(DbInt16 value)
        {
            return value.Value;
        }
    }
}
