using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BTrees.Types
{
    public readonly record struct DbBinary(byte[] Value)
        : IDbType<byte[]>
        , IComparable<DbBinary>
        , IEquatable<DbBinary>
    {
        public int Size => this.Value.Length;

        public DbType Type => DbType.Binary;

        public int CompareTo(DbBinary other)
        {
            return this.CompareTo((IDbType<byte[]>)other);
        }

        public int CompareTo(IDbType<byte[]>? other)
        {
            if (other is null)
            {
                return -1;
            }

            var left = MemoryMarshal.Cast<byte, byte>(this.Value);
            var right = MemoryMarshal.Cast<byte, byte>(other.Value);
            return left.SequenceCompareTo(right);
        }

        public bool Equals(DbBinary other)
        {
            return this.CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type, this.Size);
        }

        public static bool operator <(DbBinary left, DbBinary right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbBinary left, DbBinary right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbBinary left, DbBinary right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbBinary left, DbBinary right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator byte[](DbBinary value)
        {
            return value.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator DbBinary(byte[] value)
        {
            return new DbBinary(value);
        }
    }
}
