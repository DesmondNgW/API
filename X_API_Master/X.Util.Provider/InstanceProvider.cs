using System;
using System.Diagnostics;
using System.Reflection;
using X.Util.Core;

namespace X.Util.Provider
{
    public sealed class InstanceProvider<T>
    {
        #region 构造函数
        public readonly Type Type = typeof(T);
        public InstanceProvider() { }

        public InstanceProvider(Type t)
        {
            Type = t;
        }
        #endregion

        #region 内部实现
        private readonly Stopwatch _sw = new Stopwatch();
        private static T _instance;
        #endregion

        #region 对外公开属性和方法
        /// <summary>
        /// Provider提供的实例
        /// </summary>
        public T Instance
        {
            get
            {
                _instance = Core<T>.Instance(() => (T)Activator.CreateInstance(Type));
                _sw.Start();
                return _instance;
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close(MethodBase method, LogDomain eDomain)
        {
            _sw.Stop();
            Core<T>.Close(method, _sw.ElapsedMilliseconds, eDomain);
            _sw.Reset();
        }
        #endregion
    }
}
