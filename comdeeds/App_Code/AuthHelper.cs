using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Principal;

namespace comdeeds.App_Code
{
    public static class AuthHelper
    {

        public static void SignIn(string UserName, bool RememberMe, IEnumerable<string> Roles, string UserId, string userdata)
        {
            var identity = new ClaimsIdentity(
                   new[] { new Claim(ClaimTypes.Name, UserName) },
                   DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserId));
            identity.AddClaim(new Claim(ClaimTypes.UserData, userdata));
            foreach (var role in Roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            auth.SignIn(new AuthenticationProperties
            {
                IsPersistent = RememberMe

            }, identity);
        }

        private static IAuthenticationManager auth
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        //public static void SignOut(string RedirectUrl)
        //{
        //    HttpContext.Current.GetOwinContext().Authentication.SignOut();
        //    if (!string.IsNullOrEmpty(RedirectUrl))
        //    {
        //        HttpContext.Current.Response.Redirect(RedirectUrl);
        //    }
        //    else
        //    {
        //        HttpContext.Current.Response.Redirect("/");
        //    }
        //}
        public static void SignOut(string RedirectUrl)
        {
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.GetOwinContext().Authentication.SignOut();
            string[] myCookies = HttpContext.Current.Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                HttpContext.Current.Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            if (!string.IsNullOrEmpty(RedirectUrl))
            {
                HttpContext.Current.Response.Redirect(RedirectUrl);
            }
            else
            {
                HttpContext.Current.Response.Redirect("/");
            }
        }
        /// <summary>IsValidRequest is a method to get current logged in userid.			
        /// </summary>
        public static string IsValidRequest(IEnumerable<string> Roles, string RedirectUrl)
        {
            bool flag = false;
            var UserRoles = auth.User.Claims.Where(x => x.Type == ClaimTypes.Role);
            foreach (var role in Roles)
            {
                foreach (var uRole in UserRoles)
                {
                    if (!flag)
                    {
                        flag = uRole.Value.ToLower() == role.ToLower() ? true : false;
                    }
                    //flag = uRole.Value.ToLower() == role.ToLower() ? true : false;
                }
            }

            if (flag)
            {
                return auth.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            }
            else
            {
                HttpContext.Current.Response.Redirect(RedirectUrl);
            }
            return "";
        }

        /// <summary>GetUserName is a method to get current logged in username. This method should call after check the Auth of user.			
        /// </summary>
        public static string GetUserName()
        {
            return auth.User.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault().Value;
        }

        /// <summary>GetUserData is a method to get current logged in User's JSON Data. This method should call after check the Auth of user.			
        /// </summary>
        public static string GetUserData()
        {
            return auth.User.Claims.Where(x => x.Type == ClaimTypes.UserData).FirstOrDefault().Value;
        }
                
        public static bool IsValidRequestByRole(IEnumerable<string> Roles)
        {
            bool flag = false;
            var UserRoles = auth.User.Claims.Where(x => x.Type == ClaimTypes.Role);
            foreach (var role in Roles)
            {
                foreach (var uRole in UserRoles)
                {
                    if (!flag)
                    {
                        flag = uRole.Value.ToLower() == role.ToLower() ? true : false;
                    }
                }
            }
            return flag;
        }



        public static void AddUpdateClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
        }

        public static string GetClaimValue(this IPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }


        public static List<string> LoggedInRoles()
        {
            List<string> r = new List<string>();
            var UserRoles = auth.User.Claims.Where(x => x.Type == ClaimTypes.Role);
            foreach (var uRole in UserRoles)
            {
                r.Add(uRole.Value.ToLower());
            }
            return r;
        }

    }




    
}