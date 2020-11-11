// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Text;
using Xunit;

namespace praxis.Tests
{
    public class MurmurHash3Tests
    {
        [Fact]
        public void ComputeHash_ByteSpan_SameValues_DiffSeed_DiffHash()
        {
            var one = MurmurHash3.ComputeHash(Encoding.UTF8.GetBytes("This is a string"), new UInt128(123, 456));
            var two = MurmurHash3.ComputeHash(Encoding.UTF8.GetBytes("This is a string"), new UInt128(456, 123));
            Assert.NotEqual(one, two);
        }

        [Fact]
        public void ComputeHash_ByteSpan_SameValues_SameHash()
        {
            var one = MurmurHash3.ComputeHash(Encoding.UTF8.GetBytes("This is a string"));
            var two = MurmurHash3.ComputeHash(Encoding.UTF8.GetBytes("This is a string"));
            Assert.Equal(one, two);
        }

        [Fact]
        public void ComputeHash_ByteSpan_SameValues_SameSeed_SameHash()
        {
            var one = MurmurHash3.ComputeHash(Encoding.UTF8.GetBytes("This is a string"), new UInt128(123, 456));
            var two = MurmurHash3.ComputeHash(Encoding.UTF8.GetBytes("This is a string"), new UInt128(123, 456));
            Assert.Equal(one, two);
        }

        [Fact]
        public void ComputeHash_Int64_SameValues_DiffSeed_DiffHash()
        {
            var one = MurmurHash3.ComputeHash(1337UL, new UInt128(123, 456));
            var two = MurmurHash3.ComputeHash(1337UL, new UInt128(456, 123));
            Assert.NotEqual(one, two);
        }

        [Fact]
        public void ComputeHash_Int64_SameValues_SameHash()
        {
            var one = MurmurHash3.ComputeHash(1337UL);
            var two = MurmurHash3.ComputeHash(1337UL);
            Assert.Equal(one, two);
        }

        [Fact]
        public void ComputeHash_Int64_SameValues_SameSeed_SameHash()
        {
            var one = MurmurHash3.ComputeHash(1337UL, new UInt128(123, 456));
            var two = MurmurHash3.ComputeHash(1337UL, new UInt128(123, 456));
            Assert.Equal(one, two);
        }

        [Fact]
        public void ComputeHash_String_SameValues_DiffSeed_DiffHash()
        {
            var one = MurmurHash3.ComputeHash("This is a string", new UInt128(123, 456));
            var two = MurmurHash3.ComputeHash("This is a string", new UInt128(456, 123));
            Assert.NotEqual(one, two);
        }

        [Fact]
        public void ComputeHash_String_SameValues_SameHash()
        {
            var one = MurmurHash3.ComputeHash("This is a string");
            var two = MurmurHash3.ComputeHash("This is a string");
            Assert.Equal(one, two);
        }

        [Fact]
        public void ComputeHash_String_SameValues_SameSeed_SameHash()
        {
            var one = MurmurHash3.ComputeHash("This is a string", new UInt128(123, 456));
            var two = MurmurHash3.ComputeHash("This is a string", new UInt128(123, 456));
            Assert.Equal(one, two);
        }

        [Fact]
        public void ComputeHash_StringBuilder_SameValues_DiffSeed_DiffHash()
        {
            var one = MurmurHash3.ComputeHash(new StringBuilder("This is a string"), new UInt128(123, 456));
            var two = MurmurHash3.ComputeHash(new StringBuilder("This is a string"), new UInt128(456, 123));
            Assert.NotEqual(one, two);
        }

        [Fact]
        public void ComputeHash_StringBuilder_SameValues_SameHash()
        {
            var one = MurmurHash3.ComputeHash(new StringBuilder("This is a string"));
            var two = MurmurHash3.ComputeHash(new StringBuilder("This is a string"));
            Assert.Equal(one, two);
        }

        [Fact]
        public void ComputeHash_StringBuilder_SameValues_SameSeed_SameHash()
        {
            var one = MurmurHash3.ComputeHash(new StringBuilder("This is a string"), new UInt128(123, 456));
            var two = MurmurHash3.ComputeHash(new StringBuilder("This is a string"), new UInt128(123, 456));
            Assert.Equal(one, two);
        }
    }
}