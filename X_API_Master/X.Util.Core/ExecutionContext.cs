using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace X.Util.Core
{
    public class ExecutionContext<T>
    {
        public static T Current => (T)(CallContext.GetData(typeof(T).FullName) ?? Activator.CreateInstance<T>());

        public static T LogicalCurrent => (T)(CallContext.LogicalGetData(typeof(T).FullName) ?? Activator.CreateInstance<T>());

        public static T Init(T context)
        {
            CallContext.SetData(typeof(T).FullName, context);
            return context;
        }

        public static T LogicalInit(T context)
        {
            CallContext.LogicalSetData(typeof(T).FullName, context);
            return context;
        }

        public static T Update(string name, object value)
        {
            var context = Current;
            var props = typeof(T).GetProperties();
            foreach (var p in props.Where(p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            {
                p.SetValue(Current, value, null);
            }
            CallContext.SetData(typeof(T).FullName, context);
            return context;
        }

        public static T LogicalUpdate(string name, object value)
        {
            var context = LogicalCurrent;
            var props = typeof(T).GetProperties();
            foreach (var p in props.Where(p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            {
                p.SetValue(Current, value, null);
            }
            CallContext.LogicalSetData(typeof(T).FullName, context);
            return context;
        }

        public static T Update(Dictionary<string, object> dictionary)
        {
            var context = Current;
            var props = typeof(T).GetProperties();
            dictionary = new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
            foreach (var p in props.Where(p => dictionary.ContainsKey(p.Name)))
            {
                p.SetValue(Current, dictionary[p.Name], null);
            }
            CallContext.SetData(typeof(T).FullName, context);
            return context;
        }

        public static T LogicalUpdate(Dictionary<string, object> dictionary)
        {
            var context = LogicalCurrent;
            var props = typeof(T).GetProperties();
            dictionary = new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
            foreach (var p in props.Where(p => dictionary.ContainsKey(p.Name)))
            {
                p.SetValue(Current, dictionary[p.Name], null);
            }
            CallContext.LogicalSetData(typeof(T).FullName, context);
            return context;
        }
    }
}
