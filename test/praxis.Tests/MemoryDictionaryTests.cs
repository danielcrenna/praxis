// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Xunit;

namespace praxis.Tests
{
    public class MemoryDictionaryTests
    {
        [Fact]
        public void Collection_Add_DoesNotCheckValue()
        {
            var dictionary = new MemoryDictionary<string, string> {{"foo", "bar"}};

            // System.ArgumentException : An item with the same key has already been added. Key: foo
            Assert.Throws<ArgumentException>(() =>
            {
                var pair = new KeyValuePair<string, string>("foo", "baz");
                var collection = (ICollection<KeyValuePair<string, string>>) dictionary;
                collection.Add(pair);
            });
            Assert.Single(dictionary);
        }

        [Fact]
        public void Collection_Contains_ChecksValue_WhenNotNull()
        {
            var dictionary = new MemoryDictionary<string, string> {{"foo", "bar"}};
            var collection = (ICollection<KeyValuePair<string, string>>) dictionary;

            Assert.True(collection.Contains(new KeyValuePair<string, string>("foo", "bar")));
            Assert.False(collection.Contains(new KeyValuePair<string, string>("foo", "baz")));
        }

        [Fact]
        public void Collection_Contains_ChecksValue_WhenNull()
        {
            var withValue = new MemoryDictionary<string, string> {{"foo", "bar"}};
            var withValueCollection = (ICollection<KeyValuePair<string, string>>) withValue;
            Assert.True(withValueCollection.Contains(new KeyValuePair<string, string>("foo", "bar")));
            Assert.False(withValueCollection.Contains(new KeyValuePair<string, string>("foo", null)));

            var withoutValue = new MemoryDictionary<string, string> {{"foo", null}};
            var withoutValueCollection = (ICollection<KeyValuePair<string, string>>) withoutValue;
            Assert.True(withoutValueCollection.Contains(new KeyValuePair<string, string>("foo", null)));
            Assert.False(withoutValueCollection.Contains(new KeyValuePair<string, string>("foo", "bar")));
        }

        [Fact]
        public void Collection_Remove_ChecksValue()
        {
            var dictionary = new MemoryDictionary<string, string> {{"foo", "bar"}};
            var collection = (ICollection<KeyValuePair<string, string>>) dictionary;

            Assert.False(collection.Remove(new KeyValuePair<string, string>("foo", "baz")));
            Assert.True(collection.Remove(new KeyValuePair<string, string>("foo", "bar")));
        }

        [Fact]
        public void Dictionary_Add_SameKey()
        {
            var dictionary = new MemoryDictionary<string, string> {{"foo", "bar"}};

            // System.ArgumentException : An item with the same key has already been added. Key: foo
            Assert.Throws<ArgumentException>(() => dictionary.Add("foo", "bar"));
            Assert.Single(dictionary);
        }
    }
}