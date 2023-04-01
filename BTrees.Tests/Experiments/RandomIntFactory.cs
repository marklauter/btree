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
                _ = hashset.Add(random.Next(0, length));
            }

            return hashset.ToArray();
        }
    }
}
