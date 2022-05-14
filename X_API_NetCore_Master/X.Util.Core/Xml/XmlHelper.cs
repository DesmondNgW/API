using Microsoft.Extensions.Caching.Memory;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using X.Util.Core.Cache;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.Util.Core.Xml
{
    public class XmlHelper
    {
        #region Load && Write && Read Xml
        private const string XmlPrefix = "X.Util.Core.Xml.XmlPrefix";
        private static XmlDocument GetXmlDoc(string filePath)
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(filePath);
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { filePath }), ex, LogDomain.Util);
            }
            return doc;
        }

        public static XmlDocument GetXmlDocCache(string filePath)
        {
            var key = XmlPrefix + filePath;
            var cache = LocalCache.Default.Get<XmlDocument>(key);
            if (cache != null) return cache;
            lock (CoreUtil.Getlocker(key))
            {
                cache = LocalCache.Default.Get<XmlDocument>(key);
                if (cache != null) return cache;
                cache = GetXmlDoc(filePath);
                LocalCache.Default.SlidingExpirationSet(key, cache, new TimeSpan(0, 1, 0), CacheItemPriority.Normal, filePath);
            }
            return cache;
        }

        public static void WriteXml<T>(T item, string path)
        {
            CoreUtil.CoderLocker(path, () =>
            {
                var serializer = new XmlSerializer(typeof(T));
                try
                {
                    var xw = XmlWriter.Create(path);
                    serializer.Serialize(xw, item);
                    xw.Close();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { item, path }), ex, LogDomain.Util);
                }
            });
        }

        public static StringBuilder WriteXml<T>(T item)
        {
            var ret = new StringBuilder();
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                var xw = XmlWriter.Create(ret);
                serializer.Serialize(xw, item);
                xw.Close();
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { item }), ex, LogDomain.Util);
            }
            return ret;
        }

        public static XmlDocument GetXmlDocument(object item)
        {
            var xml = new XmlDocument();
            var serializer = new XmlSerializer(item.GetType());
            var sw = new StringWriter(CultureInfo.InvariantCulture);
            serializer.Serialize(sw, item);
            xml.LoadXml(sw.ToString());
            sw.Close();
            return xml;
        }

        public static T ReadXmlCache<T>(string path)
        {
            if (!File.Exists(path)) return default;
            var key = XmlPrefix + path + typeof(T).FullName;
            var cache = LocalCache.Default.Get<T>(key);
            if (cache != null) return cache;
            lock (CoreUtil.Getlocker(key))
            {
                cache = LocalCache.Default.Get<T>(key);
                if (cache != null) return cache;
                var serializer = new XmlSerializer(typeof(T));
                var xr = XmlReader.Create(path, null);
                cache = (T)serializer.Deserialize(xr);
                LocalCache.Default.SlidingExpirationSet(key, cache, new TimeSpan(0, 1, 0), CacheItemPriority.Normal, path);
            }
            return cache;
        }
        #endregion

        #region String
        public static string GetXmlNodeValue(XmlNode node, string defaultValue)
        {
            var ret = defaultValue;
            if (node != null && !string.IsNullOrWhiteSpace(node.InnerText.Trim())) ret = node.InnerText.Trim();
            return ret;
        }

        public static string GetXmlNodeXml(XmlNode node, string defaultValue)
        {
            var ret = defaultValue;
            if (node != null && !string.IsNullOrWhiteSpace(node.InnerXml.Trim())) ret = node.InnerXml.Trim();
            return ret;
        }

        public static string GetXmlAttributeValue(XmlNode node, string attributeName, string defaultValue)
        {
            var ret = defaultValue;
            if (Equals(node, null)) return ret;
            var attribute = node.Attributes?[attributeName];
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Value.Trim())) ret = attribute.Value.Trim();
            return ret;
        }
        #endregion

        #region Boolean
        public static bool GetXmlNodeValue(XmlNode node, bool defaultValue)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2Boolean(defaultValue);
        }

        public static bool GetXmlAttributeValue(XmlNode node, string attributeName, bool defaultValue)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2Boolean(defaultValue);
        }
        #endregion

        #region Byte
        public static byte GetXmlNodeValue(XmlNode node, byte defaultValue)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2Byte(defaultValue);
        }

        public static byte GetXmlAttributeValue(XmlNode node, string attributeName, byte defaultValue)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2Byte(defaultValue);
        }
        #endregion

        #region Int16
        public static short GetXmlNodeValue(XmlNode node, short defaultValue)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2Int16(defaultValue);
        }

        public static short GetXmlAttributeValue(XmlNode node, string attributeName, short defaultValue)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2Int16(defaultValue);
        }
        #endregion

        #region Int32
        public static int GetXmlNodeValue(XmlNode node, int defaultValue)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2Int32(defaultValue);
        }

        public static int GetXmlAttributeValue(XmlNode node, string attributeName, int defaultValue)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2Int32(defaultValue);
        }
        #endregion

        #region Int64
        public static long GetXmlNodeValue(XmlNode node, long defaultValue)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2Int64(defaultValue);
        }

        public static long GetXmlAttributeValue(XmlNode node, string attributeName, long defaultValue)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2Int64(defaultValue);
        }
        #endregion

        #region Single
        public static float GetXmlNodeValue(XmlNode node, float defaultValue)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2Single(defaultValue);
        }

        public static float GetXmlAttributeValue(XmlNode node, string attributeName, float defaultValue)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2Single(defaultValue);
        }
        #endregion

        #region Double
        public static double GetXmlNodeValue(XmlNode node, double defaultValue)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2Double(defaultValue);
        }

        public static double GetXmlAttributeValue(XmlNode node, string attributeName, double defaultValue)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2Double(defaultValue);
        }
        #endregion

        #region Decimal
        public static decimal GetXmlNodeValue(XmlNode node, decimal defaultValue)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2Decimal(defaultValue);
        }

        public static decimal GetXmlAttributeValue(XmlNode node, string attributeName, decimal defaultValue)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2Decimal(defaultValue);
        }
        #endregion

        #region DateTime
        public static DateTime GetXmlNodeValue(XmlNode node, DateTime defaultValue, string format)
        {
            return GetXmlNodeValue(node, string.Empty).Convert2DateTime(format, defaultValue);
        }

        public static DateTime GetXmlAttributeValue(XmlNode node, string attributeName, DateTime defaultValue, string format)
        {
            return GetXmlAttributeValue(node, attributeName, string.Empty).Convert2DateTime(format, defaultValue);
        }
        #endregion
    }
}
