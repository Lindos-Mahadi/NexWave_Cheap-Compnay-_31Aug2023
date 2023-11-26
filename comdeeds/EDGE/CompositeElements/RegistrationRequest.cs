using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.EDGE.CompositeElements
{
    public class RegistrationRequest
    {
        public string ProposedCompanyName { get; set; }
        public string DocumentNumber { get; set; }
        public string SignatoryName { get; set; }
        public DateTime DateSigned { get; set; }
        public bool DeclarationInN126HasBeenAssentedTo { get; set; }
    }
}