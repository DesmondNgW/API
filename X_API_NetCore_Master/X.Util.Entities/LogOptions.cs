using System;

namespace X.Util.Entities
{
    public class LogOptions<TResult>
    {
        public LogOptions(Func<TResult, bool> callSuccess)
        {
            if (callSuccess == null) callSuccess = result => true;
            NeedLogInfo = true;
            CallSuccess = callSuccess;
        }

        public LogOptions(Func<TResult, bool> callSuccess, bool needLogInfo)
        {
            if (callSuccess == null) callSuccess = result => true;
            NeedLogInfo = needLogInfo;
            CallSuccess = callSuccess;
        }

        public bool NeedLogInfo { get; set; }

        public Func<TResult, bool> CallSuccess { get; set; }
    }

    public class LogOptions
    {
        public LogOptions()
        {
            NeedLogInfo = true;
        }

        public LogOptions(bool needLogInfo)
        {
            NeedLogInfo = needLogInfo;
        }

        public bool NeedLogInfo { get; set; }
    }
}
