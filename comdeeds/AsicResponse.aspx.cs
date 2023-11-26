using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using comdeeds.dal;
using System.IO;
using comdeeds.App_Code;

namespace comdeeds
{
    public partial class AsicResponse : System.Web.UI.Page
    {
        string emaile = ""; string Role = "";
        ErrorLog oErrorLog = new ErrorLog();
        DataAccessLayer dal = new DataAccessLayer();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Request.QueryString.Get("error") != null)
                    {

                        if (Request.QueryString.Get("CompanyID") != "" && Request.QueryString.Get("Email") != "")
                        {
                            hdncompanyid.Value = Request.QueryString.Get("CompanyID").ToString();
                            hdnEmail.Value = Request.QueryString.Get("Email").ToString();
                        }
                        lblmsg.Text = Request.QueryString.Get("error");

                        if (Session["companyid"] != null)
                        {
                            AsicStatus(Session["companyid"].ToString());
                        }
                        else
                        {
                            if (Session["adminurl"] == null)
                            {
                                emaile = CryptoHelper.EncryptData(hdnEmail.Value);

                                DataTable ds = dal.getdata("select _Role from Tbl_User where Email='" + emaile + "'");
                                if (ds.Rows.Count > 0)
                                {
                                    Role = ds.Rows[0][0].ToString();

                                    if (Role.ToLower() == "admin")
                                    {
                                        Response.Redirect("/ThankYou?utm_t=a", false);
                                    }
                                    else if (Role.ToLower() == "subadmin")
                                    {
                                        Response.Redirect("/ThankYou?utm_t=a", false);
                                    }
                                    else
                                    {
                                        Response.Redirect("/ThankYou?utm_t=c", false);
                                    }

                                }
                               // Response.Redirect("~/user/order/company", false);
                            }
                            else {
                                Response.Redirect("~/AdminC/UserSearchList.aspx", false);
                            }
                               
                        }

                        //Response.Redirect("Success.aspx?cname=CompanyName&cid=" + Session["companyid"].ToString(), false);
                    }

                }
                catch (Exception ex)
                {
                    oErrorLog.WriteErrorLog(ex.ToString());
                }

            }
        }
        public void AsicStatus(string cid)
        {
            string status = "";
            try
            {
                DataTable dt = new DataTable();
                dal.DataAccessLayer dal = new dal.DataAccessLayer();
                dt = dal.getdata("select top 1 * from companysearch where id='" + cid + "'");
                if (dt.Rows.Count > 0)
                {
                    Session["companyname"] = dt.Rows[0]["companyname"].ToString();
                    string err = dt.Rows[0]["asic_error"].ToString();
                    literror.Text = err;
                    status = dt.Rows[0]["asic_status"].ToString().ToUpper();
                    lblstatus.Text = status;
                    #region Print Error
                    string str = "";
                    //if (File.Exists(dt.Rows[0]["Asic_File"].ToString()))
                    //{
                    //    string textme = System.IO.File.ReadAllText(dt.Rows[0]["Asic_File"].ToString(), System.Text.Encoding.UTF8).Replace("\r\n", "\n");
                    //    literror.Text = "<div style='font-size:10px;background-color: cornsilk;color: black;font-weight: bold;padding: 5px 20px;max-height: 200px;overflow: auto;border-bottom: 2px solid #7cacd5;'>" + textme.Replace("\n", "<br /><br />").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "</div>";
                    //}
                    string asicfile_ = dt.Rows[0]["Asic_File"].ToString();
                    if (asicfile_.Trim() != "")
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
                    else
                    {
                        literror.Text = "";
                    }
                    #endregion
                }
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }

        }
        protected void lnkretry_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString.Get("error").Contains("transmitted"))
                {
                    if (Request.QueryString.Get("CompanyID") != null && Request.QueryString.Get("Email") != null)
                    {
                        Response.Redirect("Form201Web.aspx?CompanyID=" + hdncompanyid.Value + "&Email=" + hdnEmail.Value, false);
                    }
                }
                else
                {
                    if (lblstatus.Text.Contains("ACCEPTED"))
                    {
                        Response.Redirect("Form201Web.aspx?CompanyID=" + hdncompanyid.Value + "&Email=" + hdnEmail.Value, false);
                    }
                    else
                    {
                        //Response.Redirect("step1.aspx", false);
                        Response.Redirect("Form201Web.aspx?CompanyID=" + hdncompanyid.Value + "&Email=" + hdnEmail.Value, false);
                    }
                }
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
        }
    }
}