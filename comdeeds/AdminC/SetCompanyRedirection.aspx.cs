using comdeeds.App_Code;
using comdeeds.dal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace comdeeds.AdminC
{
    public partial class SetCompanyRedirection : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["companyname"] == null)
            {
                if (Request.QueryString.Get("cname") != null)
                {
                    if (Server.UrlDecode(Request.QueryString.Get("cname").ToString()) != null)
                    {
                        Session["companyname"] = Server.UrlDecode(Request.QueryString.Get("cname").ToString());
                    }
                }
                else
                {
                    Response.Redirect("/admin/signin");
                }
            }

            if (Request.QueryString.Get("cid") != null && Request.QueryString.Get("userid") != null)
            {
                string companyname = Session["companyname"].ToString();
                string companyid = Request.QueryString.Get("cid");
                string userid = Request.QueryString.Get("userid");
                long Regid = 0;
                if (Request.QueryString.Get("Regid") != null && Request.QueryString.Get("Regid") != "")
                {
                    Regid = Convert.ToInt64(Request.QueryString.Get("Regid"));
                }

                Operation oper = new Operation();
                DataTable dtus = new DataTable();
                string email = userid;
                string pass = ""; string regNo = "";
                dtus = oper.get_registration(userid, Regid);
                if (dtus.Rows.Count > 0)
                {
                    pass = dtus.Rows[0]["pass"].ToString();
                    regNo = dtus.Rows[0]["sno"].ToString();
                }

                #region VirtualLogin

                //  string dec = CryptoHelper.DecryptString("vsJbNYTPtN/hCYPMEdbmpA==");
                email = CryptoHelper.EncryptData(email);
                pass = CryptoHelper.EncryptData(pass);
                var userData = App_Code.UserMethods.GetUserLogin_Admin(email, pass, regNo);
                if (userData != null)
                {
                    var User = userData;
                    if (User.EmailVerified == true || User.EmailVerified == true)
                    {
                        if (User.Role.ToLower() == "user" || User.Role.ToLower() == "admin" || User.Role.ToLower() == "subadmin" || User.Role.ToLower() == "subuser")
                        {
                            var uData = new comdeeds.Models.BaseModel.LoginUserData
                            {
                                email = CryptoHelper.DecryptString(User.Email),
                                LastLogin = DateTime.Now,
                                FirstName = User.FirstName,
                                LastName = User.LastName,
                                Phone = User.Phone
                            };
                            var uDataJson = JsonConvert.SerializeObject(uData);
                            AuthHelper.SignIn(User.FirstName, false, new List<string>() { User.Role }, User.Id.ToString(), uDataJson);

                            string CompanyCookieId = "company-session-id";
                            var c = new HttpCookie(CompanyCookieId, companyid);
                            c.Expires = DateTime.Now.AddMonths(1);
                            Response.Cookies.Add(c);
                            string cookie = "There is no cookie!";
                            if (Request.Cookies.AllKeys.Contains(CompanyCookieId))
                            {
                                cookie = "Yeah - Cookie: " + this.Request.Cookies[CompanyCookieId].Value;
                                Response.Redirect("../company-setup?cname=" + companyname, false);
                            }
                        }
                    }
                }

                #endregion VirtualLogin
            }
        }
    }
}