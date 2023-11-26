using comdeeds.App_Code;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using static comdeeds.Models.BaseModel;

namespace comdeeds.Areas.Admin.Controllers
{
    public class AdminApiController : ApiController
    {

        ErrorLog errorLog = new ErrorLog();

        [HttpGet]
        [ActionName("gettrusts")]
        public dynamic gettrusts(string sessionData)
        {
            var subuserid = "";
            if (sessionData != null || sessionData != "")
            {
                subuserid = sessionData.Trim().Replace('"', ' ').Trim();
            }

            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            RequestGridParam id = getparam(this.Request);
            ClassGridTrustResult res = new ClassGridTrustResult();
            int start = (id.limit * id.page) - id.limit;
            start = start == 0 ? 1 : start;
            //id.sortBy = "";

            string ob = ""; 
            if (!string.IsNullOrEmpty(id.sortBy) && !string.IsNullOrEmpty(id.direction))
            {
                ob = id.direction.ToLower() + ' ' + id.sortBy.ToLower();
            }
            id.search = string.IsNullOrEmpty(id.search) ? "" : id.search;

        
            var p = new ClassSqlGridParam { startLength = start, length = id.limit, orderBy = ob };
            var trusts = AdminMethods.getAdminTrustList(p, id.search, subuserid);

            //if (!string.IsNullOrWhiteSpace(id.search))
            //{
            //    trusts.data = trusts.data.Where(x => id.search.Contains(x.TrustName)).ToList();
            //}

            return new
            {
                records = trusts.data,
                total = trusts.Total
            };
        }

        [HttpGet]
        [ActionName("getcompany")]
        public dynamic getcompany()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            RequestGridParam id = getparam(this.Request);
            ClassGridCompanyResult res = new ClassGridCompanyResult();
            int start = (id.limit * id.page) - id.limit;
            start = start == 0 ? 1 : start;
            id.sortBy = "";
            var p = new ClassSqlGridParam { startLength = start, length = id.limit, orderBy = id.sortBy };
            var comp = AdminMethods.getAdminCompanyList(p);

            if (!string.IsNullOrWhiteSpace(id.search))
            {
                comp.data = comp.data.Where(x => id.search.Contains(x.CompanyName)).ToList();
            }

            return new
            {
                records = comp.data,
                total = comp.Total
            };
        }

        [HttpGet]
        [ActionName("getusers")]
        public dynamic getusers()
        {
           
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            RequestGridParam id = getparam(this.Request);
            int start = (id.limit * id.page) - id.limit;
            start = start == 0 ? 1 : start;
            id.sortBy = "";
            var p = new ClassSqlGridParam { startLength = start, length = id.limit, orderBy = id.sortBy };
            var users = AdminMethods.getUsersList(p);

            if (!string.IsNullOrWhiteSpace(id.search))
            {
                users.users = users.users.Where(x => id.search.Contains(x.Firstname)).ToList();
            }

            return new
            {
                records = users.users,
                total = users.Total
            };
        }


        [HttpGet]
        [ActionName("getPayments")]
        public dynamic getPayments()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN","USER" }, "/admin/signin"));
            RequestGridParam id = getparam(this.Request);
            int start = (id.limit * id.page) - id.limit;
            start = start == 0 ? 1 : start;
            id.sortBy = "";

            var p = new ClassSqlGridParam { startLength = start, length = id.limit, orderBy = id.sortBy, userid= uid };
            var paymetns = AdminMethods.getPaymentList(p);

            return new
            {
                records = paymetns.Payment,
                total = paymetns.Total
            };
        }

        public dynamic adduser(ClassUserForm form)
        {
            string msg = "";
            try
            {
                var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN" }, ""));
                long userIdNew = 0;
                string emailLink = "";
                form.userRole = !string.IsNullOrEmpty(form.userRole) ? form.userRole.ToUpper() : "USER";
                if (!string.IsNullOrEmpty(form.firstname))
                {
                    if (!string.IsNullOrEmpty(form.email) && !string.IsNullOrEmpty(form.password))
                    {
                            if (Helper.IsValidEmail(form.email))
                            {
                                var userR = new Registration
                                {
                                    GivenName = form.firstname + form.lastname,
                                    Email = form.email,
                                    Pass = form.password,
                                    Phone = form.phone,
                                    Registrationdate = DateTime.Now,

                                };

                                using (var db = new MyDbContext())
                                {

                                    if (!db.Registrations.Any(x => x.Email == userR.Email))
                                    {
                                      var uctc= db.Registrations.Add(userR);
                                        msg = db.SaveChanges() > 0 ? "success" : "error";
                                        userIdNew = uctc.Sno;
                                    }
                                    else
                                    {
                                        msg = "exists";
                                    }
                                }



                                var user = new TblUser
                                {
                                    FirstName = form.firstname,
                                    LastName = form.lastname,
                                    Email = CryptoHelper.EncryptData(form.email),
                                    Password = CryptoHelper.EncryptData(form.password),
                                    Phone = form.phone,
                                    AddedDate = DateTime.Now,
                                    Role = form.userRole.ToUpper(),
                                    EmailVerified = !form.emailoption,
                                    AddedBy = uid,
                                    Regid= userIdNew
                                };

                            using (var db = new MyDbContext())
                            {
                                if (!db.TblUsers.Any(x => x.Email == user.Email && x.Del == false))
                                {
                                    var uctx = db.TblUsers.Add(user);
                                    msg = db.SaveChanges() > 0 ? "success" : "error";
                                    userIdNew = uctx.Id;
                                }
                                else
                                {
                                    msg = "exists";
                                }
                            }
                        }
                        else
                        {
                            msg = "invalidemail";
                        }

                    }
                    else
                    {
                        msg = "emailpasserr";
                    }
                }
                else
                {
                    msg = "nameerr";
                }

                // Send Email
                if (form.emailoption && msg == "success")
                {
                    using (var db = new MyDbContext())
                    {
                        //db.Configuration.AutoDetectChangesEnabled = false;
                        //var option = db.TblOptions.AsNoTracking().Where(x => x.OptionType.ToLower() == "setting").ToList();
                        //var valid_till = option.Any(x => x.OptionName == "link_expire_time") ?
                        //     Convert.ToInt32(option.Where(x => x.OptionName == "link_expire_time").FirstOrDefault().OptionValue) : 1;
                        //var emailnoti = new TblEmailNotification()
                        //{
                        //    Case = "VERIFY_EMAIL",
                        //    AddedDate = DateTimeHelpers.Get_IST_time(),
                        //    Del = false,
                        //    ValidTill = valid_till
                        //};

                        //emailnoti.UserId = Convert.ToInt64(userIdNew);
                        //var ectx = db.TblEmailNotifications.Add(emailnoti);
                        //db.SaveChanges();

                        emailLink = Helper.GetBaseURL() + "/admin/verifyemail?id=" + userIdNew;
                        var body = "";
                        using (System.IO.StreamReader red = new System.IO.StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/deedhtml/Create_user_admin.html")))
                        {
                            body = red.ReadToEnd();
                        }
                        body = body.Replace("{user}", form.firstname);
                        body = body.Replace("{email}", form.email);
                        body = body.Replace("{password}", form.password);
                        body = body.Replace("{loginlink}", emailLink);
                        var mailer = new Class_mailer
                        {
                            fromEmail = "support@comdeeds.com.au",
                            fromName = "Comdeeds",
                            HtmlBody = body,
                            subject = "New account added- www.comdeeds.com.au",
                            toMail = form.email
                        };
                        EmailHelper.SendSmtpMail(mailer);
                        msg = "success";
                    }
                }
              
            }
            catch (Exception ex)
            {
                errorLog.WriteErrorLog(ex.ToString()); 
              
            }
            return new { msg = msg };
        }




        [HttpGet]
        [ActionName("getadminusers")]
        public dynamic getadminusers()
        {          
                var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN" }, "/admin/signin"));
                RequestGridParam id = getparam(this.Request);
                int start = (id.limit * id.page) - id.limit;
                start = start == 0 ? 1 : start;
                id.sortBy = "";
                var p = new ClassSqlGridParam { startLength = start, length = id.limit, orderBy = id.sortBy };
                var users = AdminMethods.getadminUsersList(p);

                if (!string.IsNullOrWhiteSpace(id.search))
                {
                    users.users = users.users.Where(x => id.search.Contains(x.Firstname)).ToList();
                }

                return new
                {
                    records = users.users,
                    total = users.Total
                };
        }


        [HttpPost]
        [ActionName("delcompany")]
        public dynamic delcompany(List<long> id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN" }, ""));
            var msg = "error";
            using (var db = new MyDbContext())
            {
                string xml = "";
                foreach (var o in id)
                {
                    xml += $"<Entity><Id>{o}</Id></Entity>";
                }
                xml = $"<DataSet>{xml}</DataSet>";
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@xml", xml, DbType.Xml);
                var _postid = db.Database.Connection.Query<long>("delcompany", p, commandType: CommandType.StoredProcedure).ToList();
                msg = _postid.Count > 0 ? "success" : "error";
            }
            return new { msg = msg };
        }


        [HttpPost]
        [ActionName("deltrusts")]
        public dynamic deltrusts(List<long> id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN" }, ""));
            var msg = "error";
            using (var db = new MyDbContext())
            {
                string xml = "";
                foreach (var o in id)
                {
                    xml += $"<Entity><Id>{o}</Id></Entity>";
                }
                xml = $"<DataSet>{xml}</DataSet>";
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@xml", xml, DbType.Xml);
                var _postid = db.Database.Connection.Query<long>("deltrust", p, commandType: CommandType.StoredProcedure).ToList();
                msg = _postid.Count > 0 ? "success" : "error";
            }
            return new { msg = msg };
        }


        [HttpPost]
        [ActionName("delusers")]
        public dynamic delusers(List<long> id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN" }, ""));
            var msg = "error";
            using (var db = new MyDbContext())
            {
                string xml = "";
                foreach (var o in id)
                {
                    xml += $"<Entity><Id>{o}</Id></Entity>";
                }
                xml = $"<DataSet>{xml}</DataSet>";
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@xml", xml, DbType.Xml);
                p.Add("@uid", uid, DbType.Int64);
                var _postid = db.Database.Connection.Query<long>("deluser", p, commandType: CommandType.StoredProcedure).ToList();
                msg = _postid.Count > 0 ? "success" : "error";
            }
            return new { msg = msg };
        }

		public static RequestGridParam getparam(HttpRequestMessage request)
        {
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            return new RequestGridParam
            {
                limit = Convert.ToInt32(queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value),
                page = Convert.ToInt32(queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value),
                sortBy = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "sortBy", true) == 0).Value,
                _parent = Convert.ToInt32(queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "_parent", true) == 0).Value),
                search = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "search", true) == 0).Value
            };


        }

        [HttpGet]
        [ActionName("getreport")]
        public dynamic getreport(int days)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN","USER" }, ""));
            var d = AdminMethods.GetReport(days,uid);
            List<string> dateLabels = new List<string>();
            List<long> companyReport = new List<long>();
            List<long> userReport = new List<long>();
            List<long> trustReport = new List<long>();

            for (var i = 0; i < days; i++)
            {
                DateTime dt = DateTime.Now.AddDays(-i);
                dateLabels.Add(String.Format("{0:m}", dt));
            }

            foreach (var dd in dateLabels)
            {
                DateTime dateValue;
                string[] formats = { "MM/dd/yyyy" };
                long pc = 0;
                long td = 0;
                long uc = 0;
                foreach (var e in d.company)
                {
                    if (DateTime.TryParseExact(e.dates, formats,
                        new CultureInfo("en-US"),
                        System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        DateTime dt = DateTime.ParseExact(e.dates,
                            formats[0], CultureInfo.InvariantCulture);
                        var t = (String.Format("{0:m}", dt));

                        if (t == dd)
                        {
                            pc = e.Id;
                        }
                    }

                }



                foreach (var e in d.users)
                {
                    if (DateTime.TryParseExact(e.dates, formats,
                        new CultureInfo("en-US"),
                        System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        DateTime dt = DateTime.ParseExact(e.dates,
                            formats[0], CultureInfo.InvariantCulture);
                        var t = (String.Format("{0:m}", dt));

                        if (t == dd)
                        {
                            uc = e.Id;
                        }
                    }

                }


                foreach (var e in d.trusts)
                {
                    if (DateTime.TryParseExact(e.dates, formats,
                        new CultureInfo("en-US"),
                        System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        DateTime dt = DateTime.ParseExact(e.dates,
                            formats[0], CultureInfo.InvariantCulture);
                        var t = (String.Format("{0:m}", dt));

                        if (t == dd)
                        {
                            td = e.Id;
                        }
                    }

                }
                companyReport.Add(pc);
                userReport.Add(uc);
                trustReport.Add(td);
            }


            return new { data = new { companyReport, userReport, trustReport }, labels = dateLabels };
        }

        #region contact form

        [HttpGet]
        [ActionName("getcontactform")]
        public dynamic getcontactform()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, ""));
            RequestGridParam id = getparam(this.Request);
            List<TblContact> data = new List<TblContact>();
            int start = (id.limit * id.page) - id.limit;
            var total = 0;
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                total = db.TblContacts.AsNoTracking().Count();
                data = db.TblContacts.AsNoTracking().OrderByDescending(x => x.Id).Skip(start).Take(id.limit).ToList();
            }

            return new
            {
                records = data,
                total = total
            };
        }


        [HttpPost]
        public dynamic delcontact(List<long> id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, ""));
            var msg = "error";


            var xml = string.Empty;
            foreach (var i in id)
            {
                xml += string.Format("<DataEntity><id>{0}</id></DataEntity>", i);
            }
            xml = string.Format("<DataSet>{0}</DataSet>", xml);

            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@xml", xml, dbType: System.Data.DbType.Xml);
                var i = db.Database.Connection.Query<long>("delContacts", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                msg = i.Count > 0 ? "success" : "error";
            }
            return new { msg = msg };
        }


        [HttpGet]
        public dynamic updatecontact(long id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, ""));
            var msg = "error";
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@id", id, dbType: System.Data.DbType.Int64);
                var i = db.Database.Connection.Query<long>("updateContactstatus", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                msg = i.Count > 0 ? "success" : "error";
            }
            return new { msg = msg };
        }

        [HttpGet]
        public dynamic unreadcontact()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN" ,"USER"}, ""));
            long i = 0;
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                i = db.Database.Connection.Query<long>("unreadContact", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

            }
            return new { count = i };
        }


        #endregion

    }
}
