using System;
using System.Diagnostics;
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
            try
            {
                Task.CreateThreads();
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("StartTask", e.Message, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            try
            {
                Task.AbortThreads();
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("StopTask", e.Message, EventLogEntryType.Error);
            }
        }
    }
}
