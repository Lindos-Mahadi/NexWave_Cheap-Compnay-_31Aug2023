using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace comdeeds
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ra53route",
                url: "setup/step1/{id}",
                defaults: new { controller = "RA53", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ra54route",
                url: "setup/step2/{id}",
                defaults: new { controller = "RA54", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "companyroute",
                url: "setup/step3/{id}",
                defaults: new { controller = "Lodge201", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "trustPayment",
                url: "setup/pay/{utm_pf}",
                defaults: new { controller = "Home", action = "trustpayment", utm_pf = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "companySummary",
                url: "company-setup/summary/{id}",
                defaults: new { controller = "Home", action = "companysummary", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "trustSummary",
                url: "trustsetup/summary/{id}",
                defaults: new { controller = "Home", action = "trustsummary", id = UrlParameter.Optional }
            );

            // Ignore the alternate path to the home page
            routes.IgnoreRoute("");
            //comment by praveen
            routes.MapRoute(
                name: "DefaultSite",
                url: "{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "UserMaintenance",
            url: "user/Maintenance/{action}/{id}",
            defaults: new { controller = "Maintenance", action = "MaintenanceIndex", id = UrlParameter.Optional },
            namespaces: new[] { "comdeeds.Areas.User.Controllers" } // Add the namespace for the area controllers
        );

            //comment by praveen
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}", // * URL with parameters but id is optional here *
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}