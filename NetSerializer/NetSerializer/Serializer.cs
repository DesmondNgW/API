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
using System.Reflection.Emit;
using System.Diagnostics;
using NetSerializer.TypeSerializers;

namespace NetSerializer
{
    delegate void SerializeDelegate<in T>(Serializer serializer, Stream stream, T ob);
    delegate void DeserializeDelegate<T>(Serializer serializer, Stream stream, out T ob);

    public class Serializer
    {
        readonly static ITypeSerializer[] STypeSerializers = {
			new ObjectSerializer(),
			new PrimitivesSerializer(),
			new ArraySerializer(),
			new EnumSerializer(),
			new DictionarySerializer(),
			new NullableSerializer(),
			new GenericSerializer(),
		};

        /// <summary>
        /// Initialize NetSerializer
        /// </summary>
        /// <param name="rootTypes">Types to be (de)serialized</param>
        public Serializer(IEnumerable<Type> rootTypes) : this(rootTypes, new ITypeSerializer[0])
        {
        }

        /// <summary>
        /// Initialize NetSerializer
        /// </summary>
        /// <param name="rootTypes">Types to be (de)serialized</param>
        /// <param name="userTypeSerializers">Array of custom serializers</param>
        public Serializer(IEnumerable<Type> rootTypes, ITypeSerializer[] userTypeSerializers)
        {
            if (userTypeSerializers.All(s => s is IDynamicTypeSerializer || s is IStaticTypeSerializer) == false) throw new ArgumentException("TypeSerializers have to implement IDynamicTypeSerializer or IStaticTypeSerializer");
            _mUserTypeSerializers = userTypeSerializers;
            lock (_mModifyLock)
            {
                _mRuntimeTypeMap = new TypeDictionary();
                _mRuntimeTypeIdList = new TypeIdList();
                AddTypesInternal(new[] { typeof(object) }.Concat(rootTypes));
                GenerateWriters(typeof(object));
                GenerateReaders(typeof(object));
                ObjectTypeId = _mRuntimeTypeMap[typeof(object)].TypeId;
            }
        }

        Dictionary<Type, uint> AddTypesInternal(IEnumerable<Type> roots)
        {
            AssertLocked();
            var stack = new Stack<Type>(roots);
            var addedMap = new Dictionary<Type, uint>();
            while (stack.Count > 0)
            {
                var type = stack.Pop();
                if (_mRuntimeTypeMap.ContainsKey(type)) continue;
                if (type.IsAbstract || type.IsInterface) continue;
                if (type.ContainsGenericParameters) throw new NotSupportedException(string.Format("Type {0} contains generic parameters", type.FullName));
                while (_mRuntimeTypeIdList.ContainsTypeId(_mNextAvailableTypeId)) _mNextAvailableTypeId++;
                var typeId = _mNextAvailableTypeId++;
                var serializer = GetTypeSerializer(type);
                var data = new TypeData(type, typeId, serializer);
                _mRuntimeTypeMap[type] = data;
                _mRuntimeTypeIdList[typeId] = data;
                addedMap[type] = typeId;
                foreach (var t in serializer.GetSubtypes(type).Where(t => _mRuntimeTypeMap.ContainsKey(t) == false))
                {
                    stack.Push(t);
                }
            }
            return addedMap;
        }

        /// <summary>
        /// Add rootTypes and all their subtypes, and return a mapping of all added types to typeIDs
        /// </summary>
        public Dictionary<Type, uint> AddTypes(IEnumerable<Type> rootTypes)
        {
            lock (_mModifyLock)
            {
                return AddTypesInternal(rootTypes);
            }
        }

        /// <summary>
        /// Add types obtained by a call to AddTypes in another Serializer instance
        /// </summary>
        public void AddTypes(Dictionary<Type, uint> rootTypes)
        {
            lock (_mModifyLock)
            {
                foreach (var kvp in rootTypes)
                {
                    var type = kvp.Key;
                    var typeId = kvp.Value;
                    if (type == null) throw new ArgumentNullException();
                    if (typeId == 0) throw new ArgumentException();
                    if (_mRuntimeTypeMap.ContainsKey(type)) throw new ArgumentException();
                    if (_mRuntimeTypeIdList.ContainsTypeId(typeId)) throw new ArgumentException();
                    if (type.IsAbstract || type.IsInterface) throw new ArgumentException();
                    if (type.ContainsGenericParameters) throw new NotSupportedException(string.Format("Type {0} contains generic parameters", type.FullName));
                    var serializer = GetTypeSerializer(type);
                    var data = new TypeData(type, typeId, serializer);
                    _mRuntimeTypeMap[type] = data;
                    _mRuntimeTypeIdList[typeId] = data;
                }
            }
        }

        readonly ITypeSerializer[] _mUserTypeSerializers;
        readonly TypeDictionary _mRuntimeTypeMap;
        readonly TypeIdList _mRuntimeTypeIdList;
        readonly object _mModifyLock = new object();
        uint _mNextAvailableTypeId = 1;
        internal readonly uint ObjectTypeId;

        [Conditional("DEBUG")]
        void AssertLocked()
        {
            Debug.Assert(System.Threading.Monitor.IsEntered(_mModifyLock));
        }

        public void Serialize(Stream stream, object ob)
        {
            ObjectSerializer.Serialize(this, stream, ob);
        }

        public object Deserialize(Stream stream)
        {
            object ob;
            ObjectSerializer.Deserialize(this, stream, out ob);
            return ob;
        }

        public void Deserialize(Stream stream, out object ob)
        {
            ObjectSerializer.Deserialize(this, stream, out ob);
        }

        public void SerializeDirect<T>(Stream stream, T value)
        {
            var del = (SerializeDelegate<T>)_mRuntimeTypeMap[typeof(T)].WriterDirectDelegate;
            if (del == null)
            {
                lock (_mModifyLock) del = (SerializeDelegate<T>)GenerateDirectWriterDelegate(typeof(T));
            }
            del(this, stream, value);
        }

        public void DeserializeDirect<T>(Stream stream, out T value)
        {
            var del = (DeserializeDelegate<T>)_mRuntimeTypeMap[typeof(T)].ReaderDirectDelegate;
            if (del == null)
            {
                lock (_mModifyLock) del = (DeserializeDelegate<T>)GenerateDirectReaderDelegate(typeof(T));
            }
            del(this, stream, out value);
        }

        internal uint GetTypeIdAndSerializer(Type type, out SerializeDelegate<object> del)
        {
            var data = _mRuntimeTypeMap[type];
            if (data.WriterTrampolineDelegate != null)
            {
                del = data.WriterTrampolineDelegate;
                return data.TypeId;
            }
            lock (_mModifyLock)
            {
                del = GenerateWriterTrampoline(type);
                return data.TypeId;
            }
        }

        internal DeserializeDelegate<object> GetDeserializeTrampolineFromId(uint id)
        {
            var data = _mRuntimeTypeIdList[id];
            if (data.ReaderTrampolineDelegate != null) return data.ReaderTrampolineDelegate;
            lock (_mModifyLock)
            {
                return GenerateReaderTrampoline(data.Type);
            }
        }

        ITypeSerializer GetTypeSerializer(Type type)
        {
            var serializer = _mUserTypeSerializers.FirstOrDefault(h => h.Handles(type)) ?? STypeSerializers.FirstOrDefault(h => h.Handles(type));
            if (serializer == null) throw new NotSupportedException(string.Format("No serializer for {0}", type.FullName));
            return serializer;
        }

        internal TypeData GetIndirectData(Type type)
        {
            TypeData data;
            if (!_mRuntimeTypeMap.TryGetValue(type, out data) || data.CanCallDirect == false) return _mRuntimeTypeMap[typeof(object)];
            return data;
        }

        internal MethodInfo GetDirectWriter(Type type)
        {
            return _mRuntimeTypeMap[type].WriterMethodInfo;
        }

        internal MethodInfo GetDirectReader(Type type)
        {
            return _mRuntimeTypeMap[type].ReaderMethodInfo;
        }

        IEnumerable<Type> Collect(Type rootType)
        {
            var l = new HashSet<Type>();
            var stack = new Stack<Type>();
            stack.Push(rootType);
            while (stack.Count > 0)
            {
                var type = stack.Pop();
                if (type.IsAbstract || type.IsInterface) continue;
                if (type.ContainsGenericParameters) throw new NotSupportedException(string.Format("Type {0} contains generic parameters", type.FullName));
                var serializer = _mRuntimeTypeMap[type].TypeSerializer;
                foreach (var t in serializer.GetSubtypes(type).Where(t => l.Contains(t) == false))
                {
                    stack.Push(t);
                }
                l.Add(type);
            }
            return l;
        }

        void GenerateWriterStub(Type type)
        {
            AssertLocked();
            var data = _mRuntimeTypeMap[type];
            var serializer = data.TypeSerializer;
            MethodInfo writer;
            if (serializer is IStaticTypeSerializer)
            {
                var sts = (IStaticTypeSerializer)serializer;
                writer = sts.GetStaticWriter(type);
                Debug.Assert(writer != null);
            }
            else if (serializer is IDynamicTypeSerializer)
            {
                // TODO: make it possible for dyn serializers to not have Serializer param
                writer = Helpers.GenerateDynamicSerializerStub(type);
            }
            else
            {
                throw new Exception();
            }
            data.WriterMethodInfo = writer;
        }

        void GenerateWriterBody(Type type)
        {
            AssertLocked();
            var data = _mRuntimeTypeMap[type];
            var serializer = data.TypeSerializer;
            var writer = data.WriterMethodInfo as DynamicMethod;
            if (writer == null) return;
            var dynSer = (IDynamicTypeSerializer)serializer;
            dynSer.GenerateWriterMethod(this, type, writer.GetILGenerator());
        }

        void GenerateWriters(Type rootType)
        {
            AssertLocked();
            if (_mRuntimeTypeMap[rootType].WriterMethodInfo != null) return;
            var types = Collect(rootType).Where(t => _mRuntimeTypeMap[t].WriterMethodInfo == null).ToList();
            foreach (var type in types) GenerateWriterStub(type);
            foreach (var type in types) GenerateWriterBody(type);
        }

        SerializeDelegate<object> GenerateWriterTrampoline(Type type)
        {
            AssertLocked();
            var data = _mRuntimeTypeMap[type];
            if (data.WriterTrampolineDelegate != null) return data.WriterTrampolineDelegate;
            GenerateWriters(type);
            data.WriterTrampolineDelegate = (SerializeDelegate<object>)Helpers.CreateSerializeDelegate(typeof(object), data);
            return data.WriterTrampolineDelegate;
        }

        Delegate GenerateDirectWriterDelegate(Type type)
        {
            AssertLocked();
            var data = _mRuntimeTypeMap[type];
            if (data.WriterDirectDelegate != null) return data.WriterDirectDelegate;
            GenerateWriters(type);
            data.WriterDirectDelegate = Helpers.CreateSerializeDelegate(type, data);
            return data.WriterDirectDelegate;
        }

        void GenerateReaderStub(Type type)
        {
            AssertLocked();
            var data = _mRuntimeTypeMap[type];
            var serializer = data.TypeSerializer;
            MethodInfo reader;
            if (serializer is IStaticTypeSerializer)
            {
                var sts = (IStaticTypeSerializer)serializer;
                reader = sts.GetStaticReader(type);
                Debug.Assert(reader != null);
            }
            else if (serializer is IDynamicTypeSerializer)
            {
                // TODO: make it possible for dyn serializers to not have Serializer param
                reader = Helpers.GenerateDynamicDeserializerStub(type);
            }
            else
            {
                throw new Exception();
            }
            data.ReaderMethodInfo = reader;
        }

        void GenerateReaderBody(Type type)
        {
            AssertLocked();
            var data = _mRuntimeTypeMap[type];
            var serializer = data.TypeSerializer;
            var reader = data.ReaderMethodInfo as DynamicMethod;
            if (reader == null) return;
            var dynSer = (IDynamicTypeSerializer)serializer;
            dynSer.GenerateReaderMethod(this, type, reader.GetILGenerator());
        }

        void GenerateReaders(Type rootType)
        {
            AssertLocked();
            if (_mRuntimeTypeMap[rootType].ReaderMethodInfo != null) return;
            var types = Collect(rootType).Where(t => _mRuntimeTypeMap[t].ReaderMethodInfo == null).ToList();
            foreach (var type in types) GenerateReaderStub(type);
            foreach (var type in types) GenerateReaderBody(type);
        }

        DeserializeDelegate<object> GenerateReaderTrampoline(Type type)
        {
            AssertLocked();
            var data = _mRuntimeTypeMap[type];
            if (data.ReaderTrampolineDelegate != null) return data.ReaderTrampolineDelegate;
            GenerateReaders(type);
            data.ReaderTrampolineDelegate = (DeserializeDelegate<object>)Helpers.CreateDeserializeDelegate(typeof(object), data);
            return data.ReaderTrampolineDelegate;
        }

        Delegate GenerateDirectReaderDelegate(Type type)
        {
            AssertLocked();
            var data = _mRuntimeTypeMap[type];
            if (data.ReaderDirectDelegate != null) return data.ReaderDirectDelegate;
            GenerateReaders(type);
            data.ReaderDirectDelegate = Helpers.CreateDeserializeDelegate(type, data);
            return data.ReaderDirectDelegate;
        }
        #if GENERATE_DEBUGGING_ASSEMBLY

        public static void GenerateDebugAssembly(IEnumerable<Type> rootTypes, ITypeSerializer[] userTypeSerializers)
        {
            new Serializer(rootTypes, userTypeSerializers, true);
        }

        Serializer(IEnumerable<Type> rootTypes, ITypeSerializer[] userTypeSerializers, bool debugAssembly)
        {
            if (userTypeSerializers.All(s => s is IDynamicTypeSerializer || s is IStaticTypeSerializer) == false) throw new ArgumentException("TypeSerializers have to implement IDynamicTypeSerializer or  IStaticTypeSerializer");
            _mUserTypeSerializers = userTypeSerializers;
            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("NetSerializerDebug"), AssemblyBuilderAccess.RunAndSave);
            var modb = ab.DefineDynamicModule("NetSerializerDebug.dll");
            var tb = modb.DefineType("NetSerializer", TypeAttributes.Public);
            _mRuntimeTypeMap = new TypeDictionary();
            _mRuntimeTypeIdList = new TypeIdList();
            lock (_mModifyLock)
            {
                var addedTypes = AddTypesInternal(new[] { typeof(object) }.Concat(rootTypes));
                /* generate stubs */
                foreach (var type in addedTypes.Keys) GenerateDebugStubs(type, tb);
                foreach (var type in addedTypes.Keys) GenerateDebugBodies(type);
            }
            tb.CreateType();
            ab.Save("NetSerializerDebug.dll");
        }

        void GenerateDebugStubs(Type type, TypeBuilder tb)
        {
            var data = _mRuntimeTypeMap[type];
            var serializer = data.TypeSerializer;
            MethodInfo writer;
            MethodInfo reader;
            bool writerNeedsInstance, readerNeedsInstance;
            if (serializer is IStaticTypeSerializer)
            {
                var sts = (IStaticTypeSerializer)serializer;
                writer = sts.GetStaticWriter(type);
                reader = sts.GetStaticReader(type);
                writerNeedsInstance = writer.GetParameters().Length == 3;
                readerNeedsInstance = reader.GetParameters().Length == 3;
            }
            else if (serializer is IDynamicTypeSerializer)
            {
                writer = Helpers.GenerateStaticSerializerStub(tb, type);
                reader = Helpers.GenerateStaticDeserializerStub(tb, type);
                writerNeedsInstance = readerNeedsInstance = true;
            }
            else
            {
                throw new Exception();
            }
            data.WriterMethodInfo = writer;
            data.WriterNeedsInstanceDebug = writerNeedsInstance;
            data.ReaderMethodInfo = reader;
            data.ReaderNeedsInstanceDebug = readerNeedsInstance;
        }

        void GenerateDebugBodies(Type type)
        {
            var data = _mRuntimeTypeMap[type];
            var serializer = data.TypeSerializer;
            var dynSer = serializer as IDynamicTypeSerializer;
            if (dynSer == null) return;
            var writer = data.WriterMethodInfo as MethodBuilder;
            if (writer == null) throw new Exception();
            var reader = data.ReaderMethodInfo as MethodBuilder;
            if (reader == null) throw new Exception();
            dynSer.GenerateWriterMethod(this, type, writer.GetILGenerator());
            dynSer.GenerateReaderMethod(this, type, reader.GetILGenerator());
        }
        #endif
    }
}
