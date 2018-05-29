using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace DotNet.Framework.Common
{
    public class MailSender
    {

        public static void Send(string tomail, string bccmail, string subject, string body, params string[] files)
        {
            Send(SmtpConfig.Create().SmtpSetting.Sender, tomail, bccmail, subject, body, true, Encoding.Default, true, files);
        }


        public static void Send(string frommail, string tomail, string bccmail, string subject,
                        string body, bool isBodyHtml, Encoding encoding, bool isAuthentication, params string[] files)
        {
            Send(SmtpConfig.Create().SmtpSetting.Server, SmtpConfig.Create().SmtpSetting.UserName, SmtpConfig.Create().SmtpSetting.Password, frommail,
                tomail, "", bccmail, subject, body, isBodyHtml, encoding, isAuthentication, files);
        }



        public static void Send(string server, string username, string password, string frommail, string tomail, string ccmail, string bccmail, string subject,
                        string body, bool isBodyHtml, Encoding encoding, bool isAuthentication, params string[] files)
        {


            SmtpClient smtpClient = new SmtpClient(server);
            //MailAddress from = new MailAddress("ben@contoso.com", "Ben Miller");
            //MailAddress to = new MailAddress("jane@contoso.com", "Jane Clayton");
            MailMessage message = new MailMessage(frommail, tomail);

            if (bccmail.Length > 1)
            {
                string[] maillist = Common.Helper.StringHelper.GetStrArray(bccmail);
                foreach (string m in maillist)
                {
                    if (m.Trim() != "")
                    {
                        MailAddress bcc = new MailAddress(m.Trim());
                        message.Bcc.Add(bcc);
                    }
                }
            }
            if (ccmail.Length > 1)
            {
                string[] maillist = Common.Helper.StringHelper.GetStrArray(ccmail);
                foreach (string m in maillist)
                {
                    if (m.Trim() != "")
                    {
                        MailAddress cc = new MailAddress(m.Trim());
                        message.CC.Add(cc);
                    }
                }
            }
            message.IsBodyHtml = isBodyHtml;
            message.SubjectEncoding = encoding;
            message.BodyEncoding = encoding;

            message.Subject = subject;
            message.Body = body;

            message.Attachments.Clear();
            if (files != null && files.Length != 0)
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    Attachment attach = new Attachment(files[i]);
                    message.Attachments.Add(attach);
                }
            }

            if (isAuthentication == true)
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
            }
            smtpClient.Send(message);
            message.Attachments.Dispose();

        }

        public static void Send(string recipient, string subject, string body)
        {
            Send(SmtpConfig.Create().SmtpSetting.Sender, recipient, "", subject, body, true, Encoding.Default, true, null);
        }

        public static void Send(string Recipient, string Sender, string Subject, string Body)
        {
            Send(Sender, Recipient, "", Subject, Body, true, Encoding.UTF8, true, null);
        }

    }

    public class SmtpSetting
    {
        private string _server;

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        private bool _authentication;

        public bool Authentication
        {
            get { return _authentication; }
            set { _authentication = value; }
        }
        private string _username;

        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }
        private string _sender;

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }

    public class SmtpConfig
    {
        private static SmtpConfig _smtpConfig;
        private string ConfigFile
        {
            get
            {
                string configPath = ConfigurationManager.AppSettings["SmtpConfigPath"];
                if (string.IsNullOrEmpty(configPath) || configPath.Trim().Length == 0)
                {
                    configPath = HttpContext.Current.Request.MapPath("/Config/SmtpSetting.config");
                }
                else
                {
                    if (!Path.IsPathRooted(configPath))
                        configPath = HttpContext.Current.Request.MapPath(Path.Combine(configPath, "SmtpSetting.config"));
                    else
                        configPath = Path.Combine(configPath, "SmtpSetting.config");
                }
                return configPath;
            }
        }
        public SmtpSetting SmtpSetting
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(this.ConfigFile);
                SmtpSetting smtpSetting = new SmtpSetting();
                smtpSetting.Server = doc.DocumentElement.SelectSingleNode("Server").InnerText;
                smtpSetting.Authentication = Convert.ToBoolean(doc.DocumentElement.SelectSingleNode("Authentication").InnerText);
                smtpSetting.UserName = doc.DocumentElement.SelectSingleNode("User").InnerText;
                smtpSetting.Password = doc.DocumentElement.SelectSingleNode("Password").InnerText;
                smtpSetting.Sender = doc.DocumentElement.SelectSingleNode("Sender").InnerText;

                return smtpSetting;
            }
        }
        private SmtpConfig()
        {

        }
        public static SmtpConfig Create()
        {
            if (_smtpConfig == null)
            {
                _smtpConfig = new SmtpConfig();
            }
            return _smtpConfig;
        }
    }
}
