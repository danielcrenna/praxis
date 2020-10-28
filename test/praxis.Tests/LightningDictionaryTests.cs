// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace praxis.Tests
{
    public class LightningDictionaryTests : IClassFixture<LightningDictionaryFixture>
    {
        public LightningDictionaryTests(LightningDictionaryFixture fixture)
        {
            _fixture = fixture;
            _fixture.Value.TryAdd("foo", "bar");
        }

        private readonly LightningDictionaryFixture _fixture;

        [Fact]
        public void ClearAndCount()
        {
            _fixture.Value.Clear();
            Assert.Empty(_fixture.Value);
            _fixture.Value.Add("foo", "bar");
            Assert.Single(_fixture.Value);
        }

        [Fact]
        public void Collection_Add_DoesNotCheckValue()
        {
            // System.ArgumentException : An item with the same key has already been added. Key: foo
            Assert.Throws<ArgumentException>(() =>
            {
                var pair = new KeyValuePair<string, string>("foo", "baz");
                var collection = (ICollection<KeyValuePair<string, string>>) _fixture.Value;
                collection.Add(pair);
            });
            Assert.Single(_fixture.Value);
        }

        [Fact]
        public void Collection_Contains_Found_ChecksValue_WhenNotNull()
        {
            Assert.Contains(new KeyValuePair<string, string>("foo", "bar"), _fixture.Value);
            Assert.DoesNotContain(new KeyValuePair<string, string>("foo", "baz"), _fixture.Value);
        }

        [Fact]
        public void Collection_Contains_Found_ChecksValue_WhenNull()
        {
            Assert.Contains(new KeyValuePair<string, string>("foo", "bar"), _fixture.Value);
            Assert.DoesNotContain(new KeyValuePair<string, string>("foo", null), _fixture.Value);
        }

        [Fact]
        public void Collection_Contains_NotFound()
        {
            Assert.DoesNotContain(new KeyValuePair<string, string>("faa", "bar"), _fixture.Value);
        }

        [Fact]
        public void Collection_CopyTo()
        {
            var target = new KeyValuePair<string, string>[1];
            _fixture.Value.CopyTo(target, 0);
            Assert.Single(target);
            Assert.Equal("foo", target[0].Key);
            Assert.Equal("bar", target[0].Value);
        }

        [Fact]
        public void Collection_CopyTo_BadIndex()
        {
            var target = new KeyValuePair<string, string>[1];
            Assert.Throws<ArgumentOutOfRangeException>(() => _fixture.Value.CopyTo(target, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _fixture.Value.CopyTo(target, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _fixture.Value.CopyTo(target, 2));
        }

        [Fact]
        public void Collection_Enumeration()
        {
            foreach (var (k, v) in _fixture.Value)
            {
                Assert.Equal("foo", k);
                Assert.Equal("bar", v);
            }
        }

        [Fact]
        public void Collection_GetEnumerator()
        {
            using var enumerator = _fixture.Value.GetEnumerator();
            enumerator.MoveNext();
            Assert.Equal("foo", enumerator.Current.Key);
            Assert.Equal("bar", enumerator.Current.Value);
        }

        [Fact]
        public void Collection_IsReadOnly_False()
        {
            Assert.False(_fixture.Value.IsReadOnly);
        }

        [Fact]
        public void Collection_Remove_Found_ChecksValue()
        {
            var collection = (ICollection<KeyValuePair<string, string>>) _fixture.Value;

            Assert.False(collection.Remove(new KeyValuePair<string, string>("foo", "baz")));
            Assert.True(collection.Remove(new KeyValuePair<string, string>("foo", "bar")));
        }

        [Fact]
        public void Collection_Remove_Found_ChecksValue_NullValue()
        {
            var collection = (ICollection<KeyValuePair<string, string>>) _fixture.Value;
            Assert.False(collection.Remove(new KeyValuePair<string, string>("foo", null)));

            var entry = new KeyValuePair<string, string>("null", null);
            collection.Add(entry);
            Assert.True(collection.Remove(entry));
        }

        [Fact]
        public void Collection_Remove_NotFound()
        {
            var collection = (ICollection<KeyValuePair<string, string>>) _fixture.Value;
            Assert.False(collection.Remove(new KeyValuePair<string, string>("fab", "baz")));
        }

        [Fact]
        public void Collection_Streaming_Values()
        {
            foreach (var value in _fixture.Value.Values)
                Assert.Equal("bar", value);
        }

        [Fact]
        public void ContainsKey()
        {
            Assert.True(_fixture.Value.ContainsKey("foo"));
            Assert.False(_fixture.Value.ContainsKey("bar"));
        }

        [Fact]
        public void DataStore_Init_Twice()
        {
            var filePath = $"{nameof(DataStore_Init_Twice)}_{Guid.NewGuid()}";

            var one = new LightningDictionary<string, string>(
                filePath,
                k => Encoding.UTF8.GetBytes(k),
                rk => Encoding.UTF8.GetString(rk),
                v => Encoding.UTF8.GetBytes(v),
                rv => Encoding.UTF8.GetString(rv)
            ) {{"foo", "bar"}};

            one.Init(one.FilePath);
            one.Destroy();
        }

        [Fact]
        public void DataStore_SamePath()
        {
            var filePath = $"{nameof(DataStore_SamePath)}_{Guid.NewGuid()}";

            var one = new LightningDictionary<string, string>(
                filePath,
                k => Encoding.UTF8.GetBytes(k),
                rk => Encoding.UTF8.GetString(rk),
                v => Encoding.UTF8.GetBytes(v),
                rv => Encoding.UTF8.GetString(rv)
            ) {{"foo", "bar"}};

            Assert.True(one.ContainsKey("foo"));

            var two = new LightningDictionary<string, string>(
                filePath,
                k => Encoding.UTF8.GetBytes(k),
                rk => Encoding.UTF8.GetString(rk),
                v => Encoding.UTF8.GetBytes(v),
                rv => Encoding.UTF8.GetString(rv)
            );

            Assert.True(two.ContainsKey("foo"));

            one.Destroy();
            two.Destroy();
        }

        [Fact]
        public void Dictionary_Add_SameKey()
        {
            Assert.Throws<ArgumentException>(() => _fixture.Value.Add("foo", "bar"));
            Assert.Single(_fixture.Value);
        }

        [Fact]
        public void Enumerable_GetEnumerator()
        {
            IEnumerable model = _fixture.Value;
            var enumerator = model.GetEnumerator();
            enumerator.MoveNext();
            Assert.NotNull(enumerator.Current);

            var (k, v) = (KeyValuePair<string, string>) enumerator.Current;
            Assert.Equal("foo", k);
            Assert.Equal("bar", v);

            var disposable = enumerator as IDisposable;
            disposable?.Dispose();
        }

        [Fact]
        public void Get_Found()
        {
            Assert.Equal("bar", _fixture.Value["foo"]);
        }

        [Fact]
        public void Get_NotFound()
        {
            Assert.Throws<KeyNotFoundException>(() => _fixture.Value["bar"]);
        }

        [Fact]
        public void GetKeys()
        {
            Assert.Equal(1, _fixture.Value.Keys.Count);
            Assert.Equal("foo", _fixture.Value.Keys.Single());
        }

        [Fact]
        public void Remove_Found()
        {
            _fixture.Value.Add("baz", "bar");
            Assert.True(_fixture.Value.Remove("baz"));
        }

        [Fact]
        public void Remove_NotFound()
        {
            Assert.False(_fixture.Value.Remove("baz"));
        }

        [Fact]
        public void SetAccessor()
        {
            Assert.Equal("bar", _fixture.Value["foo"]);
            _fixture.Value["foo"] = "baz";
            Assert.Equal("baz", _fixture.Value["foo"]);
            ClearAndCount();
        }

        [Fact]
        public void TryGetValue_KeyFound()
        {
            Assert.True(_fixture.Value.TryGetValue("foo", out var value));
            Assert.Equal("bar", value);
        }

        [Fact]
        public void TryGetValue_KeyNotFound()
        {
            Assert.False(_fixture.Value.TryGetValue("bar", out _));
        }
    }
}