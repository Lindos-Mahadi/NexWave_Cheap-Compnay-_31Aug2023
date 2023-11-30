using comdeeds.App_Code;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static comdeeds.Models.BaseModel;

namespace comdeeds.Controllers
{
    public class ComMainController : BaseController
    {
        static public string TrustCookieId = "trust-session-id"; // tbl trust id

        #region Site pages

        public ActionResult ThankYou(string utm_t)
        {
            ViewBag.type = utm_t;
            return View();
        }

        public ActionResult GoogleAdd()
        {
            return View();
        }

        public ActionResult knockoutAdd()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            //string CompanyCookieId = "company-session-id";
            //var c = new HttpCookie(CompanyCookieId, "2117");
            //c.Expires = DateTime.Now.AddMonths(1);
            //this.ControllerContext.HttpContext.Response.Cookies.Add(c);
            //string cookie = "There is no cookie!";
            // if(this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(CompanyCookieId))
            // {
            //     cookie = "Yeah - Cookie: " + this.ControllerContext.HttpContext.Request.Cookies[CompanyCookieId].Value;
            // }

            //    var m = new Class_mailer
            //    {
            //        fromEmail = "support@comdeeds.com.au",
            //        fromName = "Comdeeds",
            //        HtmlBody = "<p>Hi, this is a test</p>",
            //        subject = "Test Subject ",
            //        toMail = "teach.msp@gmail.com"
            //    };

            //    var res = EmailHelper.SendSmtpMail(m);
            return View();
        }

        //[HttpGet]
        //public ActionResult php()
        //{
        //    return Redirect("index.php");
        //}

        public ActionResult aboutus()
        {
            return View();
        }

        public ActionResult faq()
        {
            return View();
        }

        [ActionName("privacy-policy")]
        public ActionResult privacypolicy()
        {
            return View();
        }

        [ActionName("terms-and-conditions")]
        public ActionResult termscondition()
        {
            return View();
        }

        public ActionResult contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult contact(TblContact c)
        {
            string msg = "";
            if (!string.IsNullOrEmpty(c.Name))
            {
                if (!string.IsNullOrEmpty(c.Email))
                {
                    if (Helper.IsValidEmail(c.Email))
                    {
                        using (var db = new MyDbContext())
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            c.AddedDate = DateTime.Now;
                            db.TblContacts.Add(c);
                            int i = db.SaveChanges();
                            if (i > 0)
                            {
                                c.Name = ""; c.Email = ""; c.Subject = ""; c.Message = "";
                                msg = Helper.CreateNotification("Success, Your request has been submitted ,We'll back to you very soon.", EnumMessageType.Success);
                                ModelState.Clear();
                            }
                        }
                    }
                    else
                    {
                        msg = Helper.CreateNotification("Please enter a valid email address.", EnumMessageType.Error);
                    }
                }
                else
                {
                    msg = Helper.CreateNotification("Please enter your email address.", EnumMessageType.Error);
                }
            }
            else
            {
                msg = Helper.CreateNotification("Please enter your name.", EnumMessageType.Error);
            }
            ViewBag.msg = msg;
            return View(c);
        }

        #endregion Site pages

        #region company setup

        private dal.Operation oper = new dal.Operation();

        public string getuserid(string uid)
        {
            string useremail = "";
            DataTable dtuser = oper.get_userdetails_byuid(uid.ToString());
            if (dtuser.Rows.Count > 0)
            {
                useremail = CryptoHelper.DecryptString(dtuser.Rows[0]["Email"].ToString());
            }
            return useremail;
        }

        //[ActionName("company-setup")]
        //public ActionResult CompanySetup()
        //{
        //    comdeeds.Models.BaseModel.ClassUserDetails user = new comdeeds.Models.BaseModel.ClassUserDetails();
        //    var isauth = App_Code.AuthHelper.IsValidRequestByRole(new List<string> { "USER" });
        //    ViewBag.openpnl = 1;
        //    if (isauth)
        //    {
        //        var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER" }, "/user/signin"));
        //        user = UserMethods.GetUserById(uid);
        //        ViewBag.openpnl = 2;
        //    }
        //    ViewBag.isauth = isauth;
        //    if (TempData["registermsg"] != null)
        //    {
        //        ViewBag.msg = TempData["registermsg"];
        //        TempData["registermsg"] = null;
        //    }
        //    return View(user);
        //}
        [ActionName("company-setup")]
        public ActionResult CompanySetup(string cname)
        {
            if (Request.QueryString.Get("cname") != null)
            {
                cname = Server.UrlDecode(Request.QueryString.Get("cname").ToString());
            }

            string CompanyCookieId = "company-session-id";

            if (cname != null)
            { Session["cname"] = cname.ToString(); }
            else
            {
                if (Request.Cookies["company-session-id"] != null)
                {
                    Response.Cookies["company-session-id"].Expires = DateTime.Now.AddDays(-1);
                }
                //CompanyCookieId = "";
            }

            try
            {
                int n;
                bool isNumeric = int.TryParse(cname, out n);
                if (isNumeric == true)
                {
                    Response.Cookies[CompanyCookieId].Expires = DateTime.Now.AddDays(-1);
                    var c = new HttpCookie(CompanyCookieId, cname);
                    c.Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Add(c);
                    string cookie = "There is no cookie!";
                    if (Request.Cookies.AllKeys.Contains(CompanyCookieId))
                    {
                        cookie = "Yeah - Cookie: " + this.Request.Cookies[CompanyCookieId].Value;
                        //Response.Redirect("../company-setup", false);
                    }
                }
            }
            catch (Exception ex) { }

            // for admin to continue form
            if (Request.QueryString["type"] == "admin")
            {
                if (AuthHelper.IsValidRequestByRole(new List<string> { "ADMIN", "USER", "SUBUSER" }))
                {
                    var companyid = Request.QueryString["continue"];
                    long i = 0;
                    if (long.TryParse(companyid, out i))
                    {
                        var c = new HttpCookie(CompanyCookieId, companyid);
                        c.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(c);
                    }
                    ViewBag.adminEdit = true;
                }
            }
            else if (Request.QueryString["type"] == "company")
            {
                if (AuthHelper.IsValidRequestByRole(new List<string> { "USER", "SUBUSER" }))
                {
                    var companyid = Request.QueryString["continue"];
                    long i = 0;
                    if (long.TryParse(companyid, out i))
                    {
                        var c = new HttpCookie(CompanyCookieId, companyid);
                        c.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(c);
                    }
                    ViewBag.adminEdit = true;
                }
            }
            // for admin to continue form

            comdeeds.Models.BaseModel.ClassUserDetails user = new comdeeds.Models.BaseModel.ClassUserDetails();
            var isauth = App_Code.AuthHelper.IsValidRequestByRole(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" });
            ViewBag.openpnl = 1;
            if (isauth)
            {
                var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
                user = UserMethods.GetUserById(uid);
                ViewBag.openpnl = 2;
            }
            ViewBag.isauth = isauth;
            ViewBag.cn = cname;
            if (TempData["registermsg"] != null)
            {
                ViewBag.msg = TempData["registermsg"];
                TempData["registermsg"] = null;
            }
            return View(user);
        }

        [HttpPost]
        [ActionName("company-setup")]
        public ActionResult CompanySetup(comdeeds.Models.BaseModel.ClassUserDetails form, string hdncname)
        {
            string msg = ""; string cname1 = "";
            msg = UserMethods.RegisterUserThruForm(form);
            TempData["registermsg"] = msg;
            ViewBag.cn = hdncname;

            if (Session["cname"] != null)
                cname1 = Session["cname"].ToString();

            if (hdncname != "" && hdncname != null)
                return RedirectToAction("company-setup", new { cname = hdncname });
            else if (cname1 != "")
                return RedirectToAction("company-setup", new { cname = cname1 });
            else
                return RedirectToAction("company-setup");
        }

        public ActionResult PaymentStatus()
        {
            //var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));

            //DataTable dtuser = oper.get_userdetails_byuid(uid.ToString());
            //if (dtuser.Rows.Count > 0)
            //{
            //    string userType = dtuser.Rows[0]["_Role"].ToString();
            //    if (userType.ToLower() == "admin" || userType.ToLower() == "subadmin")
            //    { ViewBag.usertype = "admin"; }
            //    else
            //    { ViewBag.usertype = "user"; }
            //}

            return View();
        }

        public ActionResult EdgeProcessing()
        {
            //return RedirectToAction("Bypas");
            return View();
        }

        public ActionResult Bypas()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string useremail = getuserid(uid.ToString());
            var companyId = 0.0;
            if (Session["201formId"] != null)
            {
                companyId = Convert.ToInt64(Session["201formId"].ToString());
            }
            else { companyId = GetCompanyCookieId(); }
            if (companyId > 0)
            {
                Response.Redirect("Form201Web.aspx?CompanyID=" + companyId.ToString() + "&Email=" + useremail, false);
            }

            return View();
        }

        public ActionResult companysummary()

        {
            string cid = "";
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string useremail = getuserid(uid.ToString());
            if (Session["EditOption"] != null)
            {
                ViewBag.edit = true;
            }
            else
            {
                ViewBag.edit = false;
            }
            if (Request.QueryString["id"] != null)
            { cid = Request.QueryString["id"].ToString(); }

            var companyData = new comdeeds.Models.BaseModel.ClassFullCompany();
            ViewBag.isvalid = true;
            try
            {
                var companyId = GetCompanyCookieId();
                if (companyId > 0)
                {
                    ViewBag.companyidd = companyId;
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<comdeeds.Models.BaseModel.LoginUserData>(dt);
                    companyData.user = u;

                    #region For test Case

                    string datastep1 = oper.Fill1_forsummary(companyId.ToString());
                    ViewBag.step1 = datastep1;
                    DataTable dtchk = oper.getcompanysearchbyid(companyId.ToString());
                    if (dtchk.Rows.Count > 0)
                    {
                        string paymentstatus = dtchk.Rows[0]["status"].ToString().ToLower();
                        if (paymentstatus == "paid")
                        {
                            ViewBag.visisubmit = false;
                        }
                        else
                        {
                            ViewBag.visisubmit = true;
                        }
                    }

                    #endregion For test Case
                }
                else if (cid != "")
                {
                    ViewBag.companyidd = cid;
                    long ccid = Convert.ToInt64(cid);
                    companyData = CompanyMethods.GetFullCompanyData(ccid);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<comdeeds.Models.BaseModel.LoginUserData>(dt);
                    companyData.user = u;

                    string datastep1 = oper.Fill1_forsummary(cid.ToString());
                    ViewBag.step1 = datastep1;
                    DataTable dtchk = oper.getcompanysearchbyid(cid.ToString());
                    if (dtchk.Rows.Count > 0)
                    {
                        string paymentstatus = dtchk.Rows[0]["status"].ToString().ToLower();
                        if (paymentstatus == "paid")
                        {
                            ViewBag.visisubmit = false;
                        }
                        else
                        {
                            ViewBag.visisubmit = true;
                        }
                    }
                }

                if (TempData["compsummarymsg"] != null)
                {
                    ViewBag.msg = TempData["compsummarymsg"];
                    TempData["compsummarymsg"] = null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog objerro = new ErrorLog();
                objerro.WriteErrorLog(ex.ToString());
                ViewBag.isvalid = false;
                ViewBag.msg = Helper.CreateNotification("Sorry, something is not right for this request, please try again", EnumMessageType.Error, "Error");
            }
            return View(companyData);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult companysummary(comdeeds.Models.BaseModel.ClassTrustOption form, string submit)
        {
            var res = false;
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            if (form.chkagreement)
            {
                var companId = GetCompanyCookieId();
                if (companId > 0)
                {
                    res = CompanyMethods.UpdateCompanyOption(form, companId, uid);
                }
                if (submit == "lodge")
                {
                    Session["201formId"] = companId;
                    return RedirectToAction("EdgeProcessing");
                }
                else
                {
                    return RedirectToAction("trustpayment", new { utm_pf = "company" });
                }
            }
            else
            {
                TempData["compsummarymsg"] = App_Code.Helper.CreateNotification("Please agree with our terms and conditions to continue.", EnumMessageType.Error, "Error");
                return RedirectToAction("companysummary");
            }
        }

        #endregion company setup

        #region Trust setup

        public ActionResult trustsetup()
        {
            ClassUserDetails user = new ClassUserDetails();
            try
            {
                if (Request.QueryString["continue"] != null && Request.QueryString["type"] != null)
                {
                    if (Request.QueryString["type"] == "trust")
                    {
                        if (AuthHelper.IsValidRequestByRole(new List<string> { "USER", "SUBUSER" }))
                        {
                            var trustid = Request.QueryString["continue"];
                            int i = 0;
                            if (int.TryParse(trustid, out i))
                            {
                                var c = new HttpCookie(TrustCookieId, trustid);
                                c.Expires = DateTime.Now.AddMonths(6);
                                Response.Cookies.Add(c);
                            }

                            if (!TrustMethods.ConfirmCheckout(Convert.ToInt64(trustid), "trust"))
                            {
                                ViewBag.PayDone = true;
                            }
                        }
                    }
                    if (Request.QueryString["type"] == "admin")
                    {
                        if (AuthHelper.IsValidRequestByRole(new List<string> { "ADMIN" }))
                        {
                            var trustid = Request.QueryString["continue"];
                            int i = 0;
                            if (int.TryParse(trustid, out i))
                            {
                                var c = new HttpCookie(TrustCookieId, trustid);
                                c.Expires = DateTime.Now.AddDays(1);
                                Response.Cookies.Add(c);
                            }
                            ViewBag.adminEdit = true;
                        }
                    }
                }


                var isauth = App_Code.AuthHelper.IsValidRequestByRole(new List<string> { "USER", "SUBUSER" });
                if (isauth)
                {
                    var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
                    user = UserMethods.GetUserById(uid);
                    var u = AuthHelper.GetUserData();
                    var userdata = JsonConvert.DeserializeObject<LoginUserData>(u);
                    user.Email = userdata.email;
                    var trustId = GetTrustCookieId();
                    if (trustId > 0)
                    {
                        var trust = TrustMethods.GetTrustDetail(trustId);
                        user.Firstname = trust.FirstName;
                        user.Lastname = trust.LastName;
                        user.Phone = trust.Phone;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(userdata.FirstName))
                        {
                            user.Firstname = userdata.FirstName;
                            user.Lastname = userdata.LastName;
                            user.Phone = userdata.Phone;
                        }
                    }
                }
                ViewBag.isauth = isauth;
                ViewBag.openpnl = 1;
                if (TempData["trustmsg"] != null)
                {
                    ViewBag.msg = TempData["trustmsg"];
                    TempData["trustmsg"] = null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog elog = new ErrorLog();
                elog.WriteErrorLog("trustsetup-line469-" + ex.ToString());
                Console.Write("trustsetup-" + ex);
            }

            //if (Session["lawyer"] == "Lawyer")
            return View(user);
        }

        [HttpPost]
        public ActionResult trustsetup(ClassUserDetails form)
        {
            string msg = "";
            msg = UserMethods.RegisterUserThruForm(form);
            TempData["trustmsg"] = msg;
            return RedirectToAction("trustsetup");
        }

        public ActionResult trustsummary()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "USER", "SUBUSER" }, "/user/signin"));
            var trustId = GetTrustCookieId();
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult trustsummary(ClassTrustOption form)
        {
            var res = false;
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "USER", "SUBUSER" }, "/user/signin"));
            if (form.chkagreement)
            {
                var trustId = GetTrustCookieId();
                if (trustId > 0)
                {
                    res = TrustMethods.UpdateTrustOption(form, trustId, uid);
                }

                if (res)
                {
                    return RedirectToAction("trustpayment", new { utm_pf = "trust" });
                }
                else
                {
                    TempData["summarymsg"] = App_Code.Helper.CreateNotification("Server error, please try again.", EnumMessageType.Error, "Error");
                    return RedirectToAction("trustsummary");
                }
            }
            else
            {
                TempData["summarymsg"] = App_Code.Helper.CreateNotification("Please agree with our terms and conditions to continue.", EnumMessageType.Error, "Error");
                return RedirectToAction("trustsummary");
            }
        }

        #endregion Trust setup

        #region payment

        public ActionResult trustpayment(string utm_pf)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            ViewBag.type = utm_pf;
            string useremail = getuserid(uid.ToString());
            if (!string.IsNullOrEmpty(utm_pf))
            {
                ClassPaymentFormData data = new ClassPaymentFormData();
                if (utm_pf == "trust")
                {
                    var trustId = GetTrustCookieId();
                    if (trustId > 0)
                    {
                        data = TrustMethods.GetPaymentDetail(trustId, "t", useremail);
                    }
                }
                if (utm_pf == "company")
                {
                    var compId = GetCompanyCookieId();
                    if (compId > 0)
                    {
                        data = TrustMethods.GetPaymentDetail(compId, "c", useremail);
                    }
                }

                if (TempData["paymsg"] != null)
                {
                    ViewBag.msg = TempData["paymsg"];
                    TempData["paymsg"] = null;
                }
                return View(data);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult trustpayment(ClassPaymentform form)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string useremail = getuserid(uid.ToString());
            var msg = ""; bool iserror = false;
            if (!string.IsNullOrEmpty(form.hftype))
            {
                if (true)
                {
                    if (TrustMethods.ConfirmCheckout(Convert.ToInt64(form.formid), form.hftype))
                    {
                        var opt = OptionMethods.GetAllOptions();
                        if (opt.Any(x => x.OptionName == "paymentapikey" && x.Type == "setting"))
                        {
                            var apikey = opt.Where(x => x.OptionName == "paymentapikey" && x.Type == "setting").FirstOrDefault().OptionValue;
                            //PinService ps = new PinService(apikey);
                            // https://pin.net.au/docs/api/test-cards
                            // 5520000000000000 - Mastercard
                            // 4200000000000000 - Visa

                            //Stripe.CardService st = new Stripe.CardService(); 
                            // using System.Net;
                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                            //var card = new Models.Card();
                            //card.CardNumber = form.txtcno;
                            //card.CVC = form.txtcvc;
                            //card.ExpiryMonth = form.txtem;
                            //card.ExpiryYear = form.txtey;              //(DateTime.Today.Year + 1).ToString(); // Not my defaults!
                            //card.Name = form.txtname;
                            //card.Address1 = form.txtadd1;
                            //card.Address2 = form.txtadd2;
                            //card.City = form.txtcity;
                            //card.Postcode = form.txtpostcode;
                            //card.State = form.txtstate;
                            //card.Country = form.txtcountry;

                            StripeConfiguration.ApiKey = apikey;

                            var token = new TokenCreateOptions
                            {
                                Card = new TokenCardOptions
                                {
                                    Number = form.txtcno,
                                    ExpMonth = Convert.ToInt32(form.txtem),
                                    ExpYear = Convert.ToInt32(form.txtey),
                                    Cvc = form.txtcvc,
                                    Name = form.txtname,
                                    AddressLine1 = form.txtadd1,
                                    AddressLine2 = form.txtadd2,
                                    AddressCity = form.txtcity,
                                    AddressZip = form.txtpostcode,
                                    AddressState = form.txtstate,
                                    AddressCountry = form.txtcountry,
                                    Currency = "AUD"


                                },
                            };

                            try
                            {
                                Stripe.TokenService serviceToken = new Stripe.TokenService();
                                Stripe.Token newToken = serviceToken.Create(token);

                                //Create Customer Object and Register it on Stripe  
                                Stripe.CustomerCreateOptions myCustomer = new Stripe.CustomerCreateOptions();
                                myCustomer.Email = form.txtemail;
                                myCustomer.Source = newToken.Id;
                                var customerService = new Stripe.CustomerService();
                                Stripe.Customer stripeCustomer = customerService.Create(myCustomer);

                                //Create Charge Object with details of Charge  
                                var options = new Stripe.ChargeCreateOptions
                                {
                                    Amount = Convert.ToInt64(form.txtcost),
                                    Currency = "AUD",
                                    ReceiptEmail = form.txtemail,
                                    Customer = stripeCustomer.Id,
                                    Description = form.desc, //Optional  
                                };
                                //and Create Method of this object is doing the payment execution.  
                                var service = new Stripe.ChargeService();
                                Stripe.Charge charge = service.Create(options); // This will do the Payment 

                                //var response = ps.Charge(new PostCharge { Amount = Convert.ToInt64(form.txtcost), Card = card, Currency = "AUD", Description = form.desc, Email = form.txtemail });
                                //if(response.Charge.Success)
                                if (true)
                                {
                                    var txn = new TblTransaction
                                    {
                                        AddedBy = uid,
                                        AddedDate = DateTime.Now,
                                        Amount = Convert.ToDouble(form.txtcost),
                                        FormType = form.hftype,
                                        TransactionStatus = true,
                                        TrustCompanyId = form.formid,
                                        UpdatedBy = uid,
                                        UpdatedDate = DateTime.Now,
                                        FormName = form.deedname,
                                        //TxnId = response.Token
                                        TxnId = charge.BalanceTransactionId
                                    };

                                    TrustMethods.addTransaction(txn);
                                    TempData["paymsg"] = Helper.CreateNotification("Thank you for your payment. you can download your documents now.", EnumMessageType.Success, "Success");

                                    // Important - remove form id from cookie
                                    if (form.hftype == "company")
                                    {
                                        if (Request.Cookies["company-session-id"] != null)
                                        {
                                            //Response.Cookies["company-sessio-id"].Expires = DateTime.Now.AddDays(-1);
                                            var c = new HttpCookie("company-session-id");
                                            c.Expires = DateTime.Now.AddDays(-1);
                                            Response.Cookies.Add(c);
                                            ///update payment now
                                            int paYm = oper.updatePayment_companySearch(form.formid.ToString(), useremail);

                                            Session["201formId"] = form.formid;
                                            var u = JsonConvert.DeserializeObject<LoginUserData>(AuthHelper.GetUserData());
                                            var body = "";

                                            using (StreamReader red = new StreamReader(HttpContext.Server.MapPath("~/Content/EmailTemplate/companyconfirmation.html")))
                                            {
                                                body = red.ReadToEnd();
                                            }
                                            body = body.Replace("{companyname}", form.deedname);


                                            var mailerTesting = new Class_mailer
                                            {
                                                fromEmail = "infocomdeeds@gmail.com",
                                                fromName = "Cheapcompanysetup",
                                                //  HtmlBody = "Hi Team,<br/><br/>New client is tryig to register a New Company - " + DateTime.Now + "<br/><br/> Just click on it <a href='comdeeds.com.au/admin'>comdeeds.com.au/admin </a>to go with admin panel. <br/><br/> Support By <br/>Comdeeds Production Server <br/> <img src='https://comdeeds.com.au/Content/images/logo.jpg' style='width: 250px;clear: both; float: left;margin: 22px 10px;'/>",
                                                HtmlBody = body,
                                                subject = "Company Registered - Cheapcompanysetup",
                                                toMail = u.email//"deepak.dubey@gmail.com"
                                            };

                                            var mailerProduction = new Class_mailer
                                            {
                                                fromEmail = "support@comdeeds.com.au",
                                                fromName = "Cheapcompanysetup",
                                                HtmlBody = body,
                                                subject = "Company Registered - Cheapcompanysetup",
                                                toMail = u.email
                                            };

                                            //EmailHelper.SendSmtpMail1(mailerTesting);
                                            EmailHelper.SendSmtpMail(mailerProduction);
                                            return RedirectToAction("PaymentStatus");
                                        }
                                    }
                                    if (form.hftype == "trust")
                                    {
                                        if (Request.Cookies["trust-session-id"] != null)
                                        {
                                            var c = new HttpCookie("trust-session-id");
                                            c.Expires = DateTime.Now.AddDays(-1);
                                            Response.Cookies.Add(c);
                                        }

                                        // send confirmation mail
                                        var href = Helper.GetBaseURL() + "/user/order/trust";
                                        // get user data from Auth
                                        var u = JsonConvert.DeserializeObject<LoginUserData>(AuthHelper.GetUserData());
                                        var body = "";
                                        using (StreamReader red = new StreamReader(HttpContext.Server.MapPath("~/Content/EmailTemplate/deedconfirmation.html")))
                                        {
                                            body = red.ReadToEnd();
                                        }
                                        body = body.Replace("{trustname}", form.deedname);
                                        body = body.Replace("{deedhref}", href);

                                        var mailerProduction = new Class_mailer
                                        {
                                            fromEmail = "support@comdeeds.com.au",
                                            fromName = "Cheapcompanysetup",
                                            HtmlBody = body,
                                            subject = "Trust deed created - Cheapcompanysetup",
                                            toMail = u.email
                                        };

                                        var mailerTesting = new Class_mailer
                                        {
                                            fromEmail = "infocomdeeds@gmail.com",
                                            fromName = "Comdeeds",
                                            HtmlBody = body,
                                            subject = "New Trust created - Cheapcompanysetup",
                                            toMail = u.email// "deepak.dubey@gmail.com"
                                        };

                                        //EmailHelper.SendSmtpMail1(mailerTesting);
                                        EmailHelper.SendSmtpMail(mailerProduction);
                                        return RedirectToAction("ThankYou", new { utm_t = "t" });
                                    }
                                }
                                else
                                {
                                    TempData["paymsg"] = Helper.CreateNotification("There is an error occur while processing this payment, Please try again. ", EnumMessageType.Error, "Error");
                                }
                            }
                            catch (Exception ex)
                            {
                                TempData["paymsg"] = Helper.CreateNotification("There is an error occur while processing this payment, Please try again. ", EnumMessageType.Error, "Error");
                            }
                        }
                    }
                    else
                    {
                        TempData["paymsg"] = Helper.CreateNotification("You have already paid for this item. Please check your orders. ", EnumMessageType.Warning, "Warning");
                    }
                }
                else
                {
                    TempData["paymsg"] = Helper.CreateNotification("Please enter a valid card number. ", EnumMessageType.Error, "Error");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("trustpayment", new { utm_pf = form.hftype });
        }

        #endregion payment

        #region Controller helper - Do not change this

        public long GetTrustCookieId()
        {
            var cookie = Request.Cookies["trust-session-id"];//.FirstOrDefault();
            return cookie == null ? 0 : Convert.ToInt64(cookie.Value);
        }

        public long GetCompanyCookieId()
        {
            var cookie = Request.Cookies["company-session-id"];//.FirstOrDefault();
            return cookie == null ? 0 : Convert.ToInt64(cookie.Value);
        }

        public static bool IsValidCreditCard(string creditCardNumber)
        {
            //// check whether input string is null or empty
            if (string.IsNullOrEmpty(creditCardNumber))
            {
                return false;
            }

            //// 1.	Starting with the check digit double the value of every other digit
            //// 2.	If doubling of a number results in a two digits number, add up
            ///   the digits to get a single digit number. This will results in eight single digit numbers
            //// 3. Get the sum of the digits
            int sumOfDigits = creditCardNumber.Where((e) => e >= '0' && e <= '9')
                            .Reverse()
                            .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                            .Sum((e) => e / 10 + e % 10);

            //// If the final sum is divisible by 10, then the credit card number
            //   is valid. If it is not divisible by 10, the number is invalid.
            return sumOfDigits % 10 == 0;
        }

        #endregion Controller helper - Do not change this

        //public ActionResult test()
        //{
        //    Session["201formId"] = 10;
        //    return RedirectToAction("Index", "Lodge201");
        //}

        //for ASIC fail
        public ActionResult SetupError()
        {
            ViewBag.msg = App_Code.Helper.CreateNotification("There is some error while connecting to ASIC", EnumMessageType.Error, "Error");
            return View();
        }

        //For ASIC success

        public ActionResult setupcomplete()
        {
            ViewBag.msg = App_Code.Helper.CreateNotification("Your certificate is completed, please login to download it.", EnumMessageType.Success, "Success");
            return View();
        }
    }
}