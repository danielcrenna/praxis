// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Text;

namespace praxis.Tests
{
    public class LightningDictionaryFixture : IDisposable
    {
        public LightningDictionaryFixture()
        {
            Value = new LightningDictionary<string, string>(
                $"{Guid.NewGuid()}.db",
                k => StringToBytes(k),
                rk => Encoding.UTF8.GetString(rk),
                v => StringToBytes(v),
                BytesToString
            );
        }

        public LightningDictionary<string, string> Value { get; }

        public void Dispose()
        {
            try
            {
                Value.Destroy();
            }
            finally
            {
                Value.Dispose();
            }
        }

        private static string BytesToString(ReadOnlySpan<byte> bytes)
        {
            return bytes.Length == 0 ? default : Encoding.UTF8.GetString(bytes);
        }

        private static byte[] StringToBytes(string k)
        {
            return k == default ? default : Encoding.UTF8.GetBytes(k);
        }
    }
}