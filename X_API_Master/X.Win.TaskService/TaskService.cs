using System.ServiceProcess;

namespace X.Win.TaskService
{
    public partial class TaskService : ServiceBase
    {
        public TaskService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Task.CreateThreads();
        }

        protected override void OnStop()
        {
            Task.AbortThreads();
        }
    }
}
