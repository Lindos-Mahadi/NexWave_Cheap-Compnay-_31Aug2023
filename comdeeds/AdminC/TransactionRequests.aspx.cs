using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using comdeeds.dal;

namespace comdeeds.AdminC
{
    public partial class TransactionRequests : System.Web.UI.Page
    {
        private DataAccessLayer dal = new DataAccessLayer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Get("cid") != null && Request.QueryString.Get("cname") != null)
                {
                    string cid = Request.QueryString.Get("cid");
                    lblcompanyname.Text = Request.QueryString.Get("cname");
                    filltxn(cid);
                }
            }
        }

        private void filltxn(string cid)
        {
            try
            {
                // DataTable dt = dal.getdata("select  * from [dbo].[LBLmsg] where companyid='" + cid + "'  and sms != '' and sms NOT LIKE CONCAT('%', (select CompanyName from companysearch where id = '"+cid+"' - 1), '%') order by id");
                DataTable dt = dal.getdata("select  * from [dbo].[LBLmsg] where companyid='" + cid + "'  and sms != '' order by id");
                // DataTable dt = dal.getdata("select  * from [dbo].[LBLmsg] where companyid='" + cid + "' order by id");
                gvlist.DataSource = dt;
                gvlist.DataBind();
            }
            catch (Exception ex)
            {
            }
        }
    }
}