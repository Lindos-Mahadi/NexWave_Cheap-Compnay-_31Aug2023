using comdeeds.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace comdeeds.AdminC
    ///
    //Use MVC Admin Login instead of it.
{
    public partial class login : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void btnlogin_Click(object sender, EventArgs e)
        {
            try
            {
                comdeeds.dal.Operation oper = new dal.Operation();
                DataTable dt = new DataTable();


                string useremail = CryptoHelper.EncryptData(txtid.Text.Trim());
                string password = CryptoHelper.EncryptData(txtpassword.Text.Trim());
                dt = oper.getadminlogi(useremail, password);
                if (dt.Rows.Count > 0)
                {
                    //for (int i = 0; i < dt.Rows.Count; i++) {
                    // string  useremail = CryptoHelper.DecryptString(dt.Rows[0]["Email"].ToString());
                    // string password = CryptoHelper.DecryptString(dt.Rows[0]["password"].ToString());
                    //hdnid.Value += "" + useremail + "|" + password+" ";
                    //if (hdnid.Value.Split('|')[0] == useremail || hdnid.Value.Split('|')[1] == password) {
                    //    Session["adminlogin"] = useremail;
                    //    Session["adminpass"] = password;
                    //    Response.Redirect("/admin/dashboard", false);
                    //}

                    Session["adminlogin"] = txtid.Text.Trim();
                    Session["adminpass"] = txtpassword.Text.Trim();
                    Response.Redirect("/admin/dashboard", false);
                }
                else
                {
                    Response.Redirect("AdminC/login.aspx", false);
                }
                // }

            }
            catch (Exception ex)
            {

            }
        }

        protected void lnkforgot_Click(object sender, EventArgs e)
        {
            try
            {
                comdeeds.dal.Operation oper = new dal.Operation();
                DataTable dt = new DataTable();
                //dt = oper.getadminlogi();
               // for (int i = 0; i < dt.Rows.Count; i++)
               // {
                   // string useremail = CryptoHelper.DecryptString(dt.Rows[0]["Email"].ToString());
                   // string password = CryptoHelper.DecryptString(dt.Rows[0]["password"].ToString());
                    //hdnid.Value += "" + useremail + "|" + password + ",";
                    //if (useremail == txtid.Text && password == txtpassword.Text)
                    //{
                    //    //Response.Redirect("/admin/dashboard",false);
                    //}
                //}

            }
            catch (Exception ex)
            {

            }
        }
    }
}