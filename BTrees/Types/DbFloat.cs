using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbFloat(float Value)
        : IDbType<float>
        , IComparable<DbFloat>
        , IEquatable<DbFloat>
    {
        public const int Size = sizeof(float);

        int IDbType.Size => Size;

        public const DbType Type = DbType.Float;

        DbType IDbType.Type => Type;

        public int CompareTo(DbFloat other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<float>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbFloat other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value);
        }

        public static bool operator <(DbFloat left, DbFloat right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbFloat left, DbFloat right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbFloat left, DbFloat right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbFloat left, DbFloat right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbFloat(float value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(DbFloat value)
        {
            return value.Value;
        }
    }
}
