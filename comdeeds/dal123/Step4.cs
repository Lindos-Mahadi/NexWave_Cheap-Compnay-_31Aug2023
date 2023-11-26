using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Comdeeds.dal
{
    public class Step4
    {
        public string companyid { get; set; }
        public string shareholderdetails { get; set; }
        public string shareclasstype_value { get; set; }
        public string shareclasstype_text { get; set; }
        public string no_of_shares { get; set; }
        public string amountpaidpershare { get; set; }
        public string amountremainingunpaidpershare { get; set; }
        public string isheldanotherorg { get; set; }
        public string beneficialownername { get; set; }     
    }
    public class Step4_AnotherShareHolder
    {
        public int id { get; set; }
        public int dirid { get; set; }
        public string step4ID { get; set; }
        public string companyid { get; set; }
        public string shareholderdetails { get; set; }
        public string shareclasstype_value { get; set; }
        public string shareclasstype_text { get; set; }
        public string no_of_shares { get; set; }
        public string amountpaidpershare { get; set; }
        public string amountremainingunpaidpershare { get; set; }
        public string isheldanotherorg { get; set; }
        public string beneficialownername { get; set; }
        public string individual_or_company { get; set; }
        public string individual_or_company_name { get; set; }
        public string individual_or_company_address { get; set; }
        public string individual_or_company_acn { get; set; }

        public string individual_or_company_dob { get; set; }
        public string individual_or_company_unit_level_suite { get; set; }
        public string individual_or_company_streetNoName { get; set; }
        public string individual_or_company_suburb_town_city { get; set; }
        public string individual_or_company_state { get; set; }
        public string individual_or_company_postcode { get; set; }
        public string individual_or_company_country { get; set; }

        public string ISJOINT { get; set; }
        public string individual_or_company_Joint { get; set; }
        public string individual_or_company_name_Joint { get; set; }
        public string individual_or_company_address_Joint { get; set; }
        public string individual_or_company_acn_Joint { get; set; }

        public string individual_or_company_dob_Joint { get; set; }
        public string individual_or_company_unit_level_suite_Joint { get; set; }
        public string individual_or_company_streetNoName_Joint { get; set; }
        public string individual_or_company_suburb_town_city_Joint { get; set; }
        public string individual_or_company_state_Joint { get; set; }
        public string individual_or_company_postcode_Joint { get; set; }
        public string individual_or_company_country_Joint { get; set; }

    }
    public class Step4_Minut
    {
        public string companyid { get; set; }
        public string companyname { get; set; }
        public string ACN { get; set; }
        public string minute_date { get; set; }
        public string minute_time { get; set; }
        public string unit_level_suite { get; set; }
        public string streetNoName { get; set; }
        public string suburb_town_city { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string nameofdirector_secretry { get; set; }
        public string position_officeHolder { get; set; }
        public string consent_director { get; set; }
        public string appointment_publicOfficer { get; set; }
        public string appointment_secretary { get; set; }
        public string execute_constitution { get; set; }
        public string applicationATO { get; set; }
        public string information_beneficialOwner { get; set; }
        public string BankName { get; set; }
        public string bankaccount { get; set; }
        public string addresstype { get; set; }
        public string companydirector { get; set; }
        public string compnaydirectorID { get; set; }
        public string minute_date_Signing { get; set; }
     

        //meeting code
        public string DirectorList_for_meeting { get; set; }
        public string chairperson_name { get; set; }
        public string Venue { get; set; }
        public string Date { get; set; }
        public string Meeting_time { get; set; }
        public string org_fund_acc { get; set; }
        public string Contact_for_name { get; set; }
        public string Contact_for_Phone { get; set; }
        public string meetingType { get; set; }




    }
}