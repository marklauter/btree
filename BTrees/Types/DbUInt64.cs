using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbUInt64(ulong Value)
        : IDbType<ulong>
        , IComparable<DbUInt64>
        , IEquatable<DbUInt64>
    {
        public const int Size = sizeof(ulong);

        int IDbType.Size => Size;

        public const DbType Type = DbType.UInt64;

        DbType IDbType.Type => Type;

        public int CompareTo(IDbType<ulong>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbUInt64 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public int CompareTo(DbUInt64 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public static bool operator <(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbUInt64(ulong value)
        {
            return new DbUInt64(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong(DbUInt64 value)
        {
            return value.Value;
        }
    }
}
