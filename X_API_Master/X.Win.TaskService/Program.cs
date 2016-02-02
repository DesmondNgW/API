using System.ServiceProcess;

namespace X.Win.TaskService
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
                new TaskService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
