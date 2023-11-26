using comdeeds.App_Code;
using comdeeds.dal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static comdeeds.Models.BaseModel;

namespace comdeeds.Areas.User.Controllers
{
    public class UserController : comdeeds.Controllers.BaseController
    {
        ErrorLog objlog = new ErrorLog();
        public ActionResult dashboard()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER","ADMIN","SUBUSER","SUBADMIN" }, "/user/signin"));
            var u = JsonConvert.DeserializeObject<LoginUserData>(AuthHelper.GetUserData());

            if(TempData["dmsg"]!=null)
            {
                ViewBag.msg = TempData["dmsg"];
                TempData["dmsg"] = null;
            }

            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult dashboard(LoginUserData form)
        {
            string msg = "";
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            if(!string.IsNullOrEmpty( form.FirstName) && !string.IsNullOrEmpty(form.LastName))
            {
                if(!string.IsNullOrEmpty(form.email) && Helper.IsValidEmail(form.email))
                {
                    var regex = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";//https://stackoverflow.com/questions/28904826/phone-number-validation-mvc
                    var match = Regex.Match(form.Phone, regex, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrEmpty(form.Phone) && match.Success)
                    {
                        using (var db = new MyDbContext())
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            var u = new TblUser { Id = uid };

                            db.TblUsers.Attach(u);
                            u.FirstName = form.FirstName;
                            u.LastLogIn = form.LastLogin;
                            u.Phone = form.Phone;
                            u.Email = CryptoHelper.EncryptData(form.email);
                            var entry = db.Entry(u);
                            entry.Property(e => e.FirstName).IsModified = true;
                            entry.Property(e => e.LastName).IsModified = true;
                            entry.Property(e => e.Email).IsModified = true;
                            entry.Property(e => e.Phone).IsModified = true;
                            var i = db.SaveChanges()>0;
                            if(i)
                            {
                                // update user claim for login
                                var uDataJson = JsonConvert.SerializeObject(form);
                                AuthHelper.SignIn(form.FirstName, false, new List<string>() { "User" }, uid.ToString(), uDataJson);
                                msg = Helper.CreateNotification("Info updated successfully.", EnumMessageType.Success, "Success");

                            }
                        }
                    }else
                    {
                        msg = Helper.CreateNotification("Please enter a valid phone no.", EnumMessageType.Error, "Error");
                    }
                }else
                {
                    msg = Helper.CreateNotification("Please enter a valid email address.", EnumMessageType.Error, "Error");
                }

            }else
            {
                msg = Helper.CreateNotification("Please enter your name.", EnumMessageType.Error, "Error");
            }
            TempData["dmsg"] = msg;
            return RedirectToAction("dashboard");
        }



        // GET: User/User
        public ActionResult signin()
        {
            ClassUserLoginForm form = new ClassUserLoginForm();
            return View(form);
        }


        [HttpPost]
        public String forgetpwd([System.Web.Http.FromBody] String email)
        {
            ClassUserLoginForm form = new ClassUserLoginForm();
            string msg = ""; string email_incrpt = string.Empty; var d = ""; var fullname = "";
            var emails = email;
            if (Helper.IsValidEmail(email))
            {
                email_incrpt = CryptoHelper.EncryptData(email);
                using (var db = new MyDbContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    var u = db.TblUsers.Where(x => x.Email == email_incrpt && x.Del == false).FirstOrDefault();

                    if (u != null)
                    {
                        var Reg = db.Registrations.AsNoTracking().FirstOrDefault(x => x.Sno == u.Regid && x.Email == email);
                        if (Reg != null)
                        { d = Reg.Pass; fullname = Reg.GivenName; }

                        var mailer = new Class_mailer
                        {
                            fromEmail = "infocomdeeds@gmail.com",
                            fromName = "Comdeeds",
                            HtmlBody = "Dear " + fullname + ",<br/><br/> Your Registered Login ID and Password is shown below.  <br/><br/>User ID - " + email + " <br/> Password - " + d + "<br/><br/> Simply log on to our website by using the above User Name and password. <br/>Once logged on, to your secured area, you can start using our services. <br/><br/> Support By <br/>Comdeeds Sales Team <br/> <img src='https://comdeeds.com.au/Content/images/logo.jpg' style='width: 300px;clear: both; float: left;margin: 22px -9px;'/>",
                            subject = "Password Recovery From www.comdeeds.com.au",
                            toMail = email
                        };

                        EmailHelper.SendSmtpMail1(mailer);
                        msg = Helper.CreateMessage("Your password has been sent to the registered email address.", EnumMessageType.Success, "Success");
                    }
                    else
                    {
                        msg = Helper.CreateMessage("Please enter a valid email address. ", EnumMessageType.Error, "Error");
                    }
                }
            }
            else
            {
                msg = Helper.CreateMessage("Please enter a valid email address.", EnumMessageType.Error, "Error");
            }
            ViewBag.msg = msg;
            TempData["msg"] = msg;
            return msg;
        }


        //sachin added 20 Nov 


        public ActionResult companydetails(long id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> {"USER", "SUBUSER" }, "/user/signin"));
            ViewBag.id = id;
            var companyData = new ClassFullCompany();
            ViewBag.isvalid = true;
            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;
                }

                if (TempData["compsummarymsg"] != null)
                {
                    ViewBag.msg = TempData["compsummarymsg"];
                    TempData["compsummarymsg"] = null;
                }
            }
            catch (Exception ex)
            {
                objlog.WriteErrorLog(ex.ToString());
                ViewBag.isvalid = false;
                ViewBag.msg = Helper.CreateMessage("Sorry, something is not right for this request, please try again", EnumMessageType.Error, "Error");
            }
            return View(companyData);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult signin(ClassUserLoginForm form)
        {
            Operation oper = new Operation();
            DataTable dtus = new DataTable();
            if (Request.Cookies["company-session-id"] != null)
            {
                Response.Cookies["company-session-id"].Expires = DateTime.Now.AddDays(-1);
            }


            string returnurlpath = Request.QueryString["ReturnUrl"];
            if (string.IsNullOrEmpty(returnurlpath))
            {
                returnurlpath = "/user/dashboard/";
            }
            var email = form.loginemail.Trim();
            var pass = form.loginpassword.Trim();
            var reme = form.rememberme;
            string msg = ""; string pass1 = "";
            if (Helper.IsValidEmail(email))
            {
                string regNo = "";
              /*  dtus = oper.get_registration(email);
                if (dtus.Rows.Count > 0)
                {
                    pass1 = dtus.Rows[0]["pass"].ToString();
                    regNo = dtus.Rows[0]["sno"].ToString();
                } */

                email = CryptoHelper.EncryptData(email);
                pass = CryptoHelper.EncryptData(pass);
                var userData = App_Code.UserMethods.GetUserLogin_New(email, pass);
                if (userData != null)
                {
                    var User = userData;
                    if (User.EmailVerified == true)
                    {
                        if (User.Role.ToLower() == "user" || User.Role.ToLower() == "subuser")
                        {
                            var uData = new LoginUserData
                            {
                                email = CryptoHelper.DecryptString(User.Email),
                                LastLogin = DateTime.Now,
                                FirstName = User.FirstName ,
                                LastName=User.LastName,
                                Phone=User.Phone
                            };
                            var uDataJson = JsonConvert.SerializeObject(uData);
                            AuthHelper.SignIn(User.FirstName, reme, new List<string>() { User.Role }, User.Id.ToString(), uDataJson);
                            return Redirect(returnurlpath);
                        }
                        else
                        {
                            msg = Helper.CreateNotification("Incorrect email or password", EnumMessageType.Error, "Error");
                        }
                    }
                    else
                    {
                        msg = Helper.CreateNotification("Your account is not verified yet", EnumMessageType.Error, "Error");
                    }
                }
                else
                {
                    msg = Helper.CreateNotification("Incorrect email or password", EnumMessageType.Error, "Error");
                }
            }
            else
            {
                msg = Helper.CreateNotification("Please enter a valid email address.", EnumMessageType.Error, "Error");
            }
            ViewBag.msg = msg;
            return View(form);
        }

        
        public ActionResult signout()
        {
            AuthHelper.SignOut("/user/signin");
            return RedirectToAction("signin");
        }






        [ActionName("change-password")]
        public ActionResult changepassword()
        {
            var userID = Convert.ToInt64(App_Code.AuthHelper.IsValidRequest(new List<string> { "USER","SUBUSER" }, "/user/login"));
            if (TempData["cmsg"] != null)
            {
                ViewBag.msg = TempData["cmsg"];
                TempData["cmsg"] = null;
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("change-password")]
        public ActionResult changepassword(ClassUserPasswordData form)
        {
            var userID = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER","SUBUSER" }, "/user/login"));
            string msg = "";
            if (form.newpass.Length >= 8)
            {
                if (form.newpass == form.confirmpass)
                {
                    msg = UserMethods.ChangePassword(form.oldpass, form.newpass, userID);
                }
                else
                {
                    msg = Helper.CreateNotification("Password not match.", EnumMessageType.Error, "Error");
                }
            }
            else
            {
                msg = Helper.CreateNotification("Password length is not valid , please use atleast 8 characters .", EnumMessageType.Error, "Error");
            }
            TempData["cmsg"] = msg;
            ViewBag.msg = msg;
            return View();
        }




    }
}