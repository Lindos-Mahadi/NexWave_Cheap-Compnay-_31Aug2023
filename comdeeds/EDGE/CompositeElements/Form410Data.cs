using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.EDGE.CompositeElements
{
    public class Form410Data
    {
        //Reservation Details
        public string NameReserved { get; set; }
        public string Companytype { get; set; }
        public string Companyclass { get; set; }
        public bool Istheproposednameidenticaltoaregisteredbusinessname { get; set; }
        public string ABNofbusinessname { get; set; }
        //Identicalbusinessnames
        public string Identicalbusinessnames { get; set; }
        public string Placeofregistrationofbusinessname { get; set; }
        public string Registrationnumberofbusinessname { get; set; }
        public string Purposeofreservation { get; set; }
        public string PartandDivision { get; set; }

        public string Currentregisteredcompanyname { get; set; }
        public string ACNorARBN { get; set; }

        public string Reasonforextension { get; set; }
        public string Reservationdocumentnumber { get; set; }
        public DateTime Existingexpirydate { get; set; }

        public string Nameofapplicant_inv { get; set; }
        public string Nameofapplicant_org { get; set; }
        public string ACN_ARBNofapplicant { get; set; }

        public string SignatoryName { get; set; }
        public string SignatoryRole { get; set; }
        public DateTime DateSigned { get; set; }
        public bool isDeclarationisTrue { get; set; }

        public bool Requestapplicationbemanuallyreviewed { get; set; }
        public List<string> Text { get; set; }
        public bool Validate()
        {
            if (string.IsNullOrEmpty(NameReserved))
            {
                ErrorMsg = "NameReserved is mandatory.";
                return false;
            }
            // It is important to implement the other business rules; I don't have the necessary knowledge to do
            // it in a meaningfull way
            return true;
        }
        public string ErrorMsg { get; set; }
    }
}