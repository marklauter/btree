namespace BTrees.Tests.Experiments
{
    internal static class RandomIntFactory
    {
        public static int[] Generate(int length)
        {
            var random = new Random(length);
            var hashset = new HashSet<int>();
            while (hashset.Count < length)
            {
#pragma warning disable IDE0058 // Expression value is never used
                hashset.Add(random.Next(0, length));
#pragma warning restore IDE0058 // Expression value is never used
            }

            var i = 0;
            var ids = new int[length];
            foreach (var id in hashset)
            {
                ids[i++] = id;
            }

            return ids;
        }
    }
}
