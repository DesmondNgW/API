

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 股票接口
    /// </summary>
    public interface IStockManager
    {
        /// <summary>
        /// 复盘
        /// </summary>
        /// <returns></returns>
        ApiResult<bool> Replay();

        /// <summary>
        /// 选股
        /// </summary>
        /// <param name="TopCount"></param>
        /// <returns></returns>
        ApiResult<bool> SelectStock(int TopCount);

        /// <summary>
        /// 保存题材数据 
        /// </summary>
        /// <returns></returns>
        ApiResult<bool> SaveDataBase();
    }
}
