using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace comdeeds.AdminC
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Subadmin"] != null && Session["Subadmin"].ToString() != "")
            {

                lblAdmin.Text = Session["Subadmin"].ToString();
                if (Session["Subadmin"].ToString().ToLower() == "subadmin")
                {
                    settings1.Visible = false; settings2.Visible = false; settings3.Visible = false;
                }
                else
                {
                    settings1.Visible = true; settings2.Visible = true; settings3.Visible = true;
                }
              
            }

            else if (Session["admin"] != null)
            {
                lblAdmin.Text = Session["admin"].ToString();
            }
            else if (Session["superadmin"] != null)
            {
                lblAdmin.Text = Session["superadmin"].ToString();
            }
            else if (Session["specialUser"] != null)
            {
                lblAdmin.Text = Session["specialUser"].ToString();
                settings1.Visible = false; settings2.Visible = true; settings4.Visible = false;
            }
            else if (Session["lawyer"] != null)
            {
                lblAdmin.Text = Session["lawyer"].ToString();
                settings1.Visible = false; settings2.Visible = true; settings4.Visible = false;
            }
            else
            {
                Response.Redirect("/admin/signin");
            }
        }
    }
}