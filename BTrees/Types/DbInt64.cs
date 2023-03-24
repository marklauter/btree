﻿using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbInt64(long Value)
        : IDbType<long>
        , IComparable<DbInt64>
        , IEquatable<DbInt64>
    {
        public int Size => sizeof(long);

        public DbType Type => DbType.Int64;

        public int CompareTo(DbInt64 other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<long>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbInt64 other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbInt64 left, DbInt64 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbInt64 left, DbInt64 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbInt64 left, DbInt64 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbInt64 left, DbInt64 right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbInt64(long value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(DbInt64 value)
        {
            return value.Value;
        }
    }
}