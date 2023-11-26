using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Comdeeds.app_code
{
    public class LodgeInfo_
    {
        public string lid { get; set; }
        public string proposedname { get; set; }
        public string CareOf { get; set; }
        public string Unit { get; set; }
        public string StreetAddress { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }

        public string territorystate { get; set; }
        public string ddlindivisualdirector { get; set; }
        public string ddlindivisualshareholder { get; set; }
        public string ddlownshare { get; set; }
        public string ddljointshareholder { get; set; }
    }
}