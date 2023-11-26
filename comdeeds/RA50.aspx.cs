using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using comdeeds.EDGE;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.InboundMessages;
using comdeeds.EDGE.OutboundMessages;
using comdeeds.EDGE.Utils;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.IO;
using comdeeds.EDGE.DataEntities;
using System.Configuration;

namespace comdeeds
{
    public partial class RA50 : System.Web.UI.Page
    {
        #region Connection Parameters
        private static string s_certificateFileName = @"ComDeeds.pem";
        //private static string s_server = "edge1.asic.gov.au";
        //private static int s_port = 5608;
        private static string s_server = "edge1.uat.asic.gov.au";
        private static int s_port = 5610;
        private static int s_agentNumber = 40178;
        private static string s_userId = "S00196";
        private static string s_userPassword = "S00198";
        private static string s_newPassword = "S00198";
        private static bool s_validateServerCertificate = false;
        private static string s_serialidentifier = "1d2879d2595322234de0ef169fc5545e";
        #endregion
        private static int cou = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //dal.SENDEMAILNOW sm = new dal.SENDEMAILNOW();
                //string sm_ = ope.htmlEmailClient("Testing Pty Ltd");
                //sm.SendEmailRegistered("Company Registered", sm_, "bsparihar.88@gmail.com");

                s_userId = ConfigurationManager.AppSettings["userid"].ToString();
                s_userPassword = ConfigurationManager.AppSettings["oldpass"].ToString();
                s_newPassword = ConfigurationManager.AppSettings["newpass"].ToString();
                s_server = ConfigurationManager.AppSettings["asicserver"].ToString();
                s_agentNumber = Convert.ToInt32(ConfigurationManager.AppSettings["agentno"].ToString());
                s_serialidentifier = ConfigurationManager.AppSettings["s_serialidentifier"].ToString();
            }
            catch(Exception ex){}
            string price = "1,234.50";
            int actualPrice = Convert.ToInt32(Convert.ToDouble(price));
        }

        protected void btnRA50_Click(object sender, EventArgs e)
        {
            cou++;
            EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);

            //callformRA50(edgeTransportLayer);

            edgeTransportLayer.Connect(s_server, s_port);

            lblmsg.Text = cou + " : Connected to " + s_server + "</br>";

            // login
            edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true);

            InboundMessage msg = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());

            if (msg.MessageName == "SVER")
            {
                SVER sver = (SVER)msg;

                lblmsg.Text += (sver.ErrorMessage);
            }
            else if (msg.MessageName == "LOGR")
            {
                LOGR logr = (LOGR)msg;

                if (!logr.Success)
                    lblmsg.Text += ("Login not accepted");
                else
                {
                    callformRA50(edgeTransportLayer);
                    lblmsg.Text += ("LOGR next client state: {REQO Calling..}");
                    Step3ReadLastFiles(edgeTransportLayer);
                    lblmsg.Text = "Response : </br>";
                    string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                    lblmsg.Text += textme + "<br><hr>";
                }
            }
            else
            {
                lblmsg.Text += ("Unexpected reply to login: " + msg.RawMessage);
            }

            // logout
            edgeTransportLayer.Send(new LOUT().MessageToSend(), true);

            edgeTransportLayer.Disconnect();

            lblmsg.Text += ("Done");
        }

        protected void btnreq_Click(object sender, EventArgs e)
        {
            EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);

            edgeTransportLayer.Connect(s_server, s_port);

            lblmsg.Text += ("Connected to " + s_server);

            // login
            edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true);

            InboundMessage msg = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());

            if (msg.MessageName == "SVER")
            {
                SVER sver = (SVER)msg;

                lblmsg.Text += (sver.ErrorMessage);
            }
            else if (msg.MessageName == "LOGR")
            {
                LOGR logr = (LOGR)msg;

                if (!logr.Success)
                    lblmsg.Text += ("Login not accepted");
                else
                {
                    //Step2ASelfSignCertificateRA53(edgeTransportLayer);
                    lblmsg.Text += ("LOGR next client state: {REQO Calling..}");
                    Step3ReadLastFiles(edgeTransportLayer);
                    lblmsg.Text = "Response : </br>";
                    string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                    lblmsg.Text += textme + "<br><hr>";
                }
            }
            else
            {
                lblmsg.Text += ("Unexpected reply to login: " + msg.RawMessage);
            }

            // logout
            edgeTransportLayer.Send(new LOUT().MessageToSend(), true);

            edgeTransportLayer.Disconnect();

            lblmsg.Text += ("Done");
        }
        #region ASIC
        private void callformRA50(EDGETransport edgeTransportLayer)
        {
            s_certificateFileName = @"ComDeeds.pem";
            s_certificateFileName = Server.MapPath(s_certificateFileName);
            InboundMessage msgcp;
            if (true)
            {
                //X509Certificate2 x509 = new X509Certificate2();
                string certificate = "";// x509.FromPemFile(s_certificateFileName);
                string data = "";
                data = gettext("Only201.txt").Replace("\r\n", "\n");

                RegistrationRequest rq = new RegistrationRequest();
                rq.ProposedCompanyName = txtcompanyname.Text.Trim().ToUpper();
                rq.DocumentNumber = txtdocumentno.Text.Trim().ToUpper();
                rq.SignatoryName = "SMITH\tROBERT\tJOSEPH";
                rq.DateSigned = Convert.ToDateTime(txtdate.Text.Trim());
                rq.DeclarationInN126HasBeenAssentedTo = true;
                comdeeds.EDGE.OutboundMessages.RA50 datara50 = new comdeeds.EDGE.OutboundMessages.RA50(8, certificate, rq, s_userId, null);
                data = datara50.MessageToSend();

                BCHN bchn = new BCHN("TESTING8", "TXT");
                String bchnreply = bchn.MessageToSend();

                String f201reply = data;
                //String Signature_f201 = Sign(f201reply, x509);
                //String dataZXI = string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", x509.DistinguishedNameOpenSSLFormat(), "0d");
                //f201reply = data;// + dataZXI + Signature_f201;

                TXID txid = new TXID("", "99999", "1", 1, 40178, false, true, certificate, null);
                String txidreply = txid.MessageToSend(true);

                String ToSign201_TXID = f201reply + txidreply;
                String Signature_ToSign201_TXID = Sign(ToSign201_TXID, null);
                String All = bchnreply + ToSign201_TXID;// + dataZXI + Signature_ToSign201_TXID;

                edgeTransportLayer.Send(All, true);
                msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());

                lblmsg.Text += (msgcp.Success + " - " + msgcp.ErrorMessage);

                if (msgcp.Success)
                    Step3ReadLastFiles(edgeTransportLayer);
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
                    if (msgcp.MessageName.ToUpper() == "RA55")
                    {
                        //RA55 bout = (RA55)msgcp;
                    }
                    else if (msgcp.MessageName.ToUpper() == "BOUT")
                    {
                        BOUT bout = (BOUT)msgcp;

                        if (bout.NoPendingFiles)
                            break;

                        msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());
                        if (msgcp.Success)
                        {
                            SEPR sepr = (SEPR)msgcp;

                            lblmsg.Text += (sepr.Filename);
                        }
                    }
                }
                else
                    break;
            }
        }
        private string gettext(string filename)
        {
            string textme = System.IO.File.ReadAllText(@"C:/asicfiles/" + filename, System.Text.Encoding.UTF8).Replace("\r\n", "\n");
            return textme;
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
        #endregion


        private void settext(string fileContent)
        {
            File.WriteAllText("C:/asicfiles/Only201.txt", fileContent);
        }
    }
}