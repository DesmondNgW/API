using System;

namespace X.Util.Entities
{
    public class LogOptions<TResult>
    {
        public LogOptions(Func<TResult, bool> callSuccess)
        {
            NeedElapsed = true;
            NeedLogInfo = true;
            CallSuccess = callSuccess;
        }

        public LogOptions(Func<TResult, bool> callSuccess, bool needElapsed, bool needLogInfo)
        {
            NeedElapsed = needElapsed;
            NeedLogInfo = needLogInfo;
            CallSuccess = callSuccess;
        }

        public bool NeedElapsed { get; set; }
        
        public bool NeedLogInfo { get; set; }

        public Func<TResult, bool> CallSuccess { get; set; }
    }

    public class LogOptions
    {
        public LogOptions()
        {
            NeedElapsed = true;
            NeedLogInfo = true;
        }

        public LogOptions(bool needElapsed, bool needLogInfo)
        {
            NeedElapsed = needElapsed;
            NeedLogInfo = needLogInfo;
        }

        public bool NeedElapsed { get; set; }

        public bool NeedLogInfo { get; set; }
    }
}
