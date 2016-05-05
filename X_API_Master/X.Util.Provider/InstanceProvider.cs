using System;
using System.Diagnostics;
using System.Reflection;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public sealed class InstanceProvider<T> : IProvider<T>
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
        #endregion

        #region 对外公开属性和方法
        public string EndpointAddress
        {
            get { return Type.FullName; }
        }
        /// <summary>
        /// Provider提供的实例
        /// </summary>
        public T Client
        {
            get
            {
                return Core<T>.Instance(() => (T)Activator.CreateInstance(Type));
            }
        }

        public void StartElapsed()
        {
            _sw.Start();
        }

        public void LogElapsed(MethodBase method, LogDomain eDomain)
        {
            _sw.Stop();
            Core<T>.Close(method, _sw.ElapsedMilliseconds, eDomain);
            _sw.Reset();
        }
        #endregion
    }
}
