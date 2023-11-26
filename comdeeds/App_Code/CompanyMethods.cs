using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using comdeeds.Models;
using System.Data;
using Dapper;
using static comdeeds.Models.BaseModel;

namespace comdeeds.App_Code
{
    public class CompanyMethods
    {
        private ErrorLog objlog = new ErrorLog();

        #region GetCompanyDetails Bhu

        private dal.Operation oper = new dal.Operation();
        private long Regid;

        public string getuserid(string uid)
        {
            string useremail = "";
            DataTable dtuser = oper.get_userdetails_byuid(uid.ToString());
            if (dtuser.Rows.Count > 0)
            {
                useremail = CryptoHelper.DecryptString(dtuser.Rows[0]["Email"].ToString());
                Regid = Convert.ToInt64(dtuser.Rows[0]["Regid"]);
            }
            return useremail;
        }

        public static string compnydetaILS(string email)
        {
            string companyid = "";
            dal.DataAccessLayer dd = new dal.DataAccessLayer();
            dal.Operation oper = new dal.Operation();
            DataTable dt = dd.getdata("select * from companysearch where userid='" + email + "'");
            if (dt.Rows.Count > 0)
            {
                companyid = dt.Rows[0]["id"].ToString();
            }
            return companyid;
        }

        private List<comdeeds.Models.BaseModel.LoginUserData> userdetails_()
        {
            List<comdeeds.Models.BaseModel.LoginUserData> objlist = new List<BaseModel.LoginUserData>();
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            string useremail = getuserid(uid.ToString());
            comdeeds.Models.BaseModel.LoginUserData obj = new BaseModel.LoginUserData();
            DataTable dtuser = oper.get_registration(useremail, Regid);
            if (dtuser.Rows.Count > 0)
            {
                obj.email = useremail.ToString();
                obj.FirstName = dtuser.Rows[0]["givenname"].ToString();
                obj.LastName = dtuser.Rows[0]["familyname"].ToString();
                obj.Phone = dtuser.Rows[0]["Phone"].ToString();
                obj.LastLogin = DateTime.Now;
                obj.IsFirstLogin = false;
                objlist.Add(obj);
            }
            return objlist;
        }

        public static List<TblCompany> companyDetails_(string companyid)
        {
            ErrorLog objlog = new ErrorLog();

            List<TblCompany> objlist = new List<TblCompany>();
            TblCompany obj = new TblCompany();
            dal.Operation oper = new dal.Operation();
            DataTable dt = oper.get_step1(companyid);
            try
            {
                if (dt.Rows.Count > 0)
                {
                    string isproposeidentical = dt.Rows[0]["isproposeidentical"].ToString();
                    if (isproposeidentical.ToLower() == "yes")
                    {
                        obj.NameReserved = true;
                    }
                    else
                    {
                        obj.NameReserved = false;
                    }
                    obj.CompanyName = dt.Rows[0]["companyname"].ToString() + " " + dt.Rows[0]["companyname_ext"].ToString();
                    obj.Abn = dt.Rows[0]["proposeidentical_after28may_abnnumber"].ToString();
                    obj.RegistrationState = dt.Rows[0]["stateterritorry"].ToString();
                    obj.CompanyUseFor = dt.Rows[0]["companyusedfor"].ToString();
                    obj.CompanyPurpose = dt.Rows[0]["typeofcompany"].ToString();
                    obj.specialpurpose_ifapplicable = dt.Rows[0]["specialpurpose_ifapplicable"].ToString();
                    obj.isspecialpurpose = dt.Rows[0]["isspecialpurpose"].ToString();
                    objlist.Add(obj);
                }
            }
            catch (Exception ex)
            {
                objlog.WriteErrorLog(ex.ToString());
            }
            return objlist;
        }

        public static List<TblCompanyAddress> companyAddress_(string companyid)
        {
            ErrorLog objlog1 = new ErrorLog();
            dal.Operation oper = new dal.Operation();
            List<TblCompanyAddress> objlist = new List<TblCompanyAddress>();
            TblCompanyAddress obj = new TblCompanyAddress();
            DataTable dt = oper.getStep2_bycid(companyid);
            try
            {
                if (dt.Rows.Count > 0)
                {
                    string unit_level_suite = dt.Rows[0]["unit_level_suite"].ToString();
                    string streetNoName = dt.Rows[0]["streetNoName"].ToString();
                    string suburb_town_city = dt.Rows[0]["suburb_town_city"].ToString();
                    string state = dt.Rows[0]["state"].ToString();
                    string postcode = dt.Rows[0]["postcode"].ToString();

                    string unit_level_suite_primary = dt.Rows[0]["unit_level_suite_primary"].ToString();
                    string streetNoName_primary = dt.Rows[0]["streetNoName_primary"].ToString();
                    string suburb_town_city_primary = dt.Rows[0]["suburb_town_city_primary"].ToString();
                    string state_primary = dt.Rows[0]["state_primary"].ToString();
                    string postcode_primary = dt.Rows[0]["postcode_primary"].ToString();

                    string isprimaryaddress = dt.Rows[0]["isprimaryaddress"].ToString();
                    if (isprimaryaddress.ToLower() == "yes")
                    {
                        unit_level_suite_primary = unit_level_suite;
                        streetNoName_primary = streetNoName;
                        suburb_town_city_primary = suburb_town_city;
                        state_primary = state;
                        postcode_primary = postcode;
                    }
                    obj.IsRegisteredAddress = true;
                    obj.IsPrincipleAddress = false;
                    obj.UnitLevel = unit_level_suite;
                    obj.Street = streetNoName;
                    obj.Suburb = suburb_town_city;
                    obj.State = state;
                    obj.PostCode = postcode;
                    objlist.Add(obj);

                    obj = new TblCompanyAddress();
                    obj.IsRegisteredAddress = false;
                    obj.IsPrincipleAddress = true;
                    obj.UnitLevel = unit_level_suite_primary;
                    obj.Street = streetNoName_primary;
                    obj.Suburb = suburb_town_city_primary;
                    obj.State = state_primary;
                    obj.PostCode = postcode_primary;
                    objlist.Add(obj);
                }
            }
            catch (Exception ex)
            {
                objlog1.WriteErrorLog(ex.ToString());
            }
            return objlist;
        }

        private static List<TblCompanyDirector> companyDirectors_(string companyid)
        {
            ErrorLog objlog2 = new ErrorLog();
            dal.Operation oper = new dal.Operation();
            List<TblCompanyDirector> objlist = new List<TblCompanyDirector>();
            TblCompanyDirector obj = new TblCompanyDirector();
            try
            {
                DataTable dt = oper.get_step3(companyid);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        obj = new TblCompanyDirector();
                        string id = dt.Rows[i]["id"].ToString();
                        string firstname = dt.Rows[i]["firstname"].ToString();
                        string familyname = dt.Rows[i]["familyname"].ToString();
                        string dob = dt.Rows[i]["dob"].ToString();
                        string designation = dt.Rows[i]["designation"].ToString();

                        string unit_level_suite_primary = dt.Rows[i]["unit_level_suite_primary"].ToString();
                        string streetNoName_primary = dt.Rows[i]["streetNoName_primary"].ToString();
                        string suburb_town_city_primary = dt.Rows[i]["suburb_town_city_primary"].ToString();
                        string state_primary = dt.Rows[i]["state_primary"].ToString();
                        string postcode_primary = dt.Rows[i]["postcode_primary"].ToString();
                        string country = dt.Rows[i]["country"].ToString();
                        string address = "";
                        if (unit_level_suite_primary != "" && unit_level_suite_primary.Trim().IndexOf(" ") > -1)
                        {
                            address = (unit_level_suite_primary.Trim().Replace(" ", "/") + " " + streetNoName_primary + " "  + suburb_town_city_primary + " " + postcode_primary).TrimStart() + " " + state_primary;//+ " " + country
						}
                        else
                        {
                            address = (unit_level_suite_primary + "/ " + streetNoName_primary +  " " + suburb_town_city_primary + " " + postcode_primary).TrimStart() +" " + state_primary;//+ " " + country
						}
                        string cityofbirth = dt.Rows[i]["placeofbirth"].ToString();
                        string countryofbirth = dt.Rows[i]["countryofbirth"].ToString();
                        obj.Id = Convert.ToInt32(id);
                        obj.FirstName = firstname;
                        obj.LastName = familyname;
                        obj.DoBaddress = address;
                        obj.designation = designation;
                        obj.DoBcity = cityofbirth.Split(',')[0];
                        obj.DoBstate = cityofbirth.Split(',')[1];
                        obj.DoBcountry = countryofbirth;
                        obj.DoByear = Convert.ToInt32(dob.Split('-')[0].ToString());
                        obj.DoBmonth = Convert.ToInt32(dob.Split('-')[1].ToString());
                        obj.DoBday = Convert.ToInt32(dob.Split('-')[2].ToString());
                        objlist.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                objlog2.WriteErrorLog(ex.ToString());
            }

            return objlist;
        }

        private static List<TblCompanyShare> companyShares_(string companyid)
        {
            ErrorLog objlog4 = new ErrorLog();
            dal.Operation oper = new dal.Operation();
            List<TblCompanyShare> objlist = new List<TblCompanyShare>();
            TblCompanyShare obj = new TblCompanyShare();

            try
            {
                DataTable dt = oper.get_step4_anothershareholder(companyid);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        obj = new TblCompanyShare();
                        string id = dt.Rows[i]["id"].ToString();
                        string directorid = dt.Rows[i]["dirid"].ToString();
                        string isheldanotherorg = dt.Rows[i]["isheldanotherorg"].ToString();
                        string beneficialownername = dt.Rows[i]["beneficialownername"].ToString();
						string shareoption = dt.Rows[i]["shareoption"].ToString();
						string sharedetailsnotheldanotherorg = dt.Rows[i]["sharedetailsnotheldanotherorg"].ToString();
						obj.Id = Convert.ToInt32(id);
                        obj.DirectorId = Convert.ToInt64(directorid);
                        obj.ShareBehalf = isheldanotherorg == "yes" ? true : false;
                        obj.OwnerName = beneficialownername;
						obj.shareoption = shareoption;
						obj.sharedetailsnotheldanotherorg = sharedetailsnotheldanotherorg;

						DataTable dtshare = new DataTable();
                        dtshare = oper.get_step4_get_share_distributegrid(companyid, directorid);
						if (dtshare.Rows.Count > 0)
						{
						string ShareClass = dtshare.Rows[0]["ShareClass_c"].ToString();
                        string NoOfShare = dtshare.Rows[0]["noofshares_c"].ToString();
                        string ShareAmount = dtshare.Rows[0]["amountpaidpershare_c"].ToString();
                        obj.ShareClass = ShareClass;
                        obj.NoOfShare = Convert.ToDouble(NoOfShare);
						//by praveen divide by 100
                        obj.ShareAmount = Convert.ToDouble(ShareAmount);
							// obj.individual_or_company = dt.Rows[i]["individual_or_company"].ToString();
							//obj.individual_or_company_address = dt.Rows[i]["individual_or_company_address"].ToString();
							//  obj.placeofbirth = dt.Rows[i]["placeofbirth"].ToString();
						}
                        objlist.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                objlog4.WriteErrorLog(ex.ToString());
            }

            return objlist;
        }

        private static List<TblCompanyShare> indcompanyShares_(string companyid)
        {
            ErrorLog objlog4 = new ErrorLog();
            dal.Operation oper = new dal.Operation();
            List<TblCompanyShare> objlist = new List<TblCompanyShare>();
            TblCompanyShare obj = new TblCompanyShare();

            try
            {
                DataTable dt = oper.get_step4_anothershareholder1(companyid);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        obj = new TblCompanyShare();
                        string id = dt.Rows[i]["id"].ToString();
                        string directorid = dt.Rows[i]["dirid"].ToString();
                        string isheldanotherorg = dt.Rows[i]["isheldanotherorg"].ToString();
                        string beneficialownername = dt.Rows[i]["beneficialownername"].ToString();
                        string individual_or_company = dt.Rows[i]["individual_or_company"].ToString();
						string sharedetailsnotheldanotherorg = dt.Rows[i]["sharedetailsnotheldanotherorg"].ToString();

						obj.Id = Convert.ToInt32(id);
                        obj.DirectorId = Convert.ToInt64(directorid);
                        obj.ShareBehalf = isheldanotherorg == "yes" ? true : false;
                        obj.OwnerName = beneficialownername;
						obj.sharedetailsnotheldanotherorg = sharedetailsnotheldanotherorg;

						DataTable dtshare = new DataTable();
                        dtshare = oper.get_step4_get_share_distributegrid12(companyid, individual_or_company);
                        string ShareClass = dtshare.Rows[0]["ShareClass_c"].ToString();
                        string NoOfShare = dtshare.Rows[0]["noofshares_c"].ToString();
                        string ShareAmount = dtshare.Rows[0]["amountpaidpershare_c"].ToString();
                        obj.ShareClass = ShareClass;
                        obj.NoOfShare = Convert.ToDouble(NoOfShare);
                        obj.ShareAmount = Convert.ToDouble(ShareAmount);
                        obj.individual_or_company = dt.Rows[i]["individual_or_company"].ToString();
                        obj.individual_or_company_address = dt.Rows[i]["individual_or_company_address"].ToString();
                        obj.shareholderdetails = dt.Rows[i]["shareholderdetails"].ToString();
                        obj.individual_or_company_dob = dt.Rows[i]["individual_or_company_dob"].ToString();
                        obj.placeofbirth = dt.Rows[i]["placeofbirth"].ToString();
                        obj.individual_or_company_acn = dt.Rows[i]["individual_or_company_acn"].ToString();
                        objlist.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                objlog4.WriteErrorLog(ex.ToString());
            }

            return objlist;
        }

        #endregion GetCompanyDetails Bhu

        public static comdeeds.Models.BaseModel.ClassAsicSetup getAsicDetails(string companyid)
        {
            ErrorLog errorLog1 = new ErrorLog();
            var data = new comdeeds.Models.BaseModel.ClassAsicSetup();
            dal.Operation oper = new dal.Operation();
            // List<ClassAsicSetup> objlist = new List<ClassAsicSetup>();
            ClassAsicSetup obj = new ClassAsicSetup();
            DataTable dt = oper.getcompanysearchbyid(companyid);
            try
            {
                if (dt.Rows.Count > 0)
                {
                    obj.Asic_status = dt.Rows[0]["Asic_status"].ToString();
                    obj.Asic_File = dt.Rows[0]["Asic_File"].ToString();
                    obj.Asic_Error = dt.Rows[0]["Asic_Error"].ToString();
                    obj.Asic_DocNo = dt.Rows[0]["Asic_DocNo"].ToString();
                    obj.Asic_ACN = dt.Rows[0]["Asic_ACN"].ToString();
                    obj.Asic_ResType = dt.Rows[0]["Asic_ResType"].ToString();

                    // objlist.Add(obj);
                }
            }
            catch (Exception ex)
            {
                errorLog1.WriteErrorLog(ex.ToString());
            }
            return obj;
        }

        public static comdeeds.Models.BaseModel.ClassFullCompany GetFullCompanyData(long companyid)
        {
            ErrorLog errorLog1 = new ErrorLog();

            var data = new comdeeds.Models.BaseModel.ClassFullCompany();
            var comp = companyDetails_(companyid.ToString()).FirstOrDefault();
            var ca = companyAddress_(companyid.ToString()).ToList();
            var cd = companyDirectors_(companyid.ToString()).ToList();
            var cs = companyShares_(companyid.ToString()).ToList();
            var ics = indcompanyShares_(companyid.ToString()).ToList();

            try
            {
                using (var db = new MyDbContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    var p = new DynamicParameters();
                    p.Add("@cid", companyid, dbType: System.Data.DbType.Int64);
                    using (var d = db.Database.Connection.QueryMultiple("getShortCompanyDetail", p, commandType: System.Data.CommandType.StoredProcedure))
                    {
                        //var comp = d.Read<TblCompany>().FirstOrDefault();
                        //var ct = d.Read<TblCompanyTrust>().FirstOrDefault();
                        //var ca = d.Read<TblCompanyAddress>().ToList();
                        //var cd = d.Read<TblCompanyDirector>().ToList();
                        //var cs = d.Read<TblCompanyShare>().ToList();
                        var cm = d.Read<ClassCompanyShortDetail>().FirstOrDefault();
                        var ap = d.Read<Registration>().FirstOrDefault();
                        var c = d.Read<TblOption>().ToList();
                        var t = d.Read<TblTransaction>().FirstOrDefault();
                        var st = d.Read<TblCompany>().FirstOrDefault();
                        var css = d.Read<Companysearch>().FirstOrDefault();
                        if (st != null)
                        {
                            comp.Agreement = st.Agreement;
                            comp.BorrowingReview = st.BorrowingReview;
                            comp.LegalAssessment = st.LegalAssessment;
                            comp.QuoteForTax = st.QuoteForTax;

							//by praveen
							comp.CompanySecretary = st.CompanySecretary;
							comp.PublicOfficerOfCompany = st.PublicOfficerOfCompany;
							comp.HowfstmeetingOfDirheld = st.HowfstmeetingOfDirheld;
							comp.DateOfIncorporation = st.DateOfIncorporation;
							if (st.DateOfIncorporation == null)
							{
								DateTime dt = DateTime.Now;
								comp.DateOfIncorporation = dt.ToString("dd/MM/yyyy");
							}

						}
						else
						{
							comp.CompanySecretary = "";
							comp.PublicOfficerOfCompany = "";
							comp.HowfstmeetingOfDirheld = ""; 
							DateTime dt = DateTime.Now;
							comp.DateOfIncorporation = dt.ToString("dd/MM/yyyy");
						}

						var cost = new comdeeds.Models.BaseModel.ClassSetupPrice();
                        if (c != null && c.Count > 0)
                        {
                            cost.AsicFee = c.Any(x => x.OptionName.ToLower() == "asicfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "asicfee").FirstOrDefault().OptionValue) : 479;
                            cost.SetupCost = c.Any(x => x.OptionName.ToLower() == "companysetupcost") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "companysetupcost").FirstOrDefault().OptionValue) : 32;
                            cost.SetupGST = c.Any(x => x.OptionName.ToLower() == "companygst") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "companygst").FirstOrDefault().OptionValue) : 9;
                            cost.CreditCardFee = c.Any(x => x.OptionName.ToLower() == "creditcardfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "creditcardfee").FirstOrDefault().OptionValue) : 1.75;
                            cost.ProcessingFee = c.Any(x => x.OptionName.ToLower() == "processingfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "processingfee").FirstOrDefault().OptionValue) : .3;
                        }
                        else
                        {
                            cost.AsicFee = 479;
                            cost.SetupCost = 20;
                            cost.SetupGST = 2;
                            cost.CreditCardFee = 1.75;// %
                            cost.ProcessingFee = .3; // c
                        }
                        var ccf = ((cost.AsicFee + cost.SetupCost + cost.SetupGST) * cost.CreditCardFee) / 100; // Credit card fees

                        //if (css.govofcomapany == "yes" && comp.CompanyUseFor == "smsf")
                        //{
                        //    cost.TotalCost = Math.Round((cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee + cost.AsicFee + 48+10+1), 2);
                        //}
                        //else if (comp.CompanyUseFor == "smsf")
                        //{
                        //    cost.TotalCost = Math.Round((cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee + cost.AsicFee + 48), 2);
                        //}

                        if (css.govofcomapany == "yes")
                        {
                            cost.TotalCost = Math.Round((cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee + cost.AsicFee + 10 + 1), 2);
                        }
                        else
                        {
                            cost.TotalCost = Math.Round((cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee + cost.AsicFee), 2);
                        }

                        //if((comp.CompanyUseFor == "A company to operate business" || comp.CompanyUseFor == "Trustee for a Self Managed Super") && ics.Count > 0)
                        //{
                        //     cost.TotalCost = Math.Round((cost.TotalCost + 254), 2);
                        //}
                        //cost.TotalCost = cost.SetupCost + cost.SetupGST + cost.AsicFee;
                        data.Cost = cost;
                        data.Company = comp;
                        data.CompanyMeta = cm;
                        data.Address = ca;
                        data.Directors = cd;
                        data.Shares = cs;
                        data.Applicant = ap;
                        data.TransactionDetail = t;
                        data.indShares = ics;
                        data.companysearch = css;
                    }
                }
            }
            catch (Exception ex)
            {
                errorLog1.WriteErrorLog(ex.ToString());
                throw;
            }

            //var c=companyDetails_(companyid.ToString()).FirstOrDefault();
            //var ap=companyDetails_(companyid.ToString()).FirstOrDefault();
            return data;
        }

        public static bool UpdateCompanyOption(comdeeds.Models.BaseModel.ClassTrustOption opt, long trustid, long uid)
        {
            bool flag = false;
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@cid", trustid, dbType: System.Data.DbType.Int64);
                p.Add("@quotefortax", opt.chkquotefortax, dbType: System.Data.DbType.Boolean);
                p.Add("@legelassessment", opt.chklegalassesment, dbType: System.Data.DbType.Boolean);
                p.Add("@borrowing", opt.chkborrowingreview, dbType: System.Data.DbType.Boolean);
                p.Add("@agreement", opt.chkagreement, dbType: System.Data.DbType.Boolean);
				p.Add("@CompanySecretary", opt.CompanySecretary, dbType: System.Data.DbType.String);
				p.Add("@PublicOfficerOfCompany", opt.PublicOfficerOfCompany, dbType: System.Data.DbType.String);
				p.Add("@HowfstmeetingOfDirheld", opt.HowfstmeetingOfDirheld, dbType: System.Data.DbType.String);
				p.Add("@DateOfIncorporation", opt.DateOfIncorporation, dbType: System.Data.DbType.String);
				p.Add("@uid", uid, dbType: System.Data.DbType.Int64);
                var i = db.Database.Connection.Query<long>("updatecompanyOption", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                if (i != null && i > 0)
                {
                    flag = true;
                }
            }
            return flag;
        }
    }
}