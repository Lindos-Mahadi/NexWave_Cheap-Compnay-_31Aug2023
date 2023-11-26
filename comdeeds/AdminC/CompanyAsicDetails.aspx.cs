using comdeeds.dal;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using comdeeds.EDGE;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.InboundMessages;
using comdeeds.EDGE.OutboundMessages;
using comdeeds.EDGE.Utils;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using comdeeds;
using comdeeds.EDGE.DataEntities;
using System.Diagnostics;





namespace comdeeds.AdminC
{
    public partial class CompanyAsicDetails : System.Web.UI.Page
    {
        ErrorLog objErrorlog = new ErrorLog();
        DataAccessLayer dal1 = new DataAccessLayer();

        #region Connection Parameters
        private string s_certificateFileName = "";
        //private static string s_server = "edge1.asic.gov.au";
        //private static int s_port = 5608;
        private string s_server = "";
        // private static int s_port = 5610;
        private int s_port = 0;

        //private static int s_agentNumber = 40125;
        private int s_agentNumber = 0;
        private string s_userId = "";
        private string s_userPassword = "";
        private string s_newPassword = "";
        private static bool s_validateServerCertificate = false;
        private static string s_serialidentifier = "1d2879d2595322234de0ef169fc5545e";
        #endregion
        private static int cou = 0;

        protected void ddledge_SelectedIndexChanged(object sender, EventArgs e)
        {
            string message = ddledge.SelectedValue.Trim();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
            Session["serverEdge"] = message;
        }

        protected void Page_Load(object sender, EventArgs e)
         {
            try
            {
                if (Session["serverEdge"] != null)
                {
                    s_server = Session["serverEdge"].ToString();
                }

                string ss_port = ConfigurationManager.AppSettings["s_port"].ToString();
                s_port = Convert.ToInt32(ss_port);
                s_certificateFileName = ConfigurationManager.AppSettings["s_certificateFileName"].ToString();
                s_userId = ConfigurationManager.AppSettings["userid"].ToString();
                s_userPassword = ConfigurationManager.AppSettings["oldpass"].ToString();
                s_newPassword = ConfigurationManager.AppSettings["newpass"].ToString();
                s_server = ConfigurationManager.AppSettings["asicserver"].ToString();
                s_agentNumber = Convert.ToInt32(ConfigurationManager.AppSettings["agentno"].ToString());
                s_serialidentifier = ConfigurationManager.AppSettings["s_serialidentifier"].ToString();
            }
            catch (Exception ex) { objErrorlog.WriteErrorLog(ex.ToString()); }


            string dtasic = "";
            if (!IsPostBack)
            {
                if (Request.QueryString.Get("cname") != null && Request.QueryString.Get("cid") != null)
                {
                    try
                    {
                        hdncompanyid.Value = Request.QueryString.Get("cid");
                        hdnuserid.Value = Request.QueryString.Get("userid");
                        hdnRegid.Value = Request.QueryString.Get("Regid");
                        lblcompanyhead.Text = "Details of " + Request.QueryString.Get("cname");
                        Session["companyid"] = Request.QueryString.Get("cid");
                        string companyFullName= Request.QueryString.Get("cname");
                        companyFullName = companyFullName.Replace("   pty ltd","pty ltd");
                        string qryCheckTxnInsert = "select * from companysearch with(nolock) where Asic_status='DOCUMENTS ACCEPTED' and Asic_DocNo!='' and id='" + hdncompanyid.Value + "'";
                        DataTable dt = dal1.getdata(qryCheckTxnInsert);
                        if (dt.Rows.Count > 0)
                        {       
                        }
                        else
                        {
                            ReqReport();
                        }

                        CreateCustomerFolder();
                        Fill();
                    }
                    catch (Exception ex)
                    {
                        objErrorlog.WriteErrorLog(ex.ToString());
                    }         
                   
                }
            }
        }
        private void CreateCustomerFolder()
        {
            try
            {
                string directoryPath = Server.MapPath("../ExportedFiles\\" + hdncompanyid.Value);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string directoryPath2 = Server.MapPath("../ExportedFiles\\" + hdncompanyid.Value + "\\Final");
                if (!Directory.Exists(directoryPath2))
                {
                    Directory.CreateDirectory(directoryPath2);
                }
            }
            catch (Exception ex) { objErrorlog.WriteErrorLog(ex.ToString()); }
        }
        DataAccessLayer dal = new DataAccessLayer();
        private void Fill()
        {
            try
            {
                lblmsg.Text = "";
                DataTable dt = dal.getdata("select * from companysearch where id='" + hdncompanyid.Value.ToString() + "' and userid='" + Session["username"] + "'");
                if (dt.Rows.Count > 0)
                {
                    hdncompanyname.Value = dt.Rows[0]["companyname"].ToString();
                    lblname.Text = dt.Rows[0]["fullname"].ToString();
                    lblACN.Text = dt.Rows[0]["Asic_ACN"].ToString();
                    lblstatus.Text = dt.Rows[0]["Asic_status"].ToString();
                    lblfilename.Text = dt.Rows[0]["Asic_File"].ToString().Replace("C:/asicfiles/", "");
                    lblresponseType.Text = dt.Rows[0]["Asic_ResType"].ToString();
                    literror.Text = dt.Rows[0]["Asic_Error"].ToString().Replace("\r\n", "\n");
                    lbldocno.Text = dt.Rows[0]["Asic_DocNo"].ToString();
                    if (lblstatus.Text.ToUpper() == "DOCUMENTS ACCEPTED")
                    {
                        lnkretry.Visible = false;
                        if (!File.Exists("C:/asicfiles/Logs/" + lbldocno.Text + ".pdf") && (lblresponseType.Text.Trim() == "RA55" || lblresponseType.Text.Trim() == ""))
                        {
                            lnkrequestforcertificate.Visible = true;
                            downloadCertificate.Visible = true;
                            lbledge.Visible = true;
                            ddledge.Visible = true;
                        }
                        else
                        {
                            lnkrequestforcertificate.Visible = false;
                            downloadCertificate.Visible = false;
                            lbledge.Visible = false;
                            ddledge.Visible = false;
                        }
                    }
                    else
                    {
                        lnkretry.Visible = true;
                    }

                    #region certificate
                    string myfilepath = Server.MapPath("../ExportedFiles/" + hdncompanyid.Value + "/Final/" + lbldocno.Text + ".pdf");
                    string url = "../ExportedFiles/" + hdncompanyid.Value + "/Final/" + lbldocno.Text + ".pdf";
                    if (File.Exists("C:/asicfiles/Logs/" + lbldocno.Text + ".pdf"))
                    {
                        if (File.Exists(myfilepath))
                        {
                            litcertificate.Text = "<a href='" + url + "' target='_blank'>Received</a>";
                        }
                        else
                        {
                            System.IO.File.Copy("C:/asicfiles/Logs/" + lbldocno.Text + ".pdf", Server.MapPath("../ExportedFiles/" + hdncompanyid.Value + "/Final/" + lbldocno.Text + ".pdf"));
                            litcertificate.Text = "<a href='" + url + "' target='_blank'>Received</a>";
                        }
                    }
                    else if (File.Exists(myfilepath))
                    {
                        litcertificate.Text = "<a href='" + url + "' target='_blank'>Received</a>";
                    }
                    else
                    {
                        if (lblresponseType.Text == "RA56")
                        {
                            litcertificate.Text = "<span style='color:red'> RA56 will be prepared to advice that the certificate will be manually processed after 20 minute </span>";
                            lnkrequestforcertificate.Visible = true;
                        }
                        else
                        {
                            litcertificate.Text = "Not Received";
                        }
                                           
                    }
                    if (lblACN.Text.Trim() == "" && lblstatus.Text.ToUpper() == "DOCUMENTS ACCEPTED")
                    {
                        lnkrequestforcertificate.Visible = true; litcertificate.Text = "";
                    }

                    #endregion
                    #region Print Error
                    string str = "";
                    if (File.Exists(dt.Rows[0]["Asic_File"].ToString()))
                    {

                        string textme = "<div style='font-size:10px;background-color: cornsilk;color: black;font-weight: bold;padding: 5px 20px;max-height: 200px;overflow: auto;border-bottom: 2px solid #7cacd5;'>";
                        string filedata = System.IO.File.ReadAllText(dt.Rows[0]["Asic_File"].ToString(), System.Text.Encoding.UTF8).Replace("\r\n", "\n");

                        string[] segments = filedata.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        //string[] segments = filedata.ToString().Split( '\n');
                        for (int i = 0; i < segments.Length; i++)
                        {
                            if (segments[i] != "\n")
                            {
                                textme += segments[i].Replace("\n", "<br /><br />").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "<br />";
                            }
                        }
                        textme += "</div>";
                        literror.Text = textme;
                    }
                    #endregion
                }
                else
                {
                    lblmsg.Text = "Invalid Data.";
                }
            }
            catch (Exception ex) { objErrorlog.WriteErrorLog(ex.ToString()); }
        }
        protected void lnkretry_Click(object sender, EventArgs e)
        {
            try
            {
                Session["companyname"] = hdncompanyname.Value.ToString();
                Session["companyid"] = hdncompanyid.Value.ToString();
                Response.Redirect("SetCompanyRedirection.aspx?cname=" + Server.UrlEncode(hdncompanyname.Value) + "&cid=" + hdncompanyid.Value.ToString() + "&userid=" + hdnuserid.Value.Trim() + "&Regid=" + hdnRegid.Value.Trim(), false);
            }
            catch (Exception ex) { objErrorlog.WriteErrorLog(ex.ToString()); }
        }

        protected void downloadCertificate_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["DownloadCertificatepath"].ToString();
                 string final_path = path + "/" + hdncompanyid.Value + "/Doc_" + hdncompanyid.Value + ".rar";
            
                System.IO.FileInfo _file = new System.IO.FileInfo(final_path);
                if (_file.Exists)
                {
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + _file.Name);
                    Response.AddHeader("Content-Length", _file.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.WriteFile(_file.FullName);
                    Response.Flush();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                objErrorlog.WriteErrorLog(ex.ToString());
            }

        }

        protected void lnkrequestforcertificate_Click(object sender, EventArgs e)
        {
            try
            {
                string email = hdnuserid.Value;
                Session["companyname"] = hdncompanyname.Value.ToString();
                Session["companyid"] = hdncompanyid.Value.ToString();
                Response.Redirect("../Form201Web.aspx?CompanyID=" + hdncompanyid.Value.ToString() + "&Email=" +email+"&RreRequestTemp=HNY2018", false);
            }
            catch (Exception ex)
            {
                objErrorlog.WriteErrorLog(ex.ToString());
            }
        }


        int alreadylogin = 0;
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
                edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true);

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
                        objErrorlog.WriteErrorLog("LOGR next client state: {REQO Calling..}");
                        Step3ReadLastFiles(edgeTransportLayer);
                        objErrorlog.WriteErrorLog("Response");
                        string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                        lblmsg.Text += textme + "<br><hr>";
                    }
                }

                else
                {
                    if (chk.Elapsed < TimeSpan.FromSeconds(60))
                        goto send;
                    // lblmsg.Text += ("ASIC Responce message - " + msg.RawMessage + " - " + System.DateTime.Now.ToString());

                    if (!(msg.RawMessage.Contains("empty response"))) //if (!(msg.RawMessage.Contains("empty response")) || !(msg.RawMessage.ToString() == ""))
                    {
                        if (!(msg.RawMessage == ""))
                        {
                            lblmsg.Text += ("ASIC Responce message - " + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                        }

                    }
                    objErrorlog.WriteErrorLog("Unexpected reply to login:" + msg.RawMessage + " - " + System.DateTime.Now.ToString());
                        chk.Stop();
                        insertLBLmsg();
                }
                chk.Stop();
                edgeTransportLayer.Send(new LOUT().MessageToSend(), true);
                edgeTransportLayer.Disconnect();
            }
            catch (Exception ex)
            {
                objErrorlog.WriteErrorLog(ex.ToString());
            }
        }

        private void EmailSend()
        {
            try
            {
                string name = AsicCompanyNameonlyById(hdncompanyid.Value);
                string messsage = ope.htmlEmailClient(name);

                dal.SENDEMAILNOW ems = new dal.SENDEMAILNOW();
                ems.SendEmailRegistered("Company Registered", messsage, Request.QueryString.Get("Email"));
            }
            catch (Exception ex) { objErrorlog.WriteErrorLog(ex.ToString()); }
        }
        public void insertLBLmsg()
        {
            try
            {
                ope.insertLBLmsg(hdncompanyid.Value, lblmsg.Text);
            }
            catch (Exception ex) { objErrorlog.WriteErrorLog(ex.ToString()); }
        }
        dal.Operation ope = new dal.Operation();


        public string AsicCompanyNameonlyById(string cid)
        {
            return ope.AsicCompanyNameonlyById(cid);
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
                            lblmsg.Text += "Transaction Success, Certificate Generated Successfully.";
                            break;
                        }
                        else if (msgcp.MessageName.ToUpper() == "BOUT")
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
                objErrorlog.WriteErrorLog(ex.ToString());
            }


        }
        private string gettext(string filename)
        {
            string textme = "";
            try
            {
                objErrorlog.WriteErrorLog("filname" + filename);
                objErrorlog.WriteErrorLog("filname1" + filename);
                textme = System.IO.File.ReadAllText(@"C:/asicfiles/" + filename, System.Text.Encoding.UTF8).Replace("\r\n", "\n");
                objErrorlog.WriteErrorLog("filname2" + filename);
            }
            catch (Exception ex)
            {
                objErrorlog.WriteErrorLog(ex.ToString());
            }
            return textme;

        }



    }
}