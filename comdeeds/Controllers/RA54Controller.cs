using comdeeds.EDGE;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.InboundMessages;
using comdeeds.EDGE.OutboundMessages;
using comdeeds.EDGE.Utils;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace comdeeds.Controllers
{
    public class RA54Controller : Controller
    {

        // Certificate and key
        private static string s_certificateFileName = @"ComDeeds.pem";
        // EDGE test server
        //private static string s_server = "edge1.asic.gov.au";
        //private static int s_port = 5608;
        private static string s_server = "edge1.uat.asic.gov.au";
        private static int s_port = 5610;

        // EDGE test credentials
        private static int s_agentNumber = 40125;
        private static string s_userId = "S00190";
        private static string s_pin = "PB2B6N6B9D";
        //private static string s_userPassword = "S00190";
        //private static string s_newPassword = "S00190";
        private static string s_userPassword = "D00194";
        private static string s_newPassword = "D00194";
        private static bool s_validateServerCertificate = false;
        private static string s_serialidentifier = "1d2879d2595322234de0ef169fc5545e";


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            try
            {
                s_pin = ConfigurationManager.AppSettings["asic_pin"].ToString();
                s_userId = ConfigurationManager.AppSettings["userid"].ToString();
                s_userPassword = ConfigurationManager.AppSettings["oldpass"].ToString();
                s_newPassword = ConfigurationManager.AppSettings["newpass"].ToString();
                s_server = ConfigurationManager.AppSettings["asicserver"].ToString();
                s_agentNumber = Convert.ToInt32(ConfigurationManager.AppSettings["agentno"].ToString());
                s_serialidentifier = ConfigurationManager.AppSettings["s_serialidentifier"].ToString();
               // s_certificateFileName = ConfigurationManager.AppSettings["s_certificateFileName"].ToString();
                s_certificateFileName = @"ComDeeds.pem";
                s_certificateFileName = Server.MapPath(s_certificateFileName);
            }
            catch (Exception ex) { }

        }

        #region ASIC
        private void Step2ASelfSignCertificateRA54(EDGETransport edgeTransportLayer)
        {
            s_certificateFileName = @"ComDeeds.pem";
            s_certificateFileName = Server.MapPath(s_certificateFileName);
            InboundMessage msgcp;
            if (true)
            {
                X509Certificate2 x509 = new X509Certificate2();
                string certificate = x509.FromPemFile(s_certificateFileName);
                PersonName person = new PersonName();
                person.FamilyName = "Deeds";
                person.GivenName1 = "Com";

                BCHN bchn = new BCHN("TESTING8", "TXT");
                String bchnreply = bchn.MessageToSend();

                RA54 ra54 = new RA54(8, certificate, person, s_userId, x509);
                String ra53reply = ra54.MessageToSend() + "";
                String ra53replySign = Sign(ra53reply, x509);

                ra53reply += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", x509.DistinguishedNameOpenSSLFormat(), s_serialidentifier);
                ra53reply += ra53replySign;

                TXID txid = new TXID((""), "99999", "1", 1, s_agentNumber, false, true, certificate, x509);
                String txidreply = txid.MessageToSend();

                String textOut = ra53reply + txidreply;
                String textOutSign = Sign(textOut, x509);

                textOut += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", x509.DistinguishedNameOpenSSLFormat(), s_serialidentifier);
                textOut += textOutSign;
                textOut = bchnreply + textOut + "";
                //String textOut_txt = gettext("final.txt").Replace("\r\n","\n");
                //textOut = textOut_txt;
                edgeTransportLayer.Send(textOut, true);

                msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());

                //lblmsg.Text = (msgcp.Success + " - " + msgcp.ErrorMessage);

                if (msgcp.Success)
                    Step3ReadLastFiles(edgeTransportLayer);

                SaveRa54SentData(textOut);
            }
        }
        private void Step3ReadLastFiles(EDGETransport edgeTransportLayer)
        {
            edgeTransportLayer.Send(new REQO().MessageToSend(true), true);

            while (true)
            {
                InboundMessage msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());

                if (msgcp.Success)
                {
                    BOUT bout = (BOUT)msgcp;

                    if (bout.NoPendingFiles)
                        break;

                    msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());

                    if (msgcp.Success)
                    {
                        SEPR sepr = (SEPR)msgcp;
                        //lblmsg.Text = (sepr.Filename);
                    }
                }
                else
                    break;
            }
        }
        protected string Sign(string message, X509Certificate2 x509)
        {
            message = x509.Sign(message);
            string signature = "";

            for (int i = 0; i < 200; i++)
            {
                if (message.Length < 64)
                {
                    signature += string.Format("ZXS{0}\n", message);
                    break;
                }

                signature += string.Format("ZXS{0}\n", message.Substring(0, 64));
                message = message.Substring(64);
            }

            return signature;
        }

        private void SaveRa54SentData(string fileContent)
        {
            try
            {
                //string fileName = "C:/asicfiles/Logs/" + "Ra54Sent" + DateTime.Now.Second + "_" + DateTime.Now.Minute + ".txt";
                var path = App_Code.Helper.getdateMothPath(System.Web.HttpContext.Current, "/Logs/RA54/");
                var fileName = Server.MapPath("~" + path + "/Ra54Sent" + DateTime.Now.Second + "_" + DateTime.Now.Minute + ".txt");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!System.IO.File.Exists(fileName))
                {
                    FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                    fs.Close();
                    fs.Dispose();
                }
                System.IO.File.WriteAllText(fileName, fileContent);
            }
            catch (Exception ex) { }
        }


        #endregion


        public ActionResult Index()
        {
            bool flag = false;
            string Errormsg = "";
            EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);

            edgeTransportLayer.Connect(s_server, s_port);

            //lblmsg.Text = ("Connected to " + s_server);

            // login
            edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true);

            InboundMessage msg = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());

            if (msg.MessageName == "SVER")
            {
                SVER sver = (SVER)msg;
                Errormsg = (sver.ErrorMessage);
                flag = false;
            }
            else if (msg.MessageName == "LOGR")
            {
                LOGR logr = (LOGR)msg;

                if (!logr.Success)
                {
                    Errormsg = ("Login not accepted");
                    flag = false;
                }
                else
                {
                    Step2ASelfSignCertificateRA54(edgeTransportLayer);
                    //lblmsg.Text = ("LOGR next client state: {REQO Calling..}");
                    Step3ReadLastFiles(edgeTransportLayer);
                    //lblmsg.Text = "Response : </br>";
                    //string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                    //lblmsg.Text = textme + "<br><hr>";
                    flag = true;
                }
            }
            else
            {
                Errormsg = ("Unexpected reply to login: " + msg.RawMessage);
                flag = false;
            }

            // logout
            edgeTransportLayer.Send(new LOUT().MessageToSend(), true);
            edgeTransportLayer.Disconnect();

            if (flag)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SetupError", "Home");
            }            
        }
    }
}