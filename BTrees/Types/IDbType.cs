namespace BTrees.Types
{
    public interface IDbType
        : ISizeable
    {
        DbType Type { get; }
    }

    public interface IDbType<T>
        : IDbType
        , IComparable<IDbType<T>>
    {
        T Value { get; }
    }
}
