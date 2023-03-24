﻿using System.Runtime.CompilerServices;

namespace BTrees.Types
{
    public readonly record struct DbByte(byte Value)
        : IDbType<byte>
        , IComparable<DbByte>
        , IEquatable<DbByte>
    {
        public int Size => sizeof(byte);

        public DbType Type => DbType.Byte;

        public int CompareTo(DbByte other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(IDbType<byte>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public bool Equals(DbByte other)
        {
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator <(DbByte left, DbByte right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbByte left, DbByte right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbByte left, DbByte right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbByte left, DbByte right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DbByte(byte value)
        {
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte(DbByte value)
        {
            return value.Value;
        }
    }
}