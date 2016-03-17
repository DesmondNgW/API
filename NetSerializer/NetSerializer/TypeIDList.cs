/*
 * Copyright 2015 Tomi Valkeinen
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Diagnostics;

namespace NetSerializer
{
    /// <summary>
    /// Threadsafe TypeID -> TypeData list, which supports lockless reading.
    /// </summary>
    class TypeIdList
    {
        TypeData[] _mArray;
        readonly object _mWriteLock = new object();

        const int InitialLength = 256;

        public TypeIdList()
        {
            _mArray = new TypeData[InitialLength];
        }

        public bool ContainsTypeId(uint typeId)
        {
            return typeId < _mArray.Length && _mArray[typeId] != null;
        }

        public TypeData this[uint idx]
        {
            get
            {
                return _mArray[idx];
            }

            set
            {
                lock (_mWriteLock)
                {
                    if (idx >= _mArray.Length)
                    {
                        var newArray = new TypeData[NextPowOf2(idx + 1)];
                        Array.Copy(_mArray, newArray, _mArray.Length);
                        _mArray = newArray;
                    }
                    Debug.Assert(_mArray[idx] == null);
                    _mArray[idx] = value;
                }
            }
        }

        static uint NextPowOf2(uint v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }
    }
}
