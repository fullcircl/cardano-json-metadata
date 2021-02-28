using System.Text.Json;

namespace CardanoJsonMetadata
{
    public interface ITxMetadataValue<T> : ITxMetadataValue
    {
        T ValueTyped { get; set; }
    }

    public interface ITxMetadataValue
    {
        TxDataType TxDataType { get; }
        object Value { get; set; }
        void Serialize(Utf8JsonWriter writer);
        void ToJson(Utf8JsonWriter writer, string propertyName);
        void ToJsonArray(Utf8JsonWriter writer);
    }
}
