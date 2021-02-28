using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

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

        public static TxMetadata FromJson(string json)
        {
            var doc = JsonDocument.Parse(json);
            var rootMetadataValue = ParseElement(doc.RootElement);

            var metadata = new TxMetadata();
            metadata.Add(0, rootMetadataValue);

            return metadata;
        }

        public static ITxMetadataValue ParseElement(JsonElement element)
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
                    // try to parse as a bytestring first
                    try
                    {
                        result = new TxMetaBytes(Encoding.Unicode.GetBytes(element.GetString() ?? ""));
                    }
                    catch(Exception)
                    {
                        // fall back to text if bytestring fails
                        result = new TxMetaText(element.GetString() ?? "");
                    }
                    break;
                case JsonValueKind.Array:
                    var innerValues = element.EnumerateArray()
                        .Select(e => ParseElement(e))
                        .ToArray();

                    result = new TxMetaList(innerValues);
                    break;
                case JsonValueKind.Object:
                    var objectMap = element.EnumerateObject()
                        .Select(e => new TxMetaMapKVPair(new TxMetaText(e.Name), ParseElement(e.Value)))
                        .ToArray();

                    result = new TxMetaMap(objectMap);
                    break;
                default:
                    throw new InvalidOperationException("JSON type not supported: " + element.ValueKind);
            }

            return result;
        }
    }
}
