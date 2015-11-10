namespace X.Interface.Core
{
    public struct ErrorMessage
    {
        public const string FundCodeFormat = "基金代码应为6位字母数字组合";
        public const string UserNameFormat = "用户名应为8位以上字母数字组合";
        public const string RouteError = "用户名不存在或路有发生异常";
        public const string KeyError = "传输的秘钥不正确";
        public const string PasswordFormat = "密码应为8~20位非空字符";
    }
}
