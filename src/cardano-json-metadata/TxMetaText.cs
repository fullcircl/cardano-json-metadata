using System;
using System.Text;
using System.Text.Json;

namespace CardanoJsonMetadata
{
    public class TxMetaText : ITxMetadataValue<string>, IComparable<TxMetaText>, IEquatable<TxMetaText>
    {
        public TxMetaText(string value)
        {
            // guard code
            EnforceValueValidation(value);

            _value = value;
        }

        private string _value;

        public string ValueTyped
        {
            get => _value;
            set
            {
                // guard code
                EnforceValueValidation(value);

                _value = value;
            }
        }
        public object Value
        {
            get => ValueTyped;
            set
            {
                // guard code
                if (value.GetType() != typeof(string))
                {
                    throw new Exception("Value type must be 'string'");
                }

                EnforceValueValidation((string)value);

                _value = (string)value;
            }
        }

        public TxDataType TxDataType => TxDataType.String;

        public int CompareTo(object? obj)
        {
            // guard code
            if (obj == null)
            {
                return 1;
            }

            if (obj.GetType() != typeof(TxMetaText))
            {
                throw new ArgumentException("'obj' must be of type 'TxMetaText'");
            }

            return ValueTyped.CompareTo(((TxMetaText)obj).ValueTyped);
        }

        public int CompareTo(TxMetaText? other) => other == null ? 1 : ValueTyped.CompareTo(other.ValueTyped);

        public bool Equals(TxMetaText? other) => other != null && ValueTyped.Equals(other.ValueTyped);

        public override bool Equals(object? obj)
        {
            return Equals(obj as TxMetaText);
        }

        public override int GetHashCode()
        {
            return ValueTyped.GetHashCode();
        }

        public void Serialize(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteString(TxDataType.Serialize(), ValueTyped);
            writer.WriteEndObject();
        }

        public void ToJson(Utf8JsonWriter writer, string propertyName)
        {
            writer.WriteString(propertyName, ValueTyped);
        }

        public void ToJsonArray(Utf8JsonWriter writer)
        {
            writer.WriteStringValue(ValueTyped);
        }

        public static bool operator ==(TxMetaText? first, TxMetaText? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return Equals(first, second);

            return first.Equals(second);
        }

        public static bool operator !=(TxMetaText? first, TxMetaText? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return !Equals(first, second);

            return !(first.Equals(second));
        }

        private void EnforceValueValidation(string value)
        {
            // Strings must be at most 64 bytes when UTF-8 encoded.
            if (Encoding.UTF8.GetByteCount((string)value) > 64)
            {
                throw new Exception("String must be at most 64 bytes when UTF-8 encoded:" + Environment.NewLine + value);
            }
        }
    }
}
