using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbUInt32(uint Value)
        : IDbType<uint>
        , IComparable<DbUInt32>
        , IEquatable<DbUInt32>
    {
        public const int Size = sizeof(uint);

        int IDbType.Size => Size;

        public const DbType Type = DbType.UInt32;

        DbType IDbType.Type => Type;

        public int CompareTo(DbUInt32 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<uint>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbUInt32 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbUInt32(uint value)
        {
            return new DbUInt32(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(DbUInt32 value)
        {
            return value.Value;
        }
    }
}
