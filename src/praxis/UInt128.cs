// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;

namespace praxis
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UInt128 : IComparable<UInt128>, IComparable
    {
        public readonly ulong v1;
        public readonly ulong v2;

        public UInt128(ulong v1, ulong v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        internal UInt128(string value) : this(Convert.ToUInt64(value.Substring(0, 16), 16),
            Convert.ToUInt64(value.Substring(16, 16), 16))
        {
        }

        public static bool operator ==(UInt128 a, UInt128 b)
        {
            return a.v1 == b.v1 && a.v2 == b.v2;
        }

        public static bool operator !=(UInt128 a, UInt128 b)
        {
            return !(a == b);
        }

        public static bool operator >(UInt128 a, UInt128 b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(UInt128 a, UInt128 b)
        {
            return a.CompareTo(b) < 0;
        }

        public static UInt128 operator ^(UInt128 a, UInt128 b)
        {
            return new UInt128(a.v1 ^ b.v1, a.v2 ^ b.v2);
        }

        public static implicit operator UInt128(string id)
        {
            return new UInt128(id);
        }

        public override bool Equals(object obj)
        {
            return obj is UInt128 o && o == this;
        }

        public override int GetHashCode()
        {
            return (int) (v1 ^ v2);
        }

        public override string ToString()
        {
            return $"{v1:X8}{v2:X8}";
        }

        public int CompareTo(UInt128 other)
        {
            return string.Compare(ToString(), other.ToString(), StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            return obj switch
            {
                null => 1,
                UInt128 other => CompareTo(other),
                _ => throw new ArgumentException($"Object must be of type {nameof(UInt128)}")
            };
        }
    }
}