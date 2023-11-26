using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using static comdeeds.Models.BaseModel;

namespace comdeeds.App_Code
{
    public class TrustMethods
    {
        public static TblTrust GetTrustDetail(long TrustId)
        {
            TblTrust data = new TblTrust();

            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@id", TrustId, dbType: System.Data.DbType.Int64);
                data = db.Database.Connection.Query<TblTrust>("getTrustDetail", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
            }
            return data;
        }

        public static long AddTrust(ClassUserDetails user, long trustid)
        {
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@Id", trustid, dbType: System.Data.DbType.Int64);
                p.Add("@fName", user.Firstname, dbType: System.Data.DbType.String);
                p.Add("@lName", user.Lastname, dbType: System.Data.DbType.String);
                p.Add("@email", user.Email, dbType: System.Data.DbType.String);
                p.Add("@phone", user.Phone, dbType: System.Data.DbType.String);
                p.Add("@uid", user.Id, dbType: System.Data.DbType.Int64);
                trustid = db.Database.Connection.Query<long>("addtrust", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
            }
            return trustid;
        }

        public static long UpdateTrust(ClassTrustDetails trust, long uid)
        {
            long trustid = 0;
            using (var db = new MyDbContext())
            {
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    var p = new DynamicParameters();
                    p.Add("@Id", trust.Id, dbType: System.Data.DbType.Int64);
                    p.Add("@Name", trust.TrustName, dbType: System.Data.DbType.String);
                    p.Add("@Type", trust.TrustType, dbType: System.Data.DbType.String);
                    p.Add("@SetupDate", trust.Trust_Date, dbType: System.Data.DbType.DateTime);
                    p.Add("@State", trust.TrustState, dbType: System.Data.DbType.String);
                    p.Add("@smsf", trust.Smsf, dbType: System.Data.DbType.String);
                    p.Add("@acn", trust.PropertyTrusteeAcn, dbType: System.Data.DbType.String);
                    p.Add("@abn", trust.Abn, dbType: System.Data.DbType.String);
                    p.Add("@ptn", trust.PropertyTrusteeName, dbType: System.Data.DbType.String);
                    p.Add("@padd", trust.PropertyAddress, dbType: System.Data.DbType.String);
                    p.Add("@lname", trust.LenderName, dbType: System.Data.DbType.String);
                    p.Add("@uid", uid, dbType: System.Data.DbType.Int64);
                    p.Add("@SmsfCompanyName", trust.SmsfCompanyName, dbType: System.Data.DbType.String);
                    p.Add("@SmsfAcn", trust.SmsfAcn, dbType: System.Data.DbType.String);
                    p.Add("@SmsfCompanySetupDate", trust.SmsfCompanySetupDate, dbType: System.Data.DbType.DateTime);
                    p.Add("@PropertyTrusteeDate", trust.PropertyTrusteeDate, dbType: System.Data.DbType.DateTime);
                    p.Add("@ExistingSetupDate", trust.ExistingSetupDate, dbType: System.Data.DbType.DateTime);
                    p.Add("@ClauseNumber", trust.ClauseNumber, dbType: System.Data.DbType.String);

                    trustid = db.Database.Connection.Query<long>("updatetrust", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception ex) { throw ex; }
            }
            return trustid;
        }

        public static List<ClassTrustAppointer> GetTrustAppointer(long trustId)
        {
            List<ClassTrustAppointer> data = new List<ClassTrustAppointer>();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@tid", trustId, dbType: System.Data.DbType.Int64);
                data = db.Database.Connection.Query<ClassTrustAppointer>("GetTrustAppointer", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            return data;
        }

        public static bool UpdateTrustAppointer(ClassTrustAppointerform formdata, long trustid, long uid)
        {
            var xml = "";
            bool res = false;
            foreach (var a in formdata.appointer)
            {
                xml += string.Format("<Entity><Id>{0}</Id><HolderType>{1}</HolderType><FirstName>{2}</FirstName>" +
                    "<MiddleName>{3}</MiddleName><LastName>{4}</LastName><CompanyName>{5}</CompanyName>" +
                    "<CompanyACN>{6}</CompanyACN><CommanSeal>{7}</CommanSeal><UnitLevel>{8}</UnitLevel>" +
                    "<Street>{9}</Street><State>{10}</State><Suburb>{11}</Suburb><PostCode>{12}</PostCode>" +
                    "<Country>{13}</Country><TrustId>{14}</TrustId><UnitType>{15}</UnitType><UnitNumber>{16}</UnitNumber><UnitTotalAmount>{17}</UnitTotalAmount><UnitAmountOwing>{18}</UnitAmountOwing>" +
                    "<dob>{19}</dob></Entity>", a.Id, a.HolderType, a.FirstName, a.MiddleName,
                    a.LastName, a.CompanyName, a.CompanyAcn, a.CommanSeal, a.UnitLevel, a.Street, a.State, a.Suburb, a.PostCode,
                    a.Country, a.TrustId, a.UnitType, a.UnitNumber, a.UnitTotalAmount, a.UnitAmountOwing, a.dob);
            }
            xml = string.Format("<DataSet>{0}</DataSet>", xml);

            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@xml", xml, dbType: System.Data.DbType.Xml);
                p.Add("@tid", trustid, dbType: System.Data.DbType.Int64);
                p.Add("@unitcost", formdata.OrdinaryPrice, dbType: System.Data.DbType.String);
                p.Add("@totalmember", formdata.TotalUnitHolders, dbType: System.Data.DbType.String);
                p.Add("@uid", uid, dbType: System.Data.DbType.Int64);
                var i = db.Database.Connection.Query<long>("UpdateTrustAppointer", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                res = i.Count > 0;
            }
            return res;
        }

        public static ClassTrustCheckout getBeneficiaryDetails(long trustId)
        {
            var data = new ClassTrustCheckout();

            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@tid", trustId, dbType: System.Data.DbType.Int64);
                using (var d = db.Database.Connection.QueryMultiple("getBeneficiaryDetails", p, commandType: System.Data.CommandType.StoredProcedure))
                {
                    var Beneficiaries = d.Read<ClassBeneficiaryList>().ToList();
                    var c = d.Read<TblOption>().ToList();
                    //var l = new List<ClassBeneficiaryList>();
                    foreach (var b in Beneficiaries)
                    {
                        if (b.istrustee)
                        {
                            data.bType = b.HolderType == "company" ? "company" : "person";
                        }
                    }

                    var cost = new ClassSetupPrice();
                    if (c != null && c.Count > 0)
                    {
                        cost.SetupCost = c.Any(x => x.OptionName.ToLower() == "trustsetupcost") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "trustsetupcost").FirstOrDefault().OptionValue) : 100;
                        cost.SetupGST = c.Any(x => x.OptionName.ToLower() == "trustgst") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "trustgst").FirstOrDefault().OptionValue) : 8;
                    }
                    else
                    {
                        cost.SetupCost = 100;
                        cost.SetupGST = 8;
                    }
                    cost.TotalCost = cost.SetupCost + cost.SetupGST; 
                    data.Cost = cost;
                    data.BeneficiariesMembers = Beneficiaries.Where(x => x.HolderType == "member").ToList();
                    data.BeneficiariesCompany = Beneficiaries.Where(x => x.HolderType == "company").ToList();
                    data.total = Beneficiaries.Count;
                }
            }
            return data;
        }

        public static bool UpdateBeneficiary(ClassBeneficiary Beneficiary, long trustid, long uid)
        {
            bool flag = false;
            string xml = "";
            foreach (var b in Beneficiary.Members)
            {
                xml += string.Format("<Entity><Id>{0}</Id></Entity>", b.Id);
            }
            xml = string.Format("<DataSet>{0}</DataSet>", xml);

            var c = Beneficiary.Company.FirstOrDefault();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@xml", xml, dbType: System.Data.DbType.Xml);
                p.Add("@cid", c.Id, dbType: System.Data.DbType.Int64);
                p.Add("@CompanyName", c.CompanyName, dbType: System.Data.DbType.String);
                p.Add("@CompanyAcn", c.CompanyACN, dbType: System.Data.DbType.String);
                p.Add("@CompanyRegdate", c.RegDate, dbType: System.Data.DbType.DateTime);
                p.Add("@ContactName", c.ContactPerson, dbType: System.Data.DbType.String);
                p.Add("@bType", Beneficiary.bType, dbType: System.Data.DbType.String);
                p.Add("@TrustId", trustid, dbType: System.Data.DbType.Int64);
                p.Add("@uid", uid, dbType: System.Data.DbType.Int64);
                var i = db.Database.Connection.Query<long>("updateBeneficiaries", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                if (i != null && i.Count > 0)
                {
                    flag = true;
                }
            }
            return flag;
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
                        obj.Id = Convert.ToInt32(id);
                        obj.DirectorId = Convert.ToInt64(directorid);
                        obj.ShareBehalf = isheldanotherorg == "yes" ? true : false;
                        obj.OwnerName = beneficialownername;

                        DataTable dtshare = new DataTable();
                        dtshare = oper.get_step4_get_share_distributegrid12(companyid, individual_or_company);
                        string ShareClass = dtshare.Rows[0]["ShareClass_c"].ToString();
                        string NoOfShare = dtshare.Rows[0]["noofshares_c"].ToString();
                        string ShareAmount = dtshare.Rows[0]["amountpaidpershare_c"].ToString();
                        obj.ShareClass = ShareClass;
                        obj.NoOfShare = Convert.ToInt32(NoOfShare);
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

        public static ClassFullTrust GetFullTrustDetails(long trustId)
        {
            var data = new ClassFullTrust();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@tid", trustId, dbType: System.Data.DbType.Int64);
                using (var d = db.Database.Connection.QueryMultiple("GetFullTrustDetail", p, commandType: System.Data.CommandType.StoredProcedure))
                {
                    var t = d.Read<TblTrust>().FirstOrDefault();
                    var b = d.Read<TblTrustAppointer>().ToList();
                    var c = d.Read<TblOption>().ToList();
                    var tr = d.Read<TblTransaction>().ToList();

                    var cost = new ClassSetupPrice();
                    if (c != null && c.Count > 0)
                    {
                        cost.SetupCost = c.Any(x => x.OptionName.ToLower() == "trustsetupcost") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "trustsetupcost").FirstOrDefault().OptionValue) : 100;
                        cost.SetupGST = c.Any(x => x.OptionName.ToLower() == "trustgst") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "trustgst").FirstOrDefault().OptionValue) : 8;
                        cost.CreditCardFee = c.Any(x => x.OptionName.ToLower() == "creditcardfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "creditcardfee").FirstOrDefault().OptionValue) : 1.75;
                        cost.ProcessingFee = c.Any(x => x.OptionName.ToLower() == "processingfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "processingfee").FirstOrDefault().OptionValue) : .3;
                    }
                    else
                    {
                        cost.SetupCost = 100;
                        cost.SetupGST = 8;
                        cost.CreditCardFee = 1.75;// %
                        cost.ProcessingFee = .3; // c
                    }
                    if (t.TrustType.Trim() == "Super Fund Trust" || t.TrustType.Trim() =="Bare Trust")
                    {
                        var ccf = ((cost.SetupCost + cost.SetupGST) * cost.CreditCardFee) / 100; // Credit card fees
                        cost.TotalCost = Math.Round(cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee, 2, MidpointRounding.AwayFromZero);
                        data.Cost = cost;
                    }
                    else
                    {
                        cost.SetupCost = cost.SetupCost + 51;
                        cost.SetupGST = cost.SetupGST + 5.10;
                        var ccf = ((cost.SetupCost + cost.SetupGST) * cost.CreditCardFee) / 100; // Credit card fees
                        cost.TotalCost = Math.Round(cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee, 2, MidpointRounding.AwayFromZero);
                        data.Cost = cost;
                    }
                       
                    
                    data.appointers = b;
                    data.trust = t;
                    data.TransactionDetail = tr.Count > 0 ? tr.FirstOrDefault() : null;
                }
            }
            return data;
        }

        public static bool UpdateTrustOption(ClassTrustOption opt, long trustid, long uid)
        {
            bool flag = false;
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@tid", trustid, dbType: System.Data.DbType.Int64);
                p.Add("@quotefortax", opt.chkquotefortax, dbType: System.Data.DbType.Boolean);
                p.Add("@legelassessment", opt.chklegalassesment, dbType: System.Data.DbType.Boolean);
                p.Add("@borrowing", opt.chkborrowingreview, dbType: System.Data.DbType.Boolean);
                p.Add("@agreement", opt.chkagreement, dbType: System.Data.DbType.Boolean);
                p.Add("@uid", uid, dbType: System.Data.DbType.Int64);
                var i = db.Database.Connection.Query<long>("updateTrustOption", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                if (i != null && i > 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        private static List<Companysearch> companysearch(string companyid)
        {
            ErrorLog objlog = new ErrorLog();

            List<Companysearch> objlist = new List<Companysearch>();
            Companysearch obj = new Companysearch();
            dal.Operation oper = new dal.Operation();
            DataTable dt = oper.getcompanysearchbyid(companyid);
            try
            {
                if (dt.Rows.Count > 0)
                {
                    string govofcomapany = dt.Rows[0]["govofcomapany"].ToString();
                    obj.govofcomapany = govofcomapany;
                    objlist.Add(obj);
                }
            }
            catch (Exception ex)
            {
                objlog.WriteErrorLog(ex.ToString());
            }
            return objlist;
        }

        private static List<TblCompany> companyDetails_(string companyid)
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
                    objlist.Add(obj);
                }
            }
            catch (Exception ex)
            {
                objlog.WriteErrorLog(ex.ToString());
            }
            return objlist;
        }

        public static ClassPaymentFormData GetPaymentDetail(long Id, string type, string useremail)
        {
            var data = new ClassPaymentFormData();
            //var data1 = new comdeeds.Models.BaseModel.ClassFullCompany();
            var comp = companyDetails_(Id.ToString()).FirstOrDefault();
            var css = companysearch(Id.ToString()).FirstOrDefault();
            var ics = indcompanyShares_(Id.ToString()).ToList();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@Id", Id, dbType: System.Data.DbType.Int64);
                p.Add("@type", type, dbType: System.Data.DbType.String, size: 1);
                using (var d = db.Database.Connection.QueryMultiple("GetPaymentDetail", p, commandType: System.Data.CommandType.StoredProcedure))
                {
                    data = d.Read<ClassPaymentFormData>().FirstOrDefault();
                    var c = d.Read<TblOption>().ToList();

                    var cost = new ClassSetupPrice();
                    if (type == "t")
                    {
                        if (c != null && c.Count > 0)
                        {
                            if (data.TrustType.Trim() == "Bare Trust" || data.TrustType.Trim() == "Super Fund Trust")
                            {
                                cost.AsicFee = 0;
                                cost.SetupCost = c.Any(x => x.OptionName.ToLower() == "trustsetupcost") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "trustsetupcost").FirstOrDefault().OptionValue) : 100;
                                cost.SetupGST = c.Any(x => x.OptionName.ToLower() == "trustgst") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "trustgst").FirstOrDefault().OptionValue) : 8;
                                cost.CreditCardFee = c.Any(x => x.OptionName.ToLower() == "creditcardfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "creditcardfee").FirstOrDefault().OptionValue) : 1.75;
                                cost.ProcessingFee = c.Any(x => x.OptionName.ToLower() == "processingfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "processingfee").FirstOrDefault().OptionValue) : .3;
                            }
                            else
                            {
                                cost.AsicFee = 0;
                                cost.SetupCost = c.Any(x => x.OptionName.ToLower() == "trustsetupcost") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "trustsetupcost").FirstOrDefault().OptionValue) : 100;
                                cost.SetupGST = c.Any(x => x.OptionName.ToLower() == "trustgst") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "trustgst").FirstOrDefault().OptionValue) : 8;
                                cost.CreditCardFee = c.Any(x => x.OptionName.ToLower() == "creditcardfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "creditcardfee").FirstOrDefault().OptionValue) : 1.75;
                                cost.ProcessingFee = c.Any(x => x.OptionName.ToLower() == "processingfee") ? Convert.ToDouble(c.Where(x => x.OptionName.ToLower() == "processingfee").FirstOrDefault().OptionValue) : .3;

                                cost.SetupCost = cost.SetupCost + 51;
                                cost.SetupGST = cost.SetupGST + 5.10;
                            }
                        }
                        else
                        {
                            cost.AsicFee = 0;
                            cost.SetupCost = 80;
                            cost.SetupGST = 8;
                            cost.CreditCardFee = 1.75;// %
                            cost.ProcessingFee = .3; // c
                        }
                        var ccf = ((cost.SetupCost + cost.SetupGST) * cost.CreditCardFee) / 100; // Credit card fees
                        cost.TotalCost = Math.Round(cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee, 2, MidpointRounding.AwayFromZero);
                    }
                    if (type == "c")
                    {
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

                        //if (comp.CompanyUseFor == "smsf" && css.govofcomapany == "yes")
                        //{
                        //    cost.TotalCost = Math.Round((cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee + cost.AsicFee + 48 + 10 + 1), 2);
                        //}
                        //else if(comp.CompanyUseFor == "smsf")
                        //{
                        //    cost.TotalCost = Math.Round(cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee + cost.AsicFee + 48, 2, MidpointRounding.AwayFromZero);
                        //}

                        if (css.govofcomapany == "yes")
                        {
                            cost.TotalCost = Math.Round(cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee + cost.AsicFee + 10 + 1, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            cost.TotalCost = Math.Round(cost.SetupCost + cost.SetupGST + ccf + cost.ProcessingFee + cost.AsicFee, 2, MidpointRounding.AwayFromZero);
                        }

                        //if ((comp.CompanyUseFor == "A company to operate business" || comp.CompanyUseFor == "Trustee for a Self Managed Super") && ics.Count > 0)
                        //{
                        //    cost.TotalCost = Math.Round((cost.TotalCost + 254), 2);
                        //}
                    }
                    if (data == null)
                    {
                        data = new comdeeds.Models.BaseModel.ClassPaymentFormData();
                        DataTable dtuser = new DataTable();
                        DataTable dtcompany = new DataTable();
                        dal.Operation oper = new dal.Operation();
                        dtuser = oper.show_Profile(useremail);
                        dtcompany = oper.getcompanysearchbyid(Id.ToString());
                        data.ID = Id;
                        data.Name = dtcompany.Rows[0]["companyname"].ToString();
                        data.CustomerName = dtuser.Rows[0]["givenname"] + " " + dtuser.Rows[0]["familyname"];
                        data.type = "company";
                    }
                    data.Cost = cost.TotalCost;
                    data.Email = CryptoHelper.DecryptString(data.Email);
                }
            }
            return data;
        }

        public static bool addTransaction(TblTransaction txn)
        {
            var res = false;
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.TblTransactions.Add(txn);
                res = db.SaveChanges() > 0;
            }
            return res;
        }

        public static bool ConfirmCheckout(long id, string type)
        {
            var res = false;
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                res = !db.TblTransactions.AsNoTracking().Any(x => x.TrustCompanyId == id && x.FormType == type && x.TransactionStatus == true);
            }
            return res;
        }
    }
}