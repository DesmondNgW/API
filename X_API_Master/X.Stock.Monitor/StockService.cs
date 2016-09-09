﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using X.Util.Core;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.Stock.Monitor
{
    public partial class StockService : ServiceBase
    {
        public StockService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("StartTask", e.Message, EventLogEntryType.Error);
                Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Empty, string.Format("StartTask has Exception:{0}", e.ToJson()));
            }
        }

        protected override void OnStop()
        {
            try
            {
                
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("StopTask", e.Message, EventLogEntryType.Error);
                Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Empty, string.Format("StopTask has Exception:{0}", e.ToJson()));
            }
        }
    }
}
