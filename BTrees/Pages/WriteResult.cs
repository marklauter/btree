namespace BTrees.Pages
{
    internal enum WriteResult
    {
        Undefined = 0,
        Inserted = 1,
        Updated = 2,
        FailedToAquireLock = 3,
    }
}
