using System;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public sealed class InstanceProvider<T> : IProvider<T>
    {
        #region 构造函数
        public readonly Type Type = typeof(T);
        public readonly LogDomain EDomain = LogDomain.Business;
        public InstanceProvider() { }

        public InstanceProvider(LogDomain eDomain)
        {
            EDomain = eDomain;
        }

        public InstanceProvider(Type t)
        {
            Type = t;
        }

        public InstanceProvider(Type t, LogDomain eDomain)
        {
            Type = t;
            EDomain = eDomain;
        }
        #endregion

        #region 内部实现
        #endregion

        #region 对外公开属性和方法
        public string EndpointAddress
        {
            get { return Type.FullName; }
        }

        public LogDomain Domain
        {
            get { return EDomain; }
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
        #endregion
    }
}
