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
using System.Configuration;
using System.Web.Configuration;
using comdeeds.App_Code;

namespace comdeeds
{
    public partial class ChangePassword_Asic : System.Web.UI.Page
    {
        // Certificate and key
        private static string s_certificateFileName = @"ComDeeds.pem";
        // EDGE test server
        private static string s_server = "edge1.asic.gov.au";
        private static string s_server1 = "";
        private static string s_server2 = "";
        //private static int s_port = 5608;
        //private static string s_server = "edge1.uat.asic.gov.au";
        private static int s_port = 5610;

        // EDGE test credentials
        private static int s_agentNumber = 40125;
        private static string s_userId = "S00190";
        //private static string s_userPassword = "S00190";
        //private static string s_newPassword = "S00190";
        private static string s_userPassword = "D00194";
        private static string s_newPassword = "D00194";
        private static bool s_validateServerCertificate = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {

                    var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> {"ADMIN", "SUBADMIN" }, "/admin/signin"));

                    s_userId = ConfigurationManager.AppSettings["userid"].ToString();
                    s_userPassword = ConfigurationManager.AppSettings["oldpass"].ToString();
                    s_newPassword = ConfigurationManager.AppSettings["newpass"].ToString();
                    s_server = ConfigurationManager.AppSettings["asicserver"].ToString();

                    s_server1 = ConfigurationManager.AppSettings["asicserver1"].ToString();
                    s_server2 = ConfigurationManager.AppSettings["asicserver2"].ToString();

                    s_agentNumber = Convert.ToInt32(ConfigurationManager.AppSettings["agentno"].ToString());
                    txtoldpass.Text = s_userPassword;
                    txtnewpass.Text = s_newPassword;
                }
                catch (Exception ex) { }
            }
        }

        protected void btnchange_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtoldpass.Text.Trim() != "" && txtnewpass.Text.Trim() != "")
                {
                    s_userPassword = txtoldpass.Text.Trim(); // old password
                    s_newPassword = txtnewpass.Text.Trim();  // new password

                    ChangePass(s_server1); // process of changes the password 
                }
            }
            catch (Exception ex) { }
        }

        protected void btnchange1_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtoldpass.Text.Trim() != "" && txtnewpass.Text.Trim() != "")
                {
                    s_userPassword = txtoldpass.Text.Trim(); // old password
                    s_newPassword = txtnewpass.Text.Trim();  // new password
  
                    ChangePass(s_server2); // process of changes the password 
                }
            }
            catch (Exception ex) { }
        }
    


        #region ASIC
        private void ChangePass(string s_server)
        {
            EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);

            edgeTransportLayer.Connect(s_server, s_port);

            lblmsg.Text += ("Connected to " + s_server);

            int attempt = 20;
            //for (int ii = 0; ii < attempt; ii++)
            //{


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
                    lblmsg.Text += ("LOGR next client state: {logr.NextClientStateDescription : Form-201 is calling...}");

                    Step2(logr, edgeTransportLayer);// commented by BSP, i dont want to change password.
                    //Step3ReadLastFiles(edgeTransportLayer);
                    ////Step4Form201(logr, edgeTransportLayer);// call FORM201 24-11-2016
                    //Step3ReadLastFiles(edgeTransportLayer);
                    //ii = 20;//means break the loop or break;
                }
            }
            else
            {
                lblmsg.Text += ("Unexpected reply to login: " + msg.RawMessage);
            }

            //}


            // logout
            edgeTransportLayer.Send(new LOUT().MessageToSend(), true);

            edgeTransportLayer.Disconnect();

            lblmsg.Text += ("Done");

            ////bhupi Console.ReadKey(true);
        }
        private void Step2(LOGR logr, EDGETransport edgeTransportLayer)
        {
            InboundMessage msgcp;

            // if necessary change the password
            if (logr.NextClientState == 5)      // comment out if you want to force password change
            {
                edgeTransportLayer.Send(new PSWD(s_userPassword, s_newPassword).MessageToSend(true), true);

                msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive());

                if (msgcp.Success)
                {
                    lblmsg.Text += ("Password change successful. New password: " + s_newPassword);

                    // code for set asic login password in web.config file 
                    Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");
                    webConfigApp.AppSettings.Settings["oldpass"].Value = s_newPassword;
                    webConfigApp.AppSettings.Settings["newpass"].Value = s_newPassword;
                    webConfigApp.Save();
                }

                else
                    lblmsg.Text += ("Password change failed. Error: " + msgcp.ErrorMessage);
            }

            if (!string.IsNullOrEmpty(logr.LastFileSent))
                Step3ReadLastFiles(edgeTransportLayer);

            // test self-signing the certificate - uncoment to see working
            //Step2ASelfSignCertificateRA53 (edgeTransportLayer);
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

                        lblmsg.Text += (sepr.Filename);
                    }
                }
                else
                    break;
            }
        }
        #endregion
    }
}