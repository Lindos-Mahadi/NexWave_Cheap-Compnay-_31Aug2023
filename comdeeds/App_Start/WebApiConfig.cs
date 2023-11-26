﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace comdeeds.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
            name: "apiroute",
            routeTemplate: "api/{controller}/{action}/{id}",
            defaults: new
            {
                controller = "SiteApi",
                id = RouteParameter.Optional
            }
            );
        }
    }
}