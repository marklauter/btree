using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbText
        : IDbType<string>
        , IComparable<DbText>
        , IEquatable<DbText>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbText(string value)
        {
            this.Value = value;
            this.Size = String.IsNullOrEmpty(value)
                ? 0
                : System.Text.Encoding.UTF8.GetByteCount(this.Value);
        }

        public string Value { get; }

        public int Size { get; }

        public DbType Type => DbType.Text;

        public int CompareTo(DbText other)
        {
            return String.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(IDbType<string>? other)
        {
            return other is null
                ? -1
                : String.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(DbText other)
        {
            return this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbText left, DbText right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbText left, DbText right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbText left, DbText right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbText left, DbText right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbText(string value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(DbText value)
        {
            return value.Value;
        }
    }
}
