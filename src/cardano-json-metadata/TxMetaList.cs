using System;
using System.Text.Json;

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

        public TxDataType TxDataType => TxDataType.List;

        public bool Equals(TxMetaList? other) => other != null && ValueTyped.Equals(other.ValueTyped);

        public override bool Equals(object? obj)
        {
            return Equals(obj as TxMetaList);
        }

        public override int GetHashCode()
        {
            return ValueTyped.GetHashCode();
        }

        public void Serialize(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteStartArray(TxDataType.Serialize());

            foreach (var innerValue in ValueTyped)
            {
                innerValue.Serialize(writer);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public void ToJson(Utf8JsonWriter writer, string propertyName)
        {
            writer.WriteStartArray(propertyName);

            WriteInnerValuesToJson(writer);

            writer.WriteEndArray();
        }

        public void ToJsonArray(Utf8JsonWriter writer)
        {
            writer.WriteStartArray();

            WriteInnerValuesToJson(writer);

            writer.WriteEndArray();
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

        private void WriteInnerValuesToJson(Utf8JsonWriter writer)
        {
            foreach (var valueItem in ValueTyped)
            {
                valueItem.ToJsonArray(writer);
            }
        }
    }
}
