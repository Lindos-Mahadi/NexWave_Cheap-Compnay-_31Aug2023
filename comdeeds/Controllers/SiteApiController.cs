using comdeeds.App_Code;
using comdeeds.QueryNameService;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static comdeeds.Models.BaseModel;
using System.Text.RegularExpressions;
using System.Net;

namespace comdeeds.Controllers
{
    public class SiteApiController : ApiController
    {
        static public string CompanyCookieId = "company-session-id"; // tbl company id
        static public string TrustCookieId = "trust-session-id"; // tbl trust id
        private dal.Operation oper = new dal.Operation();

        #region Company form methods

        [HttpGet]
        public dynamic CheckName(string name)
        {
            string status = "", msg = "";
            if (!string.IsNullOrEmpty(name))
            {
                // Regex regExComment = new Regex(@"^[A-Za-z0-9]$");
                Regex regExComment = new Regex(@"^[A-Za-z0-9.& ]*$", RegexOptions.IgnorePatternWhitespace);
                if (!regExComment.IsMatch(name))
                {
                    msg = "Please do not use invalid Characters like (  '  ?  ,  /  ) ";
                    status = "error ";
                }
                if (name.ToUpper().Contains("BANK") || name.ToUpper().Contains("TRUST") || name.ToUpper().Contains("ROYAL") || name.ToUpper().Contains("INCORPORATED"))
                {
                    msg = "Please do not use some restricted words like ( 'Bank' , 'Trust' , 'Royal' , 'Incorporated' ) ";
                    status = "error ";
                }
                else
                {
                    //added by praveen for uat search
                    // using System.Net;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //    // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                    var res = ASIC_helper.CheckCompanyName(name, Guid.NewGuid().ToString());
                    msg = res.businessDocumentBody.longDescription;
                    if (string.Equals(res.businessDocumentBody.shortDescription, "error", StringComparison.OrdinalIgnoreCase)) // error catch
                    {
                        status = "error";
                    }
                    else // success api call
                    {
                        switch (res.businessDocumentBody.code)
                        {
                            case nameAvailabilityResponseTypeCode.Available:
                                status = "Available";
                                break;

                            case nameAvailabilityResponseTypeCode.Identical:
                                status = "Identical";
                                break;

                            case nameAvailabilityResponseTypeCode.SubjectToNamesDetermination:
                                status = "Subject To Names Determination";
                                break;

                            case nameAvailabilityResponseTypeCode.Unavailable:
                                status = "Unavailable";
                                break;

                            case nameAvailabilityResponseTypeCode.UnderTransfer:
                                status = "UnderTransfer";
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                status = "empty";
                msg = "Please enter your company name entity. ";
            }

            return new { status = status, msg = msg };
        }

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

        public string getConst()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string useremail = getuserid(uid.ToString());
            string msg = "error";
            var companyId = GetCompanyCookieId(Request);
            if (companyId > 0)
            {
                msg = oper.getconstPayment_companySearch(companyId, useremail);
            }
            return msg;
        }

        [HttpPost]
        public dynamic AddCompany(comdeeds.Models.BaseModel.ClassCompanyModel company)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string useremail = ""; long Regid = 0;
            DataTable dtuser = oper.get_userdetails_byuid(uid.ToString());
            if (dtuser.Rows.Count > 0)
            {
                useremail = CryptoHelper.DecryptString(dtuser.Rows[0]["Email"].ToString());
                Regid = Convert.ToInt64(dtuser.Rows[0]["Regid"]);
            }
            string msg = "";
            var companyId = GetCompanyCookieId(Request);

            #region insert Step1

            string fullcompantname_ = company.companyName.ToLower().Trim();
            string companyname_ = "";
            string companyextension_ = "";
            if (fullcompantname_.ToLower().Contains("pty ltd"))
            {
                companyextension_ = "pty ltd";
                companyname_ = fullcompantname_.Replace(companyextension_, "");
            }
            else if (fullcompantname_.ToLower().Contains("pty. ltd."))
            {
                companyextension_ = "pty. ltd.";
                companyname_ = fullcompantname_.Replace(companyextension_, "");
            }
            else if (fullcompantname_.ToLower().Contains("pty. ltd"))
            {
                companyextension_ = "pty. ltd";
                companyname_ = fullcompantname_.Replace(companyextension_, "");
            }
            else if (fullcompantname_.ToLower().Contains("pty ltd."))
            {
                companyextension_ = "pty ltd.";
                companyname_ = fullcompantname_.Replace(companyextension_, "");
            }
            else if (fullcompantname_.ToLower().Contains("PTY LIMITED".ToLower()))
            {
                companyextension_ = "PTY LIMITED".ToLower();
                companyname_ = fullcompantname_.Replace(companyextension_, "");
            }
            else
            {
                companyextension_ = "PTY LTD".ToLower();
                companyname_ = fullcompantname_;
            }
            companyname_ = Regex.Replace(companyname_, @"\s+", " ");
            if (companyId == 0)
            {
                var cid = oper.insert_companysearch(useremail.ToString(), companyname_, Regid);
                companyId = Convert.ToInt64(cid);
            }
            else
            {
            }

            #region Add Class Paramaters

            dal.Step1 obj = new dal.Step1();
            obj.companyid = companyId.ToString();
            obj.companyname = companyname_;
            obj.companyname_ext = companyextension_;
            obj.stateterritorry = company.regState;
            obj.isspecialpurpose = company.decl.ToString();
            obj.isreservecompany410 = "no";
            obj.reservecompany410_asicnamereservationnumber = "";
            obj.reservecompany410_fulllegalname = "";
            obj.isproposeidentical = "no";
            obj.proposeidentical_before28may = "";
            obj.proposeidentical_after28may = "";
            obj.proposeidentical_before28may_previousbusinessno1 = "";
            obj.proposeidentical_before28may_previousstateteritory1 = "";
            obj.proposeidentical_after28may_abnnumber = "";
            obj.isultimateholdingcompany = "no";
            obj.ultimateholdingcompany_fulllegalname = "";
            obj.ultimateholdingcompany_country = "";
            obj.ultimateholdingcompany_ACN_ARBN = "";
            obj.ultimateholdingcompany_ABN = "";
            obj.acn = "";
            obj.typeofcompany = company.companyPurpose;
            obj.classofcompany = "limited by shares";

			if(company.UseOfCompany=="smsf" && company.decl == true)
				obj.specialpurpose_ifapplicable = "PSTC";
			else
            obj.specialpurpose_ifapplicable = company.companyPurpose;

            obj.cash = "";
            obj.writtencontact = "";
            obj.Org_Indv = "";
            obj.Full_org_name = "";
            obj.rdo_SMSF_Yes_No = "No";
            if (company.trustName.Trim() != "") { obj.rdo_SMSF_Yes_No = "Yes"; }

            if (company.AcnasName == true)
                obj.proposed_Name_Yes_No = "YES";
            else
                obj.proposed_Name_Yes_No = "NO";

            obj.proposeidentical_before28may_totalstate = 0;
            obj.proposeidentical_before28may_previousbusinessno2 = "";
            obj.proposeidentical_before28may_previousstateteritory2 = "";
            obj.proposeidentical_before28may_previousbusinessno3 = "";
            obj.proposeidentical_before28may_previousstateteritory3 = "";
            obj.proposeidentical_before28may_previousbusinessno4 = "";
            obj.proposeidentical_before28may_previousstateteritory4 = "";
            obj.proposeidentical_before28may_previousbusinessno5 = "";
            obj.proposeidentical_before28may_previousstateteritory5 = "";
            obj.proposeidentical_before28may_previousbusinessno6 = "";
            obj.proposeidentical_before28may_previousstateteritory6 = "";
            obj.proposeidentical_before28may_previousbusinessno7 = "";
            obj.proposeidentical_before28may_previousstateteritory7 = "";
            obj.proposeidentical_before28may_previousbusinessno8 = "";
            obj.proposeidentical_before28may_previousstateteritory8 = "";
            obj.OpeningTime = "";
            obj.ClosingTime = "";
            obj.Isstandard_hours = false;

            obj.trustee_trustname = company.trustName;
            obj.trustee_abn = company.trustAbn;
            obj.trustee_tfn = company.trustTfn;
            obj.trustee_address = company.trustAddress;
            obj.trustee_country = company.trustCountry;
            obj.companyusedfor = company.UseOfCompany;
            obj.UlimateHoldingCompany = company.UlimateHoldingCompany;
            obj.ucompanyname = company.ucompanyname;
            obj.acnarbnabn = company.acnarbnabn;
            obj.countryIcor = company.countryIcor;

            if (!string.IsNullOrEmpty(obj.acnarbnabn))
            {
                obj.UlimateHoldingCompany = "True";

            }
            if (!string.IsNullOrEmpty(obj.acnarbnabn) && obj.countryIcor !="" && obj.countryIcor.ToUpper() != "Australia".ToUpper())  { }
            else {  obj.countryIcor = "Australia"; }
            #endregion Add Class Paramaters

            DataTable dtcom = oper.getStep1_bycid(companyId.ToString());
            if (dtcom.Rows.Count > 0) // run update
            {
                string ii = oper.update_step1(obj);
                if (ii != "0")
                {
                    msg = "success";
                }
            }
            else // run insert
            {
                string ii = oper.insert_step1(obj);
                if (ii != "0")
                {
                    msg = "success";

                    //Email notification when company setup.praveen                
                    var body = "";
                    using (System.IO.StreamReader red = new System.IO.StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/deedhtml/New_Company_Setup_Started.html")))
                    {
                        body = red.ReadToEnd();
                    }
                    body = body.Replace("{UserName}", "Admin");
                    body = body.Replace("{Message2}", obj.companyname + " " + obj.companyname_ext);
                    body = body.Replace("{Message1}", useremail);
                    //body = body.Replace("{Message3}", obj.trust);
                    //body = body.Replace("{Message4}", "The above company setup has been started.");
                    var mailer = new Class_mailer
                    {
                        fromEmail = "support@comdeeds.com.au",
                        fromName = "Comdeeds",
                        HtmlBody = body,
                        subject = "New Company setup- www.comdeeds.com.au",
                        toMail = "superinsure1@gmail.com"
                    };
                    EmailHelper.SendSmtpMail(mailer);
                }              

            }

            #endregion insert Step1

            /*using (var db = new MyDbContext())
            {
                if ((companyId > 0) && db.TblCompanies.Any(x => x.Id == companyId))
                {
                    var comp = new TblCompany { Id = companyId };
                    db.TblCompanies.Attach(comp);
                    comp.CompanyName = company.companyName;
                    comp.NameReserved = company.isNameReserve;
                    comp.CompanyPurpose = company.companyPurpose;
                    comp.CompanyUseFor = company.UseOfCompany;
                    comp.Abn = company.abn;
                    comp.RegistrationState = company.regState;
                    comp.UpdatedBy = uid;
                    comp.UpdatedDate = DateTime.Now;
                    comp.Registered = false;
                    comp.SmsFdeclaration = company.decl;
                    comp.Status = "init";

                    var entry = db.Entry(comp);
                    entry.Property(e => e.CompanyName).IsModified = true;
                    entry.Property(e => e.NameReserved).IsModified = true;
                    entry.Property(e => e.CompanyPurpose).IsModified = true;
                    entry.Property(e => e.CompanyUseFor).IsModified = true;
                    entry.Property(e => e.Abn).IsModified = true;
                    entry.Property(e => e.RegistrationState).IsModified = true;
                    entry.Property(e => e.UpdatedBy).IsModified = true;
                    entry.Property(e => e.UpdatedDate).IsModified = true;
                    entry.Property(e => e.Registered).IsModified = true;
                    entry.Property(e => e.SmsFdeclaration).IsModified = true;
                    entry.Property(e => e.Status).IsModified = true;

                    if ((company.tid > 0) && db.TblTrusts.Any(x => x.Id == company.tid))
                    {
                        var t = new TblCompanyTrust { Id = company.tid };
                        db.TblCompanyTrusts.Attach(t);
                        t.TrustAbn = company.trustAbn;
                        t.TrustTfn = company.trustTfn;
                        t.Country = company.trustCountry;
                        t.TrustAddress = company.trustAddress;
                        t.TrustName = company.companyName;
                        t.AddedBy = uid;
                        t.AddedDate= DateTime.Now;
                        t.UpdatedBy = uid;
                        t.UpdatedDate = DateTime.Now;

                        db.Entry(t).State = System.Data.Entity.EntityState.Modified;
                    }
                    msg = db.SaveChanges() > 0 ? "success" : "error";
                }
                else
                {
                    var comp = new TblCompany()
                    {
                        CompanyName = company.companyName,
                        NameReserved = company.isNameReserve,
                        CompanyPurpose = company.companyPurpose,
                        CompanyUseFor = company.UseOfCompany,
                        Abn = company.abn,
                        RegistrationState = company.regState,
                        AddedBy = uid,
                        AddedDate = DateTime.Now,
                        UpdatedBy = uid,
                        UpdatedDate = DateTime.Now,
                        Registered = false,
                        Status = "init",
                        SmsFdeclaration = company.decl
                    };

                    var comptrust = new TblCompanyTrust()
                    {
                        TrustAbn = company.trustAbn,
                        TrustTfn = company.trustTfn,
                        TrustName = company.trustName,
                        CompanyId = comp.Id,
                        TrustAddress = company.trustAddress,
                        Country = company.trustCountry,
                        AddedBy = uid,
                        AddedDate = DateTime.Now,
                        UpdatedBy = uid,
                        UpdatedDate = DateTime.Now
                    };
                    db.TblCompanies.Add(comp);
                    db.TblCompanyTrusts.Add(comptrust);
                    msg = db.SaveChanges() > 0 ? "success" : "error";
                    companyId = comp.Id;
                }

                if (msg == "success")
                {
                    var c = new HttpCookie(CompanyCookieId, companyId.ToString());
                    c.Expires = DateTime.Now.AddMonths(6);
                    HttpContext.Current.Response.Cookies.Add(c);
                }
            }*/

            if (msg == "success")
            {
                var c = new HttpCookie(CompanyCookieId, companyId.ToString());
                c.Expires = DateTime.Now.AddMonths(6);
                HttpContext.Current.Response.Cookies.Add(c);
            }

            return new { msg = msg };
        }

        public string STATE(string fullname)
        {
            if (fullname.ToLower().Contains("New South Wales".ToLower()))
            {
                return "NSW";
            }
            else if (fullname.ToLower().Contains("AUSTRALIAN CAPITAL TERRITORY".ToLower()))
            {
                return "ACT";
            }
            else if (fullname.ToLower().Contains("Northern Territory".ToLower()))
            {
                return "NT";
            }
            else if (fullname.ToLower().Contains("Queensland".ToLower()))
            {
                return "QLD";
            }
            else if (fullname.ToLower().Contains("South Australia".ToLower()))
            {
                return "SA";
            }
            else if (fullname.ToLower().Contains("Tasmania".ToLower()))
            {
                return "TAS";
            }
            else if (fullname.ToLower().Contains("Victoria".ToLower()))
            {
                return "VIC";
            }
            else if (fullname.ToLower().Contains("Western Australia".ToLower()))
            {
                return "WA";
            }
            else
            {
                return fullname;
            }
        }

        [HttpPost]
        public dynamic addcompanyaddress(List<comdeeds.Models.BaseModel.ClassCompanyAddressModel> address)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string msg = "error";
            var companyId = GetCompanyCookieId(Request);
            if (companyId > 0)
            {
                #region Add Class Paramaters

                dal.Step2 obj = new dal.Step2();
                obj.companyid = companyId.ToString();
                obj.contactperson = "";
                obj.unit_level_suite = address[0].unit;
                obj.streetNoName = address[0].street;
                obj.suburb_town_city = address[0].suburb;
                obj.state = address[0].state;
                obj.postcode = address[0].postcode;
                obj.isprimaryaddress = address[0].sameadd;
                obj.iscompanylocatedaboveaddress = "yes"; // always true because we dont have any option for it in UI, we need occupier name.
                obj.occupiername = "";
                obj.contactperson_primary = "";
                obj.unit_level_suite_primary = address[1].unit;
                obj.streetNoName_primary = address[1].street;
                obj.suburb_town_city_primary = address[1].suburb;
                obj.state_primary = address[1].state;
                obj.postcode_primary = address[1].postcode;
                int regFind = 0;
                int priFind = 0;
                if (oper.isValidAddress(obj.unit_level_suite, obj.streetNoName, obj.suburb_town_city, obj.postcode, obj.state) == true)
                {
                    regFind = 1;
                }
                else
                {
                    msg = "Address not found";
                }

                if (oper.isValidAddress(obj.unit_level_suite, obj.streetNoName, obj.suburb_town_city, obj.postcode, obj.state) == true)
                {
                    priFind = 1;
                }
                else
                {
                    msg = "Address not found";
                }
                if (priFind == 1 && regFind == 1)
                {
                    DataTable dtcom = oper.getStep2_bycid(companyId.ToString());
                    if (dtcom.Rows.Count > 0) // run update
                    {
                        string ii = oper.update_step2(obj);
                        //sachin Added 9-oct-2017
                        string iii = oper.update_Registration(obj);
                        if (ii != "0")
                        {
                            msg = "success";
                        }
                    }
                    else // run insert
                    {
                        string iii = oper.update_Registration(obj);
                        string ii = oper.insert_step2(obj);
                        if (ii != "0")
                        {
                            msg = "success";
                        }
                    }
                }

                #endregion Add Class Paramaters

                /*var regAdd = address.Where(x => x.type == "r").Select(a => new TblCompanyAddress
                {
                    Id = a.id,
                    CompanyId = companyId,
                    IsPrincipleAddress = false,
                    IsRegisteredAddress = true,
                    UnitLevel = a.unit,
                    State = a.state,
                    Street = a.street,
                    Suburb = a.suburb,
                    PostCode = a.postcode,
                    AddedBy = uid,
                    AddedDate = DateTime.Now,
                    UpdatedBy = uid,
                    UpdatedDate = DateTime.Now
                }).FirstOrDefault();

                var prnadd = address.Where(x => x.type == "p").Select(a => new TblCompanyAddress
                {
                    Id = a.id,
                    CompanyId = companyId,
                    IsPrincipleAddress = true,
                    IsRegisteredAddress = false,
                    UnitLevel = a.unit,
                    State = a.state,
                    Street = a.street,
                    Suburb = a.suburb,
                    PostCode = a.postcode,
                    AddedBy = uid,
                    AddedDate = DateTime.Now,
                    UpdatedBy = uid,
                    UpdatedDate = DateTime.Now
                }).FirstOrDefault();

                if (regAdd != null && prnadd != null)
                {
                    using (var db = new MyDbContext())
                    {
                        if ((regAdd.Id != 0) && db.TblCompanyAddresses.Any(x => x.Id == regAdd.Id))
                        {
                            db.TblCompanyAddresses.Attach(regAdd);
                            db.Entry(regAdd).State = System.Data.Entity.EntityState.Modified;
                        }
                        else if ((prnadd.Id != 0) && db.TblCompanyAddresses.Any(x => x.Id == prnadd.Id))
                        {
                            db.TblCompanyAddresses.Attach(prnadd);
                            db.Entry(prnadd).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            db.TblCompanyAddresses.AddRange(new List<TblCompanyAddress>() { regAdd, prnadd });
                        }
                        msg = db.SaveChanges() > 0 ? "success" : "error";
                    }
                }*/
            }

            return new { msg = msg };
        }

        [HttpPost]
        public dynamic adddirectors(List<comdeeds.Models.BaseModel.ClassDirectorModel> directors)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string msg = "error";
            int count = 0;
            var companyId = GetCompanyCookieId(Request);
            if (companyId > 0)
            {
                int cou = 0;
                int Address = 1;
                int nameCount = 0;
                foreach (var dirdetail in directors)
                {
                    foreach (var dirdetail1 in directors)
                    {
                        if (dirdetail.fname == dirdetail1.fname
                            && dirdetail.lname == dirdetail1.lname
                            && dirdetail.dobday == dirdetail1.dobday 
                            && dirdetail.dobmonth == dirdetail1.dobmonth
                            && dirdetail.dobyear == dirdetail1.dobyear)
                        {
                            nameCount++;
                        }

                    }
                    if(nameCount>1)
                    {
                        msg = "Duplicate"; break;
                    }
                    else
                    {
                        nameCount = 0;
                    }
                }
                if (nameCount == 0)
                {
                    foreach (var dirdetail in directors)
                    {
                        if (dirdetail.country != "Australia")
                        {
                            dirdetail.state = dirdetail.stateraw;
                        }

                        if (oper.isValidAddress(dirdetail.dirunit.ToString(), dirdetail.dirstreet.ToString(), dirdetail.dirsuburb.ToString(), dirdetail.dirpostcode.ToString(), dirdetail.dirstate.ToString()) == true)
                        {
                        }
                        else
                        {
                            msg = "Address not found"; Address = 0; break;
                        }
                    }

                    //add by praveen - remove existing director
                    DataTable dt = oper.get_step3(companyId.ToString());
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        bool check = false;
                        Int64 dirid = Convert.ToInt64(dt.Rows[i]["id"]);
                        foreach (var dirdetail in directors)
                        {
                            if (dirdetail.id.ToString() == dirid.ToString())
                            {
                                check = true;
                            }
                        }
                        if (!check)
                        {
                            string ii = oper.delete_step3_By_dirId(companyId.ToString(), dirid.ToString());
                        }
                    }

                    if (Address == 1)
                    {
                        foreach (var dirdetail in directors)
                        {
                            cou = cou + 1;
                            string id = dirdetail.id.ToString();
                            string fname = dirdetail.fname.ToString();
                            string lname = dirdetail.lname.ToString();
                            string dobday = dirdetail.dobday.ToString();
                            string dobmonth = dirdetail.dobmonth.ToString();
                            string dobyear = dirdetail.dobyear.ToString();
                            string address = dirdetail.address.ToString();
                            string city = dirdetail.city.ToString();
                            string state = dirdetail.state.ToString();
                            string country = dirdetail.country.ToString();

                            string dirunit = dirdetail.dirunit.ToString();
                            string dirstreet = dirdetail.dirstreet.ToString();
                            string dirsuburb = dirdetail.dirsuburb.ToString();
                            string dirpostcode = dirdetail.dirpostcode.ToString();
                            string dirstate = dirdetail.dirstate.ToString();

                            dal.Step3 obj = new dal.Step3();
                            obj.id = Convert.ToInt32(id.ToString());
                            obj.companyid = companyId.ToString();
                            if (directors.Count >= 2 && cou == 1)
                            {
                                obj.designation = "secretary";
                            }
                            else
                            {
                                obj.designation = "director";
                            }

                            obj.firstname = fname;
                            obj.middlename = "";
                            obj.familyname = lname;
                            obj.anyformername = "no";
                            obj.firstname_former = "";
                            obj.middlename_former = "";
                            obj.familyname_former = "";
                            obj.unit_level_suite_primary = dirunit;
                            obj.streetNoName_primary = dirstreet;
                            obj.suburb_town_city_primary = dirsuburb;
                            obj.state_primary = dirstate;
                            obj.postcode_primary = dirpostcode;
                            obj.country = "Australia";
                            obj.dob = dobyear + "-" + dobmonth + "-" + dobday;
                            obj.placeofbirth = city + "," + state;
                            obj.countryofbirth = country;
                            obj.IsDirector = "True";
                            obj.IsSecretary = "False";
                            obj.IsPublicOfficer = "False";
                            string ii = oper.update_step3(obj);
                            try
                            {
                                count += count + Convert.ToInt32(ii);
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
                if (count > 0) { msg = "success"; }
            }
            /* List<TblCompanyDirector> dir = new List<TblCompanyDirector>();
             if (companyId > 0)
             {
                 foreach (var d in directors)
                 {
                     dir.Add(new TblCompanyDirector
                     {
                         Id = d.id,
                         CompanyId = companyId,
                         FirstName = d.fname,
                         LastName = d.lname,
                         DoBday = d.dobday,
                         DoBmonth = d.dobmonth,
                         DoByear = d.dobyear,
                         DoBaddress = d.address,
                         DoBcity = d.city,
                         DoBcountry = d.country,
                         DoBstate = d.state,
                         AddedBy = uid,
                         AddedDate = DateTime.Now,
                         UpdatedBy = uid,
                         UpdatedDate = DateTime.Now
                     });
                 }

                 using (var db = new MyDbContext())
                 {
                     //remove deleted directors if present
                     var xml = "";
                     foreach (var f in directors)
                     {
                         if (f.id != 0)
                         {
                             xml += string.Format("<Entity><Id>{0}</Id></Entity>", f.id);
                         }
                     }
                     if (!string.IsNullOrEmpty(xml))
                     {
                         xml = "<DataSet>" + xml + "</DataSet>";
                         var p = new DynamicParameters();
                         p.Add("@xml", xml, dbType: DbType.Xml);
                         p.Add("@cid", companyId, dbType: DbType.Int64);
                         db.Database.Connection.Query<long>("deleteDirectors", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                     }
                     foreach (var d in dir)
                     {
                         if ((d.Id != 0) && db.TblCompanyDirectors.Any(x => x.Id == d.Id))
                         {
                             db.TblCompanyDirectors.Attach(d);
                             db.Entry(d).State = System.Data.Entity.EntityState.Modified;
                         }
                         else
                         {
                             db.TblCompanyDirectors.Add(d);
                         }
                     }
                     msg = db.SaveChanges() > 0 ? "success" : "error";
                 }
             }*/
            //var returnDirectors = dir.Select(d => new { Id = d.Id, Name = d.FirstName + " " + d.LastName }).ToList();
            return new { msg = msg };
        }

        [HttpPost]
        public dynamic addshare(List<comdeeds.Models.BaseModel.ClassShareModel> shares)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string useremail = getuserid(uid.ToString());
            string msg = "error";
            var companyId = GetCompanyCookieId(Request);
            if (companyId > 0)
            {
                string idel = oper.delete_share_distribute_grid_step4(companyId.ToString());
                int count = 0;

                string ide2 = oper.delete_step4_anothershareholder(companyId.ToString());

                foreach (var s in shares)
                {
                    count = count + 1;
                    string Id = s.Id.ToString();
                    string AddedBy = uid.ToString();
                    string AddedDate = DateTime.Now.ToString();
                    string CompanyId = companyId.ToString();
                    string DirectorId = s.dId.ToString();
                    string NoOfShare = s.noofshare.ToString();
                    string OwnerName = s.ownername.ToString();
                    string ShareAmount = s.sharecost.ToString();
                    string ShareBehalf = s.other.ToString();
                    string ShareClass = s.shareclass.ToString();
                    string UpdatedBy = uid.ToString();
                    string UpdatedDate = DateTime.Now.ToString();					
					string shareoption = s.shareoption.ToString();
					string isheldanotherorg = s.isheldanotherorg.ToString();


					string sharedetailsnotheldanotherorg = Convert.ToString(s.sharedetailsnotheldanotherorg);

					dal.Step4_AnotherShareHolder obj = new dal.Step4_AnotherShareHolder();
                    obj.id = Convert.ToInt32(Id);
                    obj.companyid = companyId.ToString();
                    obj.shareholderdetails = s.dirName.ToString();
                    obj.shareclasstype_value = ShareClass;
                    obj.shareclasstype_text = ShareClass;
                    obj.no_of_shares = "0";
                    obj.amountpaidpershare = "0";			
					
					obj.shareoption = shareoption;
					obj.amountremainingunpaidpershare = "0";

					if (isheldanotherorg.ToLower() == "true")
					{
						if (OwnerName.Trim() != "")
						{
							obj.isheldanotherorg = "yes";
							obj.beneficialownername = OwnerName;
							obj.sharedetailsnotheldanotherorg = "";
						}
						else
						{
							//obj.isheldanotherorg = "no";
							//done by praveen for select yes option
							obj.isheldanotherorg = "yes";
							obj.beneficialownername = s.dirName.ToString();
							obj.sharedetailsnotheldanotherorg = "";
						}
					}
					else
					{
						//if (sharedetailsnotheldanotherorg.Trim() != "")
						//{
							obj.isheldanotherorg = "no";
							obj.beneficialownername = s.dirName.ToString();
							obj.sharedetailsnotheldanotherorg = sharedetailsnotheldanotherorg;
						//}
					}

                    obj.step4ID = "0";
                    obj.individual_or_company = "Individual";
                    obj.placeofbirth = "";
                    DataTable dtdir = oper.get_step3By_dirId(companyId.ToString(), DirectorId);
                    if (dtdir.Rows.Count > 0)
                    {
                        obj.individual_or_company_name = s.dirName.ToString();
                        obj.individual_or_company_acn = "";
                        obj.individual_or_company_address = dtdir.Rows[0]["unit_level_suite_primary"].ToString() + "/ " + dtdir.Rows[0]["streetNoName_primary"].ToString() + " " + dtdir.Rows[0]["suburb_town_city_primary"].ToString() + " " + dtdir.Rows[0]["state_primary"].ToString() + " " + dtdir.Rows[0]["postcode_primary"].ToString() + " " + dtdir.Rows[0]["country"].ToString();
                        obj.individual_or_company_dob = dtdir.Rows[0]["dob"].ToString();
                        obj.individual_or_company_unit_level_suite = dtdir.Rows[0]["unit_level_suite_primary"].ToString();
                        obj.individual_or_company_streetNoName = dtdir.Rows[0]["streetNoName_primary"].ToString();
                        obj.individual_or_company_suburb_town_city = dtdir.Rows[0]["suburb_town_city_primary"].ToString();
                        obj.individual_or_company_state = dtdir.Rows[0]["state_primary"].ToString();
                        obj.individual_or_company_postcode = dtdir.Rows[0]["postcode_primary"].ToString();
                        obj.individual_or_company_country = dtdir.Rows[0]["country"].ToString();
                        obj.individual_or_company_Joint = "";
                        obj.individual_or_company_name_Joint = "";
                        obj.individual_or_company_acn_Joint = "";
                        obj.individual_or_company_address_Joint = "";
                        obj.individual_or_company_dob_Joint = "";
                        obj.individual_or_company_unit_level_suite_Joint = "";
                        obj.individual_or_company_streetNoName_Joint = "";
                        obj.individual_or_company_suburb_town_city_Joint = "";
                        obj.individual_or_company_state_Joint = "";
                        obj.individual_or_company_postcode_Joint = "";
                        obj.individual_or_company_country_Joint = "";
                        obj.ISJOINT = "no";
                    }
                    else
                    {
                        obj.individual_or_company_name = s.dirName.ToString();
                        obj.individual_or_company_acn = "";
                        obj.individual_or_company_address = "";
                        obj.individual_or_company_dob = "";
                        obj.individual_or_company_unit_level_suite = "";
                        obj.individual_or_company_streetNoName = "";
                        obj.individual_or_company_suburb_town_city = "";
                        obj.individual_or_company_state = "";
                        obj.individual_or_company_postcode = "";
                        obj.individual_or_company_country = "";
                        obj.individual_or_company_Joint = "";
                        obj.individual_or_company_name_Joint = "";
                        obj.individual_or_company_acn_Joint = "";
                        obj.individual_or_company_address_Joint = "";
                        obj.individual_or_company_dob_Joint = "";
                        obj.individual_or_company_unit_level_suite_Joint = "";
                        obj.individual_or_company_streetNoName_Joint = "";
                        obj.individual_or_company_suburb_town_city_Joint = "";
                        obj.individual_or_company_state_Joint = "";
                        obj.individual_or_company_postcode_Joint = "";
                        obj.individual_or_company_country_Joint = "";
                        obj.ISJOINT = "no";
                    }

                    obj.dirid = Convert.ToInt32(DirectorId);
                    string ii = oper.update_step4_anothershareholder(obj);
                    /// insert share values now :::::::::::
                    ///
                    //int noofshare_=0;
                    double noofshare_ = 0;
                    double amountpaid_ = 0;
                    int amountunpaid_ = 0;
                    double totalpaid_ = 0;
                    int totalunpaid_ = 0;
                    if (NoOfShare.Trim() != "")
                    {
                        noofshare_ = Convert.ToDouble(NoOfShare);
                    }
                    if (ShareAmount.Trim() != "")
                    {
                        amountpaid_ = Convert.ToDouble(ShareAmount);
                    }
                    totalpaid_ = noofshare_ * amountpaid_;

                    dal.Step1_shares_distribute objshare = new dal.Step1_shares_distribute();
                    objshare.companyid = companyId.ToString();
                    objshare.shareclass = ShareClass.ToString();
                    objshare.totalshares = 0.ToString();
                    objshare.unitprice = 0.ToString();
                    objshare.totalprice = 0.ToString();
                    objshare.c_totalshares = noofshare_.ToString();
                    objshare.c_amountpaidpershare = amountpaid_.ToString();
                    objshare.c_amountremaining_unpaidpershare = amountunpaid_.ToString();
                    objshare.c_totalamountpaidpershare = totalpaid_.ToString();
                    objshare.c_totalamountunpaidpershare = totalunpaid_.ToString();
                    objshare.c_sharerange = companyId.ToString();
                    objshare.c_certificateno = ShareClass.ToString();
                    objshare.sno = count.ToString();
                    //objshare.linkid="mgrid"+count.ToString();
                    objshare.individual_or_company = "Individual";
                    objshare.linkid = DirectorId;

                    string iii = oper.insert_step1_Share_distribute_grid(objshare);
                    msg = "success";
                }
            }

            return new { msg = msg };
        }

        [HttpPost]
        public dynamic updateConst()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string useremail = getuserid(uid.ToString());
            string msg = "error";
            var companyId = GetCompanyCookieId(Request);
            if (companyId > 0)
            {
                int upcost = oper.updateconstPayment_companySearch(companyId, useremail);
                msg = "success";
            }
            return new { msg = msg };
        }

        [HttpPost]
        public dynamic addshare1(List<comdeeds.Models.BaseModel.ClassIndShareModel> shares)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string useremail = getuserid(uid.ToString());
            string msg = "error";
            var companyId = GetCompanyCookieId(Request);
            if (companyId > 0)
            {
                string idel = oper.delete_share_distribute_grid_step4_ind(companyId.ToString()); // before insering new record delete previous record for this company
                string ide2 = oper.delete_step4_anothershareholder_ind(companyId.ToString());  // Do same thing.
                int count = 0;

                #region Foreach

                foreach (var s in shares)
                {
                    count = count + 1;
                    string Id = s.Id.ToString();
                    string AddedBy = uid.ToString();
                    string AddedDate = DateTime.Now.ToString();
                    string CompanyId = companyId.ToString();
                    string DirectorId = "";
                    string NoOfShare = s.share_noofshare.ToString();
                    string OwnerName = s.share_indshareownername.ToString();
                    string ShareAmount = s.share_amountopaidshare.ToString();
                    string placeofbirth = s.share_placeofbirth.ToString();
                    string ShareBehalf = "";
                    string ShareClass = s.share_sharetype.ToString();
                    string UpdatedBy = uid.ToString();
                    string UpdatedDate = DateTime.Now.ToString();
					//string isheldanotherorg = s.isheldanotherorg.ToString();
					string NoOfShare_comp = s.share_noofcompshare.ToString();
                    string OwnerName_comp = s.share_compshareownername.ToString();
                    string ShareAmount_comp = s.share_amountopaidcompshare.ToString();
                    string ShareClass_comp = s.share_compsharetype.ToString();
                    string rdshare_individual = s.rdshare_individual;
                    string rdshare_company = s.rdshare_company;

                    dal.Step4_AnotherShareHolder obj = new dal.Step4_AnotherShareHolder();
                    obj.id = Convert.ToInt32(Id);
                    obj.companyid = companyId.ToString();
                    obj.shareholderdetails = s.share_indname.ToString();
                    obj.shareclasstype_value = ShareClass;
                    obj.shareclasstype_text = ShareClass;
                    obj.no_of_shares = "0";
                    obj.amountpaidpershare = "0";
                    obj.amountremainingunpaidpershare = "0";

                    obj.step4ID = "0";
                    obj.placeofbirth = placeofbirth;

                    #region Company Share (Non Director)

                    if (s.share_compORtrtname != "" || rdshare_company == "no")
                    {
                        if (OwnerName_comp.Trim() != "")
                        {
                            obj.isheldanotherorg = "yes";
                            obj.beneficialownername = OwnerName_comp;
                        }
                        else
                        {
                            obj.isheldanotherorg = "no";
                            obj.beneficialownername = OwnerName_comp;
                        }

                        obj.individual_or_company = "company";
                        obj.individual_or_company_name = s.share_compORtrtname.ToString();
                        obj.shareholderdetails = s.share_compORtrtname.ToString();
                        obj.individual_or_company_acn = s.share_compORtrtABN.ToString();
                        obj.individual_or_company_address = s.sharecompunit + "/ " + s.sharecompstreet + " " + s.sharecompsuburb + " " + s.ddsharecompstate + " " + s.sharecomppostcode + " " + "Australia";
                        obj.individual_or_company_dob = "";
                        obj.individual_or_company_unit_level_suite = s.sharecompunit;
                        obj.individual_or_company_streetNoName = s.sharecompstreet;
                        obj.individual_or_company_suburb_town_city = s.sharecompsuburb;
                        obj.individual_or_company_state = s.ddsharecompstate;
                        obj.individual_or_company_postcode = s.sharecomppostcode;
                        obj.individual_or_company_country = "Australia";
                        obj.individual_or_company_Joint = "";
                        obj.individual_or_company_name_Joint = "";
                        obj.individual_or_company_acn_Joint = "";
                        obj.individual_or_company_address_Joint = "";
                        obj.individual_or_company_dob_Joint = "";
                        obj.individual_or_company_unit_level_suite_Joint = "";
                        obj.individual_or_company_streetNoName_Joint = "";
                        obj.individual_or_company_suburb_town_city_Joint = "";
                        obj.individual_or_company_state_Joint = "";
                        obj.individual_or_company_postcode_Joint = "";
                        obj.individual_or_company_country_Joint = "";
                        obj.ISJOINT = "no";

                        obj.dirid = count;  // 0
                        string ii = oper.update_step4_anothershareholder(obj);

                        double noofshare_comp = 0;
                        double amountpaid_comp = 0;
                        int amountunpaid_comp = 0;
                        double totalpaid_comp = 0;
                        int totalunpaid_comp = 0;
                        if (NoOfShare_comp.Trim() != "")
                        {
                            noofshare_comp = Convert.ToDouble(NoOfShare_comp);
                        }
                        if (ShareAmount_comp.Trim() != "")
                        {
                            amountpaid_comp = Convert.ToDouble(ShareAmount_comp);
                        }
                        totalpaid_comp = noofshare_comp * amountpaid_comp;

                        dal.Step1_shares_distribute objshare = new dal.Step1_shares_distribute();

                        objshare.companyid = companyId.ToString();
                        objshare.shareclass = ShareClass_comp;
                        objshare.totalshares = 0.ToString();
                        objshare.unitprice = 0.ToString();
                        objshare.totalprice = 0.ToString();
                        objshare.c_totalshares = noofshare_comp.ToString();
                        objshare.c_amountpaidpershare = amountpaid_comp.ToString();
                        objshare.c_amountremaining_unpaidpershare = amountunpaid_comp.ToString();
                        objshare.c_totalamountpaidpershare = totalpaid_comp.ToString();
                        objshare.c_totalamountunpaidpershare = totalunpaid_comp.ToString();
                        objshare.c_sharerange = companyId.ToString();
                        objshare.c_certificateno = ShareClass_comp.ToString();
                        objshare.sno = count.ToString();
                        objshare.linkid = count.ToString();
                        objshare.individual_or_company = "company";

                        string iii = oper.insert_step1_Share_distribute_grid(objshare);
                    }

                    #endregion Company Share (Non Director)

                    #region Individual Share  (Non Director)

                    else if (s.share_compORtrtname == "" || rdshare_individual == "yes")
                    {
                        if (OwnerName.Trim() != "")
                        {
                            obj.isheldanotherorg = "yes";
                            obj.beneficialownername = OwnerName;
                        }
                        else
                        {
                            obj.isheldanotherorg = "no";
                            obj.beneficialownername = OwnerName;
                        }

                        obj.individual_or_company = "Individual1";
                        obj.individual_or_company_name = s.share_indname.ToString();
                        obj.individual_or_company_acn = "";
                        obj.individual_or_company_address = s.shareaddunit + "/ " + s.shareaddstreet + " " + s.shareaddsuburb + " " + s.ddshareaddstate + " " + s.shareaddpostcode + " " + "Australia";
                        obj.individual_or_company_dob = s.ind_dobyear + "-" + s.ind_dobmonth + "-" + s.ind_dobday;
                        obj.individual_or_company_unit_level_suite = s.shareaddunit;
                        obj.individual_or_company_streetNoName = s.shareaddstreet;
                        obj.individual_or_company_suburb_town_city = s.shareaddsuburb;
                        obj.individual_or_company_state = s.ddshareaddstate;
                        obj.individual_or_company_postcode = s.shareaddpostcode;
                        obj.individual_or_company_country = "Australia";
                        obj.individual_or_company_Joint = "";
                        obj.individual_or_company_name_Joint = "";
                        obj.individual_or_company_acn_Joint = "";
                        obj.individual_or_company_address_Joint = "";
                        obj.individual_or_company_dob_Joint = "";
                        obj.individual_or_company_unit_level_suite_Joint = "";
                        obj.individual_or_company_streetNoName_Joint = "";
                        obj.individual_or_company_suburb_town_city_Joint = "";
                        obj.individual_or_company_state_Joint = "";
                        obj.individual_or_company_postcode_Joint = "";
                        obj.individual_or_company_country_Joint = "";
                        obj.ISJOINT = "no";
                        obj.dirid = count;
                        obj.shareoption = "";
                        obj.sharedetailsnotheldanotherorg = "";
                        string ii = oper.update_step4_anothershareholder(obj);

                        double noofshare_ = 0;
                        double amountpaid_ = 0;
                        int amountunpaid_ = 0;
                        double totalpaid_ = 0;
                        int totalunpaid_ = 0;
                        if (NoOfShare.Trim() != "")
                        {
                            noofshare_ = Convert.ToDouble(NoOfShare);
                        }
                        if (ShareAmount.Trim() != "")
                        {
                            amountpaid_ = Convert.ToDouble(ShareAmount);
                        }
                        totalpaid_ = noofshare_ * amountpaid_;
                        dal.Step1_shares_distribute objshare = new dal.Step1_shares_distribute();

                        objshare.companyid = companyId.ToString();
                        objshare.shareclass = ShareClass.ToString();
                        objshare.totalshares = 0.ToString();
                        objshare.unitprice = 0.ToString();
                        objshare.totalprice = 0.ToString();
                        objshare.c_totalshares = noofshare_.ToString();
                        objshare.c_amountpaidpershare = amountpaid_.ToString();
                        objshare.c_amountremaining_unpaidpershare = amountunpaid_.ToString();
                        objshare.c_totalamountpaidpershare = totalpaid_.ToString();
                        objshare.c_totalamountunpaidpershare = totalunpaid_.ToString();
                        objshare.c_sharerange = companyId.ToString();
                        objshare.c_certificateno = ShareClass.ToString();
                        objshare.sno = count.ToString();
                        objshare.linkid = count.ToString();
                        objshare.individual_or_company = "Individual1";
                        string iii = oper.insert_step1_Share_distribute_grid(objshare);
                    }

                    #endregion Individual Share  (Non Director)

                    msg = "success";
                }

                #endregion Foreach
            }

            return new { msg = msg };
        }

        [HttpGet]
        public dynamic GetCompanyDetails()
        {
            string msg = "";
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            var companyId = GetCompanyCookieId(Request);
            comdeeds.Models.BaseModel.ClassCompanyModel company = new comdeeds.Models.BaseModel.ClassCompanyModel();
            if (companyId > 0)
            {
                #region fillcompanydetails

                DataTable dtcom = oper.getStep1_bycid(companyId.ToString());
                if (dtcom.Rows.Count > 0)
                {
                    company.companyName = dtcom.Rows[0]["companyname"].ToString() + " " + dtcom.Rows[0]["companyname_ext"].ToString();
                    company.companyPurpose = dtcom.Rows[0]["typeofcompany"].ToString();
                    company.UseOfCompany = dtcom.Rows[0]["companyusedfor"].ToString();
                    company.abn = "";
                    company.isNameReserve = false; ;
                    company.decl = dtcom.Rows[0]["isspecialpurpose"].ToString().ToLower() == "no" ? false : true;
                    company.regState = dtcom.Rows[0]["stateterritorry"].ToString();
                    company.trustAbn = dtcom.Rows[0]["trustee_abn"].ToString();
                    company.trustAddress = dtcom.Rows[0]["trustee_address"].ToString();
                    company.tid = 0;
                    company.trustCountry = dtcom.Rows[0]["trustee_country"].ToString();
                    company.trustName = dtcom.Rows[0]["trustee_trustname"].ToString();
                    company.trustTfn = dtcom.Rows[0]["trustee_tfn"].ToString();
                    company.tuser = dtcom.Rows[0]["typeofuser"].ToString();
                    if (dtcom.Rows[0]["proposed_Name_Yes_No"].ToString().ToUpper() == "YES")
                    {
                        company.AcnasName = true;
                    }
                    else
                    {
                        company.AcnasName = false;
                    }

                    company.UlimateHoldingCompany = dtcom.Rows[0]["ulimateHoldingCompany"].ToString();
                    company.ucompanyname = dtcom.Rows[0]["ucompanyname"].ToString();
                    company.acnarbnabn = dtcom.Rows[0]["acnarbnabn"].ToString();
                    company.countryIcor = dtcom.Rows[0]["countryIcor"].ToString();

                }

                #endregion fillcompanydetails

                /*using (var db = new MyDbContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    company = db.TblCompanies.AsNoTracking().Where(x => x.Id == companyId).
                        Select(x => new ClassCompanyModel
                        {
                            companyName = x.CompanyName,
                            companyPurpose = x.CompanyPurpose,
                            UseOfCompany = x.CompanyUseFor,
                            abn = x.Abn,
                            isNameReserve = x.NameReserved,
                            decl = x.SmsFdeclaration,
                            regState = x.RegistrationState,
                            trustAbn = x.TblCompanyTrusts.FirstOrDefault().TrustAbn,
                            trustAddress = x.TblCompanyTrusts.FirstOrDefault().TrustAddress,
                            tid = x.TblCompanyTrusts.FirstOrDefault().Id,
                            trustCountry = x.TblCompanyTrusts.FirstOrDefault().Country,
                            trustName = x.TblCompanyTrusts.FirstOrDefault().TrustName,
                            trustTfn = x.TblCompanyTrusts.FirstOrDefault().TrustTfn
                        }).FirstOrDefault();
                }*/
                msg = company != null ? "success" : "null";
            }
            else
            {
                msg = "null";
            }
            return new
            {
                msg = msg,
                company = company
            };
        }

        [HttpGet]
        public dynamic GetCompanyAddress()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            var companyId = GetCompanyCookieId(Request);
            List<comdeeds.Models.BaseModel.ClassCompanyAddressModel> companyaddList = new List<comdeeds.Models.BaseModel.ClassCompanyAddressModel>();
            Models.BaseModel.ClassCompanyAddressModel companyadd1 = new Models.BaseModel.ClassCompanyAddressModel();
            Models.BaseModel.ClassCompanyAddressModel companyadd2 = new Models.BaseModel.ClassCompanyAddressModel();
            DataTable dtcom = new DataTable();
            if (companyId > 0)
            {
                #region fillcompanydetails

                dtcom = oper.getStep2_bycid(companyId.ToString());
                if (dtcom.Rows.Count > 0)
                {
                    companyadd1.id = Convert.ToInt64(dtcom.Rows[0]["id"].ToString());
                    companyadd1.unit = dtcom.Rows[0]["unit_level_suite"].ToString();
                    companyadd1.street = dtcom.Rows[0]["streetNoName"].ToString();
                    companyadd1.suburb = dtcom.Rows[0]["suburb_town_city"].ToString();
                    companyadd1.state = dtcom.Rows[0]["state"].ToString();
                    companyadd1.postcode = dtcom.Rows[0]["postcode"].ToString();
                    companyadd1.type = "r";
                    companyadd1.sameadd = dtcom.Rows[0]["isprimaryaddress"].ToString();
                    companyaddList.Add(companyadd1);

                    companyadd2.id = Convert.ToInt64(dtcom.Rows[0]["id"].ToString());
                    companyadd2.unit = dtcom.Rows[0]["unit_level_suite_primary"].ToString();
                    companyadd2.street = dtcom.Rows[0]["streetNoName_primary"].ToString();
                    companyadd2.suburb = dtcom.Rows[0]["suburb_town_city_primary"].ToString();
                    companyadd2.state = dtcom.Rows[0]["state_primary"].ToString();
                    companyadd2.postcode = dtcom.Rows[0]["postcode_primary"].ToString();
                    companyadd2.type = "p";
                    companyadd2.sameadd = dtcom.Rows[0]["isprimaryaddress"].ToString();
                    companyaddList.Add(companyadd2);

                    string isprimaryaddress = dtcom.Rows[0]["isprimaryaddress"].ToString();
                    if (isprimaryaddress.ToLower() == "yes")
                    {
                    }

                    return new
                    {
                        msg = "success",
                        regadd = companyadd1,
                        principleadd = companyadd2
                    };
                }
                else
                {
                    return new
                    {
                        msg = "null",
                        regadd = companyadd1,
                        principleadd = companyadd2
                    };
                }

                #endregion fillcompanydetails
            }
            else
            {
                return new
                {
                    msg = "null",
                    regadd = companyadd1,
                    principleadd = companyadd2
                };
            }
        }

        [HttpGet]
        public dynamic GetDirectors()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            var companyId = GetCompanyCookieId(Request);
            List<comdeeds.Models.BaseModel.ClassDirectorModel> companyDir = new List<comdeeds.Models.BaseModel.ClassDirectorModel>();
            if (companyId > 0)
            {
                DataTable dt = oper.get_step3(companyId.ToString());
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        comdeeds.Models.BaseModel.ClassDirectorModel obj = new Models.BaseModel.ClassDirectorModel();
                        obj.id = Convert.ToInt64(dt.Rows[i]["id"]);
                        obj.fname = dt.Rows[i]["firstname"].ToString();
                        obj.lname = dt.Rows[i]["familyname"].ToString();
                        obj.address = dt.Rows[i]["streetNoName_primary"].ToString();
                        string dob = dt.Rows[i]["dob"].ToString();
                        string[] DOB_ = dob.Split('-');
                        obj.dobyear = Convert.ToInt32(DOB_[0]);
                        obj.dobmonth = Convert.ToInt32(DOB_[1]);
                        obj.dobday = Convert.ToInt32(DOB_[2]);
                        string birthplace = dt.Rows[i]["placeofbirth"].ToString();
                        string[] PlaceBirth_ = birthplace.Split(',');
                        if (PlaceBirth_.Length == 1)
                        {
                            obj.city = PlaceBirth_[0].ToString();
                        }
                        else if (PlaceBirth_.Length == 2)
                        {
                            obj.city = PlaceBirth_[0].ToString();
                            obj.state = PlaceBirth_[1].ToString();
                        }
                        obj.country = dt.Rows[i]["countryofbirth"].ToString();

                        obj.dirunit = dt.Rows[i]["unit_level_suite_primary"].ToString();
                        obj.dirstreet = dt.Rows[i]["streetNoName_primary"].ToString();
                        obj.dirsuburb = dt.Rows[i]["suburb_town_city_primary"].ToString();
                        obj.dirstate = dt.Rows[i]["state_primary"].ToString();
                        obj.dirpostcode = dt.Rows[i]["postcode_primary"].ToString();

                        companyDir.Add(obj);
                    }
                }
            }

            /*using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                companyDir = db.TblCompanyDirectors.AsNoTracking().Where(x => x.CompanyId == companyId)
                    .Select(x => new comdeeds.Models.BaseModel.ClassDirectorModel
                    {
                        id=x.Id,
                        fname = x.FirstName,
                        lname = x.LastName,
                        address = x.DoBaddress,
                        city = x.DoBcity,
                        country = x.DoBcountry,
                        dobday = x.DoBday,
                        dobmonth = x.DoBmonth,
                        state = x.DoBstate,
                        dobyear = x.DoByear
                    }).ToList();
            }*/
            return new { msg = companyDir.Count > 0 ? "success" : "null", directors = companyDir };
        }

        [HttpGet]
        public dynamic GetShare()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            var companyId = GetCompanyCookieId(Request);
            List<comdeeds.Models.BaseModel.ClassShareModel> companyDir = new List<comdeeds.Models.BaseModel.ClassShareModel>();
            if (companyId > 0)
            {
                DataTable dt = oper.get_step3(companyId.ToString());
                DataTable dtanother = new DataTable();                
                dtanother = oper.get_step4_anothershareholder(companyId.ToString());
                if (dtanother.Rows.Count > 0)
                {
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        bool check = false;
                        for (int i = 0; i < dtanother.Rows.Count; i++)
                        {
                            if (dt.Rows[j]["id"].ToString() == dtanother.Rows[i]["dirid"].ToString())
                            {
                                check = true;
                                string beneficialownername = "";
                                string id = dtanother.Rows[i]["id"].ToString();
                                string dirid = dtanother.Rows[i]["dirid"].ToString();
                                string directorname = dtanother.Rows[i]["shareholderdetails"].ToString();

                                string isheldanotherorg = dtanother.Rows[i]["isheldanotherorg"].ToString();
                                if (isheldanotherorg.Trim().ToLower() == "yes")
                                {
                                    beneficialownername = dtanother.Rows[i]["beneficialownername"].ToString();
                                }
                                else
                                {
                                    beneficialownername = dtanother.Rows[i]["beneficialownername"].ToString();
                                }
                                string shareoption = dtanother.Rows[i]["shareoption"].ToString();
                                string sharedetailsnotheldanotherorg = "";
                                if (isheldanotherorg.Trim().ToLower() == "no")
                                {
                                    sharedetailsnotheldanotherorg = Convert.ToString(dtanother.Rows[i]["sharedetailsnotheldanotherorg"]);
                                    //sharedetailsnotheldanotherorg = dtanother.Rows[i]["beneficialownername"].ToString();
                                }

                                DataTable dtshares = new DataTable();
                                //dtshares=oper.get_step4_get_share_distributegrid(companyId.ToString(),"mgrid"+(i+1));
                                dtshares = oper.get_step4_get_share_distributegrid(companyId.ToString(), dirid);
                                if (dtshares.Rows.Count > 0)
                                {
                                    string noofshare = dtshares.Rows[0]["noofshares_c"].ToString();
                                    string permaountpaid = dtshares.Rows[0]["amountpaidpershare_c"].ToString();
                                    string permaountunpaid = dtshares.Rows[0]["amountunpaidpershare_c"].ToString();
                                    string permaountpaidTotal = dtshares.Rows[0]["totalamountpaidpershare_c"].ToString();
                                    string permaountunpaidTotal = dtshares.Rows[0]["totalamountunpaidpershare_c"].ToString();
                                    string shareclass = dtshares.Rows[0]["shareclass_c"].ToString();

                                    comdeeds.Models.BaseModel.ClassShareModel obj = new Models.BaseModel.ClassShareModel();
                                    obj.Id = Convert.ToInt64(id);
                                    obj.dId = Convert.ToInt64(dirid);
                                    obj.dirName = directorname;
                                    obj.noofshare = Convert.ToDouble(noofshare);
                                    obj.other = false;
                                    if (beneficialownername.Trim() != "") { obj.other = true; }
                                    obj.ownername = beneficialownername;
                                    obj.sharedetailsnotheldanotherorg = sharedetailsnotheldanotherorg;
                                    obj.shareclass = shareclass;
                                    obj.sharecost = Convert.ToDouble(permaountpaid);

                                    obj.isheldanotherorg = isheldanotherorg;
                                    obj.shareoption = shareoption;                                    
                                    companyDir.Add(obj);
                                }
                            }                            
                        }
                        if (!check)
                        {
                            comdeeds.Models.BaseModel.ClassShareModel obj = new Models.BaseModel.ClassShareModel();
                            string dirid = dt.Rows[j]["id"].ToString();
                            string firstname = dt.Rows[j]["firstname"].ToString();
                            string familyname = dt.Rows[j]["familyname"].ToString();
                            obj.dId = Convert.ToInt64(dirid);
                            obj.dirName = firstname + " " + familyname;
                            obj.Id = 0;
                            obj.noofshare = 1;
                            obj.other = false;
                            obj.ownername = firstname + " " + familyname;
                            obj.shareclass = "ordinary";
                            obj.sharecost = 1;
                            obj.isheldanotherorg = "yes";
                            obj.shareoption = "Unpaid";
                            companyDir.Add(obj);
                        }
                    }
                }
                else
                {
                    //DataTable dt = oper.get_step3(companyId.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            comdeeds.Models.BaseModel.ClassShareModel obj = new Models.BaseModel.ClassShareModel();
                            string dirid = dt.Rows[i]["id"].ToString();
                            string firstname = dt.Rows[i]["firstname"].ToString();
                            string familyname = dt.Rows[i]["familyname"].ToString();
                            obj.dId = Convert.ToInt64(dirid);
                            obj.dirName = firstname + " " + familyname;
                            obj.Id = 0;
                            obj.noofshare = 1;
                            obj.other = false;
                            obj.ownername = firstname + " " + familyname;
                            obj.shareclass = "ordinary";
                            obj.sharecost = 1;
                            obj.isheldanotherorg = "yes";
                            obj.shareoption = "Unpaid";
                            companyDir.Add(obj);
                        }
                    }
                }
            }

            return new { msg = companyDir.Count > 0 ? "success" : "null", shares = companyDir };
        }

        [HttpGet]
        public dynamic GetIndShare()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            var companyId = GetCompanyCookieId(Request);
            List<comdeeds.Models.BaseModel.ClassIndShareModel> companyDir = new List<comdeeds.Models.BaseModel.ClassIndShareModel>();
            if (companyId > 0)
            {
                DataTable dtanother = new DataTable();
                dtanother = oper.get_step4_anothershareholder1(companyId.ToString());
                if (dtanother.Rows.Count > 0)
                {
                    for (int i = 0; i < dtanother.Rows.Count; i++)
                    {
                        int dtanothercount = dtanother.Rows.Count;
                        // basic indivisual info get from datebase and fill data into page
                        string beneficialownername = "";
                        string id = dtanother.Rows[i]["id"].ToString();
                        string dirid = dtanother.Rows[i]["dirid"].ToString();
                        string shareholderdetails = dtanother.Rows[i]["shareholderdetails"].ToString();
                        string indiviual_or_company = dtanother.Rows[i]["individual_or_company"].ToString();

                        string indiviual_or_company_name = "";
                        if (dtanother.Rows[i]["individual_or_company_name"] != null)
                        {
                            indiviual_or_company_name = dtanother.Rows[i]["individual_or_company_name"].ToString();
                        }

                        string indiviual_or_company_acn = "";
                        if (dtanother.Rows[i]["individual_or_company_acn"] != null)
                        {
                            indiviual_or_company_acn = dtanother.Rows[i]["individual_or_company_acn"].ToString();
                        }

                        string indiviual_or_company_dob = dtanother.Rows[i]["individual_or_company_dob"].ToString();

                        string placeofbirth = dtanother.Rows[i]["placeofbirth"].ToString();

                        string individual_or_company_unit_level_suite = dtanother.Rows[i]["individual_or_company_unit_level_suite"].ToString();
                        string individual_or_company_streetNoName = dtanother.Rows[i]["individual_or_company_streetNoName"].ToString();
                        string individual_or_company_suburb_town_city = dtanother.Rows[i]["individual_or_company_suburb_town_city"].ToString();
                        string individual_or_company_state = dtanother.Rows[i]["individual_or_company_state"].ToString();
                        string individual_or_company_postcode = dtanother.Rows[i]["individual_or_company_postcode"].ToString();

                        string isheldanotherorg = dtanother.Rows[i]["isheldanotherorg"].ToString();
                        if (isheldanotherorg.Trim().ToLower() == "yes")
                        {
                            beneficialownername = dtanother.Rows[i]["beneficialownername"].ToString();
                        }
                        else
                        {
							beneficialownername = dtanother.Rows[i]["beneficialownername"].ToString();
                        }


						string shareoption = dtanother.Rows[i]["shareoption"].ToString();						
						string sharedetailsnotheldanotherorg = "";
						if (isheldanotherorg.Trim().ToLower() == "no")
						{
							sharedetailsnotheldanotherorg = Convert.ToString(dtanother.Rows[i]["sharedetailsnotheldanotherorg"]);
							//sharedetailsnotheldanotherorg = dtanother.Rows[i]["beneficialownername"].ToString();
						}

						DataTable dtshares = new DataTable();
                        dtshares = oper.get_step4_get_share_distributegrid12(companyId.ToString(), indiviual_or_company);
                        if (dtshares.Rows.Count > 0)
                        {
                            string noofshare = dtshares.Rows[0]["noofshares_c"].ToString();
                            string permaountpaid = dtshares.Rows[0]["amountpaidpershare_c"].ToString();
                            string permaountunpaid = dtshares.Rows[0]["amountunpaidpershare_c"].ToString();
                            string permaountpaidTotal = dtshares.Rows[0]["totalamountpaidpershare_c"].ToString();
                            string permaountunpaidTotal = dtshares.Rows[0]["totalamountunpaidpershare_c"].ToString();
                            string shareclass = dtshares.Rows[0]["shareclass_c"].ToString();

                            comdeeds.Models.BaseModel.ClassIndShareModel obj = new Models.BaseModel.ClassIndShareModel();
                            obj.Id = Convert.ToInt64(id);
                            obj.sharedirectorsCounter = dtanothercount;
                            string[] indiviual_or_company_dob_new = indiviual_or_company_dob.Split('-');

                            if (indiviual_or_company == "Individual1")
                            {
                                obj.share_indname = "Individual";
                                obj.rdshare_individual = "true";
                                obj.ind_dobyear = indiviual_or_company_dob_new[0].Trim();
                                obj.ind_dobmonth = indiviual_or_company_dob_new[1].Trim();
                                obj.ind_dobday = indiviual_or_company_dob_new[2].Trim();
                                obj.share_placeofbirth = placeofbirth;
                                obj.share_indname = shareholderdetails;

                                obj.shareaddunit = individual_or_company_unit_level_suite;
                                obj.shareaddstreet = individual_or_company_streetNoName;
                                obj.shareaddsuburb = individual_or_company_suburb_town_city;
                                obj.ddshareaddstate = individual_or_company_state.Trim();
                                obj.shareaddpostcode = individual_or_company_postcode;

                                obj.share_noofshare = Convert.ToDouble(noofshare);
                                obj.share_indshareownername = beneficialownername;
                                obj.share_sharetype = shareclass;
                                obj.share_amountopaidshare = Convert.ToDouble(permaountpaid);
								
								obj.shareoption = shareoption;								
								obj.isheldanotherorg = isheldanotherorg;
								obj.sharedetailsnotheldanotherorg = sharedetailsnotheldanotherorg;
								companyDir.Add(obj);
                            }
                            else if (indiviual_or_company == "company")
                            {
                                obj.share_comname = indiviual_or_company;
                                obj.rdshare_company = "true";

                                obj.share_compORtrtname = indiviual_or_company_name;
                                obj.share_compORtrtABN = indiviual_or_company_acn;

                                obj.sharecompunit = individual_or_company_unit_level_suite;
                                obj.sharecompstreet = individual_or_company_streetNoName;
                                obj.sharecompsuburb = individual_or_company_suburb_town_city;
                                obj.ddsharecompstate = individual_or_company_state.Trim();
                                obj.sharecomppostcode = individual_or_company_postcode;

                                obj.share_noofcompshare = Convert.ToDouble(noofshare);
                                obj.share_compshareownername = beneficialownername;
                                obj.share_compsharetype = shareclass;
                                obj.share_amountopaidcompshare = Convert.ToDouble(permaountpaid);
								obj.isheldanotherorg = isheldanotherorg;								
								obj.shareoption = shareoption;
								obj.sharedetailsnotheldanotherorg = sharedetailsnotheldanotherorg;
								companyDir.Add(obj);
                            }
                        }
                    }
                }
                else
                {
                    comdeeds.Models.BaseModel.ClassIndShareModel obj = new Models.BaseModel.ClassIndShareModel();
                    obj.Id = 0;
                    obj.share_indname = "";
                    obj.ind_dobday = "";
                    obj.ind_dobmonth = "";
                    obj.ind_dobyear = "";
                    obj.share_placeofbirth = "";
                    obj.shareaddunit = "";
                    obj.shareaddstreet = "";
                    obj.shareaddsuburb = "";
                    obj.ddshareaddstate = "";
                    obj.shareaddpostcode = "";
                    obj.share_indshareownername = "";
                    obj.share_sharetype = "ordinary";
                    obj.share_noofshare = 1;
                    obj.share_amountopaidshare = 1;
					obj.isheldanotherorg = "yes";					
					obj.shareoption = "";					
					obj.sharedetailsnotheldanotherorg = "";
					companyDir.Add(obj);
                }
            }

            return new { msg = companyDir.Count > 0 ? "success" : "null", shares = companyDir };
        }

        #endregion Company form methods

        #region Trust form methods

        [HttpGet]
        public dynamic GetTrustDetails()
        {
            string msg = "";
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            var trustId = GetTrustCookieId(Request);
            ClassTrustDetails data = new ClassTrustDetails();
            if (trustId > 0)
            {
                var t = TrustMethods.GetTrustDetail(trustId);
                if (t != null)
                {
                    data = new ClassTrustDetails
                    {
                        Id = t.Id,
                        Trust_Date = t.TrustSetupDate.HasValue ? (DateTime)t.TrustSetupDate : DateTime.Now,
                        TrustName = t.TrustName,
                        TrustType = t.TrustType,
                        TrustState = t.TrustState,
                        Abn = t.Abn,
                        PropertyTrusteeAcn = t.PropertyTrusteeAcn,
                        PropertyAddress = t.PropertyAddress,
                        LenderName = t.LenderName,
                        PropertyTrusteeName = t.PropertyTrusteeName,
                        Smsf = t.Smsf,
                        SmsfAcn = t.Smsfacn,
                        SmsfCompanyName = t.SmsfCompanyName,
                        SmsfCompanySetupDate = t.SmsfTrusteeSetupDate.HasValue ? (DateTime)t.SmsfTrusteeSetupDate : DateTime.Now,
                        PropertyTrusteeDate = t.PropertyTrusteeSetupDate.HasValue ? (DateTime)t.PropertyTrusteeSetupDate : DateTime.Now,
                        ExistingSetupDate = t.ExistingSetupDate.HasValue ? (DateTime)t.ExistingSetupDate : DateTime.Now,
                        ClauseNumber = t.ClauseNumber
                    };
                    data.TrustDate = data.Trust_Date.ToShortDateString();
                }
                msg = data != null ? "success" : "null";
            }
            else
            {
                msg = "null";
            }
            return new
            {
                msg = msg,
                trust = data
            };
        }

        [HttpPost]
        public dynamic AddTrust(ClassUserDetails user)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string msg = "";
            var trustId = GetTrustCookieId(Request);
            user.Id = uid;
            var tid = TrustMethods.AddTrust(user, trustId);
            if (tid > 0)
            {
                var c = new HttpCookie(TrustCookieId, tid.ToString());
                c.Expires = DateTime.Now.AddMonths(6);
                HttpContext.Current.Response.Cookies.Add(c);
                msg = "success";
            }
            return new { msg = msg };
        }

        [AcceptVerbs("GET", "POST")]
        //   [HttpPost]
        public dynamic updateTrust(ClassTrustDetails trust)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string msg = "";
            var trustId = GetTrustCookieId(Request);
            if (trustId > 0)
            {
                DateTime dateValue;
                string[] formats = { "dd/MM/yyyy" };
                if (DateTime.TryParseExact(trust.TrustDate, formats,
                        new CultureInfo("en-US"),
                        System.Globalization.DateTimeStyles.None, out dateValue))
                {
                    trust.Trust_Date = DateTime.ParseExact(trust.TrustDate, "dd/MM/yyyy",
                        new CultureInfo("en-US"),
                        System.Globalization.DateTimeStyles.None);
                    var tid = TrustMethods.UpdateTrust(trust, uid);
                    if (tid > 0)
                    {
                        msg = "success";
                    }
                    else
                    {
                        msg = "error";
                    }
                }
                else
                {
                    msg = "invalid_date";
                }
            }
            else
            {
                msg = "error";
            }
            return new { msg = msg };
        }

        [HttpGet]
        public dynamic GetTrustAppointers()
        {
            string msg = "";
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            var trustId = GetTrustCookieId(Request);
            ClassTrustAppointerform data = new ClassTrustAppointerform();
            if (trustId > 0)
            {
                var t = TrustMethods.GetTrustAppointer(trustId);
                if (t.Count > 0)
                {
                    data.appointer = t.ToList();
                    data.OrdinaryPrice = t.FirstOrDefault().OrdinaryPrice;
                    data.TotalUnitHolders = t.FirstOrDefault().TotalUnitHolders;
                }
                msg = data.appointer.FirstOrDefault().Id > 0 ? "success" : "null";
                data.appointer = data.appointer.FirstOrDefault().Id > 0 ? data.appointer : null;
            }
            else
            {
                msg = "null";
            }
            return new
            {
                msg = msg,
                trust = data
            };
        }

        [HttpPost]
        public dynamic AddTrustAppointer(ClassTrustAppointerform user)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string msg = "";
            var trustId = GetTrustCookieId(Request);

            var res = TrustMethods.UpdateTrustAppointer(user, trustId, uid);
            if (res)
            {
                msg = "success";
            }
            else
            {
                msg = "error";
            }
            return new { msg = msg };
        }

        [HttpGet]
        public dynamic GetTrustees()
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string msg = "";
            var trustId = GetTrustCookieId(Request);
            var data = new ClassTrustCheckout();
            var appointers = TrustMethods.GetTrustAppointer(trustId);
            if (trustId > 0)
            {
                data = TrustMethods.getBeneficiaryDetails(trustId);
                msg = data.total > 0 ? "success" : "null";
            }
            else
            {
                msg = "error";
            }
            return new
            {
                msg = msg,
                data = data
            };
        }

        [HttpPost]
        public dynamic updateBeneficiaries(ClassBeneficiary trust)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER", "SUBADMIN" }, "/user/signin"));
            string msg = "";
            var trustId = GetTrustCookieId(Request);
            var res = false;
            if (trustId > 0)
            {
                if (trust.bType.ToLower() == "person")
                {
                    var l = trust.Members.Where(x => x.istrustee == true).ToList();
                    if (l != null && l.Count > 0)
                    {
                        //res = TrustMethods.UpdateBeneficiary(l);
                        trust.Members = l;
                    }
                }

                res = TrustMethods.UpdateBeneficiary(trust, trustId, uid);
                if (res)
                {
                    msg = "success";
                }
                else
                {
                    msg = "error";
                }
            }
            else
            {
                msg = "error";
            }
            return new { msg = msg };
        }

        #endregion Trust form methods

        public static long GetCompanyCookieId(HttpRequestMessage request)
        {
            var cookie = request.Headers.GetCookies(CompanyCookieId).FirstOrDefault();

            if (cookie != null)
            {
                if (cookie[CompanyCookieId].Value != null)
                {
                    if (cookie[CompanyCookieId].Value != "")
                    {
                        return cookie == null ? 0 : Convert.ToInt64(cookie[CompanyCookieId].Value);
                    }
                }
            }
            return 0;
        }

        public static long GetTrustCookieId(HttpRequestMessage request)
        {
            var cookie = request.Headers.GetCookies(TrustCookieId).FirstOrDefault();
            return cookie == null ? 0 : Convert.ToInt64(cookie[TrustCookieId].Value);
        }
    }
}