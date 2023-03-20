using System.Runtime.InteropServices;

namespace BTrees.Types
{
    public enum GiraffeDbType
    {
#pragma warning disable CA1720 // Identifier contains type name
        Undefined = 0,
        Boolean = 1,
        Int32 = 2,
        Int64 = 3,
        UInt32 = 4,
        UInt64 = 5,
        Float = 6,
        Double = 7,
        Money = 8,
        DateTime = 9,
        Geography = 10,
        Geometry = 11,
        Binary = 12,
        Text = 13,
        TimeSpan = 14,
        UniqueId = 15,
        Json = 16,
        Xml = 17,
#pragma warning restore CA1720 // Identifier contains type name
    }

    public interface IDbType<T>
        : IComparable<IDbType<T>>
    {
        GiraffeDbType Type { get; }
        int Size { get; }
        T Value { get; }
    }

    public readonly struct DbInt32
        : IDbType<int>, IEquatable<DbInt32>
    {
        public int Size => sizeof(int);
        public int Value { get; }
        public GiraffeDbType Type => GiraffeDbType.Int32;

        public DbInt32(int value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<int>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbInt32 @int && this.Equals(@int);
        }

        public bool Equals(DbInt32 other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbInt32 left, DbInt32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbInt32 left, DbInt32 right)
        {
            return !(left == right);
        }

        public static bool operator <(DbInt32 left, DbInt32 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbInt32 left, DbInt32 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbInt32 left, DbInt32 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbInt32 left, DbInt32 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    public readonly struct DbInt64
        : IDbType<long>, IEquatable<DbInt64>
    {
        public int Size => sizeof(long);
        public long Value { get; }
        public GiraffeDbType Type => GiraffeDbType.Int64;

        public DbInt64(long value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<long>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbInt64 @int && this.Equals(@int);
        }

        public bool Equals(DbInt64 other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbInt64 left, DbInt64 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbInt64 left, DbInt64 right)
        {
            return !(left == right);
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
    }

    public readonly struct DbUniqueId
        : IDbType<Guid>, IEquatable<DbUniqueId>
    {
        public int Size => 16;
        public Guid Value { get; }
        public GiraffeDbType Type => GiraffeDbType.UniqueId;

        public DbUniqueId(Guid value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<Guid>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbUniqueId id && this.Equals(id);
        }

        public bool Equals(DbUniqueId other)
        {
            return this.Value.Equals(other.Value) &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbUniqueId left, DbUniqueId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbUniqueId left, DbUniqueId right)
        {
            return !(left == right);
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
    }

    public readonly struct DbBoolean
        : IDbType<bool>, IEquatable<DbBoolean>
    {
        public int Size => sizeof(bool);
        public bool Value { get; }
        public GiraffeDbType Type => GiraffeDbType.Boolean;

        public DbBoolean(bool value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<bool>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbBoolean boolean && this.Equals(boolean);
        }

        public bool Equals(DbBoolean other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbBoolean left, DbBoolean right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbBoolean left, DbBoolean right)
        {
            return !(left == right);
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
    }

    public readonly struct DbUInt32
        : IDbType<uint>, IEquatable<DbUInt32>
    {
        public int Size => sizeof(uint);
        public uint Value { get; }
        public GiraffeDbType Type => GiraffeDbType.UInt32;

        public DbUInt32(uint value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<uint>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbUInt32 @int && this.Equals(@int);
        }

        public bool Equals(DbUInt32 other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbUInt32 left, DbUInt32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbUInt32 left, DbUInt32 right)
        {
            return !(left == right);
        }

        public static bool operator <(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUInt32 left, DbUInt32 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    public readonly struct DbUInt64
        : IDbType<ulong>, IEquatable<DbUInt64>
    {
        public int Size => sizeof(ulong);
        public ulong Value { get; }
        public GiraffeDbType Type => GiraffeDbType.UInt64;

        public DbUInt64(ulong value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<ulong>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbUInt64 @int && this.Equals(@int);
        }

        public bool Equals(DbUInt64 other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbUInt64 left, DbUInt64 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbUInt64 left, DbUInt64 right)
        {
            return !(left == right);
        }

        public static bool operator <(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbUInt64 left, DbUInt64 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    public readonly struct DbFloat
        : IDbType<float>, IEquatable<DbFloat>
    {
        public int Size => sizeof(float);
        public float Value { get; }
        public GiraffeDbType Type => GiraffeDbType.Float;

        public DbFloat(float value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<float>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbFloat @float && this.Equals(@float);
        }

        public bool Equals(DbFloat other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbFloat left, DbFloat right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbFloat left, DbFloat right)
        {
            return !(left == right);
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
    }

    public readonly struct DbDouble
        : IDbType<double>, IEquatable<DbDouble>
    {
        public int Size => sizeof(double);
        public double Value { get; }
        public GiraffeDbType Type => GiraffeDbType.Double;

        public DbDouble(double value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<double>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbDouble @double && this.Equals(@double);
        }

        public bool Equals(DbDouble other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbDouble left, DbDouble right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbDouble left, DbDouble right)
        {
            return !(left == right);
        }

        public static bool operator <(DbDouble left, DbDouble right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbDouble left, DbDouble right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbDouble left, DbDouble right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbDouble left, DbDouble right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    public readonly struct DbDateTime
        : IDbType<DateTime>, IEquatable<DbDateTime>
    {
        public int Size => sizeof(long);
        public DateTime Value { get; }
        public GiraffeDbType Type => GiraffeDbType.DateTime;

        public DbDateTime(DateTime value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<DateTime>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbDateTime time && this.Equals(time);
        }

        public bool Equals(DbDateTime other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbDateTime left, DbDateTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbDateTime left, DbDateTime right)
        {
            return !(left == right);
        }

        public static bool operator <(DbDateTime left, DbDateTime right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbDateTime left, DbDateTime right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbDateTime left, DbDateTime right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbDateTime left, DbDateTime right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    public readonly struct DbMoney
        : IDbType<decimal>, IEquatable<DbMoney>
    {
        public int Size => sizeof(decimal);
        public decimal Value { get; }
        public GiraffeDbType Type => GiraffeDbType.Money;

        public DbMoney(decimal value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<decimal>? other)
        {
            return other is null
                ? -1
                : this.Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbMoney money && this.Equals(money);
        }

        public bool Equals(DbMoney other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbMoney left, DbMoney right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbMoney left, DbMoney right)
        {
            return !(left == right);
        }

        public static bool operator <(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DbMoney left, DbMoney right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    public readonly struct DbText
        : IDbType<string>, IEquatable<DbText>
    {
        public int Size => this.Value.Length;
        public string Value { get; }
        public GiraffeDbType Type => GiraffeDbType.Text;

        public DbText(string value)
        {
            this.Value = value;
        }

        public int CompareTo(IDbType<string>? other)
        {
            return other is null
                ? -1
                : String.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return obj is DbText text && this.Equals(text);
        }

        public bool Equals(DbText other)
        {
            return this.Value == other.Value &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type);
        }

        public static bool operator ==(DbText left, DbText right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbText left, DbText right)
        {
            return !(left == right);
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
    }

    public readonly struct DbBinary
        : IDbType<byte[]>, IEquatable<DbBinary>
    {
        public int Size => this.Value.Length;
        public byte[] Value { get; }
        public GiraffeDbType Type => GiraffeDbType.Binary;

        public DbBinary(byte[] value)
        {
            this.Value = value;
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

        public override bool Equals(object? obj)
        {
            return obj is DbBinary binary && this.Equals(binary);
        }

        public bool Equals(DbBinary other)
        {
            return EqualityComparer<byte[]>.Default.Equals(this.Value, other.Value) &&
                   this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.Type, this.Size);
        }

        public static bool operator ==(DbBinary left, DbBinary right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DbBinary left, DbBinary right)
        {
            return !(left == right);
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
