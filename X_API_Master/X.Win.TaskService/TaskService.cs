using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using X.Util.Core;

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
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("StartTask has Exception:{0}", e.ToJson()));
                //email.log or message.log
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
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("StopTask has Exception:{0}", e.ToJson()));
                //email.log or message.log
            }
        }
    }
}
