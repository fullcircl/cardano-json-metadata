using System;
namespace CardanoJsonMetadata
{
    public interface ITxMetadataValue<T> : ITxMetadataValue
    {
        T ValueTyped { get; set; }
    }

    public interface ITxMetadataValue
    {
        object Value { get; set; }
    }
}
