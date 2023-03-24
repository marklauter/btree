using BTrees.Types;
using System.Collections.Immutable;

namespace BTrees.Tests
{
    internal static class UniqueIdFactory
    {
        public static ImmutableArray<DbUniqueId> Generate(int length)
        {
            var ids = new DbUniqueId[length];
            for (var i = 0; i < length; ++i)
            {
                ids[i] = DbUniqueId.NewId();
            }

            return ids.ToImmutableArray();
        }
    }
}

