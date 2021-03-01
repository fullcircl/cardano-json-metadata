using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CardanoJsonMetadata
{
    public class TxMetaBytes : ITxMetadataValue<byte[]>, IComparable<TxMetaBytes>, IEquatable<TxMetaBytes>
    {
        public TxMetaBytes(byte[] value)
        {
            // guard code
            if (value.Length > 64)
            {
                throw new Exception("Byte array length must be 64 or less");
            }

            _value = value;
        }

        public const string BytesPrefix = "0x";

        private byte[] _value;

        public byte[] ValueTyped
        {
            get => _value;
            set
            {
                // guard code
                if (value.Length > 64)
                {
                    throw new Exception("Byte array length must be 64 or less");
                }

                _value = value;
            }
        }
        public object Value
        {
            get => ValueTyped;
            set
            {
                // guard code
                if (value.GetType() != typeof(byte[]))
                {
                    throw new Exception("Value type must be 'byte[]'");
                }

                if (((byte[])value).Length > 64)
                {
                    throw new Exception("Byte array length must be 64 or less");
                }

                _value = (byte[])value;
            }
        }

        public TxDataType TxDataType => TxDataType.Bytes;

        public string GetJsonString() => BytesPrefix + Convert.ToHexString(ValueTyped);
        public string GetCborString() => Convert.ToHexString(ValueTyped);

        public int CompareTo(object? obj)
        {
            // guard code
            if (obj == null)
            {
                return 1;
            }

            if (obj.GetType() != typeof(TxMetaBytes))
            {
                throw new ArgumentException("'obj' must be of type 'TxMetaBytes'");
            }

            return GetCborString().CompareTo(((TxMetaBytes)obj).GetCborString());
        }

        public int CompareTo(TxMetaBytes? other) => other == null ? 1 : GetCborString().CompareTo(other.GetCborString());

        public bool Equals(TxMetaBytes? other) => other != null && ValueTyped.Equals(other.ValueTyped);

        public override bool Equals(object? obj)
        {
            return Equals(obj as TxMetaBytes);
        }

        public override int GetHashCode()
        {
            return ValueTyped.GetHashCode();
        }

        public void Serialize(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteString(TxDataType.Serialize(), GetCborString());
            writer.WriteEndObject();
        }

        public void ToJson(Utf8JsonWriter writer, string propertyName)
        {
            writer.WriteString(propertyName, GetJsonString());
        }

        public void ToJsonArray(Utf8JsonWriter writer)
        {
            writer.WriteStringValue(GetJsonString());
        }

        public static bool IsJsonByteString(string s)
        {
            return new Regex($"^{BytesPrefix}[0-9a-fA-F]*$").Match(s).Success;
        }

        public static bool operator ==(TxMetaBytes? first, TxMetaBytes? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return Equals(first, second);

            return first.Equals(second);
        }

        public static bool operator !=(TxMetaBytes? first, TxMetaBytes? second)
        {
            if (((object?)first) == null || ((object?)second) == null)
                return !Equals(first, second);

            return !(first.Equals(second));
        }
    }
}
