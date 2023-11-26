using System.Web.Mvc;

namespace comdeeds.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
            "dashboard_default",
            "Agent/dashboard",
            new { controller = "Admin", action = "dashboard", id = UrlParameter.Optional }
        );

            context.MapRoute(
                "Admin_download",
                "admin/downloads/{action}/{id}",
                new { controller = "Downloads", action = "signin", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Admin_default2",
                "admin/{action}/{id}",
                new { controller="Admin",action = "signin", id = UrlParameter.Optional }
            );

          
             context.MapRoute(
                "agent_default2",
                "agentsignin",
                new { controller = "Admin", action = "agentsignin", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Admin_default",
                "admin/{controller}/{action}/{id}",
                new { action = "signin", id = UrlParameter.Optional }
            );

        }
    }
}