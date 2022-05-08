using MimeKit;
using System.Collections.Generic;

namespace X.Util.Entities
{
    public enum EnumMailKitTextPartType
    {
        Text,
        Html,
    }

    public class MailKitModel
    {
        /// <summary>
        /// 发送者名称
        /// </summary>
        public string SendName { get; set; }

        /// <summary>
        /// 发送者账号
        /// </summary>
        public string SendAccountName { get; set; }

        /// <summary>
        /// 发送者服务器地址
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        /// 服务器端口号
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// 发送者登录邮箱账号的客户端授权码
        /// </summary>
        public string AuthenticatePassword { get; set; }

        /// <summary>
        /// 接收者账号
        /// </summary>
        public Dictionary<string, string> ReceiverAccountNameList { get; set; }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// 文本html(与sendText参数互斥，传此值则 sendText传null)
        /// </summary>
        public string SendHtml { get; set; }

        /// <summary>
        /// 纯文本(与sendHtml参数互斥，传此值则 sendHtml传null)
        /// </summary>
        public string SendText { get; set; }

        /// <summary>
        /// 邮件的附件
        /// </summary>
        public List<MimePart> AccessoryList { get; set; }
    }
}
