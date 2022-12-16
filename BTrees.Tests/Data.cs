using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace BTrees.Tests
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public readonly struct Data
        : IEquatable<Data>
    {
        public readonly int Size;
        public readonly int Offset;

        public Data(int size, int offset)
        {
            this.Size = size;
            this.Offset = offset;
        }

        public bool Equals(Data other)
        {
            return this.Size == other.Size && this.Offset == other.Offset;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Data left, Data right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Data left, Data right)
        {
            return !(left == right);
        }
    }
}
