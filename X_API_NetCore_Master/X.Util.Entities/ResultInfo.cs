using System;

namespace X.Util.Entities
{
    /// <summary>
    /// 数据封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ResultInfo<T>
    {
        public ResultInfo()
        {
            Succeed = false;
        }
        public ResultInfo(bool Succeed)
        {
            this.Succeed = Succeed;
        }
        public T Result { get; set; }
        public bool Succeed { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
}
