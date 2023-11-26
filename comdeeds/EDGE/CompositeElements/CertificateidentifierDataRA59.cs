using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.EDGE.CompositeElements
{
    public class CertificateidentifierDataRA59
    {

        // Status text
        public List<string> StatusText { get; set; }
        public List<CertificateidentifierRa59> CIdetails { get; set; }

    }
    public class CertificateidentifierRa59
    {
        public string Distinguishednameofcertificateauthority { get; set; }
        public string Distinguishednameofcertificatesubject { get; set; }
        public string Authorisedtosigntransmissions { get; set; }
        public string Authorisedtosigndocuments { get; set; }
        public string Serialidentifier200 { get; set; }
    }
}