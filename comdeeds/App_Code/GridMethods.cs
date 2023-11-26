using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static comdeeds.Models.BaseModel;

namespace comdeeds.App_Code
{
    public class GridMethods
    {


        public static ClassGridTrustResult getUserTrustList(ClassSqlGridParam param,long uid)
        {
            var data = new ClassGridTrustResult();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@startLength", param.startLength, dbType: System.Data.DbType.Int32);
                p.Add("@length", param.length, dbType: System.Data.DbType.Int32);
                p.Add("@orderBy", param.orderBy, dbType: System.Data.DbType.String);
                p.Add("@uid", uid, dbType: System.Data.DbType.Int32);
                p.Add("@PageCount", 0, dbType: System.Data.DbType.Int64, direction: System.Data.ParameterDirection.Output);
                data.data = db.Database.Connection.Query<ClassGridTrustDetails>("getUserTrustList", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                data.Total = p.Get<long>("PageCount");
            }
            return data;
        }
        //public static ClassGridCompanyResult getUserCompanyList(ClassSqlGridParam param, long uid)
        public static ClassGridCompanyResult getUserCompanyList(ClassSqlGridParam param, string useremail)
        {
            var data = new ClassGridCompanyResult();
            try{
            
          
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var p = new DynamicParameters();
                p.Add("@startLength", param.startLength, dbType: System.Data.DbType.Int32);
                p.Add("@length", param.length, dbType: System.Data.DbType.Int32);
                p.Add("@orderBy", param.orderBy, dbType: System.Data.DbType.String);
                p.Add("@uid", useremail, dbType: System.Data.DbType.String);
                p.Add("@PageCount", 0, dbType: System.Data.DbType.Int64, direction: System.Data.ParameterDirection.Output);
                data.data = db.Database.Connection.Query<ClassGridCompanyDetails>("getUserCompanyList", p, commandType: System.Data.CommandType.StoredProcedure).ToList();
                data.Total = p.Get<long>("PageCount");
            }
  }
            catch(Exception ex){}
            return data;
        }






        

    }
}