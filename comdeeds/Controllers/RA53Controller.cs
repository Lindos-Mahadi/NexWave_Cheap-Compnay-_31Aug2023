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
    public class RA53Controller : Controller
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
        private void Step2ASelfSignCertificateRA53(EDGETransport edgeTransportLayer)
        {
            try
            {
                s_certificateFileName = @"ComDeeds.pem";
                s_certificateFileName = Server.MapPath(s_certificateFileName);
                var flag = false;
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

                    // RA53 ra53 = new RA53(8, certificate, person, s_userId, x509);
                    RA53 ra53 = new RA53(8, certificate, person, s_pin, x509);
                    String ra53reply = ra53.MessageToSend() + "";
                    String ra53replySign = Sign(ra53reply, x509);
                    ra53reply += ra53.getCertificate();
                    ra53reply += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", x509.DistinguishedNameOpenSSLFormat(), s_serialidentifier);
                    ra53reply += ra53replySign;

                    TXID txid = new TXID((""), "99999", "1", 1, s_agentNumber, false, true, certificate, x509);
                    String txidreply = txid.MessageToSend();

                    String textOut = ra53reply + txidreply;
                    String textOutSign = Sign(textOut, x509);
                    textOut += ra53.getCertificate();

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

                    SaveRa53SentData(textOut);
                }
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        private void Step3ReadLastFiles(EDGETransport edgeTransportLayer)
        {
            try
            {
                edgeTransportLayer.Send(new REQO().MessageToSend(true), true);
                string msg = "";
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
                            msg = (sepr.Filename);
                        }
                    }
                    else
                        break;
                }
            }
            catch (Exception)
            {

                throw;
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

        private void SaveRa53SentData(string fileContent)
        {
            try
            {
                var path = App_Code.Helper.getdateMothPath(System.Web.HttpContext.Current, "/Logs/RA53/");
                var filePath = Server.MapPath("~" + path + "/Ra53Sent" + DateTime.Now.Second + "_" + DateTime.Now.Minute + ".txt");

                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!System.IO.File.Exists(filePath))
                {
                    FileStream fs = new FileStream(filePath, FileMode.CreateNew);
                    fs.Close();
                    fs.Dispose();
                }
                System.IO.File.WriteAllText(filePath, fileContent);
            }
            catch (Exception ex) { }
        }

        #endregion











        public ActionResult Index()
        {
            bool flag = false;
            string Errormsg = "";
            //lblmsg.Text = ("Starting RA53..||");
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
                    Errormsg = "Login not accepted";
                    flag = false;
                }
                else
                {
                    Step2ASelfSignCertificateRA53(edgeTransportLayer);
                    Step3ReadLastFiles(edgeTransportLayer);
                    //string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                    //lblmsg.Text = textme + "<br><hr>";
                    flag = true;
                }
            }
            else
            {
                flag = false;
                Errormsg = ("Unexpected reply to login: " + msg.RawMessage);
            }

            // logout
            edgeTransportLayer.Send(new LOUT().MessageToSend(), true);
            edgeTransportLayer.Disconnect();

            if(flag)
            {
                //return RedirectToAction("Index", "RA54");
                return View();
            }else
            {
                return RedirectToAction("SetupError", "Home");
            }
            
        }
    }
}