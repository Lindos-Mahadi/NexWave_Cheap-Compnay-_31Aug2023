using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.OutboundMessages;
using comdeeds.dal;
using System.Text.RegularExpressions;

namespace comdeeds.EDGE.DataEntities
{
    public class Form201Entity : OutboundMessage
    {
        private ErrorLog oErrorLog = new ErrorLog();
        private dal.Operation oper = new dal.Operation();
        private DataAccessLayer dal = new DataAccessLayer();
        public string companyid { get; set; }
        public string m_userid { get; set; }
        public Form201Data m_data { get; set; }

        public Form201Entity(string userid, string companyid_)
        {
            m_userid = userid;
            companyid = companyid_;
        }

        public void getdata()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = oper.get_step1(companyid);
                if (dt.Rows.Count > 0)
                {
                    m_data.CompanyName = dt.Rows[0]["companyname"].ToString() + " " + dt.Rows[0]["companyname_ext"].ToString();
                    m_data.CompanyType = dt.Rows[0]["typeofcompany"].ToString();
                    m_data.CompanyClass = dt.Rows[0]["classofcompany"].ToString();
                    m_data.CompanySubclass = dt.Rows[0]["specialpurpose_ifapplicable"].ToString();
                    m_data.UseACNAsCompanyName = false;
                    m_data.LegalTerms = "\t";
                    string sp2 = dt.Rows[0]["isreservecompany410"].ToString();
                    if (sp2.ToLower() == "yes")
                    {
                        m_data.HasNameBeenReservedForThisBobyByForm401 = true;
                    }
                    else
                    {
                        m_data.HasNameBeenReservedForThisBobyByForm401 = false;
                    }
                    string sp3 = dt.Rows[0]["isproposeidentical"].ToString();
                    if (sp3.ToLower() == "yes")
                    {
                        m_data.IsProposedNameIdenticalToRegisteredBusinessName = true;
                    }
                    else
                    {
                        m_data.IsProposedNameIdenticalToRegisteredBusinessName = false;
                    }
                    m_data.Jurisdition = STATE(dt.Rows[0]["stateterritorry"].ToString());

                    string message = string.Format("ZCO{0}\t\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t\t{7}\t{8}\t{9}\t{10}\t\t{11}\t\t\t\t{12}\n",
                    m_data.CompanyName, m_data.CompanyType, m_data.CompanyClass, m_data.CompanySubclass,
                    BooleanRep(m_data.UseACNAsCompanyName), m_data.LegalTerms, "\t", "\t",
                    BooleanRep(m_data.HasNameBeenReservedForThisBobyByForm401),
                    BooleanRep(m_data.IsProposedNameIdenticalToRegisteredBusinessName), m_data.Jurisdition,
                    BooleanRep(m_data.AreAllOfficeholderAddressesTheUsualResidentialAddressOfOfficeholder), m_data.ABNBusinessName);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        #region segments

        public string ZCO()
        {
            string message = "";
            try
            {
                string query = "select * from step1 where companyid='" + companyid + "'";
                DataTable dt = new DataTable();
                dt = dal.getdata(query);
                if (dt.Rows.Count > 0)
                {
                    string CompanyGovernedByConstitution = "N";
                    string WillSharesBeIssuedForNonCashConsideration = "Y";
                    string HasNameBeenReservedForThisBodyByForm401 = "";
                    string IsProposedNameIdenticalToRegisteredBusinessName = "";
                    string ABNBusinessName1 = "\t";
                    string ABNBusinessName2 = "\t";
                    string ABNBusinessName3 = "\t";
                    string ABNBusinessName4 = "\t";
                    string ABNBusinessName5 = "\t";
                    string ABNBusinessName6 = "\t";
                    string ABNBusinessName7 = "\t";
                    string ABNBusinessName8 = "\t";
                    string ABNBusinessName_only = "\t";

                    string ReservationDocumentNumber = dt.Rows[0]["reservecompany410_asicnamereservationnumber"].ToString();

                    string PlaceOfRegistrationOfBusinessName1 = STATE(dt.Rows[0]["proposeidentical_before28may_previousstateteritory1"].ToString());
                    string PlaceOfRegistrationOfBusinessName2 = STATE(dt.Rows[0]["proposeidentical_before28may_previousstateteritory2"].ToString());
                    string PlaceOfRegistrationOfBusinessName3 = STATE(dt.Rows[0]["proposeidentical_before28may_previousstateteritory3"].ToString());
                    string PlaceOfRegistrationOfBusinessName4 = STATE(dt.Rows[0]["proposeidentical_before28may_previousstateteritory4"].ToString());
                    string PlaceOfRegistrationOfBusinessName5 = STATE(dt.Rows[0]["proposeidentical_before28may_previousstateteritory5"].ToString());
                    string PlaceOfRegistrationOfBusinessName6 = STATE(dt.Rows[0]["proposeidentical_before28may_previousstateteritory6"].ToString());
                    string PlaceOfRegistrationOfBusinessName7 = STATE(dt.Rows[0]["proposeidentical_before28may_previousstateteritory7"].ToString());
                    string PlaceOfRegistrationOfBusinessName8 = STATE(dt.Rows[0]["proposeidentical_before28may_previousstateteritory8"].ToString());

                    string CompanyName = (dt.Rows[0]["companyname"].ToString().Trim() + " " + dt.Rows[0]["companyname_ext"].ToString()).ToUpper().Trim();
                    string CompanyType = dt.Rows[0]["typeofcompany"].ToString();

                    //var regexItem = new Regex("^[a-zA-Z0-9 @=#%.*$]+$");
                    var regexItem = new Regex("^[a-zA-Z0-9 @=#%.*$&]+$");
                    if (!regexItem.IsMatch(CompanyName))
                    {
                        return "ERROR::Invalid Characters in CompanyName.";
                    }
                    if (CompanyType.ToUpper().Contains("PUBL"))
                    {
                        CompanyType = "APUB";
                    }
                    else if (CompanyType.ToUpper().Contains("PROP"))
                    {
                        CompanyType = "APTY";
                        CompanyGovernedByConstitution = "";
                        WillSharesBeIssuedForNonCashConsideration = ""; // Add value like this "\tY"
                    }
                    string CompanyClass = dt.Rows[0]["classofcompany"].ToString();
                    CompanyClass = COMPANYCLASS(CompanyClass);
                    string CompanySubClass = dt.Rows[0]["specialpurpose_ifapplicable"].ToString();
                    CompanySubClass = COMPANYSUBCLASS(CompanySubClass);
                    HasNameBeenReservedForThisBodyByForm401 = dt.Rows[0]["isreservecompany410"].ToString().ToUpper().Substring(0, 1) == "N" ? "" : "\tY"; //RIGHT WITH CASE 1,CASE4
                    //HasNameBeenReservedForThisBodyByForm401 = dt.Rows[0]["isreservecompany410"].ToString().ToUpper().Substring(0, 1) == "N" ? "\t" : "\tY"; //right with case 2
                    IsProposedNameIdenticalToRegisteredBusinessName = dt.Rows[0]["isproposeidentical"].ToString().ToUpper().Substring(0, 1) == "N" ? "\t" : "\tY";
                    if (IsProposedNameIdenticalToRegisteredBusinessName.Trim() == "Y")
                    {
                        if (dt.Rows[0]["proposeidentical_before28may_previousbusinessno1"].ToString().Trim() != "")
                        {
                            ABNBusinessName1 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno1"].ToString();
                        }
                        else if (dt.Rows[0]["proposeidentical_after28may_abnnumber"].ToString().Trim() != "")
                        {
                            ABNBusinessName_only = dt.Rows[0]["proposeidentical_after28may_abnnumber"].ToString();
                        }

                        if (dt.Rows[0]["proposeidentical_before28may_previousbusinessno2"].ToString().Trim() != "")
                        {
                            ABNBusinessName2 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno2"].ToString();
                        }
                        if (dt.Rows[0]["proposeidentical_before28may_previousbusinessno3"].ToString().Trim() != "")
                        {
                            ABNBusinessName3 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno3"].ToString();
                        }
                        if (dt.Rows[0]["proposeidentical_before28may_previousbusinessno4"].ToString().Trim() != "")
                        {
                            ABNBusinessName4 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno4"].ToString();
                        }
                        if (dt.Rows[0]["proposeidentical_before28may_previousbusinessno5"].ToString().Trim() != "")
                        {
                            ABNBusinessName5 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno5"].ToString();
                        }
                        if (dt.Rows[0]["proposeidentical_before28may_previousbusinessno6"].ToString().Trim() != "")
                        {
                            ABNBusinessName6 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno6"].ToString();
                        }
                        if (dt.Rows[0]["proposeidentical_before28may_previousbusinessno7"].ToString().Trim() != "")
                        {
                            ABNBusinessName7 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno7"].ToString();
                        }
                        if (dt.Rows[0]["proposeidentical_before28may_previousbusinessno8"].ToString().Trim() != "")
                        {
                            ABNBusinessName8 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno8"].ToString();
                        }
                    }
                    string Jurisdition = STATE(dt.Rows[0]["stateterritorry"].ToString());
                    bool AreAllOfficeholderAddressesTheUsualResidentialAddressOfOfficeholder = true;

                    string isproposed = dt.Rows[0]["proposed_Name_Yes_No"].ToString().ToUpper();// == "" ? "YES" : "NO";
                    if (isproposed == "")
                    {
                        isproposed = "YES";
                    }

                    string acnasNameType = "";
                    string acnasName = "N";
                    if (isproposed == "NO")
                    {
                        acnasNameType = dt.Rows[0]["companyname_ext"].ToString().ToUpper();
                        //CompanyName = "";  // Reason : To avoid the acn no. as company name show  on certificate.  comment Date : 8-aug-2018
                        acnasName = "Y";
                    }
                    isproposed = isproposed.Substring(0, 1);
                    //message += (string.Format("ZCO{0}\t\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}{8}\t{9}{10}\t\t{11}\t\t\t\t{12}",
                    //                                      CompanyName, CompanyType, CompanyClass, CompanySubClass, "N", "\t", CompanyGovernedByConstitution, WillSharesBeIssuedForNonCashConsideration,
                    //                                      HasNameBeenReservedForThisBodyByForm401, IsProposedNameIdenticalToRegisteredBusinessName, Jurisdition,
                    //                                      BooleanRep(AreAllOfficeholderAddressesTheUsualResidentialAddressOfOfficeholder), ABNBusinessName)).TrimEnd() + "\n";
                    if (CompanyClass.ToUpper() == "LMGT")
                    {
                        CompanyGovernedByConstitution = "Y";
                    }

                    message += (string.Format("ZCO{0}\t\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t\t{7}\t{8}\t{9}\t{10}\t\t{11}\t\t\t\t{12}",
                                                         CompanyName, CompanyType, CompanyClass, CompanySubClass, acnasName, acnasNameType, CompanyGovernedByConstitution.Trim(), WillSharesBeIssuedForNonCashConsideration.Trim(),
                                                         HasNameBeenReservedForThisBodyByForm401.Trim(), IsProposedNameIdenticalToRegisteredBusinessName.Trim(), Jurisdition,
                                                         BooleanRep(AreAllOfficeholderAddressesTheUsualResidentialAddressOfOfficeholder), ABNBusinessName_only)).TrimEnd() + "\n";

                    if (HasNameBeenReservedForThisBodyByForm401.Trim() == "Y")
                    {
                        string ApplicantNameIfPerson_410 = "\t\t\t\t";
                        string ApplicantNameIfOrganization_410 = "\t";
                        string indivisual_comp_410 = "";
                        indivisual_comp_410 = "individual";
                        indivisual_comp_410 = dt.Rows[0]["Indivisual_Company_Asic_form401"].ToString().ToLower();

                        if (indivisual_comp_410.ToLower() == "individual")
                        {
                            //ApplicantNameIfPerson_410 = dt.Rows[0]["Indivisual_Company_Asic_form401"].ToString().ToLower();
                            ApplicantNameIfPerson_410 = dt.Rows[0]["reservecompany410_fulllegalname"].ToString();
                            string[] shareholder = ApplicantNameIfPerson_410.Split(' ');
                            if (shareholder.Length == 1)
                            {
                                shareholder = ApplicantNameIfPerson_410.Split(',');
                            }
                            if (shareholder.Length > 2)
                            {
                                string givenname = shareholder[0];
                                string familyname = shareholder[1] + " " + shareholder[2];
                                ApplicantNameIfPerson_410 = familyname + "\t" + givenname + "\t\t";
                            }
                            else if (shareholder.Length == 2)
                            {
                                string givenname = shareholder[0];
                                string familyname = shareholder[1];
                                ApplicantNameIfPerson_410 = familyname + "\t" + givenname + "\t\t";
                            }
                            else
                            {
                                ApplicantNameIfPerson_410 = ApplicantNameIfPerson_410 + "\t\t\t\t";
                            }
                            ApplicantNameIfOrganization_410 = "";
                            message += string.Format("ZNR{0}{1}\t{2}\n", ApplicantNameIfPerson_410, ApplicantNameIfOrganization_410, ReservationDocumentNumber);
                        }
                        else
                        {
                            ApplicantNameIfOrganization_410 = dt.Rows[0]["CompanyName_Asic_from401"].ToString().ToString();
                            message += string.Format("ZNR{0}{1}\t{2}\n", ApplicantNameIfPerson_410, ApplicantNameIfOrganization_410, ReservationDocumentNumber);
                        }
                    }
                    else
                    {
                    }
                    if (IsProposedNameIdenticalToRegisteredBusinessName.Trim() == "Y")
                    {
                        if (ABNBusinessName_only.Trim() == "")
                        {
                            message += string.Format("ZPR{0}\t{1}\n", PlaceOfRegistrationOfBusinessName1, ABNBusinessName1);
                            if (ABNBusinessName2 != "\t")
                                message += string.Format("ZPR{0}\t{1}\n", PlaceOfRegistrationOfBusinessName2, ABNBusinessName2);
                            if (ABNBusinessName3 != "\t")
                                message += string.Format("ZPR{0}\t{1}\n", PlaceOfRegistrationOfBusinessName3, ABNBusinessName3);
                            if (ABNBusinessName4 != "\t")
                                message += string.Format("ZPR{0}\t{1}\n", PlaceOfRegistrationOfBusinessName4, ABNBusinessName4);
                            if (ABNBusinessName5 != "\t")
                                message += string.Format("ZPR{0}\t{1}\n", PlaceOfRegistrationOfBusinessName5, ABNBusinessName5);
                            if (ABNBusinessName6 != "\t")
                                message += string.Format("ZPR{0}\t{1}\n", PlaceOfRegistrationOfBusinessName6, ABNBusinessName6);
                            if (ABNBusinessName7 != "\t")
                                message += string.Format("ZPR{0}\t{1}\n", PlaceOfRegistrationOfBusinessName7, ABNBusinessName7);
                            if (ABNBusinessName8 != "\t")
                                message += string.Format("ZPR{0}\t{1}\n", PlaceOfRegistrationOfBusinessName8, ABNBusinessName8);
                        }
                    }
                    else
                    {
                    }
                    if (CompanyClass.ToUpper() == "LMGT") //V268
                    {
                        string querytotal = "SELECT TOP 50 * FROM STEP3 WHERE COMPANYID='" + companyid + "'";
                        DataTable dttotal = new DataTable();
                        dttotal = dal.getdata(querytotal);
                        message += string.Format("ZSC\t\t{0}\n", dttotal.Rows.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return message;
        }

        public string ZRG_ZRP()
        {
            string message = "";
            try
            {
                string query = "select * from step2 where companyid='" + companyid + "'";
                DataTable dt = new DataTable();
                dt = dal.getdata(query);
                if (dt.Rows.Count > 0)
                {
                    #region ZRG

                    string contactperson = dt.Rows[0]["contactperson"].ToString().ToUpper();
                    string unit_level_suite = dt.Rows[0]["unit_level_suite"].ToString().ToUpper();
                    string streetNoName = dt.Rows[0]["streetNoName"].ToString().ToUpper().Replace(",", " ");
                    string suburb_town_city = dt.Rows[0]["suburb_town_city"].ToString().ToUpper();
                    string state = dt.Rows[0]["state"].ToString().ToUpper();
                    string postcode = dt.Rows[0]["postcode"].ToString();
                    string iscompanylocatedaboveaddress = dt.Rows[0]["iscompanylocatedaboveaddress"].ToString().ToUpper().Substring(0, 1);
                    string occupiername = dt.Rows[0]["occupiername"].ToString();
                    AddressClass ads = new AddressClass();
                    if (contactperson != "")
                    {
                        ads.careof = contactperson;
                    }
                    if (unit_level_suite != "")
                    {
                        ads.unitlevel_line2 = unit_level_suite;
                    }
                    if (streetNoName != "")
                    {
                        ads.street = streetNoName;
                    }
                    if (suburb_town_city != "")
                    {
                        ads.locality = suburb_town_city;
                    }
                    if (state != "")
                    {
                        ads.state = STATE(state);
                    }
                    if (postcode != "")
                    {
                        ads.pcode = postcode;
                    }
                    CommonAddress ca = new CommonAddress(ads);
                    string m_address = ca.formatAddress();
                    //message += string.Format("ZRG\t\t{0}\t{1}\n", m_address, iscompanylocatedaboveaddress);
                    if (iscompanylocatedaboveaddress == "N")
                    {
                        message += string.Format("ZRG\t\t{0}\t{1}\t{2}\t{3}\n", m_address, iscompanylocatedaboveaddress, occupiername, "Y");
                    }
                    else
                    {
                        message += string.Format("ZRG\t\t{0}\t{1}\n", m_address, iscompanylocatedaboveaddress);
                    }

                    #endregion ZRG

                    #region ZRP

                    string isprimaryaddress = dt.Rows[0]["isprimaryaddress"].ToString();

                    string contactperson_primary = dt.Rows[0]["contactperson_primary"].ToString().ToUpper();
                    string unit_level_suite_primary = dt.Rows[0]["unit_level_suite_primary"].ToString().ToUpper();
                    string streetNoName_primary = dt.Rows[0]["streetNoName_primary"].ToString().ToUpper().Replace(",", " "); ;
                    string suburb_town_city_primary = dt.Rows[0]["suburb_town_city_primary"].ToString().ToUpper();
                    string state_primary = dt.Rows[0]["state_primary"].ToString().ToUpper();
                    string postcode_primary = dt.Rows[0]["postcode_primary"].ToString();
                    if (isprimaryaddress.ToUpper() == "NO")
                    {
                        ads = new AddressClass();
                        if (contactperson_primary != "")
                        {
                            ads.careof = contactperson_primary;
                        }
                        if (unit_level_suite_primary != "")
                        {
                            ads.unitlevel_line2 = unit_level_suite_primary;
                        }
                        if (streetNoName_primary != "")
                        {
                            ads.street = streetNoName_primary;
                        }
                        if (suburb_town_city_primary != "")
                        {
                            ads.locality = suburb_town_city_primary;
                        }
                        if (state_primary != "")
                        {
                            ads.state = STATE(state_primary);
                        }
                        if (postcode_primary != "")
                        {
                            ads.pcode = postcode_primary;
                        }
                        ca = new CommonAddress(ads);
                        m_address = ca.formatAddress();
                    }
                    message += string.Format("ZRP\t\t\t{0}\n", m_address.TrimEnd());

                    #endregion ZRP
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return message.ToUpper();
        }

        public string ZUH()
        {
            string message = "";
            try
            {
                string query = "select * from step1 where companyid='" + companyid + "'";
                DataTable dt = new DataTable();
                dt = dal.getdata(query);
                if (dt.Rows.Count > 0)
                {
                    string isultimateholdingcompany = dt.Rows[0]["isultimateholdingcompany"].ToString().ToUpper();
                    if (isultimateholdingcompany == "YES")
                    {
                        string ultimateholdingcompany_fulllegalname = dt.Rows[0]["ultimateholdingcompany_fulllegalname"].ToString().ToUpper();
                        string ultimateholdingcompany_country = dt.Rows[0]["ultimateholdingcompany_country"].ToString().ToUpper();
                        string ultimateholdingcompany_ACN_ARBN = dt.Rows[0]["ultimateholdingcompany_ACN_ARBN"].ToString().ToUpper();
                        string ultimateholdingcompany_ABN = dt.Rows[0]["ultimateholdingcompany_ABN"].ToString().ToUpper();
                        string ABN = ""; string ACN = "";
                        if (ISAUSTRALIAN_STATE(STATE(ultimateholdingcompany_country)) == "AUSTRALIA")
                        {
                            ABN = ultimateholdingcompany_ABN;
                            ACN = ultimateholdingcompany_ACN_ARBN;
                            message += string.Format("ZUH{0}\t{1}\t{2}\t{3}\n", ultimateholdingcompany_fulllegalname, ACN, "AUSTRALIA", ABN);
                        }
                        else
                        {
                            message += string.Format("ZUH{0}\t\t{1}\n", ultimateholdingcompany_fulllegalname, ultimateholdingcompany_country);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return message;
        }

        public string ZSD_ZFN_ZOF() // OFFICERS
        {
            string message = "";
            try
            {
                string query = "SELECT TOP 50 * FROM STEP3 WHERE COMPANYID='" + companyid + "'";
                DataTable dt = new DataTable();
                dt = dal.getdata(query);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string firstGname = "";
                        string secondGname = "";

                        string designation = dt.Rows[i]["designation"].ToString();
                        //string firstname = dt.Rows[i]["firstname"].ToString().Replace(",", " ");
                        string firstname = Regex.Replace(dt.Rows[i]["firstname"].ToString(), " *, *", ",").Replace(",", " ");
                        string middlename = dt.Rows[i]["middlename"].ToString();
                        string familyname = dt.Rows[i]["familyname"].ToString();
                        string unit_level_suite_primary = dt.Rows[i]["unit_level_suite_primary"].ToString();
                        string streetNoName_primary = dt.Rows[i]["streetNoName_primary"].ToString().Replace(",", " "); ;
                        string suburb_town_city_primary = dt.Rows[i]["suburb_town_city_primary"].ToString();
                        string state_primary = dt.Rows[i]["state_primary"].ToString();
                        string postcode_primary = dt.Rows[i]["postcode_primary"].ToString();
                        string country = dt.Rows[i]["country"].ToString();
                        string dob = dt.Rows[i]["dob"].ToString();
                        string placeofbirth = dt.Rows[i]["placeofbirth"].ToString();
                        string countryofbirth = dt.Rows[i]["countryofbirth"].ToString();
                        string isdirector = dt.Rows[i]["isdirector"].ToString();
                        string issecretary = dt.Rows[i]["issecretary"].ToString();
                        firstGname = firstname;

                        if (firstname.Contains(" "))
                        {
                            firstGname = firstname.Split(' ')[0];
                            secondGname = firstname.Split(' ')[1];
                        }
                        string NameOfOfficer = "";
                        NameOfOfficer = familyname + "\t" + (firstGname).TrimEnd() + "\t" + secondGname + "\t";
                        bool issec = false;
                        bool isdir = false;
                        if (designation.ToUpper().Contains("DIR") || isdirector.ToUpper() == "TRUE")
                        {
                            isdir = true;
                        }
                        if (designation.ToUpper().Contains("SEC") || issecretary.ToUpper() == "TRUE")
                        {
                            issec = true;
                        }
                        AddressClass ads = new AddressClass();
                        if (unit_level_suite_primary != "")
                        {
                            ads.unitlevel_line2 = unit_level_suite_primary;
                        }
                        if (streetNoName_primary != "")
                        {
                            ads.street = streetNoName_primary;
                        }
                        if (suburb_town_city_primary != "")
                        {
                            ads.locality = suburb_town_city_primary;
                        }
                        if (state_primary != "")
                        {
                            ads.state = STATE(state_primary);
                        }
                        if (postcode_primary != "")
                        {
                            ads.pcode = postcode_primary;
                        }
                        if (country != "")
                        {
                            ads.country = country;
                        }
                        CommonAddress ca = new CommonAddress(ads);
                        ///Format : If Australia then Country=STATE, IF INDIA Country=Country,
                        string City = "";
                        string State = "";
                        string Country = "";
                        if (countryofbirth.ToUpper() == "AUSTRALIA")
                        {
                            string[] loc_state = placeofbirth.Split(',');
                            if (loc_state.Length == 2)
                            {
                                City = loc_state[0];
                                State = loc_state[1];
                                Country = State;
                            }
                            else
                            {
                                /// Invalid Birth Details return "FALSE,Invalid Birth Details";
                            }
                        }
                        else
                        {
                            City = placeofbirth;
                            Country = countryofbirth;

                            if (Country.ToUpper().Trim() == "VIETNAM")
                            {
                                Country = "VIET NAM";
                                //     City = "";
                            }
                            // City = "";//Not Required DD
                        }
                        string m_address = ca.formatAddress().ToUpper().TrimEnd();
                        if (isdir == true)
                        {
                            message += string.Format("ZSD\t\t{0}\t{1}\t{2}\t{3}\t\t\t\t\t\t\t{4}\n", NameOfOfficer, DateRep(Convert.ToDateTime(dob)), City, Country, m_address).ToUpper();
                            message += string.Format("ZOF{0}\n", "DIR");
                        }
                        if (issec == true)
                        {
                            message += string.Format("ZSD\t\t{0}\t{1}\t{2}\t{3}\t\t\t\t\t\t\t{4}\n", NameOfOfficer, DateRep(Convert.ToDateTime(dob)), City, Country, m_address).ToUpper();
                            message += string.Format("ZOF{0}\n", "SEC");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return message.ToUpper();
        }

        public string ZSC()
        {
            string message = "";
            try
            {
                string query = "select * from (SELECT shareclass,SUM(c_totalshares) as Issued,sum(c_totalamountpaidpershare) as PAID,sum(c_totalamountunpaidpershare) as UNPAID  FROM [dbo].[Share_distribute_grid] where companyid='" + companyid + "'  GROUP BY SHARECLASS)t order by shareclass";
                DataTable dt = new DataTable();
                dt = dal.getdata(query);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string ShareClassCode = ShareCode(dt.Rows[i]["shareclass"].ToString());
                        string TotalNumberIssued = Convert.ToInt32(dt.Rows[i]["issued"]).ToString(); //dt.Rows[i]["issued"].ToString(); change s
                        string TotalAmountPaid = dt.Rows[i]["PAID"].ToString().Split('.')[0];
                        string TotalAmountUnpaid = dt.Rows[i]["UNPAID"].ToString().Split('.')[0];

                        //sachin changes 12 sept 2018
                        TotalAmountPaid = (Convert.ToDecimal(TotalAmountPaid) * 100).ToString();
                        TotalAmountUnpaid = (Convert.ToDecimal(TotalAmountUnpaid) * 100).ToString();

                        message += string.Format("ZSC{0}\t{1}\t\t{2}\t\t{3}\t{4}\n", ShareClassCode, ShareClassCode, TotalNumberIssued, TotalAmountPaid, TotalAmountUnpaid);
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return message;
        }

        public string ZNS() // OFFICERS
        {
            string message = "";
            try
            {
                DataTable dtchk = dal.getdata("select top 1 * from step1 where companyid='" + companyid + "'");
                if (dtchk.Rows.Count > 0)
                {
                    string CompanyClass = dtchk.Rows[0]["classofcompany"].ToString();
                    CompanyClass = COMPANYCLASS(CompanyClass);
                    if (CompanyClass.ToUpper() != "LMGT")
                    {
                        return "";
                    }
                }
                string query = "SELECT TOP 50 * FROM STEP3 WHERE COMPANYID='" + companyid + "'";
                DataTable dt = new DataTable();
                dt = dal.getdata(query);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string designation = dt.Rows[i]["designation"].ToString();
                        string firstname = dt.Rows[i]["firstname"].ToString();
                        string middlename = dt.Rows[i]["middlename"].ToString();
                        string familyname = dt.Rows[i]["familyname"].ToString();
                        string unit_level_suite_primary = dt.Rows[i]["unit_level_suite_primary"].ToString();
                        string streetNoName_primary = dt.Rows[i]["streetNoName_primary"].ToString().Replace(",", " "); ;
                        string suburb_town_city_primary = dt.Rows[i]["suburb_town_city_primary"].ToString();
                        string state_primary = dt.Rows[i]["state_primary"].ToString();
                        string postcode_primary = dt.Rows[i]["postcode_primary"].ToString();
                        string country = dt.Rows[i]["country"].ToString();
                        string dob = dt.Rows[i]["dob"].ToString();
                        string placeofbirth = dt.Rows[i]["placeofbirth"].ToString();
                        string countryofbirth = dt.Rows[i]["countryofbirth"].ToString();
                        string isdirector = dt.Rows[i]["isdirector"].ToString();
                        string issecretary = dt.Rows[i]["issecretary"].ToString();

                        string NameOfOfficer = "";
                        NameOfOfficer = familyname + "\t" + (firstname).TrimEnd() + "\t" + middlename + "\t\t";

                        AddressClass ads = new AddressClass();
                        if (unit_level_suite_primary != "")
                        {
                            ads.unitlevel_line2 = unit_level_suite_primary;
                        }
                        if (streetNoName_primary != "")
                        {
                            ads.street = streetNoName_primary;
                        }
                        if (suburb_town_city_primary != "")
                        {
                            ads.locality = suburb_town_city_primary;
                        }
                        if (state_primary != "")
                        {
                            ads.state = STATE(state_primary);
                        }
                        if (postcode_primary != "")
                        {
                            ads.pcode = postcode_primary;
                        }
                        if (country != "")
                        {
                            ads.country = country;
                        }
                        CommonAddress ca = new CommonAddress(ads);
                        ///Format : If Australia then Country=STATE, IF INDIA Country=Country,

                        string m_address = ca.formatAddress().ToUpper().TrimEnd();
                        message += string.Format("ZNS{0}\t\t{1}\n", NameOfOfficer, m_address).ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return message.ToUpper();
        }

        public string ZHH_ZSH() //	SHARE MEMBERS
        {
            try
            {
                DataTable dtchk = dal.getdata("select top 1 * from step1 where companyid='" + companyid + "'");
                if (dtchk.Rows.Count > 0)
                {
                    string CompanyClass = dtchk.Rows[0]["classofcompany"].ToString();
                    CompanyClass = COMPANYCLASS(CompanyClass);
                    if (CompanyClass.ToUpper() == "LMGT")
                    {
                        return "";
                    }
                }
                string message = "";
                DataTable dt = new DataTable();
                //  dt = dal.getdata("select * from [dbo].[step4_anothershareholder] where companyid='" + companyid + "'");
                dt = dal.getdata("select [companyid] ,[shareholderdetails]  ,[isheldanotherorg] ,[beneficialownername] ,[individual_or_company] ,[individual_or_company_name] ,[individual_or_company_address]  ,[individual_or_company_acn] ,[individual_or_company_dob] ,[individual_or_company_unit_level_suite] ,[individual_or_company_streetNoName] ,[individual_or_company_suburb_town_city] ,[individual_or_company_state] ,[individual_or_company_postcode] ,[individual_or_company_country] ,[ISJOINT] ,[JOINT_individual_or_company] ,[joint_individual_or_company_name] ,[joint_individual_or_company_acn] ,[joint_individual_or_company_dob] ,[joint_individual_or_company_unit_level_suite] ,[joint_individual_or_company_streetNoName] ,[joint_individual_or_company_suburb_town_city] ,[joint_individual_or_company_state] ,[joint_individual_or_company_postcode] ,[joint_individual_or_company_country] ,[dirid] ,[placeofbirth] from step4_anothershareholder  where companyid = '" + companyid + "' group by  [companyid] ,[shareholderdetails] ,[isheldanotherorg] ,[beneficialownername] ,[individual_or_company] ,[individual_or_company_name] ,[individual_or_company_address] ,[individual_or_company_acn] ,[individual_or_company_dob] ,[individual_or_company_unit_level_suite] ,[individual_or_company_streetNoName] ,[individual_or_company_suburb_town_city] ,[individual_or_company_state] ,[individual_or_company_postcode] ,[individual_or_company_country] ,[ISJOINT] ,[JOINT_individual_or_company] ,[joint_individual_or_company_name] ,[joint_individual_or_company_acn] ,[joint_individual_or_company_dob] ,[joint_individual_or_company_unit_level_suite] ,[joint_individual_or_company_streetNoName] ,[joint_individual_or_company_suburb_town_city] ,[joint_individual_or_company_state] ,[joint_individual_or_company_postcode] ,[joint_individual_or_company_country] ,[dirid] ,[placeofbirth]");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //string id = dt.Rows[i]["id"].ToString();
                        string dirid = dt.Rows[i]["dirid"].ToString();
                        //string shareholderdetails = dt.Rows[i]["shareholderdetails"].ToString().ToUpper().Replace(",", " ");
                        string shareholderdetails = Regex.Replace(dt.Rows[i]["shareholderdetails"].ToString().ToUpper(), " *, *", ",").Replace(",", " ");
                        string isheldanotherorg = dt.Rows[i]["isheldanotherorg"].ToString();
                        string individual_or_company = dt.Rows[i]["individual_or_company"].ToString().ToUpper();
                        string individual_or_company_name = dt.Rows[i]["individual_or_company_name"].ToString().ToUpper();
                        string individual_or_company_acn = dt.Rows[i]["individual_or_company_acn"].ToString();

                        string individual_or_company_unit_level_suite = dt.Rows[i]["individual_or_company_unit_level_suite"].ToString().ToUpper();
                        string individual_or_company_streetNoName = dt.Rows[i]["individual_or_company_streetNoName"].ToString().ToUpper().Replace(",", " "); ;
                        string individual_or_company_suburb_town_city = dt.Rows[i]["individual_or_company_suburb_town_city"].ToString().ToUpper();
                        string individual_or_company_state = dt.Rows[i]["individual_or_company_state"].ToString().ToUpper();
                        string individual_or_company_postcode = dt.Rows[i]["individual_or_company_postcode"].ToString();
                        string individual_or_company_country = dt.Rows[i]["individual_or_company_country"].ToString().ToUpper();

                        AddressClass ads = new AddressClass();
                        if (individual_or_company_unit_level_suite != "")
                        {
                            ads.unitlevel_line2 = individual_or_company_unit_level_suite;
                        }
                        if (individual_or_company_streetNoName != "")
                        {
                            ads.street = individual_or_company_streetNoName;
                        }
                        if (individual_or_company_suburb_town_city != "")
                        {
                            ads.locality = individual_or_company_suburb_town_city;
                        }
                        if (individual_or_company_state != "")
                        {
                            ads.state = STATE(individual_or_company_state);
                        }
                        if (individual_or_company_postcode != "")
                        {
                            ads.pcode = individual_or_company_postcode;
                        }
                        if (individual_or_company_country != "")
                        {
                            ads.country = individual_or_company_country;
                        }
                        CommonAddress ca = new CommonAddress(ads);
                        string m_address = ca.formatAddress();

                        DataTable dtshare = new DataTable();
                        //dtshare = dal.getdata("select * from [dbo].[Share_distribute_grid] where companyid='" + companyid + "' and linkid='mgrid" + (i + 1) + "'");
                        dtshare = dal.getdata("select * from [dbo].[Share_distribute_grid] where companyid='" + companyid + "' and linkid='" + dirid + "'");
                        // dtshare = dal.getdata("select * from [dbo].[Share_distribute_grid] where companyid='" + companyid + "'");

                        if (dtshare.Rows.Count > 0)
                        {
                            for (int j = 0; j < dtshare.Rows.Count; j++)
                            {
                                string sid = dtshare.Rows[j]["id"].ToString();
                                string shareclass = ShareCode(dtshare.Rows[j]["shareclass"].ToString().ToUpper());
                                string c_totalshares = Convert.ToInt32(dtshare.Rows[j]["c_totalshares"]).ToString(); // change s

                                // CHECK 00 IN SHARE AMOUNT : 23-AUG-2018
                                string[] c_amountpaidpershareee = dtshare.Rows[j]["c_amountpaidpershare"].ToString().Split('.');
                                string c_amountpaidpershare = c_amountpaidpershareee[0];
                                if (c_amountpaidpershareee[1] != "00")
                                {
                                    c_amountpaidpershare = dtshare.Rows[j]["c_amountpaidpershare"].ToString();
                                }

                                string[] c_amountremaining_unpaidpersharee = dtshare.Rows[j]["c_amountremaining_unpaidpershare"].ToString().Split('.');
                                string c_amountremaining_unpaidpershare = c_amountremaining_unpaidpersharee[0];
                                if (c_amountremaining_unpaidpersharee[1] != "00")
                                {
                                    c_amountremaining_unpaidpershare = dtshare.Rows[j]["c_amountremaining_unpaidpershare"].ToString();
                                }

                                string[] c_totalamountpaidpershareee = dtshare.Rows[j]["c_totalamountpaidpershare"].ToString().Split('.');
                                string c_totalamountpaidpershare = c_totalamountpaidpershareee[0];
                                if (c_totalamountpaidpershareee[1] != "00")
                                {
                                    c_totalamountpaidpershare = dtshare.Rows[j]["c_totalamountpaidpershare"].ToString();
                                }

                                string c_totalamountunpaidpersharety = dtshare.Rows[j]["c_totalamountunpaidpershare"].ToString().Split('.')[0];
                                string[] c_totalamountunpaidpersharee = dtshare.Rows[j]["c_totalamountunpaidpershare"].ToString().Split('.');
                                string c_totalamountunpaidpershare = c_totalamountunpaidpersharee[0];
                                if (c_totalamountunpaidpersharee[1] != "00")
                                {
                                    c_totalamountunpaidpershare = dtshare.Rows[j]["c_totalamountunpaidpershare"].ToString();
                                }

                                string ShareClassCode = "";
                                string NumberAgreedToBeTakenUp = "";
                                string AreTheSharesFullyPaid = "N";
                                string IsMemberTheBeneficialOwner = "N";
                                string TotalAmountPaid = "0";
                                string TotalAmountUnpaid = "0";
                                string AmountPaidPerShare = "0";
                                string AmountDueAndPayablePerShare = "0";

                                ShareClassCode = shareclass.ToUpper();
                                NumberAgreedToBeTakenUp = c_totalshares;
                                if (Convert.ToDecimal(c_amountremaining_unpaidpershare) == 0)
                                {
                                    AreTheSharesFullyPaid = "Y";
                                }
                                if (isheldanotherorg.ToUpper() == "YES")
                                {
                                    IsMemberTheBeneficialOwner = "Y";
                                }
                                // TotalAmountPaid = c_totalamountpaidpershare;
                                //TotalAmountUnpaid = c_totalamountunpaidpershare;
                                //AmountPaidPerShare = c_amountpaidpershare;
                                // AmountDueAndPayablePerShare = c_amountremaining_unpaidpershare;

                                TotalAmountPaid = (Convert.ToDecimal(c_totalamountpaidpershare) * 100).ToString();
                                TotalAmountUnpaid = (Convert.ToDecimal(c_totalamountunpaidpershare) * 100).ToString();
                                AmountPaidPerShare = (Convert.ToDecimal(c_amountpaidpershare) * 100).ToString();
                                AmountDueAndPayablePerShare = (Convert.ToDecimal(c_amountremaining_unpaidpershare) * 100).ToString();

                                message += string.Format("ZHH{0}\t{1}\t{2}\t{3}\t\t{4}\t{5}\t\t\t\t\t\t{6}\t{7}\n", ShareClassCode,
                                           NumberAgreedToBeTakenUp, AreTheSharesFullyPaid, IsMemberTheBeneficialOwner, TotalAmountPaid, TotalAmountUnpaid, AmountPaidPerShare, AmountDueAndPayablePerShare);

                                string MemberNamePerson = "";
                                string MemberNameOrganization = "";
                                string MemberACN = "N";
                                string MemberAddress = "";
                                string DoesMemberHaveAnACN = "";//Y/N
                                if (individual_or_company == "Individual".ToUpper() || individual_or_company == "" || individual_or_company == "Individual1".ToUpper())
                                {
                                    string[] shareholder = shareholderdetails.Split(' ');
                                    if (shareholder.Length > 2)
                                    {
                                        string givenname = shareholder[0];
                                        string familyname = shareholder[1] + " " + shareholder[2];

                                        string Firstgivename = "";
                                        string Secondgivename = "";
                                        familyname = shareholder[shareholder.Length - 1];
                                        Firstgivename = shareholder[0];
                                        Secondgivename = shareholder[1];
                                        MemberNamePerson = familyname + "\t" + Firstgivename + "\t" + Secondgivename + "\t\t";
                                    }
                                    else if (shareholder.Length == 2)
                                    {
                                        string givenname = shareholder[0];
                                        string familyname = shareholder[1];
                                        MemberNamePerson = familyname + "\t" + givenname + "\t\t\t";
                                    }
                                    else
                                    {
                                        MemberNamePerson = shareholderdetails + "\t\t\t\t";
                                    }
                                    MemberAddress = m_address.TrimEnd();

                                    message += string.Format("ZSH{0}\t\t{1}\n", MemberNamePerson, MemberAddress);
                                }
                                if (individual_or_company == "Company".ToUpper())
                                {
                                    MemberNameOrganization = individual_or_company_name;
                                    MemberACN = individual_or_company_acn;
                                    MemberAddress = m_address;//.TrimEnd();
                                    if (MemberACN != "")
                                    {
                                        DoesMemberHaveAnACN = "Y";
                                    }
                                    else
                                    {
                                        DoesMemberHaveAnACN = "N";
                                    }
                                    //DoesMemberHaveAnACN = "N";
                                    if (DoesMemberHaveAnACN == "N")
                                    {
                                        message += string.Format("ZSH\t\t\t\t{0}\t\t{1}\t{2}\n", MemberNameOrganization,
                                        MemberAddress, DoesMemberHaveAnACN);
                                    }
                                    else
                                    {
                                        message += string.Format("ZSH\t\t\t\t{0}\t{1}\t{2}\t{3}\n", MemberNameOrganization, MemberACN,
                                        MemberAddress, DoesMemberHaveAnACN);
                                    }
                                }

                                #region joint

                                message += create_ZSH(dt, i);

                                #endregion joint
                            }
                        }
                    }
                    return message;
                }

                /*string message = string.Format("ZHH{0}\t{1}\t{2}\t{3}\t\t{4}\t{5}\t\t\t\t\t\t{6}\t{7}\n", holdingData.ShareClassCode,
                           holdingData.NumberAgreedToBeTakenUp, BooleanRep(holdingData.AreTheSharesFullyPaid),
                           BooleanRep(holdingData.IsMemberTheBeneficialOwner), holdingData.TotalAmountPaid, holdingData.TotalAmountUnpaid,
                           holdingData.AmountPaidPerShare, holdingData.AmountDueAndPayablePerShare);

                message += string.Format("ZSH{0}\t{1}\t{2}\t{3}\t{4}\t\t{5}\n", memberData.MemberNamePerson,
                            memberData.MemberNameOrganization, memberData.MemberACN, memberData.MemberAddress,
                            BooleanRep(memberData.DoesMemberHaveAnACN), BooleanRep(memberData.AddressOverriden));*/
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }

            return "";
        }

        private string create_ZSH(DataTable dth, int rowid)
        {
            string zhs = "";
            if (dth.Rows.Count > 0)
            {
                string ISJOINT = dth.Rows[rowid]["ISJOINT"].ToString();
                if (ISJOINT == "YES")
                {
                    string individual_or_company = dth.Rows[rowid]["JOINT_individual_or_company"].ToString().ToUpper();
                    string individual_or_company_name = dth.Rows[rowid]["joint_individual_or_company_name"].ToString().ToUpper();
                    string individual_or_company_acn = dth.Rows[rowid]["joint_individual_or_company_acn"].ToString();
                    string individual_or_company_unit_level_suite = dth.Rows[rowid]["joint_individual_or_company_unit_level_suite"].ToString().ToUpper();
                    string individual_or_company_streetNoName = dth.Rows[rowid]["joint_individual_or_company_streetNoName"].ToString().ToUpper().Replace(",", " "); ;
                    string individual_or_company_suburb_town_city = dth.Rows[rowid]["joint_individual_or_company_suburb_town_city"].ToString().ToUpper();
                    string individual_or_company_state = dth.Rows[rowid]["joint_individual_or_company_state"].ToString().ToUpper();
                    string individual_or_company_postcode = dth.Rows[rowid]["joint_individual_or_company_postcode"].ToString();
                    string individual_or_company_country = dth.Rows[rowid]["joint_individual_or_company_country"].ToString().ToUpper();
                    AddressClass ads = new AddressClass();
                    if (individual_or_company_unit_level_suite != "")
                    {
                        ads.unitlevel_line2 = individual_or_company_unit_level_suite;
                    }
                    if (individual_or_company_streetNoName != "")
                    {
                        ads.street = individual_or_company_streetNoName;
                    }
                    if (individual_or_company_suburb_town_city != "")
                    {
                        ads.locality = individual_or_company_suburb_town_city;
                    }
                    if (individual_or_company_state != "")
                    {
                        ads.state = STATE(individual_or_company_state);
                    }
                    if (individual_or_company_postcode != "")
                    {
                        ads.pcode = individual_or_company_postcode;
                    }
                    if (individual_or_company_country != "")
                    {
                        ads.country = individual_or_company_country;
                    }
                    CommonAddress ca = new CommonAddress(ads);
                    string m_address = ca.formatAddress();

                    string MemberNamePerson = "";
                    string MemberNameOrganization = "";
                    string MemberACN = "N";
                    string MemberAddress = "";
                    string DoesMemberHaveAnACN = "";//Y/N
                    if (individual_or_company == "Individual".ToUpper() || individual_or_company == "")
                    {
                        string[] shareholder = individual_or_company_name.Split(' ');
                        if (shareholder.Length > 2)
                        {
                            string givenname = shareholder[0];
                            string familyname = shareholder[1] + " " + shareholder[2];
                            MemberNamePerson = familyname + "\t" + givenname + "\t\t\t";

                            string Firstgivename = "";
                            string Secondgivename = "";
                            familyname = shareholder[shareholder.Length - 1];
                            Firstgivename = shareholder[0];
                            Secondgivename = shareholder[1];
                            MemberNamePerson = familyname + "\t" + Firstgivename + "\t" + Secondgivename + "\t\t";
                        }
                        else if (shareholder.Length == 2)
                        {
                            string givenname = shareholder[0];
                            string familyname = shareholder[1];
                            MemberNamePerson = familyname + "\t" + givenname + "\t\t\t";
                        }
                        else
                        {
                            MemberNamePerson = individual_or_company_name + "\t\t\t\t";
                        }
                        MemberAddress = m_address.TrimEnd();

                        zhs += string.Format("ZSH{0}\t\t{1}\n", MemberNamePerson, MemberAddress);
                    }
                    if (individual_or_company == "Company".ToUpper())
                    {
                        MemberNameOrganization = individual_or_company_name;
                        MemberACN = individual_or_company_acn;
                        MemberAddress = m_address;//.TrimEnd();
                        if (MemberACN != "")
                        {
                            DoesMemberHaveAnACN = "Y";
                        }
                        //DoesMemberHaveAnACN = "N";
                        if (DoesMemberHaveAnACN == "N")
                        {
                            zhs += string.Format("ZSH\t\t\t\t{0}\t\t{1}\t{2}\n", MemberNameOrganization,
                            MemberAddress, DoesMemberHaveAnACN);
                        }
                        else
                        {
                            zhs += string.Format("ZSH\t\t\t\t{0}\t{1}\t{2}\t{3}\n", MemberNameOrganization, MemberACN,
                            MemberAddress, DoesMemberHaveAnACN);
                        }
                    }
                }
            }

            return zhs;
        }

        private dal.Operation objOp = new dal.Operation();

        public string ZCG_ZDC_ZAM() // APPLICANT SIGNATURE
        {
            DataTable dt = new DataTable();

            dt = objOp.show_Profile(m_userid);
            if (dt.Rows.Count > 0)
            {
                string GivenName = dt.Rows[0]["GivenName"].ToString();
                string FamilyName = dt.Rows[0]["FamilyName"].ToString();

                AddressClass ads = new AddressClass();
                if (dt.Rows[0]["UnitLevelSuits"].ToString() != "")
                {
                    ads.unitlevel_line2 = dt.Rows[0]["UnitLevelSuits"].ToString();
                }
                if (dt.Rows[0]["StreetNumberStreetName"].ToString() != "")
                {
                    ads.street = dt.Rows[0]["StreetNumberStreetName"].ToString().Replace(",", " "); ;
                }
                if (dt.Rows[0]["Suburb"].ToString() != "")
                {
                    ads.locality = dt.Rows[0]["Suburb"].ToString();
                }
                if (dt.Rows[0]["State"].ToString() != "")
                {
                    ads.state = STATE(dt.Rows[0]["State"].ToString());
                }
                if (dt.Rows[0]["Postcode"].ToString() != "")
                {
                    ads.pcode = dt.Rows[0]["Postcode"].ToString();
                }
                CommonAddress ca = new CommonAddress(ads);
                string m_address = ca.formatAddress();
                //string message = string.Format("ZCG{0}\t{1}\t\t\t\t\t{2}\n", FamilyName, GivenName, m_address.TrimEnd()); Doubt for 230_validTP536
                string message = string.Format("ZCG{0}\t{1}\t\t\t\t\t{2}\n", FamilyName, GivenName, m_address.TrimEnd());
                message += string.Format("ZDC{0}\t{1}\t\t\t\t{2}\t{3}\n", FamilyName, GivenName, DateRep(DateTime.Now), "Y");
                message += string.Format("ZAM\t\t{0}\n", "PDF");
                return message.ToUpper();
            }
            return "";
        }

        #endregion segments

        public string STATE(string fullname)
        {
            if (fullname.ToLower().Contains("New South Wales".ToLower()))
            {
                return "NSW";
            }
            else if (fullname.ToLower().Contains("AUSTRALIAN CAPITAL TERRITORY".ToLower()))
            {
                return "ACT";
            }
            else if (fullname.ToLower().Contains("Northern Territory".ToLower()))
            {
                return "NT";
            }
            else if (fullname.ToLower().Contains("Queensland".ToLower()))
            {
                return "QLD";
            }
            else if (fullname.ToLower().Contains("South Australia".ToLower()))
            {
                return "SA";
            }
            else if (fullname.ToLower().Contains("Tasmania".ToLower()))
            {
                return "TAS";
            }
            else if (fullname.ToLower().Contains("Victoria".ToLower()))
            {
                return "VIC";
            }
            else if (fullname.ToLower().Contains("Western Australia".ToLower()))
            {
                return "WA";
            }
            else
            {
                return fullname;
            }
        }

        public string ISAUSTRALIAN_STATE(string fullname)
        {
            if (fullname.ToUpper() == "NSW" || fullname.ToUpper() == "ACT" || fullname.ToUpper() == "NT" || fullname.ToUpper() == "QLD" || fullname.ToUpper() == "SA" || fullname.ToUpper() == "TAS" || fullname.ToUpper() == "VIC" || fullname.ToUpper() == "WA")
            {
                return "AUSTRALIA";
            }
            else
            {
                return fullname;
            }
        }

        public string COMPANYCLASS(string fullname)
        {
            if (fullname.ToLower().Contains("limited by shares".ToLower()))
            {
                return "LMSH";
            }
            else if (fullname.ToLower().Contains("limited by guarantee".ToLower()))
            {
                return "LMGT";
            }
            else if (fullname.ToLower().Contains("unlimited with a share capital".ToLower()))
            {
                return "UNLM";
            }
            else if (fullname.ToLower().Contains("no liability".ToLower()))
            {
                return "NLIA";
            }
            else
            {
                return fullname;
            }
        }

        public string COMPANYSUBCLASS(string fullname)
        {
            if (fullname.ToLower().Contains("superannuation trustee (ULSS)".ToLower()))
            {
                return "ULSS";
            }
            else if (fullname.ToLower().Contains("charitable purposes only (ULSN)".ToLower()))
            {
                return "ULSN";
            }
            else if (fullname.ToLower().Contains("home unit (HUNT)".ToLower()))
            {
                return "HUNT";
            }
            else if (fullname.ToLower().Contains("superannuation trustee (PSTC)".ToLower()))
            {
                return "PSTC";
            }
            else if (fullname.ToLower().Contains("charitable purposes only (PNPC)".ToLower()))
            {
                return "PNPC";
            }
            else if (fullname.ToLower().Contains("PROPRIETARY (PROP)".ToLower()))
            {
                return "PROP";
            }
            else
            {
                return fullname;
            }
        }

        public string ShareCode(string sharename)
        {
            if (sharename.ToLower().Contains("ordi"))
            {
                return "ORD";
            }
            if (sharename.ToLower().Contains("redee"))
            {
                return "REDP";
            }
            return sharename;
        }
    }
}