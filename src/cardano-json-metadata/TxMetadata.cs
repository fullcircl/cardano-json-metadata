using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
    }
}
