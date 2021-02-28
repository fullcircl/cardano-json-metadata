namespace CardanoJsonMetadata
{
    public enum TxDataType
    {
        Int,
        Bytes,
        String,
        List,
        Map,
    }

    public static class TxDataTypeExtensions
    {
        public static string Serialize(this TxDataType txDataType)
        {
            return txDataType.ToString().ToLowerInvariant();
        }
    }
}
