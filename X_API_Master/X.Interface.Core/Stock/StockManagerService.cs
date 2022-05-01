using X.Business.Core.Stock;
using X.Business.Entities.Enums;
using X.Business.Helper.Stock;
using X.Interface.Dto;
using X.Interface.Dto.Interface;

namespace X.Interface.Core.Stock
{
    public class StockManagerService: IStockManager
    {
        /// <summary>
        /// 复盘
        /// </summary>
        /// <param name="AQS"></param>
        /// <param name="Wave"></param>
        /// <returns></returns>
        public ApiResult<bool> Replay()
        {
            var AQS = StockDealIO.GetMyStock(MyStockMode.AQS);
            var Wave = StockDealIO.GetMyStock(MyStockMode.Wave);
            StockDealIO.Deal(AQS, Wave);
            return new ApiResult<bool>
            {
                Data = true,
                Success = true
            };
        }

        /// <summary>
        /// 选股
        /// </summary>
        /// <param name="TopCount"></param>
        /// <returns></returns>
        public ApiResult<bool> SelectStock(int TopCount)
        {
            var first = StockDealIO.GetMyMonitorStock(MyStockType.First);
            var zt = StockDealIO.GetMyMonitorStock(MyStockType.ZT);
            var kernel = StockDealIO.GetMyMonitorStock(MyStockType.Kernel);
            var kernelH = StockDealIO.GetMyMonitorStock(MyStockType.KernelH);
            var kernelL = StockDealIO.GetMyMonitorStock(MyStockType.KernelL);
            var coreT = StockDealIO.GetMyMonitorStock(MyStockType.CoreT);
            var coreT2 = StockDealIO.GetMyMonitorStock(MyStockType.CoreT2);
            var coreT3 = StockDealIO.GetMyMonitorStock(MyStockType.CoreT3);
            var ldx = StockDealIO.GetMyMonitorStock(MyStockType.LDX);
            var ddx = StockDealIO.GetDDXList();
            var AQS = StockDealIO.GetMyStock(MyStockMode.AQS);
            var Wave = StockDealIO.GetMyStock(MyStockMode.Wave);
            var all = StockDealBase.Union(AQS, Wave);
            StockDealIO.FilterStock(first, zt, kernel, kernelH, kernelL, coreT, coreT2, coreT3, ldx, ddx, AQS, all, TopCount);
            return new ApiResult<bool>
            {
                Data = true,
                Success = true
            };
        }

        /// <summary>
        /// 保存题材数据 
        /// </summary>
        /// <param name="Bak"></param>
        /// <returns></returns>
        public ApiResult<bool> SaveDataBase()
        {
            var bak = StockDealIO.GetStockDes(StockDesType.Bak);
            StockDealIO.SaveDataBase(bak);
            return new ApiResult<bool>
            {
                Data = true,
                Success = true
            };
        }
    }
}
