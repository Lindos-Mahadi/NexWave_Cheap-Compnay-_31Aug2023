using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using comdeeds.dal;
using System.IO;

namespace comdeeds
{
    public partial class Success : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Get("cname") != null && Request.QueryString.Get("cid") != null)
                {
                    hdncompanyid.Value = Request.QueryString.Get("cid");
                    lblcompanyhead.Text = "Details of " + Request.QueryString.Get("cname");
                    CreateCustomerFolder();
                    Fill();
                }
            }
        }
        private void CreateCustomerFolder()
        {
            try
            {
                //string directoryPath = Server.MapPath(string.Format("ExportedFiles\\", hdncompanyid.Value));
                string directoryPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string directoryPath2 = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Final");
                if (!Directory.Exists(directoryPath2))
                {
                    Directory.CreateDirectory(directoryPath2);
                }
            }
            catch (Exception ex) { }
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
                        }
                        else
                        {
                            lnkrequestforcertificate.Visible = false;
                        }
                    }
                    else
                    {
                        lnkretry.Visible = true;
                    }


                    #region certificate
                    string myfilepath = Server.MapPath("ExportedFiles/" + hdncompanyid.Value + "/Final/" + lbldocno.Text + ".pdf");
                    string url = "ExportedFiles/" + hdncompanyid.Value + "/Final/" + lbldocno.Text + ".pdf";
                    if (File.Exists("C:/asicfiles/Logs/" + lbldocno.Text + ".pdf"))
                    {
                        if (File.Exists(myfilepath))
                        {
                            litcertificate.Text = "<a href='" + url + "' target='_blank'>Received</a>";
                        }
                        else
                        {
                            System.IO.File.Copy("C:/asicfiles/Logs/" + lbldocno.Text + ".pdf", Server.MapPath("ExportedFiles/" + hdncompanyid.Value + "/Final/" + lbldocno.Text + ".pdf"));
                            litcertificate.Text = "<a href='" + url + "' target='_blank'>Received</a>";
                        }
                    }
                    else if (File.Exists(myfilepath))
                    {
                        litcertificate.Text = "<a href='" + url + "' target='_blank'>Received</a>";
                    }
                    else
                    {
                        litcertificate.Text = "Not Received";
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
                        //textme = System.IO.File.ReadAllText(dt.Rows[0]["Asic_File"].ToString(), System.Text.Encoding.UTF8).Replace("\r\n", "\n");
                        //literror.Text = "<div style='font-size:10px;'>" + textme.Replace("\n", "<br /><br />").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "</div>";
                    }
                    #endregion
                }
                else
                {
                    lblmsg.Text = "Invalid Data.";
                }
            }
            catch (Exception ex) { }
        }
        protected void lnkretry_Click(object sender, EventArgs e)
        {
            try
            {

                Session["companyname"] = hdncompanyname.Value.ToString();
                Session["companyid"] = 107;
                Response.Redirect("step1.aspx", false);
            }
            catch (Exception ex) { }
        }

        protected void lnkcertificate_Click(object sender, EventArgs e)
        {

        }

        protected void lnkrequestforcertificate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["Email"] != null)
                {
                    Session["companyname"] = hdncompanyname.Value.ToString();
                    Session["companyid"] = hdncompanyid.Value.ToString();
                    Response.Redirect("../Form201Web.aspx?CompanyID=" + hdncompanyid.Value.ToString() + "&Email=" + Session["username"].ToString(), false);
                }
                else
                {
                    Session["companyname"] = hdncompanyname.Value.ToString();
                    Session["companyid"] = hdncompanyid.Value.ToString();
                   // Response.Redirect("step6.aspx?asicstatus=" + lblstatus.Text, false);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}