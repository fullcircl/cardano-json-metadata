using System;
namespace CardanoJsonMetadata
{
    public class TxMetaMap : ITxMetadataValue<TxMetaMapKVPair[]>, IEquatable<TxMetaMap>
    {
        public TxMetaMap(TxMetaMapKVPair[] value)
        {
            ValueTyped = value;
        }

        public TxMetaMapKVPair[] ValueTyped { get; set; }
        public object Value
        {
            get => ValueTyped;
            set
            {
                // guard code
                if (value.GetType() != typeof(TxMetaMapKVPair[]))
                {
                    throw new Exception("Value type must be 'TxMetaMapKVPair[]'");
                }

                ValueTyped = (TxMetaMapKVPair[])value;
            }
        }

        public bool Equals(TxMetaMap? other) => other != null && ValueTyped.Equals(other.ValueTyped);

        public override bool Equals(object? obj)
        {
            return Equals(obj as TxMetaMap);
        }

        public override int GetHashCode()
        {
            return ValueTyped.GetHashCode();
        }

        public static bool operator ==(TxMetaMap? first, TxMetaMap? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return Equals(first, second);

            return first.Equals(second);
        }

        public static bool operator !=(TxMetaMap? first, TxMetaMap? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return !Equals(first, second);

            return !(first.Equals(second));
        }
    }
}
