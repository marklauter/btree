using System.Runtime.InteropServices;

namespace BTrees.Types
{
    public readonly record struct DbBinary(byte[] Value)
        : IDbType<byte[]>
        , IEquatable<DbBinary>
    {
        public int Size => this.Value.Length;
        public GiraffeDbType Type => GiraffeDbType.Binary;

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
    }
}
