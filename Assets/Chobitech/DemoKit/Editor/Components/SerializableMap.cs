// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections.Generic;
using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A generic wrapper that enables <see cref="Dictionary{TKey, TValue}"/> serialization in Unity.
    /// It implements <see cref="ISerializationCallbackReceiver"/> to synchronize a fast, non-serializable 
    /// dictionary with a serializable list of key-value pairs, allowing complex data mappings to be 
    /// persisted within the Unity Editor environment.
    /// </summary>
    [System.Serializable]
    internal class SerializableMap<K, V> : ISerializationCallbackReceiver
    {
        [System.Serializable]
        internal class SerializableMapEntry<KK, VV>
        {
            [SerializeField]
            internal KK key;

            [SerializeField]
            internal VV value;

            internal SerializableMapEntry(KK key, VV value)
            {
                this.key = key;
                this.value = value;
            }
        }

        private readonly Dictionary<K, V> _innerMap = new();

        [SerializeField]
        private List<SerializableMapEntry<K, V>> entryList;

        private List<SerializableMapEntry<K, V>> InnerEntryList => entryList ??= new();


        internal void Restore()
        {
            _innerMap.Clear();

            foreach (var e in InnerEntryList)
            {
                if (e == null)
                {
                    continue;
                }
                _innerMap[e.key] = e.value;
            }
        }

        internal void Commit()
        {
            InnerEntryList.Clear();

            foreach (var m in _innerMap)
            {
                InnerEntryList.Add(new(m.Key, m.Value));
            }
        }

        public void OnBeforeSerialize()
        {
            Commit();
        }

        public void OnAfterDeserialize()
        {
            Restore();
        }


        internal V this[K key]
        {
            get
            {
                if (_innerMap.TryGetValue(key, out var v))
                {
                    return v;
                }
                return default;
            }
            set => _innerMap[key] = value;
        }


        internal bool TryGetValue(K key, out V value)
        {
            return _innerMap.TryGetValue(key, out value);
        }

        internal IEnumerable<K> Keys => _innerMap.Keys;
        internal IEnumerable<V> Values => _innerMap.Values;
    }
}
