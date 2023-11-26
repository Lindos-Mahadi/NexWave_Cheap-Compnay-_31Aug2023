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
using System.Configuration;
using comdeeds;
using comdeeds.EDGE.DataEntities;
using System.Diagnostics;

namespace comdeeds
{
    public partial class Form201Web : System.Web.UI.Page
    {
        private ErrorLog oErrorLog = new ErrorLog();

        #region Connection Parameters

        private  string s_certificateFileName = @"ComDeeds.pem"; //production server
        //private string s_certificateFileName = @"ComDeedsTest.pem"; //testing server

        private string s_server = "";
        private int s_port = 0;
        private int s_agentNumber = 0;
        private string s_userId = "";
        private string s_userPassword = "";
        private string s_newPassword = "";
        private static bool s_validateServerCertificate = false;

         //private static string s_serialidentifier = "0d";// testing server
        private static string s_serialidentifier = "6fce562f1f7ede63f8bddf5c73e6b07f"; // production server

        private static string RreRequestTemp = "";
        private static string version = "";
        private static int cou = 0;
        private static string STAT = "";

        #endregion Connection Parameters

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string ss_port = ConfigurationManager.AppSettings["s_port"].ToString();
                s_port = Convert.ToInt32(ss_port);
                s_certificateFileName = ConfigurationManager.AppSettings["s_certificateFileName"].ToString();
                s_userId = ConfigurationManager.AppSettings["userid"].ToString();
                s_userPassword = ConfigurationManager.AppSettings["oldpass"].ToString();
                s_newPassword = ConfigurationManager.AppSettings["newpass"].ToString();
                s_server = ConfigurationManager.AppSettings["asicserver"].ToString();
                s_agentNumber = Convert.ToInt32(ConfigurationManager.AppSettings["agentno"].ToString());
                s_serialidentifier = ConfigurationManager.AppSettings["s_serialidentifier"].ToString();
                version = ConfigurationManager.AppSettings["version"].ToString();
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }

            if (!IsPostBack)
            {
                if (Request.QueryString.Get("CompanyID") != null && Request.QueryString.Get("Email") != null)
                {
                    hdncompanyid.Value = Request.QueryString.Get("CompanyID");
                    txtcompanyid.Text = Request.QueryString.Get("CompanyID");
                    txtemailid.Text = Request.QueryString.Get("Email");
                    RreRequestTemp = Request.QueryString.Get("RreRequestTemp");

                    #region setdata to Txt D307

                    string str201 = Prepared201().ToUpper();
                    settext(str201);

                    #endregion setdata to Txt D307

                    #region Asic process with User Module

                    if (Session["adminurl"] == null)
                    {
                        ReqReport_User();

                        string STAT = AsicStatus(txtcompanyid.Text.ToString());
                        if (STAT.ToUpper() != "DOCUMENTS ACCEPTED")
                        {
                            System.Threading.Thread.Sleep(500);
                            sendform201toasic_User(); //send Form201 txt data to ASIC server
                            ReqReport_User();
                        }
                    }

                    #endregion Asic process with User Module

                    #region Asic process with Admin & Subadmin Module

                    if (Session["adminurl"] != null)
                    {
                        if (RreRequestTemp != "" && RreRequestTemp != null)
                        {
                            ReqReport();  // Request for Accept/Decline report
                        }

                        string STAT = AsicStatus(txtcompanyid.Text.ToString());
                        if (STAT.ToUpper() != "DOCUMENTS ACCEPTED")
                        {
                            System.Threading.Thread.Sleep(500);
                            sendform201toasic(); //send Form201 txt data to ASIC server
                            ReqReport();
                        }
                    }

                    #endregion Asic process with Admin & Subadmin Module

                    insertLBLmsg();
                    STAT = AsicStatus(txtcompanyid.Text.ToString());     // check company document status here
                    if (STAT.ToUpper().Contains("REJECTED"))
                    {
                        Session["companyid"] = txtcompanyid.Text;
                        if (Request.QueryString.Get("authorised") != null)
                        {
                            Response.Redirect("AsicResponse.aspx?error=Your Form201 contain Errors, please remove the error and process the data again.", true);
                        }
                        else
                        {
                            string companyn = AsicCompanyNameById(txtcompanyid.Text.Trim());
                            string body = "<b>" + companyn.ToUpper() + "</b> of Customer-Id <b>" + txtemailid.Text.Trim() + "</b> is Rejected by ASIC, Please have a look on it and Retry it with the Correct Data.<br>";
                        }
                    }
                    else if (STAT.ToUpper().Contains("ACCEPTED") && AcnNo(txtcompanyid.Text).Trim() != "")
                    {
                        Response.Redirect("CreateForm201.aspx?Email=" + Request.QueryString.Get("Email") + "&companyid=" + Request.QueryString.Get("CompanyID"), false);
                    }
                    else
                    {
                        Session["companyid"] = txtcompanyid.Text;
                        if (Session["adminurl"] != null)
                        {
                            oErrorLog.WriteErrorLog(lblasicerr_.Text.ToString());
                            Response.Redirect("AsicResponse.aspx?error=Your request is under process to asic. Please try after sometime." + "&CompanyID=" + Request.QueryString.Get("CompanyID") + "&Email=" + Request.QueryString.Get("Email"), true);
                        }
                        else
                        {
                            string companyn = AsicCompanyNameById(txtcompanyid.Text.Trim());
                            string body = "<b>" + companyn.ToUpper() + "</b> of Customer-Id <b>" + txtemailid.Text.Trim() + "</b> is not Transmitted to ASIC, Please have a look on it and Transmit the Data again.<br>";
                            oErrorLog.WriteErrorLog(lblasicerr_.Text.ToString());
                        }
                    }
                    /**/
                    if (Session["adminurl"] == null)
                    {
                        EmailSend();
                        Response.Redirect("CreateForm201.aspx?Email=" + Request.QueryString.Get("Email") + "&companyid=" + Request.QueryString.Get("CompanyID"), false);
                    }
                }

                int companyId = 0;
                if (Session["201formId"] != null)
                {
                    companyId = Convert.ToInt32(Session["201formId"].ToString());
                }
            }
        }

        private void EmailSend()
        {
            try
            {
                string name = AsicCompanyNameonlyById(txtcompanyid.Text);
                string messsage = ope.htmlEmailClient(name);

                dal.SENDEMAILNOW ems = new dal.SENDEMAILNOW();
                ems.SendEmailRegistered("Company Registered", messsage, Request.QueryString.Get("Email"));
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        public void insertLBLmsg()
        {
            try
            {
                //lblmsg.Text = "ASIC//09.03.2018 VALIDATION REPORT NUMBER //VALID_SE.62 COMDEEDS//itm deeds 36194 PO BOX 878 NORTH SYDNEY NSW 2059 TRANS NO. 51994362 DATE/TIME: 09.03.2018 16:59:34 SENT 1 RECEIVED 1 ACCEPTED 1 DOCUMENTS ACCEPTED 001//201//5E5385359////K J SMSF PROPERTY PTY LTD//8 **E10 End of validation report** ";
                // txtcompanyid.Text = "4254";

                // string name = AsicCompanyNameonlyById(txtcompanyid.Text);
                // if (lblmsg.Text.ToUpper().Contains(name.ToUpper()))
                ope.insertLBLmsg(txtcompanyid.Text, lblmsg.Text);
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        private dal.Operation ope = new dal.Operation();

        public string AsicStatus(string cid)
        {
            return ope.AsicStatus(cid);
        }

        public string AsicDocumentNo(string cid)
        {
            return ope.DocumentNo(cid);
        }

        public string AcnNo(string cid)
        {
            return ope.AcnNo(cid);
        }

        public string AsicCompanyNameById(string cid)
        {
            return ope.AsicCompanyNameById(cid);
        }

        public string AsicCompanyNameonlyById(string cid)
        {
            return ope.AsicCompanyNameonlyById(cid);
        }

        public string Prepared201()
        {
            string message = "";
            try
            {
                lblmsg.Text += "";
                Form201Entity frm = new Form201Entity(txtemailid.Text.Trim(), txtcompanyid.Text.Trim());
                string str = string.Format("ZHDASC201\t1000\t{0}\n", "0004");
                string ZCO = frm.ZCO();
                if (ZCO.Contains("ERROR::"))
                {
                    lblmsg.Text = "ZCO" + ZCO;
                    return ZCO;
                }
                string ZRG_ZRP = frm.ZRG_ZRP();
                if (ZRG_ZRP.Contains("ERROR::"))
                {
                    lblmsg.Text = "ZRG_ZRP" + ZRG_ZRP;
                    return ZRG_ZRP;
                }
                string ZUH = frm.ZUH();
                if (ZUH.Contains("ERROR::"))
                {
                    lblmsg.Text = "ZUH" + ZUH;
                    return ZUH;
                }
                string ZSD_ZFN_ZOF = frm.ZSD_ZFN_ZOF();
                if (ZSD_ZFN_ZOF.Contains("ERROR::"))
                {
                    lblmsg.Text = "ZSD_ZFN_ZOF" + ZSD_ZFN_ZOF;
                    return ZSD_ZFN_ZOF;
                }
                string ZSC = frm.ZSC();
                if (ZSC.Contains("ERROR::"))
                {
                    lblmsg.Text = "ZSC" + ZSC;
                    return ZSC;
                }
                string ZNS = frm.ZNS();
                if (ZNS.Contains("ERROR::"))
                {
                    lblmsg.Text = "ZNS" + ZNS;
                    return ZNS;
                }
                string ZHH_ZSH = frm.ZHH_ZSH();
                if (ZHH_ZSH.Contains("ERROR::"))
                {
                    lblmsg.Text = "ZHH_ZSH" + ZHH_ZSH;
                    return ZHH_ZSH;
                }
                string ZCG_ZDC_ZAM = frm.ZCG_ZDC_ZAM();
                if (ZCG_ZDC_ZAM.Contains("ERROR::"))
                {
                    lblmsg.Text = "ZCG_ZDC_ZAM" + ZCG_ZDC_ZAM;
                    return ZCG_ZDC_ZAM;
                }

                str += ZCO + ZRG_ZRP + ZUH + ZSD_ZFN_ZOF + ZSC + ZNS + ZHH_ZSH + ZCG_ZDC_ZAM;
                str += string.Format("ZTREND201\t{0}\n", str.Count(c => c == '\n') + 1);
                message = str;
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return message;
        }

        protected void btnform201_Click(object sender, EventArgs e)
        {
            sendform201toasic();
        }

        private void sendform201toasic()
        {
            try
            {
                if (Session["serverEdge"] != null) // based on selection of admin user
                {
                    s_server = Session["serverEdge"].ToString();
                }

                cou++;
                EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);

                Stopwatch chk = new Stopwatch();

                chk.Start();

                System.Threading.Thread.Sleep(500);
                connect:

                if (!edgeTransportLayer.Connect(s_server, s_port)) // connection process with ASIC server
                {
                    if (chk.Elapsed < TimeSpan.FromSeconds(120))
                        goto connect;
                }

                lblasicerr_.Text += "::creden " + s_userId + " , " + s_userPassword + " :: ";
                chk.Stop();
                chk.Start();
                send:
                edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true); // login process here

                System.Threading.Thread.Sleep(500);
                InboundMessage msg = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), hdncompanyid.Value);

                if (msg.MessageName == "SVER") //	Error detected - send message and close connection.
                {
                    SVER sver = (SVER)msg;

                    lblmsg.Text += (sver.ErrorMessage);
                    lblasicerr_.Text += "trace 2 " + sver.ErrorMessage + "::";

                    if (chk.Elapsed < TimeSpan.FromSeconds(120))
                        goto send;
                }
                else if (msg.MessageName == "LOGR")  //	Login response indicating success or failure of login request
                {
                    LOGR logr = (LOGR)msg;

                    if (!logr.Success)
                    {
                        lblmsg.Text += ("unable to connect with Asic server.");
                        lblasicerr_.Text += "trace 3 not login::";

                        if (chk.Elapsed < TimeSpan.FromSeconds(120))
                            goto send;
                    }
                    else
                    {
                        lblasicerr_.Text += "trace 4 login::";
                        callform201(edgeTransportLayer);  // call form201 data and send to asic server
                        lblmsg.Text += ("LOGR next client state: {REQO Calling..}");
                        Step3ReadLastFiles(edgeTransportLayer);
                        lblmsg.Text += "Response : </br>";
                        string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                        lblmsg.Text += textme + "<br><hr>";
                    }
                }
                else
                {
                    if (chk.Elapsed < TimeSpan.FromSeconds(60))
                        goto send;

                    // lblmsg.Text += ("Unexpected reply to login: " + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                    if (!(msg.RawMessage.Contains("empty response")))
                    {
                        if (!(msg.RawMessage == ""))
                        {
                            lblmsg.Text += ("ASIC Responce message - " + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                        }
                    }
                    chk.Stop();

                    insertLBLmsg();

                    if (Session["adminurl"] == null)
                    {
                        EmailSend();
                        Response.Redirect("CreateForm201.aspx?Email=" + Request.QueryString.Get("Email") + "&companyid=" + Request.QueryString.Get("CompanyID"), false);
                    }
                    else
                    {
                        Response.Redirect("AsicResponse.aspx?error=Asic could not be connected right now. Please try after 5 minute ." + "&CompanyID=" + Request.QueryString.Get("CompanyID") + "&Email=" + Request.QueryString.Get("Email"), false);
                    }
                }

                // logout process here
                edgeTransportLayer.Send(new LOUT().MessageToSend(), true);

                edgeTransportLayer.Disconnect();

                // lblmsg.Text += ("Done");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        protected void btnreq_Click(object sender, EventArgs e)
        {
            ReqReport();
        }

        private int alreadylogin = 0;

        private void ReqReport()
        {
            try
            {
                if (Session["serverEdge"] != null)
                {
                    s_server = Session["serverEdge"].ToString();
                }

                EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);
                Stopwatch chk = new Stopwatch();

                chk.Start();
                connect:
                System.Threading.Thread.Sleep(500);
                if (!edgeTransportLayer.Connect(s_server, s_port))
                {
                    if (chk.Elapsed < TimeSpan.FromSeconds(120))
                        goto connect;
                }
                chk.Stop();
                chk.Start();
                send:

                edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true);   // login process start from here
                System.Threading.Thread.Sleep(500);
                InboundMessage msg = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), hdncompanyid.Value);

                if (msg.MessageName == "SVER")
                {
                    SVER sver = (SVER)msg;

                    lblmsg.Text += (sver.ErrorMessage);

                    if (chk.Elapsed < TimeSpan.FromSeconds(120))
                        goto send;
                }
                else if (msg.MessageName == "LOGR")
                {
                    LOGR logr = (LOGR)msg;

                    if (!logr.Success)
                    {
                        lblmsg.Text += ("unable to connect with Asic server.");
                        if (chk.Elapsed < TimeSpan.FromSeconds(120))
                            goto send;
                    }
                    else
                    {
                        alreadylogin = 1;
                        //Step2ASelfSignCertificateRA53(edgeTransportLayer);
                        oErrorLog.WriteErrorLog("LOGR next client state: {REQO Calling..}");
                        Step3ReadLastFiles(edgeTransportLayer);
                        oErrorLog.WriteErrorLog("Response");

                        string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                        lblmsg.Text += textme + "<br><hr>";
                    }
                }
                else
                {
                    if (chk.Elapsed < TimeSpan.FromSeconds(60))
                        goto send;
                    if (!(msg.RawMessage.Contains("empty response")))
                    {
                        if (!(msg.RawMessage == ""))
                        {
                            lblmsg.Text += ("ASIC Responce message - " + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                        }
                    }
                    oErrorLog.WriteErrorLog("Unexpected reply to login:" + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                    chk.Stop();
                    insertLBLmsg();

                    if (Session["adminurl"] == null)
                    {
                        EmailSend();
                        Response.Redirect("CreateForm201.aspx?Email=" + Request.QueryString.Get("Email") + "&companyid=" + Request.QueryString.Get("CompanyID"), false);
                    }
                    else
                    {
                        Response.Redirect("AsicResponse.aspx?error=Asic could not be connected right now. Please try after 5 minute ." + "&CompanyID=" + Request.QueryString.Get("CompanyID") + "&Email=" + Request.QueryString.Get("Email"), true);
                    }
                }
                chk.Stop();
                edgeTransportLayer.Send(new LOUT().MessageToSend(), true);  // logout or stop the process here
                edgeTransportLayer.Disconnect();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void ReqReport_User()
        {
            try
            {
                if (Session["serverEdge"] != null)
                {
                    s_server = Session["serverEdge"].ToString();
                }

                EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);

                System.Threading.Thread.Sleep(500);
                if (!edgeTransportLayer.Connect(s_server, s_port))
                {
                }

                edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true);   // login process start from here
                System.Threading.Thread.Sleep(500);
                InboundMessage msg = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), hdncompanyid.Value);

                if (msg.MessageName == "SVER")
                {
                    SVER sver = (SVER)msg;

                    lblmsg.Text += (sver.ErrorMessage); ;
                }
                else if (msg.MessageName == "LOGR")
                {
                    LOGR logr = (LOGR)msg;

                    if (!logr.Success)
                    {
                        lblmsg.Text += ("unable to connect with Asic server.");
                    }
                    else
                    {
                        alreadylogin = 1;
                        //Step2ASelfSignCertificateRA53(edgeTransportLayer);
                        oErrorLog.WriteErrorLog("LOGR next client state: {REQO Calling..}");
                        Step3ReadLastFiles(edgeTransportLayer);
                        oErrorLog.WriteErrorLog("Response");

                        string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                        lblmsg.Text += textme + "<br><hr>";
                    }
                }
                else
                {
                    if (!(msg.RawMessage.Contains("empty response")))
                    {
                        if (!(msg.RawMessage == ""))
                        {
                            lblmsg.Text += ("ASIC Responce message - " + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                        }
                    }
                    oErrorLog.WriteErrorLog("Unexpected reply to login:" + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                    insertLBLmsg();

                    if (Session["adminurl"] == null)
                    {
                        EmailSend();
                        Response.Redirect("CreateForm201.aspx?Email=" + Request.QueryString.Get("Email") + "&companyid=" + Request.QueryString.Get("CompanyID"), false);
                    }
                    else
                    {
                        Response.Redirect("AsicResponse.aspx?error=Asic could not be connected right now. Please try after 5 minute ." + "&CompanyID=" + Request.QueryString.Get("CompanyID") + "&Email=" + Request.QueryString.Get("Email"), true);
                    }
                }
                edgeTransportLayer.Send(new LOUT().MessageToSend(), true);  // logout or stop the process here
                edgeTransportLayer.Disconnect();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void sendform201toasic_User()
        {
            try
            {
                if (Session["serverEdge"] != null) // based on selection of admin user
                {
                    s_server = Session["serverEdge"].ToString();
                }
                cou++;
                EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);

                if (!edgeTransportLayer.Connect(s_server, s_port)) // connection process with ASIC server
                {
                }

                System.Threading.Thread.Sleep(500);
                lblasicerr_.Text += "::creden " + s_userId + " , " + s_userPassword + " :: ";
                edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true); // login process here

                System.Threading.Thread.Sleep(500);
                InboundMessage msg = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), hdncompanyid.Value);

                if (msg.MessageName == "SVER") //	Error detected - send message and close connection.
                {
                    SVER sver = (SVER)msg;
                    lblmsg.Text += (sver.ErrorMessage);
                    lblasicerr_.Text += "trace 2 " + sver.ErrorMessage + "::";
                }
                else if (msg.MessageName == "LOGR")  //	Login response indicating success or failure of login request
                {
                    LOGR logr = (LOGR)msg;

                    if (!logr.Success)
                    {
                        lblmsg.Text += ("unable to connect with Asic server.");
                        oErrorLog.WriteErrorLog("Login not accepted");
                        lblasicerr_.Text += "trace 3 not login::";
                    }
                    else
                    {
                        lblasicerr_.Text += "trace 4 login::";
                        callform201(edgeTransportLayer);  // call form201 data and send to asic server
                        lblmsg.Text += ("LOGR next client state: {REQO Calling..}");
                        Step3ReadLastFiles(edgeTransportLayer);
                        lblmsg.Text += "Response : </br>";
                        string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                        lblmsg.Text += textme + "<br><hr>";
                    }
                }
                else
                {
                    //   lblmsg.Text += ("Unexpected reply to login: " + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                    if (!(msg.RawMessage.Contains("empty response")))
                    {
                        if (!(msg.RawMessage == ""))
                        {
                            lblmsg.Text += ("ASIC Responce message - " + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                        }
                    }
                    insertLBLmsg();

                    if (Session["adminurl"] == null)
                    {
                        EmailSend();
                        Response.Redirect("CreateForm201.aspx?Email=" + Request.QueryString.Get("Email") + "&companyid=" + Request.QueryString.Get("CompanyID"), false);
                    }
                    else
                    {
                        Response.Redirect("AsicResponse.aspx?error=Asic could not be connected right now. Please try after 5 minute ." + "&CompanyID=" + Request.QueryString.Get("CompanyID") + "&Email=" + Request.QueryString.Get("Email"), true);
                    }
                }
                // logout process here
                edgeTransportLayer.Send(new LOUT().MessageToSend(), true);
                edgeTransportLayer.Disconnect();
                // lblmsg.Text += ("Done");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        #region ASIC

        private void callform201(EDGETransport edgeTransportLayer)
        {
            try
            {
                // s_certificateFileName = @"Comdeeds.pem"; //production certificate
                // s_certificateFileName = @"OnlyDeeds.pem";  //testing certificate
                //s_certificateFileName = @"ComdeedsTest.pem";  //testing certificate
                s_certificateFileName = Server.MapPath(s_certificateFileName);

                InboundMessage msgcp;
                if (true)
                {
                    X509Certificate2 x509 = new X509Certificate2();
                    string certificate =  x509.FromPemFile(s_certificateFileName);
                    string data = "";
                    string finename = txtcompanyid.Text + "_Only201.txt";
                    data = gettext(finename).Replace("\r\n", "\n");

                    BCHN bchn = new BCHN("TESTING8", "TXT");
                    String bchnreply = bchn.MessageToSend();

                    String f201reply = data;
                    //String Signature_f201 = Sign(f201reply, x509);

                    //String dataZXI = string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", x509.DistinguishedNameOpenSSLFormat(), s_serialidentifier);
                    //String dataZXI = string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", x509.DistinguishedNameOpenSSLFormat(), x509.SerialNumber);
                    f201reply = data;// + dataZXI;// + Signature_f201;

                    // TXID txid = new TXID("", "2083", "1", 1, s_agentNumber, false, true, certificate, x509); // production server
                    // TXID txid = new TXID("", "99999", "1", 1, s_agentNumber, false, true, certificate, x509); // testing server
                    TXID txid = new TXID("", version, "1", 1, s_agentNumber, false, true, certificate, x509);
                    //TXID txid = new TXID("", version, "1", 1, s_agentNumber, false, true, "", null);
                    String txidreply = txid.MessageToSend(true);

                    String ToSign201_TXID = f201reply + txidreply;
                    //String Signature_ToSign201_TXID = Sign(ToSign201_TXID, x509);
                    String All = bchnreply + ToSign201_TXID;// + dataZXI;// + Signature_ToSign201_TXID;

                    edgeTransportLayer.Send(All, true);
                    msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), hdncompanyid.Value);

                    lblmsg.Text += (msgcp.Success + " - " + msgcp.ErrorMessage);

                    if (msgcp.Success)
                        Step3ReadLastFiles(edgeTransportLayer);
                    else
                        oErrorLog.WriteErrorLog("Asic Error : " + msgcp.Success + " - " + msgcp.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void Step3ReadLastFiles(EDGETransport edgeTransportLayer)
        {
            try
            {
                edgeTransportLayer.Send(new REQO().MessageToSend(true), true);

                while (true)
                {
                    InboundMessage msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), hdncompanyid.Value);

                    if (msgcp.Success)
                    {
                        if (msgcp.MessageName.ToUpper() == "RA55" || msgcp.MessageName.ToUpper() == "RA56")
                        {
                            //RA55 bout = (RA55)msgcp;
                            lblmsg.Text += "Transaction Success, Certificate Generated Successfully.";
                            break;
                        }
                        else if (msgcp.MessageName.ToUpper() == "BOUT") // 	Obtain waiting reports from DIS and send BOUT to client
                        {
                            BOUT bout = (BOUT)msgcp;

                            if (bout.NoPendingFiles)
                                break;

                            msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), hdncompanyid.Value);
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
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private string gettext(string filename)
        {
            string textme = "";
            try
            {
                oErrorLog.WriteErrorLog("filname" + filename);
                oErrorLog.WriteErrorLog("filname1" + filename);
                textme = System.IO.File.ReadAllText(@"C:/asicfiles/" + filename, System.Text.Encoding.UTF8).Replace("\r\n", "\n");
                oErrorLog.WriteErrorLog("filname2" + filename);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return textme;
        }

        protected string Sign(string message, X509Certificate2 x509)
        {
            string signature = "";
            try
            {
                message = x509.Sign(message);

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
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog("PemCertificateHelper-" + ex.ToString());
            }
            return signature;
        }

        #endregion ASIC

        protected void btnget201database_Click(object sender, EventArgs e)
        {
            try
            {
                hdncompanyid.Value = txtcompanyid.Text.Trim();
                //hdncompanyid.Value = "0";
                string str201 = Prepared201().ToUpper();
                settext(str201);
                lblmsg.Text = "<div style='font-size:10px;'>" + str201.Replace("\n", "<br /><br />").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "</div>";
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void settext(string fileContent)
        {
            try
            {
                string fileName = "C:/asicfiles/" + txtcompanyid.Text.Trim() + "_Only201.txt";
                oErrorLog.WriteErrorLog("settext FileName" + fileName);

                if (File.Exists(fileName))
                {
                    oErrorLog.WriteErrorLog("settext FileName Delete" + fileName);
                    File.Delete(fileName);
                    //File.o
                    oErrorLog.WriteErrorLog("settext FileName Delete Done" + fileName);
                }
                //  File.Create(fileName);
                oErrorLog.WriteErrorLog("settext FileName Create" + fileName);

                File.WriteAllText(fileName, fileContent);
                oErrorLog.WriteErrorLog("settext FileName Create Done" + fileName);
                //File.WriteAllText("C:/asicfiles/Only201.txt", fileContent);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }
    }
}