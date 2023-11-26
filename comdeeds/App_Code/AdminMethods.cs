using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static comdeeds.Models.BaseModel;
using comdeeds.Models;

namespace comdeeds.App_Code
{
    public class AdminMethods
    {

        public static ClassGridTrustResult getAdminTrustList(ClassSqlGridParam param, string searchKey,string subuserid)
        {
            var data = new ClassGridTrustResult();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@startLength", param.startLength, dbType: System.Data.DbType.Int32);
                p.Add("@length", param.length, dbType: System.Data.DbType.Int32);
                p.Add("@orderBy", param.orderBy, dbType: System.Data.DbType.String);
                p.Add("@search", searchKey, dbType: System.Data.DbType.String);
                p.Add("@PageCount", 0, dbType: System.Data.DbType.Int64, direction: System.Data.ParameterDirection.Output);
                p.Add("@subuserid", subuserid, dbType:System.Data.DbType.String);
                data.data = db.Database.Connection.Query<ClassGridTrustDetails>("getAdminTrustList", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                data.Total = p.Get<long>("PageCount");
            }
            return data;
        }

        public static ClassGridCompanyResult getAdminCompanyList(ClassSqlGridParam param)
        {
            var data = new ClassGridCompanyResult();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@startLength", param.startLength, dbType: System.Data.DbType.Int32);
                p.Add("@length", param.length, dbType: System.Data.DbType.Int32);
                p.Add("@orderBy", param.orderBy, dbType: System.Data.DbType.String);
                p.Add("@PageCount", 0, dbType: System.Data.DbType.Int64, direction: System.Data.ParameterDirection.Output);
                data.data = db.Database.Connection.Query<ClassGridCompanyDetails>("getAdminCompanyList", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                data.Total = p.Get<long>("PageCount");
            }
            return data;
        }

        public static bool saveOptions(List<ClassadminOptions> opts,long uid)
        {
            bool flag = false;
            string xml = "";
            foreach (var b in opts)
            {
                xml += string.Format("<Entity><OptionName>{0}</OptionName><OptionValue>{1}</OptionValue><Type>{2}</Type></Entity>", b.Key, b.value, b.type);
            }
            xml = string.Format("<DataSet>{0}</DataSet>", xml);
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@xml", xml, dbType: System.Data.DbType.Xml);
                p.Add("@uid", uid, dbType: System.Data.DbType.Int64);
                var i = db.Database.Connection.Query<long>("saveOptions", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                if (i != null && i.Count > 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        internal static ClassGridUserResult getUsersList(ClassSqlGridParam param)
        {
            var data = new ClassGridUserResult();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@startLength", param.startLength, dbType: System.Data.DbType.Int32);
                p.Add("@length", param.length, dbType: System.Data.DbType.Int32);
                p.Add("@orderBy", param.orderBy, dbType: System.Data.DbType.String);
                p.Add("@PageCount", 0, dbType: System.Data.DbType.Int64, direction: System.Data.ParameterDirection.Output);
               // p.Add("@subuserid", subuserid, dbType: System.Data.DbType.String);
                data.users = db.Database.Connection.Query<ClassUserDetails>("getUsersList", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                data.Total = p.Get<long>("PageCount");

                foreach(var u in data.users)
                {
                    u.Email = CryptoHelper.DecryptString(u.Email);
                }
            }
            return data;
        }


        internal static ClassGridUserResult getadminUsersList(ClassSqlGridParam param)
        {
            var data = new ClassGridUserResult();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@startLength", param.startLength, dbType: System.Data.DbType.Int32);
                p.Add("@length", param.length, dbType: System.Data.DbType.Int32);
                p.Add("@orderBy", param.orderBy, dbType: System.Data.DbType.String);
                p.Add("@PageCount", 0, dbType: System.Data.DbType.Int64, direction: System.Data.ParameterDirection.Output);
                data.users = db.Database.Connection.Query<ClassUserDetails>("getadminUsersList", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                data.Total = p.Get<long>("PageCount");

                foreach (var u in data.users)
                {
                    u.Email = CryptoHelper.DecryptString(u.Email);
                }
            }
            return data;
        }


        internal static ClassGridPaymentList getPaymentList(ClassSqlGridParam param)
        {
            var data = new ClassGridPaymentList();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@startLength", param.startLength, dbType: System.Data.DbType.Int32);
                p.Add("@length", param.length, dbType: System.Data.DbType.Int32);
                p.Add("@orderBy", param.orderBy, dbType: System.Data.DbType.String);
                p.Add("@id", param.userid, dbType: System.Data.DbType.Int32);
                p.Add("@PageCount", 0, dbType: System.Data.DbType.Int64, direction: System.Data.ParameterDirection.Output);
                data.Payment = db.Database.Connection.Query<ClassPaymentList>("getPaymentList", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                data.Total = p.Get<long>("PageCount");  
                foreach(var a in data.Payment)
                {
                    a.adate = a.AddedDate.ToString("dd/MM/yyyy , hh:MM", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            return data;
        }


        internal static ClassDashboardCounters GetCounters(string subuserid,string Esubuserid)
        {
            ClassDashboardCounters data = new ClassDashboardCounters();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@subuserid", subuserid, dbType: System.Data.DbType.String);
                p.Add("@Esubuserid",Esubuserid, dbType: System.Data.DbType.String);
                using (var d = db.Database.Connection.QueryMultiple("GetDashboardCounters", p, commandType: System.Data.CommandType.StoredProcedure))
                {
                    data.users = d.Read<Classduser>().ToList();
                    data.company = d.Read<Classdcompany>().ToList();
                    data.trust = d.Read<Classdtrust>().ToList();
                    data.contact = d.Read<Classdcontact>().ToList();
                }
            }

            return data;
        }

        internal static string GetID(int id)
        {
            string data = "";
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@id", id, dbType: System.Data.DbType.Int32);
                using (var d = db.Database.Connection.QuerySingleOrDefault("Getuserid", p, commandType: System.Data.CommandType.StoredProcedure))
                {
                    data = d.ToString();
                }
            }

            return data;
        }

        internal static ClassReport GetReport(int days,int uid)
        {
            ClassReport data = new ClassReport();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@time", days, dbType: System.Data.DbType.Int32);
                p.Add("@uid", uid, dbType: System.Data.DbType.Int32);
                using (var d = db.Database.Connection.QueryMultiple("GetReport", p, commandType: System.Data.CommandType.StoredProcedure))
                {
                    data.company = d.Read<ClassChartData>().ToList();
                    data.users = d.Read<ClassChartData>().ToList();
                    data.trusts = d.Read<ClassChartData>().ToList();
                }
            }

            return data;
        }
    }
}