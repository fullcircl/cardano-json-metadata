using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CardanoJsonMetadata
{
    public class TxMetadata : IDictionary<long, ITxMetadataValue>
    {
        private readonly IDictionary<long, ITxMetadataValue> _internalDict = new Dictionary<long, ITxMetadataValue>();

        public ITxMetadataValue this[long key] { get => _internalDict[key]; set => _internalDict[key] = value; }

        public ICollection<long> Keys => _internalDict.Keys;

        public ICollection<ITxMetadataValue> Values => _internalDict.Values;

        public int Count => _internalDict.Count;

        public bool IsReadOnly => _internalDict.IsReadOnly;

        public void Add(long key, ITxMetadataValue value)
        {
            //guard code
            if (key < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(key), $"[{nameof(key)}] must be non-negative.");
            }

            _internalDict.Add(key, value);
        }

        public void Add(KeyValuePair<long, ITxMetadataValue> item)
        {
            //guard code
            if (item.Key < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(item.Key), $"[{nameof(item.Key)}] must be non-negative.");
            }

            _internalDict.Add(item.Key, item.Value);
        }

        public void Clear() => _internalDict.Clear();

        public bool Contains(KeyValuePair<long, ITxMetadataValue> item) => _internalDict.Contains(item);

        public bool ContainsKey(long key) => _internalDict.ContainsKey(key);

        public void CopyTo(KeyValuePair<long, ITxMetadataValue>[] array, int arrayIndex) => _internalDict.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<long, ITxMetadataValue>> GetEnumerator() => _internalDict.GetEnumerator();

        public bool Remove(long key) => _internalDict.Remove(key);

        public bool Remove(KeyValuePair<long, ITxMetadataValue> item) => _internalDict.Remove(item);

        public bool TryGetValue(long key, [MaybeNullWhen(false)] out ITxMetadataValue value) => _internalDict.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_internalDict).GetEnumerator();

        public string ToJson()
        {
            if (Keys.Count == 0)
            {
                return "{}";
            }

            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);

            if (Keys.Count == 1)
            {
                Values.First().ToJsonArray(writer);
            }
            else
            {
                writer.WriteStartObject();

                foreach (var kvp in _internalDict)
                {
                    kvp.Value.ToJson(writer, kvp.Key.ToString(CultureInfo.InvariantCulture));
                }

                writer.WriteEndObject();
            }

            writer.Flush();

            ms.Position = 0;
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public string Serialize()
        {
            if (Keys.Count == 0)
            {
                return "{}";
            }

            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);

            writer.WriteStartObject();

            foreach (var key in Keys)
            {
                writer.WriteStartObject(key.ToString(CultureInfo.InvariantCulture));
                this[key].Serialize(writer);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
            writer.Flush();

            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static TxMetadata Deserialize(string json)
        {
            var doc = JsonDocument.Parse(json);
            var metadata = new TxMetadata();

            foreach (var childElement in doc.RootElement.EnumerateObject())
            {
                metadata.Add(long.Parse(childElement.Name), ParseSchemaElement(childElement.Value));
            }

            return metadata;
        }

        public static TxMetadata FromJson(string json)
        {
            var doc = JsonDocument.Parse(json);
            var rootMetadataValue = ParseNonSchemaElement(doc.RootElement);

            var metadata = new TxMetadata();
            metadata.Add(0, rootMetadataValue);

            return metadata;
        }

        public static ITxMetadataValue ParseNonSchemaElement(JsonElement element)
        {
            ITxMetadataValue result;

            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (!element.TryGetInt64(out _))
                    {
                        throw new InvalidOperationException("TxMetadata does not support non-integer numbers: " + element.GetRawText());
                    }

                    result = new TxMetaNumber(element.GetInt64());
                    break;
                case JsonValueKind.String:
                    string elementString = element.GetString() ?? "";
                    if (TxMetaBytes.IsJsonByteString(elementString))
                    {
                        //remove leading "0x"
                        elementString = new Regex($"^{TxMetaBytes.BytesPrefix}").Replace(elementString, "");
                        result = new TxMetaBytes(Encoding.Unicode.GetBytes(elementString));
                    }
                    else
                    {
                        result = new TxMetaText(elementString);
                    }
                    break;
                case JsonValueKind.Array:
                    var innerValues = element.EnumerateArray()
                        .Select(e => ParseNonSchemaElement(e))
                        .ToArray();

                    result = new TxMetaList(innerValues);
                    break;
                case JsonValueKind.Object:
                    var objectMap = element.EnumerateObject()
                        .Select(e => new TxMetaMapKVPair(new TxMetaText(e.Name), ParseNonSchemaElement(e.Value)))
                        .ToArray();

                    result = new TxMetaMap(objectMap);
                    break;
                default:
                    throw new InvalidOperationException("JSON type not supported: " + element.ValueKind);
            }

            return result;
        }

        public static ITxMetadataValue ParseSchemaElement(JsonElement element)
        {
            ITxMetadataValue result;

            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException("All schema elements must be objects");
            }

            JsonProperty prop = element.EnumerateObject().Single();
            var dataType = Enum.Parse<TxDataType>(prop.Name, ignoreCase: true);

            switch (dataType)
            {
                case TxDataType.Int:
                    result = new TxMetaNumber(element.GetInt64());
                    break;
                case TxDataType.Bytes:
                    result = new TxMetaBytes(Encoding.Unicode.GetBytes(element.GetString() ?? ""));
                    break;
                case TxDataType.String:
                    result = new TxMetaText(element.GetString() ?? "");
                    break;
                case TxDataType.List:
                    var innerValues = element.EnumerateArray()
                        .Select(e => ParseSchemaElement(e))
                        .ToArray();

                    result = new TxMetaList(innerValues);
                    break;
                case TxDataType.Map:
                    var objectMap = element.EnumerateArray()
                        .Select(e =>
                        {
                            var keyObject = e.GetProperty("k");
                            var valueObject = e.GetProperty("v");

                            return new TxMetaMapKVPair(ParseSchemaElement(keyObject), ParseSchemaElement(valueObject));
                        })
                        .ToArray();

                    result = new TxMetaMap(objectMap);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected data type type: " + dataType);
            }

            return result;
        }
    }
}
