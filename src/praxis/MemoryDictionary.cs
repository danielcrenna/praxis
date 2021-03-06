﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace praxis
{
    public sealed class MemoryDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionarySlim<TKey, TValue>
    {
    }
}