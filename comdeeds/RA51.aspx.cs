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
    public partial class RA51 : System.Web.UI.Page
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
            catch (Exception ex) { }
        }

        protected void btnprint_Click(object sender, EventArgs e)
        {
            cou++;
            EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);

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
                    CertificateRA51(edgeTransportLayer);
                    lblmsg.Text += ("--------FINISH---------");
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
        private void CertificateRA51(EDGETransport edgeTransportLayer)
        {
            s_certificateFileName = @"ComDeeds.pem";
            s_certificateFileName = Server.MapPath(s_certificateFileName);
            InboundMessage msgcp;
            if (true)
            {
                //X509Certificate2 x509 = new X509Certificate2();
                string certificate = "";// x509.FromPemFile(s_certificateFileName);
                string textOut = "";
                textOut = gettext("RA51.txt").Replace("\r\n", "\n");

                CompanyDataRA51 rA51 = new CompanyDataRA51();
                rA51.CompanyName = "ROB SUPER TESTING 2 PTY LTD";
                rA51.acn = "600779217";
                rA51.RegistrationCertificatedeliveryoption = "PDF";
                //comdeeds.EDGE.OutboundMessages.RA51 datara51 = new comdeeds.EDGE.OutboundMessages.RA51(8, certificate, rA51, s_userId, x509);
                comdeeds.EDGE.OutboundMessages.RA51 datara51 = new comdeeds.EDGE.OutboundMessages.RA51(8, certificate, rA51, s_userId, null);
                textOut = datara51.MessageToSend();

                BCHN bchn = new BCHN("TESTING8", "TXT");
                String bchnreply = bchn.MessageToSend();

                //TXID txid = new TXID("", "99999", "1", 1, 40178, false, true, certificate, x509);
                TXID txid = new TXID("", "99999", "1", 1, 40178, false, true, certificate, null);
                String txidreply = txid.MessageToSend(true);

                String ToSign201_TXID = textOut + txidreply;
                //String Signature_ToSign201_TXID = Sign(ToSign201_TXID, x509);
                String All = bchnreply + ToSign201_TXID;// + dataZXI + Signature_ToSign201_TXID;

                edgeTransportLayer.Send(All, true);

                //edgeTransportLayer.Send(textOut, true);
                msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());
                lblmsg.Text += (msgcp.Success + " - " + msgcp.ErrorMessage);

                if (msgcp.Success)
                    Step3ReadLastFiles(edgeTransportLayer);

                //msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());
                //if (msgcp.Success)
                //    Step3ReadLastFiles(edgeTransportLayer);
                //msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());
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
    }
}