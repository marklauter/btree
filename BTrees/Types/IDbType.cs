namespace BTrees.Types
{
    public interface IDbType<T>
        : IComparable<IDbType<T>>
    {
        GiraffeDbType Type { get; }
        int Size { get; }
        T Value { get; }
    }
}
