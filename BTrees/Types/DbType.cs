namespace BTrees.Types
{
    public enum DbType
    {
#pragma warning disable CA1720 // Identifier contains type name
        Undefined = 0,

        #region logical
        Boolean = 1,
        Bit = 2,
        #endregion

        #region numeric - integer
        SByte = 3,
        Byte = 4,
        Int16 = 5,
        UInt16 = 6,
        Int32 = 7,
        UInt32 = 8,
        Int64 = 9,
        UInt64 = 10,
        UniqueId = 11,
        #endregion

        #region numeric - float
        Float = 12,
        Double = 13,
        Decimal = 14,
        Money = 15,
        #endregion

        #region time
        DateTime = 16,
        TimeSpan = 17,
        Date = 18, // not implemented
        Time = 19, // not implemented
        #endregion

        #region strings and spans
        Binary = 20,
        Text = 21,
        Json = 22, // not implemented
        Xml = 23, // not implemented
        #endregion

        #region space
        Point2D = 24, // not implemented
        Point3D = 25, // not implemented
        Geography = 26, // not implemented
        Geometry = 27, // not implemented
        #endregion

#pragma warning restore CA1720 // Identifier contains type name
    }
}
