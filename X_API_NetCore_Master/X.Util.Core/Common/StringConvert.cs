using System;
using System.Collections.Generic;
using System.Text;

namespace X.Util.Core.Common
{
    public class StringConvert
    {
        private const string Base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private const string ChangedAlphabet = "BCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-/A";
        private static IDictionary<char, char> _base64ToChange;
        private static IDictionary<char, char> _change2Base64;

        public static IDictionary<char, char> GetBase64ToChange()
        {
            if (_base64ToChange != null) return _base64ToChange;
            _base64ToChange = new Dictionary<char, char>();
            for (var i = 0; i < Base64Alphabet.Length; i++) _base64ToChange[Base64Alphabet[i]] = ChangedAlphabet[i];
            return _base64ToChange;
        }

        public static IDictionary<char, char> GetChange2Base64()
        {
            if (_change2Base64 != null) return _change2Base64;
            _change2Base64 = new Dictionary<char, char>();
            for (var i = 0; i < Base64Alphabet.Length; i++) _change2Base64[ChangedAlphabet[i]] = Base64Alphabet[i];
            return _change2Base64;
        }

        public static Random SysRandom => new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));

        public static string JsEncode(string value)
        {
            var base64 = value.ToBase64();
            var sb = new StringBuilder();
            foreach (var t in base64) sb.Append(GetBase64ToChange().ContainsKey(t) ? GetBase64ToChange()[t] : t);
            return sb.ToString();
        }

        public static string JsDecode(string base64)
        {
            var sb = new StringBuilder();
            foreach (var t in base64) sb.Append(GetChange2Base64().ContainsKey(t) ? GetChange2Base64()[t] : t);
            return sb.ToString().FromBase64();
        }
    }
}
