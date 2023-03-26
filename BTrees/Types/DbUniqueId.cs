using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbUniqueId(Guid Value)
        : IDbType<Guid>
        , IComparable<DbUniqueId>
        , IEquatable<DbUniqueId>
    {
        public const int Size = 16;

        int IDbType.Size => Size;

        public const DbType Type = DbType.UniqueId;

        DbType IDbType.Type => Type;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbUniqueId NewId()
        {
            return new DbUniqueId(Guid.NewGuid());
        }

        public int CompareTo(DbUniqueId other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<Guid>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbUniqueId other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbUniqueId left, DbUniqueId right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUniqueId left, DbUniqueId right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUniqueId left, DbUniqueId right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUniqueId left, DbUniqueId right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Guid(DbUniqueId value)
        {
            return value.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbUniqueId(Guid value)
        {
            return new DbUniqueId(value);
        }
    }
}
