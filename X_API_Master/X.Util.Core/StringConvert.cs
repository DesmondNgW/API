using System;
using System.Collections.Generic;
using System.Text;

namespace X.Util.Core
{
    public class StringConvert
    {
        private const string Base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private const string ChangedAlphabet = "BCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-/A";
        private static IDictionary<char, char> _base64ToChange;
        private static IDictionary<char, char> _change2Base64;
        public static IDictionary<char, char> Base64ToChange
        {
            get
            {
                if (_base64ToChange != null) return _base64ToChange;
                _base64ToChange = new Dictionary<char, char>();
                for (var i = 0; i < Base64Alphabet.Length; i++) _base64ToChange[Base64Alphabet[i]] = ChangedAlphabet[i];
                return _base64ToChange;
            }
        }

        public static IDictionary<char, char> Change2Base64
        {
            get
            {
                if (_change2Base64 != null) return _change2Base64;
                _change2Base64 = new Dictionary<char, char>();
                for (var i = 0; i < Base64Alphabet.Length; i++) _change2Base64[ChangedAlphabet[i]] = Base64Alphabet[i];
                return _change2Base64;
            }
        }

        public static Random SysRandom
        {
            get { return new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)); }
        }

        //public static string Bytes2Hex(byte[] bytes)
        //{
        //    var hexString = new StringBuilder();
        //    if (Equals(bytes, null)) return hexString.ToString();
        //    foreach (var t in bytes) hexString.Append(t.ToString("x2"));
        //    return hexString.ToString();
        //}

        //public static byte[] Hex2Bytes(string src)
        //{
        //    var l = src.Length / 2;
        //    var ret = new byte[l];
        //    for (var i = 0; i < l; i++)
        //    {
        //        var str = src.Substring(i * 2, 2);
        //        ret[i] = Convert.ToByte(str, 16);
        //    }
        //    return ret;
        //}

        public static string JsEncode(string value)
        {
            var base64 = value.ToBase64();
            var sb = new StringBuilder();
            foreach (var t in base64) sb.Append(Base64ToChange.ContainsKey(t) ? Base64ToChange[t] : t);
            return sb.ToString();
        }

        public static string JsDecode(string base64)
        {
            var sb = new StringBuilder();
            foreach (var t in base64) sb.Append(Change2Base64.ContainsKey(t) ? Change2Base64[t] : t);
            return sb.ToString().FromBase64();
        }
    }
}
