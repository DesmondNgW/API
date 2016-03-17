/*
 * Copyright 2015 Tomi Valkeinen
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NetSerializer
{
    /// <summary>
    /// Threadsafe Type -> T dictionary, which supports lockless reading.
    /// </summary>
    class TypeDictionary
    {
        struct Pair
        {
            public Pair(Type key, TypeData value)
            {
                Key = key;
                Value = value;
            }

            public readonly Type Key;
            public readonly TypeData Value;
        }

        Pair[][] _mBuckets;
        readonly object _mWriteLock = new object();
        int _mNumItems;
        const int InitialListSize = 1;
        const float LoadLimit = 0.50f;
        const int InitialLength = 256;

        public TypeDictionary()
        {
            const int numBuckets = (int)(InitialLength * (1.0f / LoadLimit));
            _mBuckets = new Pair[numBuckets][];
        }

        public bool ContainsKey(Type key)
        {
            TypeData value;
            return TryGetValue(key, out value);
        }

        public bool TryGetValue(Type key, out TypeData value)
        {
            var buckets = Volatile.Read(ref _mBuckets);
            var idx = Hash(key, buckets.Length);
            var arr = Volatile.Read(ref buckets[idx]);
            if (arr == null) goto not_found;
            for (var i = 0; i < arr.Length; ++i)
            {
                if (arr[i].Key != key) continue;
                value = arr[i].Value;
                return true;
            }
            not_found:
                value = null;
                return false;
        }

        public TypeData this[Type key]
        {
            get
            {
                var buckets = Volatile.Read(ref _mBuckets);
                var idx = Hash(key, buckets.Length);
                var arr = Volatile.Read(ref buckets[idx]);
                if (arr == null) throw new KeyNotFoundException();
                for (var i = 0; i < arr.Length; ++i)
                {
                    if (arr[i].Key == key) return arr[i].Value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                lock (_mWriteLock)
                {
                    Debug.Assert(ContainsKey(key) == false);
                    if (_mNumItems >= _mBuckets.Length * LoadLimit)
                    {
                        var newBuckets = new Pair[_mBuckets.Length * 2][];
                        foreach (var pair in _mBuckets.Where(l => l != null).SelectMany(list => list.Where(p => p.Key != null)))
                        {
                            Add(newBuckets, pair.Key, pair.Value);
                        }
                        Volatile.Write(ref _mBuckets, newBuckets);
                    }
                    Add(_mBuckets, key, value);
                    _mNumItems++;
                }
            }
        }

        static void Add(IList<Pair[]> buckets, Type key, TypeData value)
        {
            var idx = Hash(key, buckets.Count);
            var arr = buckets[idx];
            if (arr == null) buckets[idx] = arr = new Pair[InitialListSize];
            for (var i = 0; i < arr.Length; ++i)
            {
                if (arr[i].Key != null) continue;
                arr[i] = new Pair(key, value);
                return;
            }
            var newArr = new Pair[arr.Length * 2];
            Array.Copy(arr, newArr, arr.Length);
            newArr[arr.Length] = new Pair(key, value);
            buckets[idx] = newArr;
        }

        static int Hash(Type key, int bucketsLen)
        {
            var h = (uint)System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(key);
            h %= (uint)bucketsLen;
            return (int)h;
        }

        [Conditional("DEBUG")]
        public void DebugDump()
        {
            var occupied = _mBuckets.Count(i => i != null);
            Console.WriteLine("bucket arr len {0}, items {1}, occupied buckets {2}", _mBuckets.Length, _mNumItems, occupied);
            var countmap = new Dictionary<int, int>();
            foreach (var c in from list in _mBuckets where list != null select list.TakeWhile(p => p.Key != null).Count())
            {
                if (countmap.ContainsKey(c) == false) countmap[c] = 0;
                countmap[c]++;
            }
            foreach (var kvp in countmap.OrderBy(kvp => kvp.Key)) Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
        }
    }
}
