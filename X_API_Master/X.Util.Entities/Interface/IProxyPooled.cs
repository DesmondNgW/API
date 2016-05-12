namespace X.Util.Entities.Interface
{
    public interface IProxyPooled<TChannel>
    {
        /// <summary>
        /// 连接池
        /// </summary>
        CoreChannelFactoryPool<TChannel> CoreFactoryPool { get; }

        /// <summary>
        /// 获取Client
        /// </summary>
        /// <returns></returns>
        TChannel GetClient();

        /// <summary>
        /// 放回连接池
        /// </summary>
        /// <param name="channel"></param>
        void ReleaseClient(TChannel channel);
    }
}
