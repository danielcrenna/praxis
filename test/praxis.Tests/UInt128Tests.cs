// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using Xunit;

namespace praxis.Tests
{
    public class UInt128Tests
    {
        [Fact]
        public void CompareTo_UInt128()
        {
            var a = new UInt128(0, 3);
            var b = new UInt128(1, 2);
            Assert.True(b.CompareTo(a) > 0);
            Assert.True(a.CompareTo(b) < 0);
        }

        [Fact]
        public void Operators_Compare()
        {
            var a = new UInt128(0, 3);
            var b = new UInt128(1, 2);
            Assert.True(b > a);
            Assert.True(a < b);
        }

        [Fact]
        public void Operators_Equality()
        {
            var a = new UInt128(3, 1);
            var b = new UInt128(3, 1);
            Assert.True(b == a);
            Assert.True(b.Equals(a));
            Assert.True(a == b);
            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Operators_ImplicitString()
        {
            const string hash = "6DAE2A78FA1B9821BD677861ACA46C7D";

            var h1 = hash.Substring(0, 16);
            var h2 = hash.Substring(16, 16);

            var v1 = Convert.ToUInt64(h1, 16);
            var v2 = Convert.ToUInt64(h2, 16);

            Assert.Equal(7903301095162353697UL, v1);
            Assert.Equal(13648009556673195133, v2);

            UInt128 a = hash;
            var b = new UInt128(7903301095162353697UL, 13648009556673195133UL);

            Assert.True(b == a);
            Assert.True(a == b);

            Assert.Equal(7903301095162353697UL, a.v1);
            Assert.Equal(13648009556673195133, a.v2);
        }

        [Fact]
        public void Operators_Inequality()
        {
            var a = new UInt128(2, 2);
            var b = new UInt128(3, 2);
            Assert.True(b != a);
            Assert.False(b.Equals(a));
            Assert.True(a != b);
            Assert.False(a.Equals(b));
        }
    }
}