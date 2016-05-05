using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.Util.Entities
{
    public enum LogDomain
    {
        Util,
        ThirdParty,
        Core,
        CoreExtend,
        Cache,
        Business,
        Interface,
        Db,
        Ui
    }
    /// <summary>
    /// 日志监控模块
    /// </summary>
    public enum LogMonitorDomain
    {
        Trade,
        User,
        Query,
        Pay,
        Test,
        Other
    }
    public enum LogType
    {
        Error,
        Info,
        Debug
    }
}
