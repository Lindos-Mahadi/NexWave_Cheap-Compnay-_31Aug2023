using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;

namespace comdeeds.dal
{
    public class SENDEMAILNOW
    {
        //ErrorLog oErrorLog = new ErrorLog();
        public void SendEmailRegistered(string subj, string Message, string clientemail)
        {
            try
            {
                string email = clientemail;
                //string email = ConfigurationManager.AppSettings["superemail"].ToString();
                Message = "<h1 style=' margin-left: 97px; margin-bottom: 25px; margin-top: 20px;'>Company <span style='color:windowtext'>R</span>egistered<u></u><u></u></h1><div style=' font-size: 13px; font-family:Calibri,sans-serif;width:70%;padding-bottom: 15px;border-bottom: solid #909090 2.25pt;border-top:solid #505050 4.5pt;padding-top: 30px;'>" + Message + "<br><img style='margin-left: -5px;height: 30px;' src='http://setup.comdeeds.com.au/Content/images/logo.jpg'></div><div style=''></div>";

                string CommpanyEmailID = ConfigurationManager.AppSettings["support"].ToString();
                string accounts_Pwd = ConfigurationManager.AppSettings["support_Pwd"].ToString();

                string Mailserver = ConfigurationManager.AppSettings["mailserver"].ToString();
                string Port = ConfigurationManager.AppSettings["Port"].ToString();
                string ssl = ConfigurationManager.AppSettings["ssl"].ToString();


                string toMailCC = ConfigurationManager.AppSettings["toMailCC"].ToString();
                string toMailBCC = ConfigurationManager.AppSettings["toMailBCC"].ToString();

                MailMessage Msg = new MailMessage();
                string fromeamil1 = CommpanyEmailID.ToString();
                Msg.From = new System.Net.Mail.MailAddress(fromeamil1);
                Msg.To.Add(new MailAddress(email));
                Msg.Bcc.Add(new MailAddress(toMailCC));
               // Msg.Bcc.Add(new MailAddress(toMailBCC));
                Msg.Subject = subj;// "Temporary Password";
                Msg.Body = Message.ToString();
                //Msg.Attachments.Add(new Attachment(path));
                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Mailserver;
                smtp.Port = Convert.ToInt32(Port);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(fromeamil1.ToString(), accounts_Pwd.ToString());
                if (ssl == "true")
                {
                    smtp.EnableSsl = true;
                }
                if (ssl == "false")
                {
                    smtp.EnableSsl = false;
                }

                smtp.Send(Msg);

            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        public void SendEmail_errorlog(string subj, string Message)
        {
            try
            {
                string email = ConfigurationManager.AppSettings["support"].ToString();
                Message = "<div style=' font-size: 16px; color: black; padding: 11px;'><b>" + Message + "</b><br><img style='margin-top: 25px;' src='http://dev.comdeeds.com.au/img/d2.jpg'></div><div style='float:left;width: 312px;color: gray;margin-left: 12px;font-size: 14px;'>ACN/ABN: 83 138 817 571<br>Australia<br><span style='text-decoration:underline;color:blue;'>info@comdeeds.com.au</span> <br><span style='text-decoration:underline;color:blue;'>Tel: 1300512388</span> </div>";

                string CommpanyEmailID = ConfigurationManager.AppSettings["info"].ToString();
                string accounts_Pwd = ConfigurationManager.AppSettings["info_Pwd"].ToString();

                string Mailserver = ConfigurationManager.AppSettings["mailserver"].ToString();
                string Port = ConfigurationManager.AppSettings["Port"].ToString();
                string ssl = ConfigurationManager.AppSettings["ssl"].ToString();


                MailMessage Msg = new MailMessage();
                string fromeamil1 = CommpanyEmailID.ToString();
                Msg.From = new System.Net.Mail.MailAddress(fromeamil1);
                Msg.To.Add(new MailAddress(email));
                Msg.Bcc.Add(new MailAddress("deepak.dubey@gmail.com"));
                //Msg.To.Add(new MailAddress("bsparihar.88@gmail.com"));
                Msg.Subject = subj;// "Temporary Password";
                Msg.Body = Message.ToString();
                //Msg.Attachments.Add(new Attachment(path));
                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Mailserver;
                smtp.Port = Convert.ToInt32(Port);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(fromeamil1.ToString(), accounts_Pwd.ToString());
                if (ssl == "true")
                {
                    smtp.EnableSsl = true;
                }
                if (ssl == "false")
                {
                    smtp.EnableSsl = false;
                }

                smtp.Send(Msg);

            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        public void SendEmail_Info(string subj, string Message)
        {
            try
            {
                string email = ConfigurationManager.AppSettings["superemail"].ToString();
                Message = "<div style=' font-size: 16px; color: black; padding: 11px;'>Dear User, <br /><b>" + Message + "</b><br><img style='margin-top: 25px;' src='http://198.38.94.99/custom/logoPayEmail.png'></div><div style='float:left;width: 312px;color: gray;margin-left: 12px;font-size: 14px;'>ACN/ABN: 85 927 028 979<br>21/37 Shedworth Street,<br>Marayong NSW 2148, <br>Australia<br><span style='text-decoration:underline;color:blue;'>info@onlydeeds.com.au</span> <br><span style='text-decoration:underline;color:blue;'>Tel: 1300512388</span> </div>";

                string CommpanyEmailID = ConfigurationManager.AppSettings["info"].ToString();
                string accounts_Pwd = ConfigurationManager.AppSettings["info_Pwd"].ToString();

                string Mailserver = ConfigurationManager.AppSettings["mailserver"].ToString();
                string Port = ConfigurationManager.AppSettings["Port"].ToString();
                string ssl = ConfigurationManager.AppSettings["ssl"].ToString();


                MailMessage Msg = new MailMessage();
                string fromeamil1 = CommpanyEmailID.ToString();
                Msg.From = new System.Net.Mail.MailAddress(fromeamil1);
                Msg.To.Add(new MailAddress(email));

                //Msg.To.Add(new MailAddress("bsparihar.88@gmail.com"));
                Msg.Subject = subj;// "Temporary Password";
                Msg.Body = Message.ToString();
                //Msg.Attachments.Add(new Attachment(path));
                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Mailserver;
                smtp.Port = Convert.ToInt32(Port);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(fromeamil1.ToString(), accounts_Pwd.ToString());
                if (ssl == "true")
                {
                    smtp.EnableSsl = true;
                }
                if (ssl == "false")
                {
                    smtp.EnableSsl = false;
                }

                smtp.Send(Msg);

            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString());
            }
        }
    }
}