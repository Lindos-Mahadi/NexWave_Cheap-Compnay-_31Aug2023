using comdeeds.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static comdeeds.Models.BaseModel;

namespace comdeeds.Areas.User.Controllers
{
    public class MaintenanceController : BaseController
    {
        // GET: User/Maintenance
        //private readonly MyDbContext _db;
        //public MaintenanceController(MyDbContext db)
        //{
        //    _db = db;
        //}
        public ActionResult MaintenanceIndex()
        {
            //var companyAddressList = _db.TblCompanyAddresses.ToString();
            //return View(companyAddressList);
            return View();
        }
    }
}