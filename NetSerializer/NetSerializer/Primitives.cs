/*
 * Copyright 2015 Tomi Valkeinen
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace NetSerializer
{
    public static class Primitives
    {
        public static MethodInfo GetWritePrimitive(Type type)
        {
            return typeof(Primitives).GetMethod("WritePrimitive", BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null, new[] { typeof(Stream), type }, null);
        }

        public static MethodInfo GetReaderPrimitive(Type type)
        {
            return typeof(Primitives).GetMethod("ReadPrimitive", BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null, new[] { typeof(Stream), type.MakeByRefType() }, null);
        }

        static uint EncodeZigZag32(int n)
        {
            return (uint)((n << 1) ^ (n >> 31));
        }

        static ulong EncodeZigZag64(long n)
        {
            return (ulong)((n << 1) ^ (n >> 63));
        }

        static int DecodeZigZag32(uint n)
        {
            return (int)(n >> 1) ^ -(int)(n & 1);
        }

        static long DecodeZigZag64(ulong n)
        {
            return (long)(n >> 1) ^ -(long)(n & 1);
        }

        static uint ReadVarint32(Stream stream)
        {
            var result = 0;
            var offset = 0;
            for (; offset < 32; offset += 7)
            {
                var b = stream.ReadByte();
                if (b == -1) throw new EndOfStreamException();
                result |= (b & 0x7f) << offset;
                if ((b & 0x80) == 0) return (uint)result;
            }
            throw new InvalidDataException();
        }

        static void WriteVarint32(Stream stream, uint value)
        {
            for (; value >= 0x80u; value >>= 7) stream.WriteByte((byte)(value | 0x80u));
            stream.WriteByte((byte)value);
        }

        static ulong ReadVarint64(Stream stream)
        {
            long result = 0;
            var offset = 0;
            for (; offset < 64; offset += 7)
            {
                var b = stream.ReadByte();
                if (b == -1) throw new EndOfStreamException();
                result |= ((long)(b & 0x7f)) << offset;
                if ((b & 0x80) == 0) return (ulong)result;
            }
            throw new InvalidDataException();
        }

        static void WriteVarint64(Stream stream, ulong value)
        {
            for (; value >= 0x80u; value >>= 7) stream.WriteByte((byte)(value | 0x80u));
            stream.WriteByte((byte)value);
        }

        public static void WritePrimitive(Stream stream, bool value)
        {
            stream.WriteByte(value ? (byte)1 : (byte)0);
        }

        public static void ReadPrimitive(Stream stream, out bool value)
        {
            var b = stream.ReadByte();
            value = b != 0;
        }

        public static void WritePrimitive(Stream stream, byte value)
        {
            stream.WriteByte(value);
        }

        public static void ReadPrimitive(Stream stream, out byte value)
        {
            value = (byte)stream.ReadByte();
        }

        public static void WritePrimitive(Stream stream, sbyte value)
        {
            stream.WriteByte((byte)value);
        }

        public static void ReadPrimitive(Stream stream, out sbyte value)
        {
            value = (sbyte)stream.ReadByte();
        }

        public static void WritePrimitive(Stream stream, char value)
        {
            WriteVarint32(stream, value);
        }

        public static void ReadPrimitive(Stream stream, out char value)
        {
            value = (char)ReadVarint32(stream);
        }

        public static void WritePrimitive(Stream stream, ushort value)
        {
            WriteVarint32(stream, value);
        }

        public static void ReadPrimitive(Stream stream, out ushort value)
        {
            value = (ushort)ReadVarint32(stream);
        }

        public static void WritePrimitive(Stream stream, short value)
        {
            WriteVarint32(stream, EncodeZigZag32(value));
        }

        public static void ReadPrimitive(Stream stream, out short value)
        {
            value = (short)DecodeZigZag32(ReadVarint32(stream));
        }

        public static void WritePrimitive(Stream stream, uint value)
        {
            WriteVarint32(stream, value);
        }

        public static void ReadPrimitive(Stream stream, out uint value)
        {
            value = ReadVarint32(stream);
        }

        public static void WritePrimitive(Stream stream, int value)
        {
            WriteVarint32(stream, EncodeZigZag32(value));
        }

        public static void ReadPrimitive(Stream stream, out int value)
        {
            value = DecodeZigZag32(ReadVarint32(stream));
        }

        public static void WritePrimitive(Stream stream, ulong value)
        {
            WriteVarint64(stream, value);
        }

        public static void ReadPrimitive(Stream stream, out ulong value)
        {
            value = ReadVarint64(stream);
        }

        public static void WritePrimitive(Stream stream, long value)
        {
            WriteVarint64(stream, EncodeZigZag64(value));
        }

        public static void ReadPrimitive(Stream stream, out long value)
        {
            value = DecodeZigZag64(ReadVarint64(stream));
        }

        #if !NO_UNSAFE
        public static unsafe void WritePrimitive(Stream stream, float value)
        {
            var v = *(uint*)(&value);
            WriteVarint32(stream, v);
        }

        public static unsafe void ReadPrimitive(Stream stream, out float value)
        {
            var v = ReadVarint32(stream);
            value = *(float*)(&v);
        }

        public static unsafe void WritePrimitive(Stream stream, double value)
        {
            var v = *(ulong*)(&value);
            WriteVarint64(stream, v);
        }

        public static unsafe void ReadPrimitive(Stream stream, out double value)
        {
            var v = ReadVarint64(stream);
            value = *(double*)(&v);
        }

        #else
		public static void WritePrimitive(Stream stream, float value)
		{
			WritePrimitive(stream, (double)value);
		}

		public static void ReadPrimitive(Stream stream, out float value)
		{
			double v;
			ReadPrimitive(stream, out v);
			value = (float)v;
		}

		public static void WritePrimitive(Stream stream, double value)
		{
			ulong v = (ulong)BitConverter.DoubleToInt64Bits(value);
			WriteVarint64(stream, v);
		}

		public static void ReadPrimitive(Stream stream, out double value)
		{
			ulong v = ReadVarint64(stream);
			value = BitConverter.Int64BitsToDouble((long)v);
		}
        #endif

        public static void WritePrimitive(Stream stream, DateTime value)
        {
            var v = value.ToBinary();
            WritePrimitive(stream, v);
        }

        public static void ReadPrimitive(Stream stream, out DateTime value)
        {
            long v;
            ReadPrimitive(stream, out v);
            value = DateTime.FromBinary(v);
        }

        [ThreadStatic]
        static int[] _sDecimalBitsArray;

        public static void WritePrimitive(Stream stream, decimal value)
        {
            var bits = decimal.GetBits(value);
            ulong low = (uint)bits[0];
            var mid = ((ulong)(uint)bits[1]) << 32;
            var lowmid = low | mid;
            var high = (uint)bits[2];
            var scale = ((uint)bits[3] >> 15) & 0x01fe;
            var sign = ((uint)bits[3]) >> 31;
            var scaleSign = scale | sign;
            WritePrimitive(stream, lowmid);
            WritePrimitive(stream, high);
            WritePrimitive(stream, scaleSign);
        }

        public static void ReadPrimitive(Stream stream, out decimal value)
        {
            ulong lowmid;
            uint high, scaleSign;
            ReadPrimitive(stream, out lowmid);
            ReadPrimitive(stream, out high);
            ReadPrimitive(stream, out scaleSign);
            var scale = (int)((scaleSign & ~1) << 15);
            var sign = (int)((scaleSign & 1) << 31);
            var arr = _sDecimalBitsArray ?? (_sDecimalBitsArray = new int[4]);
            arr[0] = (int)lowmid;
            arr[1] = (int)(lowmid >> 32);
            arr[2] = (int)high;
            arr[3] = scale | sign;
            value = new decimal(arr);
        }

        #if NO_UNSAFE
		public static void WritePrimitive(Stream stream, string value)
		{
			if (value == null)
			{
				WritePrimitive(stream, (uint)0);
				return;
			}
			var encoding = new UTF8Encoding(false, true);
			int len = encoding.GetByteCount(value);
			WritePrimitive(stream, (uint)len + 1);
			var buf = new byte[len];
			encoding.GetBytes(value, 0, value.Length, buf, 0);
			stream.Write(buf, 0, len);
		}

		public static void ReadPrimitive(Stream stream, out string value)
		{
			uint len;
			ReadPrimitive(stream, out len);
			if (len == 0)
			{
				value = null;
				return;
			}
			else if (len == 1)
			{
				value = string.Empty;
				return;
			}
			len -= 1;
			var encoding = new UTF8Encoding(false, true);
			var buf = new byte[len];
			int l = 0;
			while (l < len)
			{
				int r = stream.Read(buf, l, (int)len - l);
				if (r == 0)
					throw new EndOfStreamException();
				l += r;
			}
			value = encoding.GetString(buf);
		}
        #else

        sealed class StringHelper
        {
            public StringHelper()
            {
                Encoding = new UTF8Encoding(false, true);
            }
            private const int Bytebufferlen = 256;
            public const int Charbufferlen = 128;
            Encoder _mEncoder;
            Decoder _mDecoder;
            byte[] _mByteBuffer;
            char[] _mCharBuffer;
            private UTF8Encoding Encoding { get; set; }
            public Encoder Encoder 
            { 
                get { return _mEncoder ?? (_mEncoder = Encoding.GetEncoder()); }
            }
            public Decoder Decoder 
            { 
                get { return _mDecoder ?? (_mDecoder = Encoding.GetDecoder()); }
            }
            public byte[] ByteBuffer 
            { 
                get { return _mByteBuffer ?? (_mByteBuffer = new byte[Bytebufferlen]); }
            }
            public char[] CharBuffer
            {
                get { return _mCharBuffer ?? (_mCharBuffer = new char[Charbufferlen]); }
            }
        }

        [ThreadStatic]
        static StringHelper _sStringHelper;

        public unsafe static void WritePrimitive(Stream stream, string value)
        {
            if (value == null)
            {
                WritePrimitive(stream, (uint)0);
                return;
            }
            if (value.Length == 0)
            {
                WritePrimitive(stream, (uint)1);
                return;
            }
            var helper = _sStringHelper;
            if (helper == null) _sStringHelper = helper = new StringHelper();
            var encoder = helper.Encoder;
            var buf = helper.ByteBuffer;
            var totalChars = value.Length;
            int totalBytes;
            fixed (char* ptr = value) totalBytes = encoder.GetByteCount(ptr, totalChars, true);
            WritePrimitive(stream, (uint)totalBytes + 1);
            WritePrimitive(stream, (uint)totalChars);
            var p = 0;
            var completed = false;
            while (completed == false)
            {
                int charsConverted;
                int bytesConverted;
                fixed (char* src = value)
                fixed (byte* dst = buf)
                {
                    encoder.Convert(src + p, totalChars - p, dst, buf.Length, true, out charsConverted, out bytesConverted, out completed);
                }
                stream.Write(buf, 0, bytesConverted);
                p += charsConverted;
            }
        }

        public static void ReadPrimitive(Stream stream, out string value)
        {
            uint totalBytes;
            ReadPrimitive(stream, out totalBytes);
            if (totalBytes == 0)
            {
                value = null;
                return;
            }
            if (totalBytes == 1)
            {
                value = string.Empty;
                return;
            }
            totalBytes -= 1;
            uint totalChars;
            ReadPrimitive(stream, out totalChars);
            var helper = _sStringHelper;
            if (helper == null) _sStringHelper = helper = new StringHelper();
            var decoder = helper.Decoder;
            var buf = helper.ByteBuffer;
            var chars = totalChars <= StringHelper.Charbufferlen ? helper.CharBuffer : new char[totalChars];
            var streamBytesLeft = (int)totalBytes;
            var cp = 0;
            while (streamBytesLeft > 0)
            {
                var bytesInBuffer = stream.Read(buf, 0, Math.Min(buf.Length, streamBytesLeft));
                if (bytesInBuffer == 0) throw new EndOfStreamException();
                streamBytesLeft -= bytesInBuffer;
                var flush = streamBytesLeft == 0;
                var completed = false;
                var p = 0;
                while (completed == false)
                {
                    int charsConverted;
                    int bytesConverted;
                    decoder.Convert(buf, p, bytesInBuffer - p, chars, cp, (int)totalChars - cp, flush, out bytesConverted, out charsConverted, out completed);
                    p += bytesConverted;
                    cp += charsConverted;
                }
            }
            value = new string(chars, 0, (int)totalChars);
        }
        #endif

        public static void WritePrimitive(Stream stream, byte[] value)
        {
            if (value == null)
            {
                WritePrimitive(stream, (uint)0);
                return;
            }
            WritePrimitive(stream, (uint)value.Length + 1);
            stream.Write(value, 0, value.Length);
        }

        static readonly byte[] SEmptyByteArray = new byte[0];

        public static void ReadPrimitive(Stream stream, out byte[] value)
        {
            uint len;
            ReadPrimitive(stream, out len);
            if (len == 0)
            {
                value = null;
                return;
            }
            if (len == 1)
            {
                value = SEmptyByteArray;
                return;
            }
            len -= 1;
            value = new byte[len];
            var l = 0;
            while (l < len)
            {
                var r = stream.Read(value, l, (int)len - l);
                if (r == 0) throw new EndOfStreamException();
                l += r;
            }
        }
    }
}
