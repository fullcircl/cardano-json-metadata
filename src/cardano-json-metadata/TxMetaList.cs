using System;
namespace CardanoJsonMetadata
{
    public class TxMetaList : ITxMetadataValue<ITxMetadataValue[]>, IEquatable<TxMetaList>
    {
        public TxMetaList(ITxMetadataValue[] value)
        {
            ValueTyped = value;
        }

        public ITxMetadataValue[] ValueTyped { get; set; }
        public object Value
        {
            get => ValueTyped;
            set
            {
                // guard code
                if (value.GetType() != typeof(ITxMetadataValue[]))
                {
                    throw new Exception("Value type must be 'ITxMetadataValue[]'");
                }

                ValueTyped = (ITxMetadataValue[])value;
            }
        }

        public bool Equals(TxMetaList? other) => other != null && ValueTyped.Equals(other.ValueTyped);

        public override bool Equals(object? obj)
        {
            return Equals(obj as TxMetaList);
        }

        public override int GetHashCode()
        {
            return ValueTyped.GetHashCode();
        }

        public static bool operator ==(TxMetaList? first, TxMetaList? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return Equals(first, second);

            return first.Equals(second);
        }

        public static bool operator !=(TxMetaList? first, TxMetaList? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return !Equals(first, second);

            return !(first.Equals(second));
        }
    }
}
