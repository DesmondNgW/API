using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Extend.Mongo;

namespace X.Win.TaskService
{
    public class TaskLogModel : MongoBaseModel
    {
        public string Content { get; set; }
    }

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
                Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Empty, string.Format("StartTask has Exception:{0}", e.ToJson()));
                //email.log or message.log or db.log
                LogMonitor.Error(new TaskLogModel() { Content = string.Format("StartTask has Exception:{0}", e.ToJson()) }, LogMonitorDomain.Other);
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
                Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Empty, string.Format("StopTask has Exception:{0}", e.ToJson()));
                //email.log or message.log or db.log
                LogMonitor.Error(new TaskLogModel() { Content = string.Format("StopTask has Exception:{0}", e.ToJson()) }, LogMonitorDomain.Other);
            }
        }
    }
}
