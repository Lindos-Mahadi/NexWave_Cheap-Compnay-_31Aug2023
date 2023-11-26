using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.EDGE.CompositeElements
{
    public class DATA_RA58
    {
        public string SignatoryName { get; set; }
        public DateTime DateSigned { get; set; }
        public bool CanDigitallySignTransmission { get; set; }
        public bool CanDigitallySignCompanyForms { get; set; }
    }
}