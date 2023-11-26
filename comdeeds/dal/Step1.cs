using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.dal
{
    public class Step1
    {
        public string companyid { get; set; }
        public string acn { get; set; }
        public string companyname { get; set; }
        public string companyname_ext { get; set; }
        public string stateterritorry { get; set; }
        public string isspecialpurpose { get; set; }

        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public bool Isstandard_hours { get; set; }

        public string isreservecompany410 { get; set; }
        public string reservecompany410_asicnamereservationnumber { get; set; }
        public string reservecompany410_fulllegalname { get; set; }
        public string isproposeidentical { get; set; }
        public string proposeidentical_before28may { get; set; }
        public string proposeidentical_after28may { get; set; }

        public int proposeidentical_before28may_totalstate { get; set; }
        public string proposeidentical_before28may_previousbusinessno1 { get; set; }
        public string proposeidentical_before28may_previousbusinessno2 { get; set; }
        public string proposeidentical_before28may_previousbusinessno3 { get; set; }
        public string proposeidentical_before28may_previousbusinessno4 { get; set; }
        public string proposeidentical_before28may_previousbusinessno5 { get; set; }
        public string proposeidentical_before28may_previousbusinessno6 { get; set; }
        public string proposeidentical_before28may_previousbusinessno7 { get; set; }
        public string proposeidentical_before28may_previousbusinessno8 { get; set; }
        public string proposeidentical_before28may_previousstateteritory1 { get; set; }
        public string proposeidentical_before28may_previousstateteritory2 { get; set; }
        public string proposeidentical_before28may_previousstateteritory3 { get; set; }
        public string proposeidentical_before28may_previousstateteritory4 { get; set; }
        public string proposeidentical_before28may_previousstateteritory5 { get; set; }
        public string proposeidentical_before28may_previousstateteritory6 { get; set; }
        public string proposeidentical_before28may_previousstateteritory7 { get; set; }
        public string proposeidentical_before28may_previousstateteritory8 { get; set; }

        public string proposeidentical_after28may_abnnumber { get; set; }
        public string rdo_SMSF_Yes_No { get; set; }
        public string isultimateholdingcompany { get; set; }
        public string ultimateholdingcompany_fulllegalname { get; set; }
        public string ultimateholdingcompany_country { get; set; }
        public string ultimateholdingcompany_ACN_ARBN { get; set; }
        public string ultimateholdingcompany_ABN { get; set; }
        public string typeofcompany { get; set; }
        public string classofcompany { get; set; }
        public string specialpurpose_ifapplicable { get; set; }
        public string cash { get; set; }
        public string writtencontact { get; set; }

        public string Org_Indv { get; set; }

        public string Full_org_name { get; set; }
        public string proposed_Name_Yes_No { get; set; }

        public string trustee_trustname { get; set; }
        public string trustee_abn { get; set; }
        public string trustee_tfn { get; set; }
        public string trustee_address { get; set; }
        public string trustee_country { get; set; }
        public string companyusedfor { get; set; }
        public string tuser { get; set; }
        public string UlimateHoldingCompany { get; set; }
        public string ucompanyname { get; set; }
        public string acnarbnabn { get; set; }
        public string countryIcor { get; set; }
    }
    public class Step1_shares_Allocate
    {
        public string sno { get; set; }
        public string companyid { get; set; }
        public string shareclass { get; set; }
        public string totalshares { get; set; }
        public string unitprice { get; set; }
        public string totalprice { get; set; }
    }

    public class Address
    {
        public string UnitLevelSuits { get; set; }
        public string StreetNumberStreetName { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }

    public class dropdown
    {
        public string AddressName { get; set; }
        public string Id { get; set; }
    
    }
    
    public class Step1_shares_distribute
    {
        public string sno { get; set; }
        public string linkid { get; set; }
        public string companyid { get; set; }
        public string shareclass { get; set; }
        public string totalshares { get; set; }
        public string unitprice { get; set; }
        public string totalprice { get; set; }
        public string c_totalshares { get; set; }
        public string c_amountpaidpershare { get; set; }
        public string c_amountremaining_unpaidpershare { get; set; }
        public string c_totalamountpaidpershare { get; set; }
        public string c_totalamountunpaidpershare { get; set; }
        public string c_sharerange { get; set; }
        public string c_certificateno { get; set; }
        public string individual_or_company { get; set; }
    }

    public class ProfilePage
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationABN { get; set; }

       // public string StreetAddress { get; set; }

        public string UnitLevelSuits { get; set; }
        public string StreetNumberStreetName { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Website { get; set; }

        public string Logopath { get; set; }

    }

    public class Subscribe
    {

        public int ID { get; set; }
        public string Email { get; set; }
        public bool Subscribe_Status { get; set; }
        public string Date { get; set; }
    }
}