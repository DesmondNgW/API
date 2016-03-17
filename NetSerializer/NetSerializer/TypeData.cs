/*
 * Copyright 2015 Tomi Valkeinen
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NetSerializer
{
    sealed class TypeData
    {
        public TypeData(Type type, uint typeId, ITypeSerializer typeSerializer)
        {
            Type = type;
            TypeId = typeId;
            TypeSerializer = typeSerializer;
        }

        public Type Type { get; private set; }
        public uint TypeId { get; private set; }
        public ITypeSerializer TypeSerializer { get; private set; }
        public MethodInfo WriterMethodInfo;
        public MethodInfo ReaderMethodInfo;
        public SerializeDelegate<object> WriterTrampolineDelegate;
        public Delegate WriterDirectDelegate;
        public DeserializeDelegate<object> ReaderTrampolineDelegate;
        public Delegate ReaderDirectDelegate;

        public bool WriterNeedsInstance
        {
            get
            {
                #if GENERATE_DEBUGGING_ASSEMBLY
                if (WriterMethodInfo is MethodBuilder) return WriterNeedsInstanceDebug;
                #endif
                return WriterMethodInfo.GetParameters().Length == 3;
            }
        }

        public bool ReaderNeedsInstance
        {
            get
            {
                #if GENERATE_DEBUGGING_ASSEMBLY
                if (ReaderMethodInfo is MethodBuilder) return ReaderNeedsInstanceDebug;
                #endif
                return ReaderMethodInfo.GetParameters().Length == 3;
            }
        }

        #if GENERATE_DEBUGGING_ASSEMBLY
        // MethodBuilder doesn't support GetParameters(), so we need to track this separately
        public bool WriterNeedsInstanceDebug;
        public bool ReaderNeedsInstanceDebug;
        #endif

        public bool CanCallDirect
        {
            get
            {
                // We can call the (De)serializer method directly for:
                // - Value types
                // - Array types
                // - Sealed types with static (De)serializer method, as the method will handle null
                // Other types go through the ObjectSerializer
                var type = Type;
                if (type.IsValueType || type.IsArray) return true;
                return type.IsSealed && (TypeSerializer is IStaticTypeSerializer);
            }
        }
    }
}