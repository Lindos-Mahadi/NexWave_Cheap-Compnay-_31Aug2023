using System.Web.Mvc;

namespace comdeeds.Areas.User
{
    public class UserAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "User";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.MapRoute(
                "User_order",
                "user/order/{action}/{id}",
                new { controller = "Order", action = "trust", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "User_default",
                "user/{action}/{id}",
                new {controller="User",action = "signin", id = UrlParameter.Optional }
            );
        }
    }
}