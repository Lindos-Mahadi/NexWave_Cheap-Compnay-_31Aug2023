using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.App_Code
{
    public class OptionMethods
    {

        public static List<TblOption> GetAllOptions()
        {
            List<TblOption> options = new List<TblOption>();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                options = db.TblOptions.AsNoTracking().Where(x=>x.Del==false).ToList();
            }
            return options;
        }

    }
}