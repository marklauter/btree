namespace BTrees.Types
{
    public interface IDbType
    {
        DbType Type { get; }
        int Size { get; }
    }

    public interface IDbType<T>
        : IDbType
        , IComparable<IDbType<T>>
    {
        T Value { get; }
    }
}
