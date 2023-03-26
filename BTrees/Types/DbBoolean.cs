using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbBoolean(bool Value)
        : IDbType<bool>
        , IComparable<DbBoolean>
        , IEquatable<DbBoolean>
    {
        public const int Size = sizeof(bool);

        int IDbType.Size => Size;

        public const DbType Type = DbType.Boolean;

        DbType IDbType.Type => Type;

        public int CompareTo(DbBoolean other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<bool>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbBoolean other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbBoolean left, DbBoolean right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbBoolean left, DbBoolean right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbBoolean left, DbBoolean right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbBoolean left, DbBoolean right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(DbBoolean value)
        {
            return value.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbBoolean(bool value)
        {
            return new DbBoolean(value);
        }
    }
}
