using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace comdeeds.Controllers
{
    public class BaseController : Controller
    {
        protected bool IsAuthUser { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //IsAuthUser = App_Code.AuthHelper.IsValidRequestByRole(new List<string> { "USER" });
            IsAuthUser = App_Code.AuthHelper.IsValidRequestByRole(new List<string> { "USER","ADMIN", "SUBADMIN","SUBUSER" });
            ViewBag.IsAuthUser = IsAuthUser;
            ViewBag.LoggedInRoles = App_Code.AuthHelper.LoggedInRoles();
            if(IsAuthUser)
            {
                ViewBag.username = App_Code.AuthHelper.GetUserName();
            }

        }
    }
}