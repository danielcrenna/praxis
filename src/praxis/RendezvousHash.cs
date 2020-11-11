// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;

namespace praxis
{
    public static class RendezvousHash
    {
        public static IEnumerable<string> Choose(this IEnumerable<KeyValuePair<UInt128, string>> destinations,
            string key, int k = 1, UInt128 seed = default)
        {
            var keyHash = MurmurHash3.ComputeHash(key, seed);
            var annotated = destinations.ToDictionary(
                d => keyHash ^ MurmurHash3.ComputeHash(d.Value, seed),
                d => d.Value);
            var sorted = new SortedDictionary<UInt128, string>(annotated);

            var index = 0;
            foreach (var entry in sorted)
                if (index++ < k)
                    yield return entry.Value;
                else
                    yield break;
        }
    }
}