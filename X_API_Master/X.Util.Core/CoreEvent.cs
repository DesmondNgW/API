using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using X.Util.Entities;

namespace X.Util.Core
{
    public class CoreEvent<T>
    {
        public static ConcurrentDictionary<int, ConcurrentDictionary<string, List<Action<T, Event<T>>>>> Events = new ConcurrentDictionary<int, ConcurrentDictionary<string, List<Action<T, Event<T>>>>>();

        public static void On(T obj, string name, Action<T, Event<T>> callBack)
        {
            var code = obj.GetHashCode();
            if (!Events.ContainsKey(code)) Events[code] = new ConcurrentDictionary<string, List<Action<T, Event<T>>>>();
            if (!Events[code].ContainsKey(name))
            {
                Events[code][name] = new List<Action<T, Event<T>>> {callBack};
            }
            else
            {
                Events[code][name].Add(callBack);
            }
        }

        public static void Off(T obj, string name = "", Action<T, Event<T>> callBack = null)
        {
            var code = obj.GetHashCode();
            if (string.IsNullOrEmpty(name))
            {
                ConcurrentDictionary<string, List<Action<T, Event<T>>>> c;
                Events.TryRemove(code, out c);
            }
            else if (callBack == null)
            {
                List<Action<T, Event<T>>> c;
                Events[code].TryRemove(name, out c);
            }
            else
            {
                Events[code][name].Remove(callBack);
            }
        }

        public static void Fire(T obj, string name)
        {
            var code = obj.GetHashCode();
            if (Events == null || !Events.ContainsKey(code) || !Events[code].ContainsKey(name)) return;
            var list = Events[code][name];
            if (list == null || list.Count <= 0) return;
            foreach (var act in list)
            {
                act(obj, new Event<T>
                {
                    HashCode = obj.GetHashCode(),
                    Target = obj,
                    Name = name
                });
            }
        }
    }
}
