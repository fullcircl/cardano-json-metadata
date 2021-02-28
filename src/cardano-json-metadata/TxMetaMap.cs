using System;
using System.Text.Json;

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

        public TxDataType TxDataType => TxDataType.Map;

        public bool Equals(TxMetaMap? other) => other != null && ValueTyped.Equals(other.ValueTyped);

        public override bool Equals(object? obj)
        {
            return Equals(obj as TxMetaMap);
        }

        public override int GetHashCode()
        {
            return ValueTyped.GetHashCode();
        }

        public void Serialize(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteStartArray(TxDataType.Serialize());

            foreach (var kvPair in ValueTyped)
            {
                writer.WriteStartObject("k");
                kvPair.K.Serialize(writer);
                writer.WriteEndObject();

                writer.WriteStartObject("v");
                kvPair.V.Serialize(writer);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public void ToJson(Utf8JsonWriter writer, string propertyName)
        {
            writer.WriteStartObject(propertyName);

            WriteInnerKVPairsToJson(writer);

            writer.WriteEndObject();
        }

        public void ToJsonArray(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();

            WriteInnerKVPairsToJson(writer);

            writer.WriteEndObject();
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

        private void WriteInnerKVPairsToJson(Utf8JsonWriter writer)
        {
            foreach (var kvPair in ValueTyped)
            {
                if (kvPair.K.TxDataType != TxDataType.String)
                {
                    throw new InvalidOperationException($"Data type not supported as property name: [{kvPair.K.TxDataType}], {kvPair.K.Value}");
                }

                if (kvPair.K.GetType() != typeof(TxMetaText))
                {
                    throw new InvalidOperationException($"Unexpected MetadataValue type: " + kvPair.K.GetType());
                }

                string innerPropertyName = ((TxMetaText)kvPair.K).ValueTyped;

                kvPair.V.ToJson(writer, innerPropertyName);
            }
        }
    }
}
