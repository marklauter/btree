using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public sealed record DbText
        : IDbType<string>
        , IComparable<DbText>
        , IEquatable<DbText>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbText(string value)
        {
            this.Value = value;
            this.size = String.IsNullOrEmpty(value)
                ? Size
                : Size + System.Text.Encoding.UTF8.GetByteCount(this.Value);
        }

        public string Value { get; }

        public const int Size = sizeof(int);

        private readonly int size;
        int ISizeable.ByteSize => this.size;

        public const DbType Type = DbType.Text;

        DbType IDbType.Type => Type;

        public int CompareTo(DbText? other)
        {
            return other is null
                ? -1
                : String.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(IDbType<string>? other)
        {
            return other is null
                ? -1
                : String.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(DbText? other)
        {
            return other is not null && this.Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, this.Value, ((IDbType)this).ByteSize);
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
