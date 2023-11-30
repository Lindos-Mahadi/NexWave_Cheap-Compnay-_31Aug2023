using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace comdeeds.Areas.User.Controllers
{
    public class MaintainController : ApiController
    {
        private readonly FakeMyDbContext _context;

        public MaintainController(FakeMyDbContext context)
        {
            _context = context;
        }

        [System.Web.Http.HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var allCompanyAddresses = _context.TblCompanyAddresses.ToList();
                return (IActionResult)Ok(allCompanyAddresses);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new Exception(ex.Message);
            }
        }
    }
}
