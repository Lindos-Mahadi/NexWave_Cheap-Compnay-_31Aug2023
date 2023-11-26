using comdeeds.App_Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Globalization;
using System.IO;
using static comdeeds.Models.BaseModel;
using comdeeds.dal;
using System.Data;

namespace comdeeds.Areas.Admin.Controllers
{
    public class AdminController : comdeeds.Controllers.BaseController
    {
        private ErrorLog objlog = new ErrorLog();

        // GET: Admin/Admin
        public ActionResult signin()
        {
            Operation oper = new Operation();
            DataTable dtus = new DataTable();
            ClassUserLoginForm form = new ClassUserLoginForm();

            #region Login if already have session

            if (Session["adminlogin"] != null)
            {
                string returnurlpath = Request.QueryString["ReturnUrl"];
                if (string.IsNullOrEmpty(returnurlpath))
                {
                    returnurlpath = "/admin/dashboard";
                }
                var email = Session["adminlogin"].ToString();
                var pass = Session["adminpass"].ToString();
                var reme = form.rememberme;
                string msg = ""; string pass1 = "";
                if (Helper.IsValidEmail(email))
                {
                    email = CryptoHelper.EncryptData(email);
                    pass = CryptoHelper.EncryptData(pass);
                    var userData = App_Code.UserMethods.GetUserLogin_New(email, pass);
                    if (userData != null)
                    {
                        var User = userData;
                        if (User.EmailVerified == true)
                        {
                            if (User.Role.ToLower() == "admin" || User.Role.ToLower() == "subadmin")
                            {
                                var uData = new LoginUserData
                                {
                                    email = CryptoHelper.DecryptString(User.Email),
                                    LastLogin = DateTime.Now,
                                    FirstName = User.FirstName,
                                    LastName = User.LastName,
                                    Phone = User.Phone
                                };

                                var uDataJson = JsonConvert.SerializeObject(uData);
                                AuthHelper.SignIn(User.FirstName, reme, new List<string>() { User.Role }, User.Id.ToString(), uDataJson);
                                return Redirect(returnurlpath);
                            }
                        }
                    }
                }

                ViewBag.msg = msg;
            }

            #endregion Login if already have session

            return View(form);
        }

        public ActionResult agentsignin()
        {
            Operation oper = new Operation();
            DataTable dtus = new DataTable();
            ClassUserLoginForm form = new ClassUserLoginForm();

            #region Login if already have session

            if (Session["adminlogin"] != null)
            {
                string returnurlpath = Request.QueryString["ReturnUrl"];
                if (string.IsNullOrEmpty(returnurlpath))
                {
                    returnurlpath = "/admin/dashboard";
                }
                var email = Session["adminlogin"].ToString();
                var pass = Session["adminpass"].ToString();
                var reme = form.rememberme;
                string msg = ""; string pass1 = "";
                if (Helper.IsValidEmail(email))
                {
                    email = CryptoHelper.EncryptData(email);
                    pass = CryptoHelper.EncryptData(pass);
                    var userData = App_Code.UserMethods.GetUserLogin_New(email, pass);
                    if (userData != null)
                    {
                        var User = userData;
                        if (User.EmailVerified == true)
                        {
                            if (User.Role.ToLower() == "admin" || User.Role.ToLower() == "subadmin")
                            {
                                var uData = new LoginUserData
                                {
                                    email = CryptoHelper.DecryptString(User.Email),
                                    LastLogin = DateTime.Now,
                                    FirstName = User.FirstName,
                                    LastName = User.LastName,
                                    Phone = User.Phone
                                };

                                var uDataJson = JsonConvert.SerializeObject(uData);
                                AuthHelper.SignIn(User.FirstName, reme, new List<string>() { User.Role }, User.Id.ToString(), uDataJson);
                                return Redirect(returnurlpath);
                            }
                        }
                    }
                }

                ViewBag.msg = msg;
            }

            #endregion Login if already have session

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult signin(ClassUserLoginForm form)
        {
            Operation oper = new Operation();
            DataTable dtus = new DataTable();
            string returnurlpath = Request.QueryString["ReturnUrl"];
            if (string.IsNullOrEmpty(returnurlpath))
            {
                returnurlpath = "/admin/dashboard";
            }
            var email = form.loginemail;
            var emailid = form.loginemail;
            var pass = form.loginpassword;
            var reme = form.rememberme;
            string msg = "";
            if (Helper.IsValidEmail(email))
            {
                email = CryptoHelper.EncryptData(email);
                pass = CryptoHelper.EncryptData(pass);
                var userData = App_Code.UserMethods.GetUserLogin_New(email, pass);
                if (userData != null)
                {
                    var User = userData;
                    if (User.EmailVerified == true)
                    {
                        if (User.Role.ToLower() == "admin")
                        {
                            var uData = new LoginUserData
                            {
                                email = CryptoHelper.DecryptString(User.Email),
                                LastLogin = DateTime.Now,
                                FirstName = User.FirstName,
                                LastName = User.LastName,
                                Phone = User.Phone
                            };

                            if (User.Role.ToLower() == "admin")
                            {
                                Session["admin"] = User.FirstName + " " + User.LastName + "(" + User.Role.ToLower() + ")";
                                Session["Subadmin"] = User.Role.ToLower();
                                Session["lblAdmin"] = User.FirstName + " " + User.LastName + "(" + User.Role.ToLower() + ")";
                            }
                            else
                            {
                                msg = Helper.CreateMessage("You do not have sufficient permissions to access this page, Please contact with Administrator.", EnumMessageType.Error, "Error");
                                ViewBag.msg = msg;
                                return View(form);
                            }

                            Session["adminlogin"] = form.loginemail;
                            Session["adminpass"] = form.loginpassword;
                            var uDataJson = JsonConvert.SerializeObject(uData);
                            AuthHelper.SignIn(User.FirstName, reme, new List<string>() { User.Role }, User.Id.ToString(), uDataJson);
                            return Redirect(returnurlpath);
                        }
                        else
                        {
                            msg = Helper.CreateMessage("Incorrect email or password", EnumMessageType.Error, "Error");
                        }
                    }
                    else
                    {
                        msg = Helper.CreateMessage("Your account is not verified yet", EnumMessageType.Error, "Error");
                    }
                }
                else
                {
                    msg = Helper.CreateMessage("Incorrect email or password", EnumMessageType.Error, "Error");
                }
            }
            else
            {
                msg = Helper.CreateMessage("Please enter a valid email address.", EnumMessageType.Error, "Error");
            }
            ViewBag.msg = msg;
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult agentsignin(ClassUserLoginForm form)
        {
            Operation oper = new Operation();
            DataTable dtus = new DataTable();
            string returnurlpath = Request.QueryString["ReturnUrl"];
            if (string.IsNullOrEmpty(returnurlpath))
            {
                returnurlpath = "/Agent/dashboard";
            }
            var email = form.loginemail;
            var emailid = form.loginemail;
            var pass = form.loginpassword;
            var reme = form.rememberme;
            string msg = "";
            if (Helper.IsValidEmail(email))
            {
                email = CryptoHelper.EncryptData(email);
                pass = CryptoHelper.EncryptData(pass);
                var userData = App_Code.UserMethods.GetUserLogin_New(email, pass);
                if (userData != null)
                {
                    var User = userData;
                    if (User.EmailVerified == true)
                    {
                        if (User.Role.ToLower() == "subadmin" || User.Role.ToLower() == "superadmin" || User.Role.ToLower() == "user")
                        {
                            var uData = new LoginUserData
                            {
                                email = CryptoHelper.DecryptString(User.Email),
                                LastLogin = DateTime.Now,
                                FirstName = User.FirstName,
                                LastName = User.LastName,
                                Phone = User.Phone
                            };

                            //if (User.Role.ToLower() == "admin")
                            //{
                            //    Session["admin"] = User.FirstName + " " + User.LastName + "(" + User.Role.ToLower() + ")";
                            //    Session["Subadmin"] = User.Role.ToLower();
                            //    Session["lblAdmin"] = User.FirstName + " " + User.LastName + "(" + User.Role.ToLower() + ")";
                            //}

                            if (User.Role.ToLower() == "superadmin")
                            {
                                Session["superadmin"] = User.FirstName + " " + User.LastName + "(" + User.Role.ToLower() + ")";
                                Session["Subadmin"] = User.Role.ToLower();
                                Session["lblAdmin"] = User.FirstName + " " + User.LastName + "(" + User.Role.ToLower() + ")";
                            }
                            else if (User.Role.ToLower() == "subadmin")
                            {
                                Session["Subadmin"] = User.Role.ToLower();
                                Session["lblAdmin"] = User.FirstName + " " + User.LastName + "(" + User.Role.ToLower() + ")";
                                Session["subUserID"] = emailid.ToString();
                            }
                            else if (User.Role.ToLower() == "user" && (User.Tuser.ToLower() == "financial adviser" || User.Tuser.ToLower() == "accountant" || User.Tuser.ToLower() == "lawyer" || User.Tuser.ToLower() == "mortgage broker"))
                            {
                                if (User.Tuser.ToLower() == "lawyer")
                                {
                                    Session["lawyer"] = "Lawyer";
                                }
                                else
                                {
                                    Session["specialUser"] = "specialuser";
                                }
                                Session["Subadmin"] = "";
                                Session["subUserID"] = emailid.ToString();
                            }
                            else
                            {
                                msg = Helper.CreateMessage("You do not have sufficient permissions to access this page, Please contact with Administrator.", EnumMessageType.Error, "Error");
                                ViewBag.msg = msg;
                                return View(form);
                            }

                            Session["adminlogin"] = form.loginemail;
                            Session["adminpass"] = form.loginpassword;
                            var uDataJson = JsonConvert.SerializeObject(uData);
                            AuthHelper.SignIn(User.FirstName, reme, new List<string>() { User.Role }, User.Id.ToString(), uDataJson);
                            return Redirect(returnurlpath);
                        }
                        else
                        {
                            msg = Helper.CreateMessage("Incorrect email or password", EnumMessageType.Error, "Error");
                        }
                    }
                    else
                    {
                        msg = Helper.CreateMessage("Your account is not verified yet", EnumMessageType.Error, "Error");
                    }
                }
                else
                {
                    msg = Helper.CreateMessage("Incorrect email or password", EnumMessageType.Error, "Error");
                }
            }
            else
            {
                msg = Helper.CreateMessage("Please enter a valid email address.", EnumMessageType.Error, "Error");
            }
            ViewBag.msg = msg;
            return View(form);
        }

        public ActionResult signout()
        {
            AuthHelper.SignOut("/admin");
            return RedirectToAction("signin");
        }

        public ActionResult dashboard()
        {
            var subuserid = ""; var Esubuserid = "";
            if (Session["Subadmin"] != null)
            {
                TempData["subadmin"] = Session["Subadmin"].ToString();
            }

            if (Session["lawyer"] != null)
            {
                TempData["lawyer"] = Session["lawyer"].ToString();
            }

            if (Session["subUserID"] != null)
            {
                TempData["subUserID"] = Session["subUserID"].ToString();
                subuserid = Session["subUserID"].ToString();
                Esubuserid = CryptoHelper.EncryptData(subuserid);
            }
            if (Session["specialUser"] != null)
            {
                TempData["specialUser"] = Session["specialUser"].ToString();
            }

            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            var counterdata = AdminMethods.GetCounters(subuserid, Esubuserid);
            return View(counterdata);
        }

        public ActionResult users()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            return View();
        }

        public ActionResult trusts()
        {
            if (Session["Subadmin"] != null)
            {
                TempData["subadmin"] = Session["Subadmin"].ToString();
            }

            if (Session["subUserID"] != null)
            {
                TempData["subUserID"] = Session["subUserID"].ToString();
            }
            if (Session["specialUser"] != null)
            {
                TempData["specialUser"] = Session["specialUser"].ToString();
            }

            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            return View();
        }

        public ActionResult payments()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            if (AuthHelper.IsValidRequestByRole(new List<string> { "ADMIN", "USER" }))
            {
                // ViewBag.Email = uid;
                return View();
            }
            else
            {
                return View("noaccess");
            }
        }

        public ActionResult settings()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN" }, "/admin/signin"));
            if (AuthHelper.IsValidRequestByRole(new List<string> { "ADMIN" }))
            {
                var list = OptionMethods.GetAllOptions();
                if (TempData["settmsg"] != null)
                {
                    ViewBag.msg = TempData["settmsg"];
                    TempData["settmsg"] = null;
                }
                return View(list);
            }
            else
            {
                return View("noaccess");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult settings(FormCollection form)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN" }, "/admin/signin"));
            List<ClassadminOptions> data = new List<ClassadminOptions>();
            foreach (var k in form.AllKeys)
            {
                var o = new ClassadminOptions();
                o.Key = k;
                o.value = form[k];
                o.type = "setting";
                if (k == "trustsetupcost" || k == "trustgst")
                {
                    o.type = "trustcost";
                }
                if (k == "companysetupcost" || k == "companygst" || k == "asicfee")
                {
                    o.type = "companycost";
                }
                data.Add(o);
            }

            var res = AdminMethods.saveOptions(data, uid);
            if (res)
            {
                TempData["settmsg"] = Helper.CreateMessage("Changes saved successfully.", EnumMessageType.Success, "Success");
            }
            else
            {
                TempData["settmsg"] = Helper.CreateMessage("No changes has been saved.", EnumMessageType.Error, "Error");
            }
            return RedirectToAction("settings");
        }

        public ActionResult profile()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            if (AuthHelper.IsValidRequestByRole(new List<string> { "ADMIN", "USER" }))
            {
                var user = UserMethods.GetUserById(uid);
                string email = user.Email;
                string companyid = CompanyMethods.compnydetaILS(email.ToString());
                var comp = CompanyMethods.companyDetails_(companyid.ToString()).FirstOrDefault();
                var ca = CompanyMethods.companyAddress_(companyid.ToString()).ToList();
                ViewBag.email = user.Email;
                ViewBag.phone = user.Phone;
                ViewBag.companyname = comp.CompanyName.ToString().Replace("  ", "");
                ViewBag.address = ca[0].UnitLevel.ToString() + " " + ca[0].Street.ToString() + " " + ca[0].Suburb.ToString() + " " + ca[0].State.ToString() + " Australia";
                ViewBag.msg = "";
                var userdata = new TblUser
                {
                    Id = user.Id,
                    FirstName = user.Firstname,
                    LastName = user.Lastname,
                    LastLogIn = user.Lastlogin,
                    Email = user.Email,
                    Phone = user.Phone,
                    AddedDate = user.AddedDate
                };
                return View(userdata);
            }
            else
            {
                return View("noaccess");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult profile(TblUser form)
        {
            var AdminID = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN" }, "/admin/signin"));
            string msg = "";

            if (!string.IsNullOrEmpty(form.FirstName) && !string.IsNullOrEmpty(form.LastName))
            {
                if (!string.IsNullOrEmpty(form.Email) && Helper.IsValidEmail(form.Email))
                {
                    //var regex = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";//https://stackoverflow.com/questions/28904826/phone-number-validation-mvc

                    if (!string.IsNullOrEmpty(form.Phone))
                    {
                        using (var db = new MyDbContext())
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            var em = CryptoHelper.EncryptData(form.Email);
                            if (!db.TblUsers.Any(x => x.Email == em && x.Id != AdminID && x.Del == false))
                            {
                                var u = new TblUser { Id = AdminID };

                                db.TblUsers.Attach(u);
                                u.FirstName = form.FirstName;
                                u.LastName = form.LastName;
                                u.Phone = form.Phone;
                                u.Email = CryptoHelper.EncryptData(form.Email);
                                var entry = db.Entry(u);
                                entry.Property(e => e.FirstName).IsModified = true;
                                entry.Property(e => e.LastName).IsModified = true;
                                entry.Property(e => e.Email).IsModified = true;
                                entry.Property(e => e.Phone).IsModified = true;
                                var i = db.SaveChanges() > 0;
                                if (i)
                                {
                                    // update user claim for login
                                    var uDataJson = JsonConvert.SerializeObject(form);
                                    var roles = AuthHelper.LoggedInRoles();
                                    AuthHelper.SignIn(form.FirstName, false, roles, AdminID.ToString(), uDataJson);
                                    msg = Helper.CreateMessage("Info updated successfully.", EnumMessageType.Success, "Success");
                                }
                            }
                            else
                            {
                                msg = Helper.CreateMessage("Sorry, But this email is already registered.", EnumMessageType.Error, "Error");
                            }
                        }
                    }
                    else
                    {
                        msg = Helper.CreateMessage("Please enter a valid phone no.", EnumMessageType.Error, "Error");
                    }
                }
                else
                {
                    msg = Helper.CreateMessage("Please enter a valid email address.", EnumMessageType.Error, "Error");
                }
            }
            else
            {
                msg = Helper.CreateMessage("Please enter your name.", EnumMessageType.Error, "Error");
            }
            ViewBag.msg = msg;
            return View(form);
        }

        public ActionResult updatetrust()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN" }, "/admin/signin"));
            if (Request.Cookies["trust-session-id"] != null)
            {
                var c = new HttpCookie("trust-session-id");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            return RedirectToAction("trusts");
        }

        public ActionResult changepassword()
        {
            var AdminID = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            if (TempData["cmsg"] != null)
            {
                ViewBag.msg = TempData["cmsg"];
                TempData["cmsg"] = null;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changepassword(ClassUserPasswordData form)
        {
            var AdminID = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));

            string msg = "";
            if (!string.IsNullOrEmpty(form.newpass) &&
                !string.IsNullOrEmpty(form.confirmpass) &&
                !string.IsNullOrEmpty(form.oldpass))
            {
                if (form.newpass.Length >= 6)
                {
                    if (form.newpass == form.confirmpass)
                    {
                        msg = UserMethods.ChangePassword(form.oldpass, form.newpass, AdminID);
                    }
                    else
                    {
                        msg = Helper.CreateMessage("Password not match.", EnumMessageType.Error, "Error");
                    }
                }
                else
                {
                    msg = Helper.CreateMessage("Password length is not valid , please use atleast 6 characters .", EnumMessageType.Error, "Error");
                }
            }
            else
            {
                msg = Helper.CreateMessage("All fields are mandatory.", EnumMessageType.Error, "Error");
            }
            TempData["cmsg"] = msg;
            return RedirectToAction("changepassword");
        }

		public ActionResult resetuserpassword()
		{
			var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
			if (AuthHelper.IsValidRequestByRole(new List<string> { "ADMIN", "USER" }))
			{
				// ViewBag.Email = uid;
				return View();
			}
			else
			{
				return View("noaccess");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult resetuserpassword(ClassResetUserPasswordData form)
		{
			var AdminID = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
			string email = CryptoHelper.EncryptData(form.useremail);

			string msg = "";
			if (!string.IsNullOrEmpty(form.newpass) &&
				!string.IsNullOrEmpty(form.confirmpass) &&
				!string.IsNullOrEmpty(form.useremail))
			{
				if (form.newpass.Length >= 6)
				{
					using (var db = new MyDbContext())
					{
						var u = db.TblUsers.Where(x => x.Email == email && x.Del == false).FirstOrDefault();

						if (form.newpass == form.confirmpass)
						{
							msg = UserMethods.ResetUserPassword(form.useremail, form.newpass, u.Id);
						}
						else
						{
							msg = Helper.CreateMessage("Password not match.", EnumMessageType.Error, "Error");
						}
					}
				}
				else
				{
					msg = Helper.CreateMessage("Password length is not valid , please use atleast 6 characters .", EnumMessageType.Error, "Error");
				}
			}
			else
			{
				msg = Helper.CreateMessage("All fields are mandatory.", EnumMessageType.Error, "Error");
			}
			ViewBag.msg = msg;
			//return RedirectToAction("resetuserpassword");
			return View();
		}

		public ActionResult adduser()
        {
            var AdminID = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN" }, "/admin/signin"));
            if (!AuthHelper.IsValidRequestByRole(new List<string> { "admin" }))
            {
                return View("noaccess");
            }
            return View();
        }

        public ActionResult adminusers()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN" }, "/admin/signin"));
            if (AuthHelper.IsValidRequestByRole(new List<string> { "ADMIN" }))
            {
                return View();
            }
            else
            {
                return View("noaccess");
            }
        }

        public ActionResult company()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            if (TempData["c_error"] != null)
            {
                ViewBag.msg = TempData["c_error"];
                TempData["c_error"] = null;
            }
            return View();
        }

        public void downloadCertificate_Admin(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            var companyData = new ClassFullCompany();
            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;
                    //if (companyData.TransactionDetail.TransactionStatus == true) // Changing this to ASIC status
                    if (companyData.CompanyMeta.ASICStatus.ToUpper() == "DOCUMENTS ACCEPTED"
                        && companyData.CompanyMeta.BillStatus.ToLower() == "paid"
                        )
                    {
                        string htmlPath = string.Empty, body = "";
                        if (companyData.Company != null)
                        {
                            htmlPath = Server.MapPath("~/Content/deedhtml/company.html");
                            using (StreamReader red = new StreamReader(htmlPath))
                            {
                                body = red.ReadToEnd();
                            }
                            body = BuildCompanyPDF.BuildCertPDF(body, companyData);
                        }
                        var f = companyData.Company.CompanyName.Replace(" ", "-");

                        StringReader sr = new StringReader(body);
                        Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                            pdfDoc.Open();
                            string imageFilePath = Server.MapPath("~/Content/deedhtml/comdeeds.png");
                            Image jpg = Image.GetInstance(imageFilePath);
                            //jpg.ScaleToFit();
                            jpg.ScaleAbsolute(pdfDoc.PageSize.Width - 20, pdfDoc.PageSize.Height - 20);
                            jpg.Alignment = Image.UNDERLYING;
                            jpg.Border = 0;
                            jpg.SetAbsolutePosition(10, 10);
                            jpg.PaddingTop = 0;
                            writer.PageEvent = new ImageBackgroundHelper(jpg);
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                            pdfDoc.Close();
                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();
                            Response.Clear();
                            Response.ContentType = "application/pdf";
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + f + "-Certificate.pdf");
                            Response.Buffer = true;
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.BinaryWrite(bytes);
                            Response.End();
                            Response.Close();
                        }
                    }
                    else
                    {
                        TempData["c_error"] = Helper.CreateNotification("Sorry, there is some problem while downloading the document.", EnumMessageType.Warning, "");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            if (TempData["c_error"] != null)
            {
                string msg = TempData["c_error"].ToString();
                Response.Redirect("UserSearchList.aspx?msg=" + msg, false);
            }
            else
            {
                Response.Redirect("UserSearchList.aspx");
            }
        }

        public ActionResult downloadconstitution(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            string htmlPath = string.Empty, body = "";
            var companyData = new ClassFullCompany();
            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;
                    if (companyData.TransactionDetail.TransactionStatus == true)
                    {
                        htmlPath = Server.MapPath("~/Content/deedhtml/constitution.html");
                        using (StreamReader red = new StreamReader(htmlPath))
                        {
                            body = red.ReadToEnd();
                        }

                        var RegOfcAddModel = companyData.Address.Where(x => x.IsRegisteredAddress == true).FirstOrDefault();
                        string RegOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                            RegOfcAddModel.UnitLevel,
                            RegOfcAddModel.Street,
                            RegOfcAddModel.Suburb,
                            RegOfcAddModel.State,
                            RegOfcAddModel.PostCode);
                        string dirsign = string.Empty;
                        foreach (var d in companyData.Directors)
                        {
                            dirsign += string.Format(@"<p style='margin-bottom:0;'>............................................. <br />[Name], [Signature], Member[Date]</p><br/>");
                        }
                        // replace names
                        body = body.Replace("{companyname}", companyData.Company.CompanyName);
                        body = body.Replace("{acn}", companyData.CompanyMeta.CompanyACN); // Please insert ACN here
                        body = body.Replace("{companyaddress}", RegOfcAdd);
                        body = body.Replace("{directorsign}", dirsign);

                        var f = companyData.Company.CompanyName + "-constitution.pdf";
                        StringReader sr = new StringReader(body);
                        Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                            writer.PageEvent = new ITextEvents();
                            pdfDoc.Open();
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                            pdfDoc.Close();
                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();
                            Response.Clear();
                            Response.ContentType = "application/pdf";
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + f);
                            Response.Buffer = true;
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.BinaryWrite(bytes);
                            Response.End();
                            Response.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return View();
        }

        public ActionResult downloadreginvoice(int id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            var companyData = new ClassFullCompany();
            if (id > 0)
            {
                companyData = CompanyMethods.GetFullCompanyData(id);
                var dt = AuthHelper.GetUserData();
                var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                companyData.user = u;
            }
            if (companyData.TransactionDetail.TransactionStatus == true)
            {
                string htmlPath = string.Empty, body = "";
                if (companyData.Company != null)
                {
                    htmlPath = Server.MapPath("~/Content/deedhtml/companyinvoice.html");
                    using (StreamReader red = new StreamReader(htmlPath))
                    {
                        body = red.ReadToEnd();
                    }
                    var cr = companyData.TransactionDetail;
                    var c = companyData.Company;

                    var member = companyData.Applicant;

                    var m = $"{member.GivenName} {member.FamilyName}";
                    var RegOfcAddModel = companyData.Address.Where(x => x.IsRegisteredAddress == true).FirstOrDefault();
                    string RegOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                        RegOfcAddModel.UnitLevel,
                        RegOfcAddModel.Street,
                        RegOfcAddModel.Suburb,
                        RegOfcAddModel.State,
                        RegOfcAddModel.PostCode);

                    body = body.Replace("{invoiceno}", cr.Id.ToString());
                    body = body.Replace("{date}", cr.AddedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                    body = body.Replace("{username}", m);
                    body = body.Replace("{address}", RegOfcAdd);

                    body = body.Replace("{asicfee}", "$" + companyData.Cost.AsicFee);
                    body = body.Replace("{asictotal}", "$" + companyData.Cost.AsicFee);

                    body = body.Replace("{deedname}", $"Company ({companyData.Company.CompanyName}) - Setup Fee");
                    body = body.Replace("{unitcost}", "$" + companyData.Cost.SetupCost);
                    body = body.Replace("{unittotal}", "$" + companyData.Cost.SetupCost);
                    body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee));
                    body = body.Replace("{gst}", "$" + companyData.Cost.SetupGST);
                    body = body.Replace("{total}", "$" + companyData.Cost.TotalCost);
                    var f = companyData.Company.CompanyName.Replace(" ", "-");
                    createpdf(body, f + "-setup-invoice.pdf");
                }
            }
            return View();
        }

        public void createpdf(string body, string filename)
        {
            StringReader sr = new StringReader(body);
            Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();

                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                Response.Buffer = true;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
                //Response.Close();
            }
        }

        #region Pdf Events

        private class ImageBackgroundHelper : PdfPageEventHelper
        {
            private Image img;

            public ImageBackgroundHelper(Image img)
            {
                this.img = img;
            }

            /**
             * @see com.itextpdf.text.pdf.PdfPageEventHelper#onEndPage(
             *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
             */

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                writer.DirectContentUnder.AddImage(img);
            }
        }

        private class ITextEvents : PdfPageEventHelper
        {
            // This is the contentbyte object of the writer
            private PdfContentByte cb;

            // we will put the final number of pages in a template
            private PdfTemplate footerTemplate;

            // this is the BaseFont we are going to use for the header / footer
            private BaseFont bf = null;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    footerTemplate = cb.CreateTemplate(50, 50);
                }
                catch (DocumentException de)
                {
                    //handle exception here
                }
                catch (System.IO.IOException ioe)
                {
                    //handle exception here
                }
            }

            public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);
                if (writer.PageNumber > 1)
                {
                    iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                    String text = "Page " + (writer.PageNumber - 1) + " of ";
                    //Add paging to footer
                    {
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 10);
                        cb.SetTextMatrix(document.PageSize.GetRight(150), document.PageSize.GetBottom(10));
                        cb.ShowText(text);
                        cb.EndText();
                        float len = bf.GetWidthPoint(text, 10);
                        cb.AddTemplate(footerTemplate, document.PageSize.GetRight(150) + len, document.PageSize.GetBottom(10));
                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, document.PageSize.GetBottom(25));
                        cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(25));
                        cb.Stroke();
                        BaseColor b = new BaseColor(9, 9, 9);
                        cb.SetColorStroke(b);
                    }
                }
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                if (writer.PageNumber > 1)
                {
                    footerTemplate.BeginText();
                    footerTemplate.SetFontAndSize(bf, 10);
                    footerTemplate.SetTextMatrix(0, 0);
                    footerTemplate.ShowText((writer.PageNumber - 1).ToString());
                    footerTemplate.EndText();
                }
            }
        }

        #endregion Pdf Events

        //Rajat added here on 10 oct

        public ActionResult companydetails(long id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
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

        public ActionResult queries()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            return View();
        }

        public ActionResult verifyemail(long id)
        {
            try
            {
                if (id > 0)
                {
                    using (var db = new MyDbContext())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        var u = db.TblUsers.Where(x => x.Id == id).FirstOrDefault();
                        u.EmailVerified = true;
                        var entry = db.Entry(u);
                        entry.Property(e => e.EmailVerified).IsModified = true;
                        int i = db.SaveChanges();
                        if (i > 0)
                        {
                            ViewBag.verified = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.verified = false;
            }

            return View();
        }

        public ActionResult trustdetails(long id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            var trustId = id;
            var trustData = new ClassFullTrust();
            if (trustId > 0)
            {
                trustData = TrustMethods.GetFullTrustDetails(trustId);
            }

            if (TempData["summarymsg"] != null)
            {
                ViewBag.msg = TempData["summarymsg"];
                TempData["summarymsg"] = null;
            }

            return View(trustData);
        }

        public ActionResult noaccess()
        {
            return View();
        }
    }
}