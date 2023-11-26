using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using static comdeeds.Models.BaseModel;

namespace comdeeds.App_Code
{
    public class EmailHelper
    {
        
        public static string SendSmtpMail33(Class_mailer para)
        {
            // This is email service from elasticemail.com 
            // Replace the code for SMTP email if you want
            var apikey = "a8442815-3256-4c37-81d9-239c00b748b2";
            NameValueCollection values = new NameValueCollection();
            values.Add("apikey", apikey);
            values.Add("from", !string.IsNullOrEmpty(para.fromEmail) ? para.fromEmail : "support@comdeeds.com.au");
            values.Add("fromName", para.fromName);
            values.Add("to", para.toMail);
            values.Add("subject", para.subject);
            values.Add("bodyText", "");
            values.Add("bodyHtml", para.HtmlBody);
            values.Add("isTransactional", "true");
            string address = "https://api.elasticemail.com/v2/email/send";
            string response = Send(address, values);
            return response;
        }

        private static string Send(string address, NameValueCollection values)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    byte[] apiResponse = client.UploadValues(address, values);
                    return Encoding.UTF8.GetString(apiResponse);

                }
                catch (Exception ex)
                {
                    return "Exception caught: " + ex.Message + "\n" + ex.StackTrace;
                }
            }
        }

        public static string SendSmtpMail1(Class_mailer para)
        {
            ErrorLog objerror = new ErrorLog();
            string Password = ""; string toMailBCC1 = "";
            string Mailserver = "";
            string Port = "";
            bool ssl = true;

            string toMailCC = ConfigurationManager.AppSettings["toMailCC"].ToString();
            string toMailBCC  = ConfigurationManager.AppSettings["toMailBCC"].ToString();
            toMailBCC = "teach.msp@gmail.com"; toMailBCC1 = "deepak.dubey@gmail.com";
             Mailserver = ConfigurationManager.AppSettings["Host"].ToString();
            Password = "info3737";
            Port = "587";
            MailMessage Msg = new MailMessage();
            string fromeamil1 = para.fromEmail.ToString();
            Msg.From = new System.Net.Mail.MailAddress(fromeamil1);
            Msg.To.Add(new MailAddress(para.toMail.ToString()));
            if (!string.IsNullOrEmpty(toMailBCC))
            {
                Msg.Bcc.Add(new MailAddress(toMailBCC));
            }
            if (!string.IsNullOrEmpty(toMailBCC1))
            {
                Msg.Bcc.Add(new MailAddress(toMailBCC1));
            }            
            Msg.Subject = para.subject;
            Msg.Body = para.HtmlBody;
            Msg.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = Mailserver;
            smtp.Port = Convert.ToInt32(Port);
            smtp.UseDefaultCredentials = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new System.Net.NetworkCredential(fromeamil1.ToString(), Password.ToString());
            smtp.Timeout = 600000;
            smtp.EnableSsl = ssl;
            try
            { smtp.Send(Msg);}
            catch (Exception ex) { objerror.WriteErrorLog("_EmailHelper.cs_Page_LineNo_87_" + ex.ToString()); };
            string responce = " " + Msg.Subject + "  send successfully";
            return responce;
        }

        public static string SendSmtpMail(Class_mailer para)
        {
            ErrorLog objerror = new ErrorLog();
                string Password = "";
                string Mailserver = "";
                string Port = "";
                bool ssl = true;

                para.fromEmail = ConfigurationManager.AppSettings["FromMail"].ToString();
                Password = ConfigurationManager.AppSettings["Password"].ToString();
                Mailserver = ConfigurationManager.AppSettings["Host"].ToString();


                string toMailCC = ConfigurationManager.AppSettings["toMailCC"].ToString();
                string toMailBCC = ConfigurationManager.AppSettings["toMailBCC"].ToString();
                Port = "587";
                MailMessage Msg = new MailMessage();
                string fromeamil1 = para.fromEmail.ToString();
                Msg.From = new System.Net.Mail.MailAddress(fromeamil1);
                Msg.To.Add(new MailAddress(para.toMail.ToString()));
                if (!string.IsNullOrEmpty(toMailCC))
                {
                    Msg.CC.Add(new MailAddress(toMailCC));
                }
                if (!string.IsNullOrEmpty(toMailBCC))
                {
                    Msg.Bcc.Add(new MailAddress(toMailBCC));
                }            
                Msg.Subject = para.subject;
                Msg.Body = para.HtmlBody;

                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Mailserver;
                smtp.Port = Convert.ToInt32(Port);
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(fromeamil1.ToString(), Password.ToString());
                smtp.Timeout = 600000;
                smtp.EnableSsl = ssl;
                try
                {
                    smtp.Send(Msg);
                }
                catch (Exception ex) { objerror.WriteErrorLog("_EmailHelper.cs_Page_LineNo_129_" + ex.ToString()); };
                string responce = " "+Msg.Subject+"  send successfully";
                return responce;
        }
    }
}