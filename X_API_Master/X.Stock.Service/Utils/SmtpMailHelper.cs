using System.Net.Mail;
using System.Text;

namespace X.Stock.Service.Utils
{
    public class SmtpMailHelper
    {
        private const string UserName = "wuxd711722";

        private const string Password = "wuxiaodong711";

        private const string Host = "smtp.126.com";

        private const string Domain = "126.com";

        public static void Send(string subject, string content)
        {
            try
            {
                var client = new SmtpClient
                {
                    Host = Host,
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(UserName, Password)
                };
                const string to = UserName + "@" + Domain;
                var message = new MailMessage {From = new MailAddress(to)};
                message.To.Add(to);
                message.Subject = subject;
                message.Body = content;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;
                message.Priority = MailPriority.High;
                message.IsBodyHtml = true;
                client.Send(message);
            }
            catch
            {
            }
        }
    }
}
