using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(comdeeds.Startup))]

namespace comdeeds
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            OwinConfig.ConfigureAuth(app);
        }


        public static class OwinConfig
        {
            public static void ConfigureAuth(IAppBuilder app)
            {
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new Microsoft.Owin.PathString("/admin/signin"),
                    CookieName = "SiteAuth",
                    LogoutPath = Microsoft.Owin.PathString.FromUriComponent("/admin/signin"),
                    SlidingExpiration = false,
                    ExpireTimeSpan = TimeSpan.FromDays(1)
                });
            }
        }
    }
}
