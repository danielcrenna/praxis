// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace praxis.Tests
{
    public class RendezvousHashTests
    {
        private static KeyValuePair<UInt128, string> AddDestination(string host)
        {
            return new KeyValuePair<UInt128, string>(MurmurHash3.ComputeHash(host), host);
        }

        [Fact]
        public void PickDestinations_ConsistentWhenHostNotRemoved()
        {
            var destinations = new List<KeyValuePair<UInt128, string>>
            {
                AddDestination("host-a"),
                AddDestination("host-b"),
                AddDestination("host-c")
            };

            var target = destinations.Choose("ABC").ToList();
            Assert.Equal("host-c", Assert.Single(target));

            destinations.RemoveAt(1);
            Assert.Equal(2, destinations.Count);

            var newTarget = destinations.Choose("ABC").ToList();
            Assert.Equal("host-c", Assert.Single(newTarget));
        }

        [Fact]
        public void PickDestinations_RebalancedWhenHostRemoved()
        {
            var destinations = new List<KeyValuePair<UInt128, string>>
            {
                AddDestination("host-a"),
                AddDestination("host-b"),
                AddDestination("host-c")
            };

            var target = destinations.Choose("ABC").ToList();
            Assert.Equal("host-c", Assert.Single(target));

            destinations.RemoveAt(2);
            Assert.Equal(2, destinations.Count);

            var one = destinations.Choose("ABC").ToList();
            Assert.Equal("host-a", Assert.Single(one));

            destinations.RemoveAt(0);
            Assert.Single(destinations);

            Assert.Equal("host-b", Assert.Single(destinations.Choose("ABC")));
        }
    }
}