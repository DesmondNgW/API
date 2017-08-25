using System.ServiceProcess;


namespace X.Cache.Service
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
                new CacheService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
