using System;
using System.Text.Json;

namespace CardanoJsonMetadata
{
    public class TxMetaNumber : ITxMetadataValue<long>, IComparable<TxMetaNumber>, IEquatable<TxMetaNumber>
    {
        public TxMetaNumber(long value)
        {
            ValueTyped = value;
        }

        public long ValueTyped { get; set; }
        public object Value
        {
            get => ValueTyped;
            set
            {
                if (value.GetType() == typeof(long))
                {
                    ValueTyped = (long)value;
                    return;
                }

                throw new Exception("Value type must be 'long'");
            }
        }

        public TxDataType TxDataType => TxDataType.Int;

        public int CompareTo(object? obj)
        {
            // guard code
            if (obj == null)
            {
                return 1;
            }

            if (obj.GetType() != typeof(TxMetaNumber))
            {
                throw new ArgumentException("'obj' must be of type 'TxMetaNumber'");
            }

            return ValueTyped.CompareTo(((TxMetaNumber)obj).ValueTyped);
        }

        public int CompareTo(TxMetaNumber? other) => other == null ? 1 : ValueTyped.CompareTo(other.ValueTyped);

        public bool Equals(TxMetaNumber? other) => other != null && ValueTyped.Equals(other.ValueTyped);

        public override bool Equals(object? obj)
        {
            return Equals(obj as TxMetaNumber);
        }

        public override int GetHashCode()
        {
            return ValueTyped.GetHashCode();
        }

        public void Serialize(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteNumber(TxDataType.Serialize(), ValueTyped);
            writer.WriteEndObject();
        }

        public void ToJson(Utf8JsonWriter writer, string propertyName)
        {
            writer.WriteNumber(propertyName, ValueTyped);
        }

        public void ToJsonArray(Utf8JsonWriter writer)
        {
            writer.WriteNumberValue(ValueTyped);
        }

        public static bool operator ==(TxMetaNumber? first, TxMetaNumber? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return Equals(first, second);

            return first.Equals(second);
        }

        public static bool operator !=(TxMetaNumber? first, TxMetaNumber? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return !Equals(first, second);

            return !(first.Equals(second));
        }
    }
}
