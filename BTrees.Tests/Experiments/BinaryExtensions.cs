using System.Runtime.InteropServices;

namespace BTrees.Tests.Experiments
{
    public static class BinaryExtensions
    {
        public static void WriteStruct<T>(this BinaryWriter writer, T theStruct) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var bytes = new byte[size];
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(theStruct, handle.AddrOfPinnedObject(), false);
            writer.Write(bytes);
            handle.Free();
        }

        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            var bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var address = handle.AddrOfPinnedObject();
            var ptr = Marshal.PtrToStructure(address, typeof(T));
            if (ptr == null)
            {
                throw new InvalidDataException($"Could not read struct of type {typeof(T).Name} from stream.");
            }

            var value = (T)ptr;
            handle.Free();
            return value;
        }
    }
}
