namespace CardanoJsonMetadata
{
    public struct TxMetaMapKVPair
    {
        public TxMetaMapKVPair(ITxMetadataValue k, ITxMetadataValue v)
        {
            K = k;
            V = v;
        }

        public ITxMetadataValue K { get; set; }
        public ITxMetadataValue V { get; set; }
    }
}
