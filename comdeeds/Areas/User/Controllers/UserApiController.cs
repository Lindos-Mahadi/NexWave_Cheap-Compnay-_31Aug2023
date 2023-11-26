using comdeeds.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static comdeeds.Models.BaseModel;

namespace comdeeds.Areas.User.Controllers
{
    public class UserApiController : ApiController
    {


        [HttpGet]
        [ActionName("gettrusts")]
        public dynamic gettrusts()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER","ADMIN","SUBUSER" }, "/user/signin"));
            RequestGridParam id = getparam(this.Request);
            ClassGridTrustResult res = new ClassGridTrustResult();
            int start = (id.limit * id.page) - id.limit;
            start = start == 0 ? 1 : start;
            id.sortBy = "";
            var p = new ClassSqlGridParam { startLength = start, length = id.limit, orderBy = id.sortBy };
            var trusts = GridMethods.getUserTrustList(p, uid);

            if (!string.IsNullOrWhiteSpace(id.search))
            {
                trusts.data = trusts.data.Where(x => id.search.Contains(x.TrustName)).ToList();
            }

            return new
            {
                records = trusts.data,
                total = trusts.Total
            };
        }


        [HttpGet]
        [ActionName("getcompany")]
        public dynamic getcompany()
        {
            //Sachin added
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER","ADMIN","SUBUSER","SUBADMIN" }, "/user/signin"));
             //var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER" }, "/admin/signin"));
             string useremail=getuserid(uid.ToString());
            RequestGridParam id = getparam(this.Request);
            ClassGridCompanyResult res = new ClassGridCompanyResult();
            int start = (id.limit * id.page) - id.limit;
            start = start == 0 ? 1 : start;
            id.sortBy = "";
            var p = new ClassSqlGridParam { startLength = start, length = id.limit, orderBy = id.sortBy };
           // var comp = GridMethods.getUserCompanyList(p, uid);
            var comp = GridMethods.getUserCompanyList(p, useremail);
            if (!string.IsNullOrWhiteSpace(id.search))
            {
                comp.data = comp.data.Where(x => id.search.Contains(x.CompanyName)).ToList();
            }

            return new
            {
                records = comp.data,
                total = comp.Total
            };
        }


          dal.Operation oper=new dal.Operation();
         public string getuserid(string uid)
        {
            string useremail="";
            System.Data.DataTable dtuser=oper.get_userdetails_byuid(uid.ToString());
            if(dtuser.Rows.Count>0)
            {
                useremail=CryptoHelper.DecryptString(dtuser.Rows[0]["Email"].ToString());
            }
            return useremail;
        }
        
        public static RequestGridParam getparam(HttpRequestMessage request)
        {
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            return new RequestGridParam
            {
                limit = Convert.ToInt32(queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value),
                page = Convert.ToInt32(queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value),
                sortBy = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "sortBy", true) == 0).Value,
                _parent = Convert.ToInt32(queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "_parent", true) == 0).Value),
                search = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "search", true) == 0).Value
            };


        }


    }
}
