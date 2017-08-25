using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Cache.Service
{
    public interface ICache
    {
        object Get(string key);

        bool Set(string key, object value, DateTime expire);

        bool Set(string key, object value, TimeSpan expire);

        bool Remove(string key);
    }
}
