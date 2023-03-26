using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbUInt16(ushort Value)
        : IDbType<ushort>
        , IComparable<DbUInt16>
        , IEquatable<DbUInt16>
    {
        public const int Size = sizeof(ushort);

        int IDbType.Size => Size;

        public const DbType Type = DbType.UInt16;

        DbType IDbType.Type => Type;

        public int CompareTo(DbUInt16 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<ushort>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbUInt16 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbUInt16 left, DbUInt16 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUInt16 left, DbUInt16 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUInt16 left, DbUInt16 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUInt16 left, DbUInt16 right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbUInt16(ushort value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ushort(DbUInt16 value)
        {
            return value.Value;
        }
    }
}
