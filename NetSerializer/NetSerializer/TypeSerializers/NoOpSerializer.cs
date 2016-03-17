/*
 * Copyright 2015 Tomi Valkeinen
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NetSerializer.TypeSerializers
{
    /// <summary>
    /// A "no-op" TypeSerializer which can be used to make the NetSerializer ignore fields of certain type.
    /// For example, Delegates cannot be serializer by default, and NoOpSerializer could be used to ignore all subclasses of Delegate
    /// </summary>
    sealed class NoOpSerializer : IStaticTypeSerializer
    {
        readonly Type[] _mTypes;
        readonly bool _mHandleSubclasses;

        public NoOpSerializer(IEnumerable<Type> types, bool handleSubclasses)
        {
            _mTypes = types.ToArray();
            _mHandleSubclasses = handleSubclasses;
        }

        public bool Handles(Type type)
        {
            return _mHandleSubclasses ? _mTypes.Any(type.IsSubclassOf) : _mTypes.Contains(type);
        }

        public IEnumerable<Type> GetSubtypes(Type type)
        {
            return new Type[0];
        }

        public MethodInfo GetStaticWriter(Type type)
        {
            return GetType().GetMethod("Serialize", BindingFlags.Static | BindingFlags.Public);
        }

        public MethodInfo GetStaticReader(Type type)
        {
            return GetType().GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public);
        }

        public static void Serialize(Serializer serializer, Stream stream, object ob)
        {
        }

        public static void Deserialize(Serializer serializer, Stream stream, out object ob)
        {
            ob = null;
        }
    }
}
