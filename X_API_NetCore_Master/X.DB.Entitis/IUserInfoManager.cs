using X.Util.Entities;

namespace X.DB.Entitis
{
    public interface IUserInfoManager
    {
        ResultInfo<bool> BindEmail(string CustomerNo);
        ResultInfo<bool> BindTelephone(string CustomerNo);
        ResultInfo<bool> Edit(string CustomerNo, UserInfo user);
        ResultInfo<bool> ForgetPassword(string AccountNo, string Telephone, string Email, string newPassword);
        ResultInfo<UserInfo> GetUserInfo(string CustomerNo);
        ResultInfo<UserInfo> GetUserInfoByAccountNo(string AccountNo);
        ResultInfo<UserInfo> Login(string accountNo, string password);
        ResultInfo<bool> Register(string AccountNo, string Password, string Telephone, string Email, string CustomerName);
        ResultInfo<bool> Register(UserInfo user);
        ResultInfo<bool> ResetPassword(string CustomerNo, string oldPassword, string newPassword);
    }
}
