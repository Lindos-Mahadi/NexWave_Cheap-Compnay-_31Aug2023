using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Data;
using comdeeds.App_Code;
using static comdeeds.Models.BaseModel;
using Newtonsoft.Json;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using comdeeds;
using System.Globalization;
using Ionic.Zip;

namespace comdeeds.AdminC
{
    public partial class UserSearchList : System.Web.UI.Page
    {
        private List<String> DynamicPdfName = new List<string>();
        private ErrorLog oErrorLog = new ErrorLog();

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBADMIN" }, "/admin/signin"));

                    if (Session["Subadmin"] != null && Session["Subadmin"].ToString().ToUpper() == "ADMIN")
                    {
                        gvcompanylist.Columns[1].Visible = true;
                        gvcompanylist.Columns[7].Visible = true;
                    }
                    else
                    {
                        gvcompanylist.Columns[1].Visible = false;
                        gvcompanylist.Columns[7].Visible = false;
                    }
                    Fillgrid();
                    if (Request.QueryString.Get("paymentstatus") != null)
                    {
                        if (Request.QueryString.Get("paymentstatus").ToLower() == "paid")
                        {
                            chkpaid.Checked = true;
                        }
                        if (Request.QueryString.Get("paymentstatus").ToLower() == "unpaid")
                        {
                            chkunpaid.Checked = true;
                        }
                    }
                    if (Request.QueryString.Get("asicstatus") != null)
                    {
                        if (Request.QueryString.Get("asicstatus").ToUpper().Contains("ACCEPT"))
                        {
                            chkasicsuccess.Checked = true;
                            chkpaid.Checked = true;
                        }
                        if (Request.QueryString.Get("asicstatus").ToUpper().Contains("REJECT"))
                        {
                            chkasicerror.Checked = true;
                            chkpaid.Checked = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    oErrorLog.WriteErrorLog(ex.ToString());
                }
            }
        }

        #endregion Page_Load

        #region Fill_Grid

        private void Fillgrid()
        {
            try
            {
                string where = "  ";
                if (Session["subUserID"] != null)
                {
                    if (where.Contains("where"))
                    {
                        where = "userid='" + Session["subUserID"].ToString() + "'";
                    }
                    else
                    {
                        where = "where userid='" + Session["subUserID"].ToString() + "'";
                    }
                }

                if (Request.QueryString.Get("paymentstatus") != null)
                {
                    where = " where status='" + Request.QueryString.Get("paymentstatus") + "'";
                }
                if (Request.QueryString.Get("asicstatus") != null)
                {
                    if (where.Contains("where"))
                    {
                        where += " and asic_status='" + Request.QueryString.Get("asicstatus") + "'";
                    }
                    else
                    {
                        where += "  where  asic_status='" + Request.QueryString.Get("asicstatus") + "'";
                    }
                }
                if (Request.QueryString.Get("userid") != null)
                {
                    if (where.Contains("where"))
                    {
                        where += " and userid Like '" + '%' + Request.QueryString.Get("userid") + '%' + "' OR FULLNAME Like'" + '%' + Request.QueryString.Get("userid") + '%' + "'";
                    }
                    else
                    {
                        where += " where userid Like '" + '%' + Request.QueryString.Get("userid") + '%' + "' OR FULLNAME Like'" + '%' + Request.QueryString.Get("userid") + '%' + "'";
                    }
                }
                if (where.Contains("where"))
                {
                    where += "and show_status=" + 1 + "";
                }
                else
                {
                    where += "where  show_status=" + 1 + "";
                }

                DataTable dt = new DataTable();
                dal.DataAccessLayer dal = new comdeeds.dal.DataAccessLayer();
                dt = dal.getdata("select id,companyname,userid,status,convert(varchar, SearchOn, 103) as searchon,[FULLNAME],[Asic_status],[Asic_Error],[Asic_File],[Asic_ACN],[Asic_DocNo],[Asic_ResType],Regid,govofcomapany from [dbo].[companysearch] " + where + " order by convert(datetime,SearchOn) desc");

                gvcompanylist.DataSource = dt;
                gvcompanylist.DataBind();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        protected void gvcompanylist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Session["Subadmin"] != null && Session["Subadmin"].ToString().ToUpper() == "ADMIN")
                {
                    //admin can see all Constitution PDf
                    var status = (LinkButton)e.Row.FindControl("LinkButton3");
                    status.Visible = true;
                }
                else
                {
                    // but only those agent can see, who has done your constitution payment $10.
                    //  var status = (LinkButton)e.Row.FindControl("LinkButton3");
                    //  status.Visible = false;
                }
            }
        }

        #endregion Fill_Grid

        #region Grid_Page_Index_Change

        protected void gvcompanylist_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvcompanylist.PageIndex = e.NewPageIndex;
            Fillgrid();
        }

        #endregion Grid_Page_Index_Change

        #region Some Link Buttton Code

        protected void lnkview_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                Label lblcm = (Label)gvr.FindControl("lblcompanyname");
                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");
                Label lbluserid = (Label)gvr.FindControl("lbluserid");
                Session["username"] = lbluserid.Text.ToString();
                Session["adminurl"] = "../adminc/UserSearchList.aspx";

                Response.Redirect("SetCompanyRedirection.aspx?cname=" + lblcm.Text + "&cid=" + hdnid.Value.ToString() + "&userid=" + lbluserid.Text.Trim(), false);
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        protected void lnkshowdetials_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                Label lblcm = (Label)gvr.FindControl("lblcompanyname");
                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");
                Label lbluserid = (Label)gvr.FindControl("lbluserid");
                // Session["username"] = lbluserid.Text.ToString();
                //Session["adminurl"] = "../AdminC/UserSearchList.aspx";
                Response.Redirect("../admin/companydetails?id=" + hdnid.Value.ToString());
                //  Response.Redirect("SetCompanyRedirection.aspx?cname=" + lblcm.Text + "&cid=" + hdnid.Value.ToString() + "&userid=" + lbluserid.Text.Trim(), false);
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        protected void lnkdownload_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                //Response.Redirect("CompanyFileDownload.aspx", false);

                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");

                string path = "../ExportedFiles\\" + hdnid.Value + "\\Form201.pdf";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "RunCode", "javascript:executeAfter('" + hdnid.Value + "');", true);
                //ScriptManager.RegisterClientScriptBlock(this, GetType(), "none", "<script>executeAfter();</script>", false);

                //Response.ContentType = "Application/pdf";
                //Response.AppendHeader("Content-Disposition", "attachment; filename=Form201.pdf");
                //Response.TransmitFile(Server.MapPath(path));
                //Response.Flush();
                // Response.Redirect("../ExportedFiles\\" + hdnid.Value + "\\Doc_" + hdnid.Value + ".zip");
                // Response.Close();
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        protected void btnemaildocument_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                GridViewRow gvr = (GridViewRow)btn.NamingContainer;
                int inde = gvr.RowIndex;
                //Response.Redirect("CompanyFileDownload.aspx", false);

                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");
                Label userid = (Label)gvr.FindControl("lbluserid");
                Label companyname = (Label)gvr.FindControl("lblcompanyname");
                Label Emailsent = (Label)gvr.FindControl("lblemailsent");
                //string path = "../ExportedFiles\\" + hdnid.Value + "\\Form201.pdf";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "RunCode", "javascript:executeAfter('" + hdnid.Value + "');", true);
                string username = "";
                string Password = "";
                string Mailserver = "";
                string Port = "";
                string ssl = "";
                string apiurl = "";
                string Message = "";

                string path1 = Server.MapPath("../ExportedFiles\\" + hdnid.Value + "\\Form201.pdf");
                string path2 = Server.MapPath("../ExportedFiles\\" + hdnid.Value + "\\Doc_" + hdnid.Value + ".zip");

                //string path ="F:\\OnlyDeads\\Deed\\web\\onlydeeds\\ExportedFiles_SMSF\\121\\121.rar";// Server.MapPath("../ExportedFiles_SMSF\\" + hdncompanyid.Value + "\\Doc_" + hdncompanyid.Value + ".zip");
                username = ConfigurationManager.AppSettings["CommpanyEmailID"].ToString();
                Password = ConfigurationManager.AppSettings["accounts_Pwd"].ToString();
                Mailserver = ConfigurationManager.AppSettings["Host"].ToString();
                string CompanyEmailID = ConfigurationManager.AppSettings["CommpanyEmailID"].ToString();
                Port = "25";
                ssl = "false";
                MailMessage Msg = new MailMessage();
                string fromeamil1 = username.ToString();
                Msg.From = new System.Net.Mail.MailAddress(fromeamil1);

                Msg.Bcc.Add(new MailAddress("deepak.dubey@gmail.com"));
                //Msg.To.Add(new MailAddress("bsparihar.88@gmail.com"));
                //Msg.To.Add(new MailAddress(userid.Text.Trim()));
                //Msg.To.Add(new MailAddress(CompanyEmailID, "onlydeeds"));
                Msg.To.Add(new MailAddress(userid.Text));
                Msg.Subject = "Company documents for " + companyname.Text.Trim();
                Msg.Body = Message.ToString();
                for (int File = 0; File < DynamicPdfName.Count; File++)
                {
                    if (DynamicPdfName.Count > 0)
                    {
                        Msg.Attachments.Add(new Attachment(DynamicPdfName[File]));
                    }
                }
                //Msg.Attachments.Add(new Attachment(path));
                Msg.Attachments.Add(new Attachment(path1));
                Msg.Attachments.Add(new Attachment(path2));

                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Mailserver;
                smtp.Port = Convert.ToInt32(Port);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(fromeamil1.ToString(), Password.ToString());
                smtp.Timeout = 600000;
                smtp.EnableSsl = false;
                smtp.Send(Msg);

                btn.Visible = false;
                Emailsent.Visible = true;
                Emailsent.Text = "email sent.";
                //ShowErrorMessage("Documents sent successfuly..!!");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        protected void lnkdelete_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                HiddenField lblcm = (HiddenField)gvr.FindControl("hdnid");
                Label lbluserid = (Label)gvr.FindControl("lbluserid");

                //obj.remove_companysearch(lbluserid.Text, lblcm.Value.ToString());
                dal.DataAccessLayer dal = new comdeeds.dal.DataAccessLayer();
                int stus = dal.executesql("update [dbo].[companysearch] set show_status=" + 0 + " where id=" + lblcm.Value);

                if (stus == 1 || stus > 0)
                {
                    Fillgrid();
                }
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        protected void lnkasic_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");
                HiddenField hdnfullname = (HiddenField)gvr.FindControl("hdnfullname");
                Label lbluserid = (Label)gvr.FindControl("lbluserid");
                HiddenField hdnRegid = (HiddenField)gvr.FindControl("hdnRegid");
                Session["username"] = lbluserid.Text.ToString();
                Session["adminurl"] = "../admin/UserSearchList.aspx";
                //Response.Redirect("CompanyAsicDetails.aspx?cid=" + hdnid.Value + "&cname=" + hdnfullname.Value, false);
                Response.Redirect("CompanyAsicDetails.aspx?cname=" + hdnfullname.Value + "&cid=" + hdnid.Value.ToString() + "&userid=" + lbluserid.Text.Trim() + "&Regid=" + hdnRegid.Value.ToString(), false);
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        protected void lnkasic1_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");
                HiddenField hdnfullname = (HiddenField)gvr.FindControl("hdnfullname");
                Label lbluserid = (Label)gvr.FindControl("lbluserid");
                HiddenField hdnRegid = (HiddenField)gvr.FindControl("hdnRegid");
                Session["username"] = lbluserid.Text.ToString();
                Session["adminurl"] = "../admin/UserSearchList.aspx";
                //Response.Redirect("CompanyAsicDetails.aspx?cid=" + hdnid.Value + "&cname=" + hdnfullname.Value, false);
                //Response.Redirect("CompanyAsicDetails.aspx?cname=" + hdnfullname.Value + "&cid=" + hdnid.Value.ToString() + "&userid=" + lbluserid.Text.Trim(), false);
                Response.Redirect("CompanyAsicDetails.aspx?cname=" + hdnfullname.Value + "&cid=" + hdnid.Value.ToString() + "&userid=" + lbluserid.Text.Trim() + "&Regid=" + hdnRegid.Value.ToString(), false);
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        protected void lnksend_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");
                HiddenField hdnfullname = (HiddenField)gvr.FindControl("hdnfullname");
                HiddenField hdnStatus = (HiddenField)gvr.FindControl("hdnStatus");
                Label lbluserid = (Label)gvr.FindControl("lbluserid");

                string hdncompanyid = hdnid.Value;
                string hdnfullName = hdnfullname.Value;
                string userid = lbluserid.Text;
                string Status = hdnStatus.Value;
                if (Status.Trim().ToLower() == "paid")
                {
                    createpdfZip(hdncompanyid, hdnfullName, userid);
                }
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        private void createpdfZip(string hdncompanyid, string hdnfullName, string userid)
        {
            try
            {
                string directoryPath = Server.MapPath("ExportedFiles\\" + hdncompanyid + "\\Final\\");
                string[] filename = Directory.GetFiles(directoryPath);
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddFiles(filename, "file");
                    zip.Save(Server.MapPath("ExportedFiles\\" + hdncompanyid + "\\Doc_" + hdncompanyid + ".zip"));

                    string username = "";
                    string Password = "";
                    string Mailserver = "";
                    string Port = "";
                    bool ssl = true;
                    string apiurl = "";
                    string Message = "";
                    string path_Form201 = Server.MapPath("ExportedFiles\\" + hdncompanyid + "\\Form201.pdf");
                    username = ConfigurationManager.AppSettings["FromMail"].ToString();
                    Password = ConfigurationManager.AppSettings["Password"].ToString();
                    Mailserver = ConfigurationManager.AppSettings["mailserver"].ToString();
                    Port = "587";
                    MailMessage Msg = new MailMessage();
                    string fromeamil1 = username.ToString();
                    Msg.From = new System.Net.Mail.MailAddress(fromeamil1);
                    Msg.To.Add(new MailAddress(userid.ToString()));
                    Msg.Subject = "Company Documents";
                    Msg.Body = Message.ToString();
                    for (int File = 0; File < DynamicPdfName.Count; File++)
                    {
                        if (DynamicPdfName.Count > 0)
                        {
                            Msg.Attachments.Add(new Attachment(DynamicPdfName[File]));
                        }
                    }
                    Msg.Attachments.Add(new Attachment(path_Form201));
                    Msg.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = Mailserver;
                    smtp.Port = Convert.ToInt32(Port);
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new System.Net.NetworkCredential(fromeamil1.ToString(), Password.ToString());
                    smtp.Timeout = 600000;
                    smtp.EnableSsl = ssl;
                    smtp.Send(Msg);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        protected void lnkpaid_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserSearchList.aspx?paymentstatus=paid", false);
        }

        protected void lnkunpaid_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserSearchList.aspx?paymentstatus=Unpaid", false);
        }

        protected void lnksearch_Click(object sender, EventArgs e)
        {
            try
            {
                string parameter = "";

                if (chkpaid.Checked == true && chkunpaid.Checked == false)
                {
                    parameter += "?paymentstatus=paid";
                }
                if (chkpaid.Checked == false && chkunpaid.Checked == true)
                {
                    parameter += "?paymentstatus=Unpaid";
                }

                if (chkasicerror.Checked == true && chkasicsuccess.Checked == false)
                {
                    if (parameter.Contains("?"))
                    {
                        parameter += "&asicstatus=DOCUMENTS REJECTED";
                    }
                    else
                    {
                        parameter += "?asicstatus=DOCUMENTS REJECTED";
                    }
                }
                if (chkasicerror.Checked == false && chkasicsuccess.Checked == true)
                {
                    if (parameter.Contains("?"))
                    {
                        parameter += "&asicstatus=DOCUMENTS ACCEPTED";
                    }
                    else
                    {
                        parameter += "?asicstatus=DOCUMENTS ACCEPTED";
                    }
                }
                Response.Redirect("UserSearchList.aspx" + parameter, false);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        protected void lnksearchu_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtemail.Text.Trim() != "")
                {
                    Response.Redirect("UserSearchList.aspx?userid=" + txtemail.Text.Trim(), false);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        protected void lnkdms_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");
                HiddenField hdnfullname = (HiddenField)gvr.FindControl("hdnfullname");
                Label lbluserid = (Label)gvr.FindControl("lbluserid");

                //dmsid.Attributes.Add("src",  "ViewerMedium.aspx?cid=" + hdnid.Value + "&cname=" + hdnfullname.Value+"&userid="+lbluserid.Text);
                dmsid.Attributes["src"] = "ViewerMedium.aspx?cid=" + hdnid.Value + "&cname=" + hdnfullname.Value + "&userid=" + lbluserid.Text;
                //string pagepath = "ViewerMedium.aspx?cid=" + hdnid.Value + "&cname=" + hdnfullname.Value + "&userid=" + lbluserid.Text;
                //string str= "<iframe id='dmsid' width='100%' height='515px' src='"+pagepath+"'></iframe>";
                //Literal1.Text = pagepath;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        #endregion Some Link Buttton Code

        #region Show_Error_Message

        public void ShowErrorMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Registering", String.Format("ShowErrorMessage('{0}');", message), true);
        }

        #endregion Show_Error_Message

        #region Pdf Downloading for Company Registraion Documents

        public void DownloadCertificate(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            var companyData = new ClassFullCompany();
            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;

                    if (companyData.CompanyMeta != null)
                    {
                        if (companyData.TransactionDetail != null)
                        {
                            if (companyData.CompanyMeta.BillStatus.ToLower() == "paid")
                            {
                                string htmlPath = string.Empty, body = "";
                                if (companyData.Company != null)
                                {
                                    htmlPath = Server.MapPath("~/Content/deedhtml/company.html");
                                    using (StreamReader red = new StreamReader(htmlPath))
                                    {
                                        body = red.ReadToEnd();
                                    }
                                    body = BuildCompanyPDF.BuildCertPDF(body, companyData);
                                }
                                var f = companyData.Company.CompanyName.Replace(" ", "-");

                                StringReader sr = new StringReader(body);
                                Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                                    pdfDoc.Open();
                                    string imageFilePath = Server.MapPath("~/Content/deedhtml/comdeeds.png");
                                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageFilePath);
                                    //jpg.ScaleToFit();
                                    jpg.ScaleAbsolute(pdfDoc.PageSize.Width - 20, pdfDoc.PageSize.Height - 20);
                                    jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
                                    jpg.Border = 0;
                                    jpg.SetAbsolutePosition(10, 10);
                                    jpg.PaddingTop = 0;
                                    writer.PageEvent = new ImageBackgroundHelper(jpg);
                                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                                    pdfDoc.Close();
                                    byte[] bytes = memoryStream.ToArray();
                                    memoryStream.Close();
                                    Response.Clear();
                                    Response.ContentType = "application/pdf";
                                    Response.AddHeader("Content-Disposition", "attachment; filename=" + f + "-Certificate.pdf");
                                    Response.Buffer = true;
                                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                    Response.BinaryWrite(bytes);
                                    Response.End();
                                    Response.Close();
                                }
                            }
                            else
                            {
                                ViewState["c_error"] = Helper.CreateNotification("Sorry, there is some problem while downloading the document.", EnumMessageType.Warning, "");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            if (ViewState["c_error"] != null)
            {
                string message = Helper.CreateNotification(ViewState["c_error"].ToString(), EnumMessageType.Error, "");
                errormsg.InnerHtml = message.Replace("notification closeable error", "");
            }
            else
            {
            }
        }

        private class ImageBackgroundHelper : PdfPageEventHelper
        {
            private iTextSharp.text.Image img;

            public ImageBackgroundHelper(iTextSharp.text.Image img)
            {
                this.img = img;
            }

            /**
             * @see com.itextpdf.text.pdf.PdfPageEventHelper#onEndPage(
             *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
             */

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                writer.DirectContentUnder.AddImage(img);
            }
        }

        public void downloadconstitution(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            string htmlPath = string.Empty, filename = string.Empty;
            var companyData = new ClassFullCompany();

            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;

                    if (companyData.TransactionDetail != null)
                    {
                        if (companyData.TransactionDetail.TransactionStatus == true)
                        {
                            if (companyData.Company.CompanyUseFor == "A company to operate business" || companyData.Company.CompanyUseFor.Contains("A company to operate business"))
                            {
                                string defaultPath = string.Empty;
                                defaultPath = Server.MapPath("~/Content/deedhtml/standard_company_constitution.pdf");
                                string pdfTemplate = ""; pdfTemplate = defaultPath;
                                string exportPath = Server.MapPath("../ExportedFiles\\" + id + "\\constitution_newone.pdf");
                                string newFile = exportPath;
                                filename = companyData.Company.CompanyName.Replace(" ", "-") + " - constitution";
                                PdfReader pdfReader = new PdfReader(pdfTemplate);
                                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                                AcroFields pdfFormFields = pdfStamper.AcroFields;

                                pdfFormFields.SetField("txtcompanyname", companyData.Company.CompanyName.ToUpper());
                                pdfFormFields.SetField("txtacn", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtacn1", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtType", companyData.Company.CompanyPurpose);
                                pdfFormFields.SetField("txtName", companyData.Company.CompanyName.ToUpper());

                                pdfStamper.FormFlattening = false;
                                pdfStamper.Close();

                                Response.ContentType = "Application/pdf";
                                Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");
                                Response.TransmitFile(exportPath);
                                Response.End();
                            }
                            else if (companyData.Company.CompanyUseFor == "smsf" && companyData.Company.specialpurpose_ifapplicable == "PSTC")
                            {
                                string defaultPath = string.Empty;
                                defaultPath = Server.MapPath("~/Content/deedhtml/SMSF_Trustee_Constitution.pdf");
                                string pdfTemplate = ""; pdfTemplate = defaultPath;
                                string exportPath = Server.MapPath("../ExportedFiles\\" + id + "\\smsf_constitution_newone.pdf");
                                string newFile = exportPath;
                                filename = companyData.Company.CompanyName.Replace(" ", "-") + "-smsf-constitution";
                                PdfReader pdfReader = new PdfReader(pdfTemplate);
                                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                                AcroFields pdfFormFields = pdfStamper.AcroFields;

                                pdfFormFields.SetField("txtCompnayName", companyData.Company.CompanyName.ToUpper());
                                pdfFormFields.SetField("txtACN", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtAcn1", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtDate", companyData.TransactionDetail.AddedDate.Value.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                pdfFormFields.SetField("txtName", companyData.Company.CompanyName.ToUpper());

                                pdfStamper.FormFlattening = false;
                                pdfStamper.Close();

                                Response.ContentType = "Application/pdf";
                                Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");
                                Response.TransmitFile(exportPath);
                                Response.End();
                            }
                            else
                            {
                                string defaultPath = string.Empty;
                                defaultPath = Server.MapPath("~/Content/deedhtml/standard_company_constitution.pdf");
                                string pdfTemplate = ""; pdfTemplate = defaultPath;
                                string exportPath = Server.MapPath("../ExportedFiles\\" + id + "\\constitution_newone.pdf");
                                string newFile = exportPath;
                                filename = companyData.Company.CompanyName.Replace(" ", "-") + " - constitution";
                                PdfReader pdfReader = new PdfReader(pdfTemplate);
                                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                                AcroFields pdfFormFields = pdfStamper.AcroFields;

                                pdfFormFields.SetField("txtcompanyname", companyData.Company.CompanyName.ToUpper());
                                pdfFormFields.SetField("txtacn", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtacn1", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtType", companyData.Company.CompanyPurpose);
                                pdfFormFields.SetField("txtName", companyData.Company.CompanyName.ToUpper());

                                pdfStamper.FormFlattening = false;
                                pdfStamper.Close();

                                Response.ContentType = "Application/pdf";
                                Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");
                                Response.TransmitFile(exportPath);
                                Response.End();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void downloadconstitution_old(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            string htmlPath = string.Empty, body = "";
            var companyData = new ClassFullCompany();
            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;

                    if (companyData.TransactionDetail != null)
                    {
                        if (companyData.TransactionDetail.TransactionStatus == true)
                        {
                            htmlPath = Server.MapPath("~/Content/deedhtml/constitution.html");
                            using (StreamReader red = new StreamReader(htmlPath))
                            {
                                body = red.ReadToEnd();
                            }

                            var RegOfcAddModel = companyData.Address.Where(x => x.IsRegisteredAddress == true).FirstOrDefault();
                            string RegOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                                RegOfcAddModel.UnitLevel,
                                RegOfcAddModel.Street,
                                RegOfcAddModel.Suburb,
                                RegOfcAddModel.State,
                                RegOfcAddModel.PostCode);
                            string dirsign = string.Empty;
                            foreach (var d in companyData.Directors)
                            {
                                //dirsign += string.Format(@"<p style='margin-bottom:0;'>............................................. <br />[Name], [Signature], Member[Date]</p><br/>");
                                dirsign += string.Format("<p style='margin-bottom:0;'>............................................. <br />Name - {0} <br /> Date - {1}</p><br/>", d.FirstName + " " + d.LastName, DateTime.Now.ToString("dd-MM-yyyy"));
                            }
                            // replace names
                            body = body.Replace("{companyname}", companyData.Company.CompanyName.ToUpper());
                            body = body.Replace("{acn}", companyData.CompanyMeta.CompanyACN); // Please insert ACN here
                            body = body.Replace("{companyaddress}", RegOfcAdd);
                            body = body.Replace("{directorsign}", dirsign);

                            var f = companyData.Company.CompanyName + "-constitution.pdf";
                            StringReader sr = new StringReader(body);
                            Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                                writer.PageEvent = new ITextEvents();
                                pdfDoc.Open();
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                                pdfDoc.Close();
                                byte[] bytes = memoryStream.ToArray();
                                memoryStream.Close();
                                Response.Clear();
                                Response.ContentType = "application/pdf";
                                Response.AddHeader("Content-Disposition", "attachment; filename=" + f);
                                Response.Buffer = true;
                                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                Response.BinaryWrite(bytes);

                                Response.Flush(); // Sends all currently buffered output to the client.
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        public void downloadreginvoice(long id)
        {
            try
            {
                var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
                var companyData = new ClassFullCompany();
                if (id > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(id);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;
                }
                if (companyData.TransactionDetail != null)
                {
                    if (companyData.TransactionDetail.TransactionStatus == true)
                    {
                        string htmlPath = string.Empty, body = "";
                        if (companyData.Company != null)
                        {
                            htmlPath = Server.MapPath("~/Content/deedhtml/companyinvoice.html");
                            using (StreamReader red = new StreamReader(htmlPath))
                            {
                                body = red.ReadToEnd();
                            }
                            var cr = companyData.TransactionDetail;
                            var c = companyData.Company;

                            var member = companyData.Applicant;
                            var ccf = ((companyData.Cost.AsicFee + companyData.Cost.SetupCost + companyData.Cost.SetupGST) * companyData.Cost.CreditCardFee) / 100; // Credit card fees

                            var m = $"{member.GivenName} {member.FamilyName}";
                            var RegOfcAddModel = companyData.Address.Where(x => x.IsRegisteredAddress == true).FirstOrDefault();
                            string RegOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                                RegOfcAddModel.UnitLevel,
                                RegOfcAddModel.Street,
                                RegOfcAddModel.Suburb,
                                RegOfcAddModel.State,
                                RegOfcAddModel.PostCode);

                            body = body.Replace("{invoiceno}", cr.Id.ToString());
                            body = body.Replace("{date}", cr.AddedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            body = body.Replace("{username}", m);
                            body = body.Replace("{address}", RegOfcAdd);

                            body = body.Replace("{asicfee}", "$" + companyData.Cost.AsicFee);
                            body = body.Replace("{asictotal}", "$" + companyData.Cost.AsicFee);

                            body = body.Replace("{deedname}", $"Company ({companyData.Company.CompanyName}) - Setup Fee");
                            body = body.Replace("{unitcost}", "$" + companyData.Cost.SetupCost);
                            body = body.Replace("{unittotal}", "$" + companyData.Cost.SetupCost);

                            //if (companyData.Company.CompanyUseFor == "smsf")
                            //{
                            //    body = body.Replace("{smsffee}", "$" + 48);
                            //    body = body.Replace("{smsftotel}", "$" + 48);
                            //}
                            //else
                            //{
                            //    body = body.Replace("{smsffee}", "$" + 0);
                            //    body = body.Replace("{smsftotel}", "$" + 0);
                            //}

                            if (companyData.companysearch.govofcomapany == "yes")
                            {
                                body = body.Replace("{constdfees}", "$" + 11);
                                body = body.Replace("{consttotel}", "$" + 11);
                            }
                            else
                            {
                                body = body.Replace("{constdfees}", "$" + 0);
                                body = body.Replace("{consttotel}", "$" + 0);
                            }
                            //if(companyData.Company.CompanyUseFor == "smsf" && companyData.companysearch.govofcomapany == "yes")
                            //  {
                            //      body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee + 48 + 10+1));
                            //  }
                            // else if(companyData.Company.CompanyUseFor == "smsf")
                            //  {
                            //      body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee + 48));
                            //  }

                            if (companyData.companysearch.govofcomapany == "yes")
                            {
                                body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee + 10 + 1));
                            }
                            else
                            {
                                body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee));
                            }

                            body = body.Replace("{gst}", "$" + companyData.Cost.SetupGST);
                            body = body.Replace("{total}", "$" + companyData.Cost.TotalCost);

                            body = body.Replace("{creditcardfeesp}", companyData.Cost.CreditCardFee + "%");
                            body = body.Replace("{creditcardfees}", "$" + ccf);
                            body = body.Replace("{processingfees}", companyData.Cost.ProcessingFee + "cents");
                            var f = companyData.Company.CompanyName.Replace(" ", "-");
                            createpdf(body, f + "-setup-invoice.pdf");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        public void DownloadASICCertificate(long id)
        {
            try
            {
                var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));

                var companyData = new ClassFullCompany();
                if (id > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(id);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;
                }

                if (companyData.TransactionDetail != null)
                {
                    if (companyData.TransactionDetail.TransactionStatus == true)
                    {
                        string htmlPath = string.Empty;
                        if (companyData.Company != null)
                        {
                            string idd = companyData.CompanyMeta.Id.ToString();
                            htmlPath = ConfigurationManager.AppSettings["DownloadCertificatepath"].ToString();

                            var AsicStatus = new ClassAsicSetup();
                            AsicStatus = CompanyMethods.getAsicDetails(id.ToString());
                            //string lbldocno = "3E9568948"; testing purpose only
                            string lbldocno = "";
                            string lblresponseType = "";
                            string lblpath = "";

                            lbldocno = AsicStatus.Asic_DocNo.ToString();
                            lblresponseType = AsicStatus.Asic_ResType.ToString();
                            if (System.IO.File.Exists("C:/asicfiles/Logs/" + lbldocno + ".pdf") && (lblresponseType.Trim() == "RA55" || lblresponseType.Trim() == "RA56"))
                            {
                                lblpath = "C:/asicfiles/Logs/" + lbldocno + ".pdf";
                                System.IO.FileInfo _file = new System.IO.FileInfo(lblpath);
                                if (_file.Exists)
                                {
                                    Response.Clear();
                                    Response.AddHeader("Content-Disposition", "attachment; filename=" + _file.Name);
                                    Response.AddHeader("Content-Length", _file.Length.ToString());
                                    Response.ContentType = "application/octet-stream";
                                    Response.WriteFile(_file.FullName);
                                    Response.Flush();

                                    // Response.End();
                                }
                                else
                                {
                                    Response.Redirect("/user/order/waitforAisc", false);
                                }
                            }
                            else
                            {
                                Response.Redirect("/user/order/waitforAisc", false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }

            // ExportedFiles\2130\Final
        }

        public void createpdf(string body, string filename)
        {
            try
            {
                StringReader sr = new StringReader(body);
                Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.Close();

                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                    Response.Buffer = true;
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                    //Response.Close();
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        protected void gvcompanylist_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DownloadCertificate")
                {
                    long id = Convert.ToInt64(e.CommandArgument.ToString());
                    DownloadCertificate(id);
                }

                if (e.CommandName == "downloadconstitution")
                {
                    long id = Convert.ToInt64(e.CommandArgument.ToString());
                    downloadconstitution(id);
                }

                if (e.CommandName == "downloadreginvoice")
                {
                    long id = Convert.ToInt64(e.CommandArgument.ToString());
                    downloadreginvoice(id);
                }

                if (e.CommandName == "DownloadASICCertificate")
                {
                    long id = Convert.ToInt64(e.CommandArgument.ToString());
                    DownloadASICCertificate(id);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        #region Pdf Events

        private class ITextEvents : PdfPageEventHelper
        {
            private ErrorLog oErrorLog = new ErrorLog();

            // This is the contentbyte object of the writer
            private PdfContentByte cb;

            // we will put the final number of pages in a template
            private PdfTemplate footerTemplate;

            // this is the BaseFont we are going to use for the header / footer
            private BaseFont bf = null;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    footerTemplate = cb.CreateTemplate(50, 50);
                }
                catch (DocumentException de)
                {
                    oErrorLog.WriteErrorLog(de.ToString());
                }
                catch (System.IO.IOException ioe)
                {
                    oErrorLog.WriteErrorLog(ioe.ToString());
                }
            }

            public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                try
                {
                    base.OnEndPage(writer, document);
                    if (writer.PageNumber > 1)
                    {
                        iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                        String text = "Page " + (writer.PageNumber - 1) + " of ";
                        //Add paging to footer
                        {
                            cb.BeginText();
                            cb.SetFontAndSize(bf, 10);
                            cb.SetTextMatrix(document.PageSize.GetRight(150), document.PageSize.GetBottom(10));
                            cb.ShowText(text);
                            cb.EndText();
                            float len = bf.GetWidthPoint(text, 10);
                            cb.AddTemplate(footerTemplate, document.PageSize.GetRight(150) + len, document.PageSize.GetBottom(10));
                            //Move the pointer and draw line to separate footer section from rest of page
                            cb.MoveTo(40, document.PageSize.GetBottom(25));
                            cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(25));
                            cb.Stroke();
                            BaseColor b = new BaseColor(9, 9, 9);
                            cb.SetColorStroke(b);
                        }
                    }
                }
                catch (Exception ex)
                {
                    oErrorLog.WriteErrorLog(ex.ToString());
                }
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                try
                {
                    base.OnCloseDocument(writer, document);
                    if (writer.PageNumber > 1)
                    {
                        footerTemplate.BeginText();
                        footerTemplate.SetFontAndSize(bf, 10);
                        footerTemplate.SetTextMatrix(0, 0);
                        footerTemplate.ShowText((writer.PageNumber - 1).ToString());
                        footerTemplate.EndText();
                    }
                }
                catch (Exception ex)
                {
                    oErrorLog.WriteErrorLog(ex.ToString());
                }
            }
        }

        #endregion Pdf Events

        #endregion Pdf Downloading for Company Registraion Documents

        #region Delete Company Details

        protected void btnDel_Click(object sender, EventArgs e)
        {
            int len = gvcompanylist.Rows.Count;
            for (int i = 0; i < len; i++)
            {
                CheckBox chk = (CheckBox)gvcompanylist.Rows[i].FindControl("cbSelect");
                if (chk.Checked)
                {
                    try
                    {
                        HiddenField lblcm = (HiddenField)gvcompanylist.Rows[i].FindControl("hdnid");
                        dal.DataAccessLayer dal = new comdeeds.dal.DataAccessLayer();
                        int stus = dal.executesql("update [dbo].[companysearch] set show_status=" + 0 + " where id=" + lblcm.Value);
                    }
                    catch (Exception ex)
                    {
                        // Fillgrid();
                        oErrorLog.WriteErrorLog(ex.ToString());
                    }
                }
            }
            //Page_Load(null, null);

             Fillgrid();
        }

        #endregion Delete Company Details

        #region Edit Company Details

        protected void lnkedit_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                GridViewRow gvr = (GridViewRow)lnk.NamingContainer;
                int inde = gvr.RowIndex;
                HiddenField hdnid = (HiddenField)gvr.FindControl("hdnid");
                HiddenField hdnfullname = (HiddenField)gvr.FindControl("hdnfullname");
                Label lbluserid = (Label)gvr.FindControl("lbluserid");
                HiddenField hdnRegid = (HiddenField)gvr.FindControl("hdnRegid");
                //  Session["username"] = lbluserid.Text.ToString();
                // Session["adminurl"] = "../admin/UserSearchList.aspx";
                Session["EditOption"] = true;
                Response.Redirect("SetCompanyRedirection.aspx?cname=" + Server.UrlEncode(hdnfullname.Value) + "&cid=" + hdnid.Value.ToString() + "&userid=" + lbluserid.Text + "&Regid=" + hdnRegid.Value.Trim(), false);
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        #endregion Edit Company Details

        #region Some CheckBox Button

        protected void chkSelect_CheckedChanged(Object sender, EventArgs e)
        {
            CheckBox lnk = (CheckBox)sender;
            if (lnk.Checked)
            {
                btnDel.Visible = true;
            }
            else
            {
                btnDel.Visible = false;
            }
        }

        protected void chkBxHeader_CheckedChanged(Object sender, EventArgs e)
        {
            CheckBox chkBxHeader = (CheckBox)gvcompanylist.HeaderRow.FindControl("chkBxHeader");

            int len = gvcompanylist.Rows.Count;
            for (int i = 0; i < len; i++)
            {
                if (chkBxHeader.Checked)
                {
                    CheckBox chk = (CheckBox)gvcompanylist.Rows[i].FindControl("cbSelect");
                    chk.Checked = true;
                    btnDel.Visible = true;
                }
                else
                {
                    CheckBox chk = (CheckBox)gvcompanylist.Rows[i].FindControl("cbSelect");
                    chk.Checked = false;
                    btnDel.Visible = false;
                }
            }
        }

        #endregion Some CheckBox Button
    }
}