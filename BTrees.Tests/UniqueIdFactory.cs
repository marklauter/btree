using BTrees.Types;

namespace BTrees.Tests
{
    internal static class UniqueIdFactory
    {
        public static DbUniqueId[] Generate(int length)
        {
            var ids = new DbUniqueId[length];
            for (var i = 0; i < length; ++i)
            {
                ids[i] = DbUniqueId.NewId();
            }

            return ids;
        }
    }
}

