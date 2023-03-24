using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbFloat(float Value)
        : IDbType<float>
        , IComparable<DbFloat>
        , IEquatable<DbFloat>
    {
        public int Size => sizeof(float);

        public DbType Type => DbType.Float;

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
            return HashCode.Combine(this.Value, this.Type);
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
        public static explicit operator DbFloat(float value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(DbFloat value)
        {
            return value.Value;
        }
    }
}
