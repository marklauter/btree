using System.Runtime.InteropServices;

namespace BTrees.Tests.Experiments
{
    public class StreamTests
    {
        [Fact]
        public void Test()
        {
            //using var file = new FileStream("", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096 * 2, FileOptions.SequentialScan);
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true);
            using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true);

            var expected = new Data(1, 2);
            writer.WriteStruct(expected);

            stream.Position = 0;
            var actual = reader.ReadStruct<Data>();

            Assert.Equal(expected, actual);

            var h1 = GCHandle.Alloc(expected, GCHandleType.Pinned);
            var a1 = h1.AddrOfPinnedObject();

            var h2 = GCHandle.Alloc(actual, GCHandleType.Pinned);
            var a2 = h2.AddrOfPinnedObject();

            Assert.NotEqual(a1, a2);
        }
    }
}
