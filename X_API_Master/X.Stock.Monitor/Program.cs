using System.ServiceProcess;

namespace X.Stock.Monitor
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
            { 
                new StockService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
