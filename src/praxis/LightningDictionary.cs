// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using LightningDB;

namespace KeyValueStore
{
    public sealed class LightningDictionary<TKey, TValue> : LightningDataStore,
        IDictionarySlim<TKey, TValue>,
        IDictionary<TKey, TValue>
    {
        private readonly ObjectToMemory<TKey> _keyToMemory;
        private readonly MemoryToObject<TKey> _memoryToKey;
        private readonly MemoryToObject<TValue> _memoryToValue;
        private readonly ObjectToMemory<TValue> _valueToMemory;

        public LightningDictionary(string path,
            ObjectToMemory<TKey> keyToMemory,
            MemoryToObject<TKey> memoryToKey,
            ObjectToMemory<TValue> valueToMemory,
            MemoryToObject<TValue> memoryToValue)
        {
            _keyToMemory = keyToMemory;
            _memoryToKey = memoryToKey;
            _valueToMemory = valueToMemory;
            _memoryToValue = memoryToValue;

            Init(path);
        }

        #region IDictionarySlim<TKey, TValue>

        public TValue this[TKey key]
        {
            get => Get(key);
            set => Set(key, value, false);
        }

        public bool ContainsKey(TKey key) => Exists(key);

        public void Add(TKey key, TValue value) => Set(key, value, true);

        public bool Remove(TKey key) => DeleteByKey(key);

        public bool TryGetValue(TKey key, out TValue value) => TryGetByKey(key, out value);

        #endregion

        #region IEnumerable<out T>

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var kvp in StreamKeyValuePairs())
                yield return kvp;
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region ICollection<TValue>

        public int Count => (int) GetLength();

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!TryGetValue(item.Key, out var value))
                return false;
            return item.Value != null && item.Value.Equals(value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if ((uint) arrayIndex > (uint) array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < Count)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            var index = 0;
            foreach (var entry in StreamKeyValuePairs())
                array[index++] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!TryGetValue(item.Key, out var value))
                return false;

            if (item.Value == null && value != null)
                return false;

            if (item.Value == null && value == null)
                return DeleteByKey(item.Key);

            if (item.Value != null && item.Value.Equals(value))
                return DeleteByKey(item.Key);

            return false;
        }

        #endregion

        #region IDictionary<TKey, TValue>

        public ICollection<TKey> Keys => StreamKeys();

        public ICollection<TValue> Values => StreamValues();

        #endregion

        #region Implementation

        private bool TryGetByKey(TKey key, out TValue value)
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.ReadOnly);
            using var db = tx.OpenDatabase(configuration: Config);
            using var cursor = tx.CreateCursor(db);

            var (sr, _, _) = cursor.SetKey(_keyToMemory(key));
            if (sr != MDBResultCode.Success)
            {
                value = default;
                return false;
            }

            var (_, _, cv) = cursor.GetCurrent();
            value = _memoryToValue(cv.AsSpan());
            return true;
        }

        private bool Exists(TKey key)
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.ReadOnly);
            using var db = tx.OpenDatabase(configuration: Config);
            using var cursor = tx.CreateCursor(db);

            var (sr, _, _) = cursor.SetKey(_keyToMemory(key));
            if (sr != MDBResultCode.Success)
                return default;

            var (gr, _, _) = cursor.GetCurrent();
            return gr == MDBResultCode.Success;
        }

        private bool DeleteByKey(TKey key)
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.None);
            using var db = tx.OpenDatabase(configuration: Config);

            var result = tx.Delete(db, _keyToMemory(key));
            if (result != MDBResultCode.Success)
                return false;

            return tx.Commit() == MDBResultCode.Success;
        }

        private TValue Get(TKey key)
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.ReadOnly);
            using var db = tx.OpenDatabase(configuration: Config);
            using var cursor = tx.CreateCursor(db);

            var (sr, _, _) = cursor.SetKey(_keyToMemory(key));
            if (sr != MDBResultCode.Success)
                throw new KeyNotFoundException();

            var (_, _, cv) = cursor.GetCurrent();
            return _memoryToValue(cv.AsSpan());
        }

        private void Set(TKey key, TValue value, bool isKeyUnique)
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.None);
            using var db = tx.OpenDatabase(configuration: Config);

            var keyBytes = _keyToMemory(key);
            var valueBytes = _valueToMemory(value);

            if (isKeyUnique)
            {
                using var cursor = tx.CreateCursor(db);

                var (sr, _, _) = cursor.SetKey(_keyToMemory(key));
                if (sr == MDBResultCode.Success && cursor.GetCurrent().resultCode == MDBResultCode.Success)
                    throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
            }

            tx.Put(db, keyBytes, valueBytes, PutOptions.NoDuplicateData);
            tx.Commit();
        }

        private ICollection<TKey> StreamKeys()
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.ReadOnly);
            using var db = tx.OpenDatabase(configuration: Config);
            using var cursor = tx.CreateCursor(db);

            var keys = new List<TKey>(); // FIXME: allocation

            foreach (var (k, _) in cursor.AsEnumerable())
                keys.Add(_memoryToKey(k.AsSpan()));

            return keys;
        }

        private ICollection<TValue> StreamValues()
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.ReadOnly);
            using var db = tx.OpenDatabase(configuration: Config);
            using var cursor = tx.CreateCursor(db);

            var values = new List<TValue>(); // FIXME: allocation

            foreach (var (_, v) in cursor.AsEnumerable())
                values.Add(_memoryToValue(v.AsSpan()));

            return values;
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> StreamKeyValuePairs()
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.ReadOnly);
            using var db = tx.OpenDatabase(configuration: Config);
            using var cursor = tx.CreateCursor(db);

            foreach (var (k, v) in cursor.AsEnumerable())
                yield return new KeyValuePair<TKey, TValue>(_memoryToKey(k.AsSpan()), _memoryToValue(v.AsSpan()));
        }

        #endregion
    }
}