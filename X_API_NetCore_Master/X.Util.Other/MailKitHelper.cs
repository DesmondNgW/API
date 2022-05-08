using MailKit.Net.Smtp;
using MimeKit;
using X.Util.Entities;
using System.Linq;

namespace X.Util.Other
{
    public class MailKitHelper
    {
        public const string MailKitTextPartPlain = "plain";
        public const string MailKitTextPartHtml = "html";
        public const string MailKitMultipart = "alternative";
        public const string MailKitMultipartMixed = "mixed";
        /// <summary>
        /// GetTextPart
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        private static TextPart GetTextPart(EnumMailKitTextPartType type, string Text)
        {
            return new TextPart(EnumMailKitTextPartType.Text == type ? MailKitTextPartPlain : MailKitTextPartHtml) { Text = Text };
        }

        /// <summary>
        /// GetMultipart
        /// </summary>
        /// <param name="subtype"></param>
        /// <param name="type"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        private static Multipart GetMultipart(string subtype, EnumMailKitTextPartType type, string Text)
        {
            return new Multipart(subtype)
            {
                GetTextPart(type, Text)
            };
        }

        /// <summary>
        /// SendMail
        /// </summary>
        /// <param name="MailKitModel"></param>
        public static void SendMail(MailKitModel MailKitModel)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(MailKitModel.SendName, MailKitModel.SendAccountName));
            var mailboxAddressList = (from item in MailKitModel.ReceiverAccountNameList select new MailboxAddress(item.Key, item.Value)).ToList();
            message.To.AddRange(mailboxAddressList);
            message.Subject = MailKitModel.MailSubject;
            var Type = !string.IsNullOrWhiteSpace(MailKitModel.SendHtml) ? EnumMailKitTextPartType.Html : EnumMailKitTextPartType.Text;
            var Text = Type == EnumMailKitTextPartType.Text ? MailKitModel.SendText : MailKitModel.SendHtml;
            var multipart = new Multipart(MailKitMultipartMixed)
            {
                GetMultipart(MailKitMultipart, Type, Text)
            };
            if (MailKitModel.AccessoryList != null)
            {
                foreach (var item in MailKitModel.AccessoryList)
                {
                    multipart.Add(item);
                }
            }
            message.Body = multipart;
            using (var client = new SmtpClient())
            {
                client.Connect(MailKitModel.SmtpHost, MailKitModel.SmtpPort, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(MailKitModel.SendAccountName, MailKitModel.AuthenticatePassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
