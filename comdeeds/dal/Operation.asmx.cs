using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using comdeeds.EDGE.CompositeElements;
using comdeeds.dal;

namespace comdeeds.dal
{
    /// <summary>
    /// Summary description for Operation
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Operation : System.Web.Services.WebService
    {
        ErrorLog oErrorLog = new ErrorLog();
        DataAccessLayer dal = new DataAccessLayer();
        public DataTable get_userdetails_byuid(string uid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@uid", SqlDbType.VarChar);
                para[0].Value = uid;
                dt = dal.executedtprocedure("get_userdetails_byuid", para, false);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return dt;
        }
        public DataTable getcompanysearchbyid(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                dt = dal.executedtprocedure("getcompanysearchbyid", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }
        public DataTable get_companysearch_byName(string companyname)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyname", SqlDbType.VarChar);
                para[0].Value = companyname;
                dt = dal.executedtprocedure("get_companysearch_byName", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }

        public DataTable get_AdminDetails_byEmail(string companyEmail)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyname", SqlDbType.VarChar);
                para[0].Value = companyEmail;
                dt = dal.executedtprocedure("get_companysearch_byName", para, false);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public string insert_companysearch(string userid, string companyname,long Regid)
        {
            SqlParameter[] para = new SqlParameter[3];
            para[0] = new SqlParameter("@userid", SqlDbType.VarChar);
            para[0].Value = userid;
            para[1] = new SqlParameter("@companyname", SqlDbType.VarChar);
            para[1].Value = companyname;
            para[2] = new SqlParameter("@Regid", SqlDbType.BigInt);
            para[2].Value = Regid;
            string dt = dal.executeprocedure_returnid("sp_insert_companysearch", para).ToString();
            return dt.ToString();
        }
        public DataTable getStep1_bycid(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                dt = dal.executedtprocedure("getStep1_bycid", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }
        public string insert_step1(Step1 obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[57];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@companyname", SqlDbType.VarChar);
                para[1].Value = obj.companyname;
                para[2] = new SqlParameter("@companyname_ext", SqlDbType.VarChar);
                para[2].Value = obj.companyname_ext;
                para[3] = new SqlParameter("@stateterritorry", SqlDbType.VarChar);
                para[3].Value = obj.stateterritorry;
                para[4] = new SqlParameter("@isspecialpurpose", SqlDbType.VarChar);
                para[4].Value = obj.isspecialpurpose;
                para[5] = new SqlParameter("@isreservecompany410", SqlDbType.VarChar);
                para[5].Value = obj.isreservecompany410;
                para[6] = new SqlParameter("@reservecompany410_asicnamereservationnumber", SqlDbType.VarChar);
                para[6].Value = obj.reservecompany410_asicnamereservationnumber;
                para[7] = new SqlParameter("@reservecompany410_fulllegalname", SqlDbType.VarChar);
                para[7].Value = obj.reservecompany410_fulllegalname;
                para[8] = new SqlParameter("@isproposeidentical", SqlDbType.VarChar);
                para[8].Value = obj.isproposeidentical;
                para[9] = new SqlParameter("@proposeidentical_before28may", SqlDbType.VarChar);
                para[9].Value = obj.proposeidentical_before28may;
                para[10] = new SqlParameter("@proposeidentical_after28may", SqlDbType.VarChar);
                para[10].Value = obj.proposeidentical_after28may;
                para[11] = new SqlParameter("@proposeidentical_before28may_previousbusinessno1", SqlDbType.VarChar);
                para[11].Value = obj.proposeidentical_before28may_previousbusinessno1;
                para[12] = new SqlParameter("@proposeidentical_before28may_previousstateteritory1", SqlDbType.VarChar);
                para[12].Value = obj.proposeidentical_before28may_previousstateteritory1;
                para[13] = new SqlParameter("@proposeidentical_after28may_abnnumber", SqlDbType.VarChar);
                para[13].Value = obj.proposeidentical_after28may_abnnumber;
                para[14] = new SqlParameter("@isultimateholdingcompany", SqlDbType.VarChar);
                para[14].Value = obj.isultimateholdingcompany;
                para[15] = new SqlParameter("@ultimateholdingcompany_fulllegalname", SqlDbType.VarChar);
                para[15].Value = obj.ultimateholdingcompany_fulllegalname;
                para[16] = new SqlParameter("@ultimateholdingcompany_country", SqlDbType.VarChar);
                para[16].Value = obj.ultimateholdingcompany_country;
                para[17] = new SqlParameter("@ultimateholdingcompany_ACN_ARBN", SqlDbType.VarChar);
                para[17].Value = obj.ultimateholdingcompany_ACN_ARBN;
                para[18] = new SqlParameter("@ultimateholdingcompany_ABN", SqlDbType.VarChar);
                para[18].Value = obj.ultimateholdingcompany_ABN;
                para[19] = new SqlParameter("@acn", SqlDbType.VarChar);
                para[19].Value = obj.acn;
                para[20] = new SqlParameter("@typeofcompany", SqlDbType.VarChar);
                para[20].Value = obj.typeofcompany;
                para[21] = new SqlParameter("@classofcompany", SqlDbType.VarChar);
                para[21].Value = obj.classofcompany;
                para[22] = new SqlParameter("@specialpurpose_ifapplicable", SqlDbType.VarChar);
                para[22].Value = obj.specialpurpose_ifapplicable;
                para[23] = new SqlParameter("@cash", SqlDbType.VarChar);
                para[23].Value = obj.cash;
                para[24] = new SqlParameter("@writtencontact", SqlDbType.VarChar);
                para[24].Value = obj.writtencontact;
                para[25] = new SqlParameter("@Org_Indv", SqlDbType.VarChar);
                para[25].Value = obj.Org_Indv;
                para[26] = new SqlParameter("@Full_org_name", SqlDbType.VarChar);
                para[26].Value = obj.Full_org_name;
                para[27] = new SqlParameter("@rdo_SMSF_Yes_No", SqlDbType.VarChar);
                para[27].Value = obj.rdo_SMSF_Yes_No;
                para[28] = new SqlParameter("@proposed_Name_Yes_No", SqlDbType.VarChar);
                para[28].Value = obj.proposed_Name_Yes_No;
                para[29] = new SqlParameter("@proposeidentical_before28may_totalstate", SqlDbType.Int);
                para[29].Value = obj.proposeidentical_before28may_totalstate;
                para[30] = new SqlParameter("@proposeidentical_before28may_previousbusinessno2", SqlDbType.VarChar);
                para[30].Value = obj.proposeidentical_before28may_previousbusinessno2;
                para[31] = new SqlParameter("@proposeidentical_before28may_previousstateteritory2", SqlDbType.VarChar);
                para[31].Value = obj.proposeidentical_before28may_previousstateteritory2;
                para[32] = new SqlParameter("@proposeidentical_before28may_previousbusinessno3", SqlDbType.VarChar);
                para[32].Value = obj.proposeidentical_before28may_previousbusinessno3;
                para[33] = new SqlParameter("@proposeidentical_before28may_previousstateteritory3", SqlDbType.VarChar);
                para[33].Value = obj.proposeidentical_before28may_previousstateteritory3;
                para[34] = new SqlParameter("@proposeidentical_before28may_previousbusinessno4", SqlDbType.VarChar);
                para[34].Value = obj.proposeidentical_before28may_previousbusinessno4;
                para[35] = new SqlParameter("@proposeidentical_before28may_previousstateteritory4", SqlDbType.VarChar);
                para[35].Value = obj.proposeidentical_before28may_previousstateteritory4;
                para[36] = new SqlParameter("@proposeidentical_before28may_previousbusinessno5", SqlDbType.VarChar);
                para[36].Value = obj.proposeidentical_before28may_previousbusinessno5;
                para[37] = new SqlParameter("@proposeidentical_before28may_previousstateteritory5", SqlDbType.VarChar);
                para[37].Value = obj.proposeidentical_before28may_previousstateteritory5;
                para[38] = new SqlParameter("@proposeidentical_before28may_previousbusinessno6", SqlDbType.VarChar);
                para[38].Value = obj.proposeidentical_before28may_previousbusinessno6;
                para[39] = new SqlParameter("@proposeidentical_before28may_previousstateteritory6", SqlDbType.VarChar);
                para[39].Value = obj.proposeidentical_before28may_previousstateteritory6;
                para[40] = new SqlParameter("@proposeidentical_before28may_previousbusinessno7", SqlDbType.VarChar);
                para[40].Value = obj.proposeidentical_before28may_previousbusinessno7;
                para[41] = new SqlParameter("@proposeidentical_before28may_previousstateteritory7", SqlDbType.VarChar);
                para[41].Value = obj.proposeidentical_before28may_previousstateteritory7;
                para[42] = new SqlParameter("@proposeidentical_before28may_previousbusinessno8", SqlDbType.VarChar);
                para[42].Value = obj.proposeidentical_before28may_previousbusinessno8;
                para[43] = new SqlParameter("@proposeidentical_before28may_previousstateteritory8", SqlDbType.VarChar);
                para[43].Value = obj.proposeidentical_before28may_previousstateteritory8;
                para[44] = new SqlParameter("@OpeningTime", SqlDbType.VarChar);
                para[44].Value = obj.OpeningTime;
                para[45] = new SqlParameter("@ClosingTime", SqlDbType.VarChar);
                para[45].Value = obj.ClosingTime;
                para[46] = new SqlParameter("@Isstandard_hours", SqlDbType.VarChar);
                para[46].Value = obj.Isstandard_hours;

                para[47] = new SqlParameter("@trustee_trustname", SqlDbType.VarChar);
                para[47].Value = obj.trustee_trustname;
                para[48] = new SqlParameter("@trustee_abn", SqlDbType.VarChar);
                para[48].Value = obj.trustee_abn;
                para[49] = new SqlParameter("@trustee_tfn", SqlDbType.VarChar);
                para[49].Value = obj.trustee_tfn;
                para[50] = new SqlParameter("@trustee_address", SqlDbType.VarChar);
                para[50].Value = obj.trustee_address;
                para[51] = new SqlParameter("@trustee_country", SqlDbType.VarChar);
                para[51].Value = obj.trustee_country;
                para[52] = new SqlParameter("@companyusedfor", SqlDbType.VarChar);
                para[52].Value = obj.companyusedfor;

                para[53] = new SqlParameter("@ulimateHoldingCompany", SqlDbType.VarChar);
                para[53].Value = obj.UlimateHoldingCompany;
                para[54] = new SqlParameter("@ucompanyname", SqlDbType.VarChar);
                para[54].Value = obj.ucompanyname;
                para[55] = new SqlParameter("@acnarbnabn", SqlDbType.VarChar);
                para[55].Value = obj.acnarbnabn;
                para[56] = new SqlParameter("@countryIcor", SqlDbType.VarChar);
                para[56].Value = obj.countryIcor;

                ii = dal.executeprocedure("insert_step1", para).ToString();
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }
        public string update_step1(Step1 obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[57];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@companyname", SqlDbType.VarChar);
                para[1].Value = obj.companyname;
                para[2] = new SqlParameter("@companyname_ext", SqlDbType.VarChar);
                para[2].Value = obj.companyname_ext;
                para[3] = new SqlParameter("@stateterritorry", SqlDbType.VarChar);
                para[3].Value = obj.stateterritorry;
                para[4] = new SqlParameter("@isspecialpurpose", SqlDbType.VarChar);
                para[4].Value = obj.isspecialpurpose;
                para[5] = new SqlParameter("@isreservecompany410", SqlDbType.VarChar);
                para[5].Value = obj.isreservecompany410;
                para[6] = new SqlParameter("@reservecompany410_asicnamereservationnumber", SqlDbType.VarChar);
                para[6].Value = obj.reservecompany410_asicnamereservationnumber;
                para[7] = new SqlParameter("@reservecompany410_fulllegalname", SqlDbType.VarChar);
                para[7].Value = obj.reservecompany410_fulllegalname;
                para[8] = new SqlParameter("@isproposeidentical", SqlDbType.VarChar);
                para[8].Value = obj.isproposeidentical;
                para[9] = new SqlParameter("@proposeidentical_before28may", SqlDbType.VarChar);
                para[9].Value = obj.proposeidentical_before28may;
                para[10] = new SqlParameter("@proposeidentical_after28may", SqlDbType.VarChar);
                para[10].Value = obj.proposeidentical_after28may;
                para[11] = new SqlParameter("@proposeidentical_before28may_previousbusinessno1", SqlDbType.VarChar);
                para[11].Value = obj.proposeidentical_before28may_previousbusinessno1;
                para[12] = new SqlParameter("@proposeidentical_before28may_previousstateteritory1", SqlDbType.VarChar);
                para[12].Value = obj.proposeidentical_before28may_previousstateteritory1;
                para[13] = new SqlParameter("@proposeidentical_after28may_abnnumber", SqlDbType.VarChar);
                para[13].Value = obj.proposeidentical_after28may_abnnumber;
                para[14] = new SqlParameter("@isultimateholdingcompany", SqlDbType.VarChar);
                para[14].Value = obj.isultimateholdingcompany;
                para[15] = new SqlParameter("@ultimateholdingcompany_fulllegalname", SqlDbType.VarChar);
                para[15].Value = obj.ultimateholdingcompany_fulllegalname;
                para[16] = new SqlParameter("@ultimateholdingcompany_country", SqlDbType.VarChar);
                para[16].Value = obj.ultimateholdingcompany_country;
                para[17] = new SqlParameter("@ultimateholdingcompany_ACN_ARBN", SqlDbType.VarChar);
                para[17].Value = obj.ultimateholdingcompany_ACN_ARBN;
                para[18] = new SqlParameter("@ultimateholdingcompany_ABN", SqlDbType.VarChar);
                para[18].Value = obj.ultimateholdingcompany_ABN;
                para[19] = new SqlParameter("@acn", SqlDbType.VarChar);
                para[19].Value = obj.acn;
                para[20] = new SqlParameter("@typeofcompany", SqlDbType.VarChar);
                para[20].Value = obj.typeofcompany;
                para[21] = new SqlParameter("@classofcompany", SqlDbType.VarChar);
                para[21].Value = obj.classofcompany;
                para[22] = new SqlParameter("@specialpurpose_ifapplicable", SqlDbType.VarChar);
                para[22].Value = obj.specialpurpose_ifapplicable;
                para[23] = new SqlParameter("@cash", SqlDbType.VarChar);
                para[23].Value = obj.cash;
                para[24] = new SqlParameter("@writtencontact", SqlDbType.VarChar);
                para[24].Value = obj.writtencontact;
                para[25] = new SqlParameter("@Org_Indv", SqlDbType.VarChar);
                para[25].Value = obj.Org_Indv;
                para[26] = new SqlParameter("@Full_org_name", SqlDbType.VarChar);
                para[26].Value = obj.Full_org_name;
                para[27] = new SqlParameter("@rdo_SMSF_Yes_No", SqlDbType.VarChar);
                para[27].Value = obj.rdo_SMSF_Yes_No;
                para[28] = new SqlParameter("@proposed_Name_Yes_No", SqlDbType.VarChar);
                para[28].Value = obj.proposed_Name_Yes_No;
                para[29] = new SqlParameter("@proposeidentical_before28may_totalstate", SqlDbType.Int);
                para[29].Value = obj.proposeidentical_before28may_totalstate;
                para[30] = new SqlParameter("@proposeidentical_before28may_previousbusinessno2", SqlDbType.VarChar);
                para[30].Value = obj.proposeidentical_before28may_previousbusinessno2;
                para[31] = new SqlParameter("@proposeidentical_before28may_previousstateteritory2", SqlDbType.VarChar);
                para[31].Value = obj.proposeidentical_before28may_previousstateteritory2;
                para[32] = new SqlParameter("@proposeidentical_before28may_previousbusinessno3", SqlDbType.VarChar);
                para[32].Value = obj.proposeidentical_before28may_previousbusinessno3;
                para[33] = new SqlParameter("@proposeidentical_before28may_previousstateteritory3", SqlDbType.VarChar);
                para[33].Value = obj.proposeidentical_before28may_previousstateteritory3;
                para[34] = new SqlParameter("@proposeidentical_before28may_previousbusinessno4", SqlDbType.VarChar);
                para[34].Value = obj.proposeidentical_before28may_previousbusinessno4;
                para[35] = new SqlParameter("@proposeidentical_before28may_previousstateteritory4", SqlDbType.VarChar);
                para[35].Value = obj.proposeidentical_before28may_previousstateteritory4;
                para[36] = new SqlParameter("@proposeidentical_before28may_previousbusinessno5", SqlDbType.VarChar);
                para[36].Value = obj.proposeidentical_before28may_previousbusinessno5;
                para[37] = new SqlParameter("@proposeidentical_before28may_previousstateteritory5", SqlDbType.VarChar);
                para[37].Value = obj.proposeidentical_before28may_previousstateteritory5;
                para[38] = new SqlParameter("@proposeidentical_before28may_previousbusinessno6", SqlDbType.VarChar);
                para[38].Value = obj.proposeidentical_before28may_previousbusinessno6;
                para[39] = new SqlParameter("@proposeidentical_before28may_previousstateteritory6", SqlDbType.VarChar);
                para[39].Value = obj.proposeidentical_before28may_previousstateteritory6;
                para[40] = new SqlParameter("@proposeidentical_before28may_previousbusinessno7", SqlDbType.VarChar);
                para[40].Value = obj.proposeidentical_before28may_previousbusinessno7;
                para[41] = new SqlParameter("@proposeidentical_before28may_previousstateteritory7", SqlDbType.VarChar);
                para[41].Value = obj.proposeidentical_before28may_previousstateteritory7;
                para[42] = new SqlParameter("@proposeidentical_before28may_previousbusinessno8", SqlDbType.VarChar);
                para[42].Value = obj.proposeidentical_before28may_previousbusinessno8;
                para[43] = new SqlParameter("@proposeidentical_before28may_previousstateteritory8", SqlDbType.VarChar);
                para[43].Value = obj.proposeidentical_before28may_previousstateteritory8;
                para[44] = new SqlParameter("@OpeningTime", SqlDbType.VarChar);
                para[44].Value = obj.OpeningTime;
                para[45] = new SqlParameter("@ClosingTime", SqlDbType.VarChar);
                para[45].Value = obj.ClosingTime;
                para[46] = new SqlParameter("@Isstandard_hours", SqlDbType.VarChar);
                para[46].Value = obj.Isstandard_hours;
                para[47] = new SqlParameter("@trustee_trustname", SqlDbType.VarChar);
                para[47].Value = obj.trustee_trustname;
                para[48] = new SqlParameter("@trustee_abn", SqlDbType.VarChar);
                para[48].Value = obj.trustee_abn;
                para[49] = new SqlParameter("@trustee_tfn", SqlDbType.VarChar);
                para[49].Value = obj.trustee_tfn;
                para[50] = new SqlParameter("@trustee_address", SqlDbType.VarChar);
                para[50].Value = obj.trustee_address;
                para[51] = new SqlParameter("@trustee_country", SqlDbType.VarChar);
                para[51].Value = obj.trustee_country;
                para[52] = new SqlParameter("@companyusedfor", SqlDbType.VarChar);
                para[52].Value = obj.companyusedfor;
                para[53] = new SqlParameter("@ulimateHoldingCompany", SqlDbType.VarChar);
                para[53].Value = obj.UlimateHoldingCompany;
                para[54] = new SqlParameter("@ucompanyname", SqlDbType.VarChar);
                para[54].Value = obj.ucompanyname;
                para[55] = new SqlParameter("@acnarbnabn", SqlDbType.VarChar);
                para[55].Value = obj.acnarbnabn;
                para[56] = new SqlParameter("@countryIcor", SqlDbType.VarChar);
                para[56].Value = obj.countryIcor;

                ii = dal.executeprocedure("update_step1", para).ToString();
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }
        public string insert_step2(Step2 obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[16];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@contactperson", SqlDbType.VarChar);
                para[1].Value = obj.contactperson;
                para[2] = new SqlParameter("@unit_level_suite", SqlDbType.VarChar);
                para[2].Value = obj.unit_level_suite;
                para[3] = new SqlParameter("@streetNoName", SqlDbType.VarChar);
                para[3].Value = obj.streetNoName;
                para[4] = new SqlParameter("@suburb_town_city", SqlDbType.VarChar);
                para[4].Value = obj.suburb_town_city;
                para[5] = new SqlParameter("@state", SqlDbType.VarChar);
                para[5].Value = obj.state;
                para[6] = new SqlParameter("@postcode", SqlDbType.VarChar);
                para[6].Value = obj.postcode;
                para[7] = new SqlParameter("@iscompanylocatedaboveaddress", SqlDbType.VarChar);
                para[7].Value = obj.iscompanylocatedaboveaddress;
                para[8] = new SqlParameter("@isprimaryaddress", SqlDbType.VarChar);
                para[8].Value = obj.isprimaryaddress;
                para[9] = new SqlParameter("@contactperson_primary", SqlDbType.VarChar);
                para[9].Value = obj.contactperson_primary;
                para[10] = new SqlParameter("@unit_level_suite_primary", SqlDbType.VarChar);
                para[10].Value = obj.unit_level_suite_primary;
                para[11] = new SqlParameter("@streetNoName_primary", SqlDbType.VarChar);
                para[11].Value = obj.streetNoName_primary;
                para[12] = new SqlParameter("@suburb_town_city_primary", SqlDbType.VarChar);
                para[12].Value = obj.suburb_town_city_primary;
                para[13] = new SqlParameter("@state_primary", SqlDbType.VarChar);
                para[13].Value = obj.state_primary;
                para[14] = new SqlParameter("@postcode_primary", SqlDbType.VarChar);
                para[14].Value = obj.postcode_primary;
                para[15] = new SqlParameter("@occupiername", SqlDbType.VarChar);
                para[15].Value = obj.occupiername;
                //ii = dal.executeprocedure_returnid("insert_step2", para);
                ii = dal.executeprocedure("insert_step2", para).ToString();
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }
        public string update_step2(Step2 obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[16];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@contactperson", SqlDbType.VarChar);
                para[1].Value = obj.contactperson;
                para[2] = new SqlParameter("@unit_level_suite", SqlDbType.VarChar);
                para[2].Value = obj.unit_level_suite;
                para[3] = new SqlParameter("@streetNoName", SqlDbType.VarChar);
                para[3].Value = obj.streetNoName;
                para[4] = new SqlParameter("@suburb_town_city", SqlDbType.VarChar);
                para[4].Value = obj.suburb_town_city;
                para[5] = new SqlParameter("@state", SqlDbType.VarChar);
                para[5].Value = obj.state;
                para[6] = new SqlParameter("@postcode", SqlDbType.VarChar);
                para[6].Value = obj.postcode;
                para[7] = new SqlParameter("@iscompanylocatedaboveaddress", SqlDbType.VarChar);
                para[7].Value = obj.iscompanylocatedaboveaddress;
                para[8] = new SqlParameter("@isprimaryaddress", SqlDbType.VarChar);
                para[8].Value = obj.isprimaryaddress;
                para[9] = new SqlParameter("@contactperson_primary", SqlDbType.VarChar);
                para[9].Value = obj.contactperson_primary;
                para[10] = new SqlParameter("@unit_level_suite_primary", SqlDbType.VarChar);
                para[10].Value = obj.unit_level_suite_primary;
                para[11] = new SqlParameter("@streetNoName_primary", SqlDbType.VarChar);
                para[11].Value = obj.streetNoName_primary;
                para[12] = new SqlParameter("@suburb_town_city_primary", SqlDbType.VarChar);
                para[12].Value = obj.suburb_town_city_primary;
                para[13] = new SqlParameter("@state_primary", SqlDbType.VarChar);
                para[13].Value = obj.state_primary;
                para[14] = new SqlParameter("@postcode_primary", SqlDbType.VarChar);
                para[14].Value = obj.postcode_primary;
                para[15] = new SqlParameter("@occupiername", SqlDbType.VarChar);
                para[15].Value = obj.occupiername;
                //ii = dal.executeprocedure_returnid("insert_step2", para);
                ii = dal.executeprocedure("update_step2", para).ToString();
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }

        public string update_Registration(Step2 obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@unit_level_suite", SqlDbType.VarChar);
                para[1].Value = obj.unit_level_suite;
                para[2] = new SqlParameter("@streetNoName", SqlDbType.VarChar);
                para[2].Value = obj.streetNoName;
                para[3] = new SqlParameter("@suburb_town_city", SqlDbType.VarChar);
                para[3].Value = obj.suburb_town_city;
                para[4] = new SqlParameter("@state", SqlDbType.VarChar);
                para[4].Value = obj.state;
                para[5] = new SqlParameter("@postcode", SqlDbType.VarChar);
                para[5].Value = obj.postcode;
          
                //ii = dal.executeprocedure_returnid("insert_step2", para);
                ii = dal.executeprocedure("_update_Registration", para).ToString();
            }
            catch (Exception ex)
            {

            }
            return ii;
        }

        public DataTable getStep2_bycid(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                dt = dal.executedtprocedure("getStep2_bycid", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }
        public string update_step3(Step3 obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[22];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@designation", SqlDbType.VarChar);
                para[1].Value = obj.designation;
                para[2] = new SqlParameter("@firstname", SqlDbType.VarChar);
                para[2].Value = obj.firstname;
                para[3] = new SqlParameter("@middlename", SqlDbType.VarChar);
                para[3].Value = obj.middlename;
                para[4] = new SqlParameter("@familyname", SqlDbType.VarChar);
                para[4].Value = obj.familyname;
                para[5] = new SqlParameter("@anyformername", SqlDbType.VarChar);
                para[5].Value = obj.anyformername;
                para[6] = new SqlParameter("@firstname_former", SqlDbType.VarChar);
                para[6].Value = obj.firstname_former;
                para[7] = new SqlParameter("@middlename_former", SqlDbType.VarChar);
                para[7].Value = obj.middlename_former;
                para[8] = new SqlParameter("@familyname_former", SqlDbType.VarChar);
                para[8].Value = obj.familyname_former;
                para[9] = new SqlParameter("@unit_level_suite_primary", SqlDbType.VarChar);
                para[9].Value = obj.unit_level_suite_primary;
                para[10] = new SqlParameter("@suburb_town_city_primary", SqlDbType.VarChar);
                para[10].Value = obj.suburb_town_city_primary;
                para[11] = new SqlParameter("@state_primary", SqlDbType.VarChar);
                para[11].Value = obj.state_primary;
                para[12] = new SqlParameter("@postcode_primary", SqlDbType.VarChar);
                para[12].Value = obj.postcode_primary;
                para[13] = new SqlParameter("@country", SqlDbType.VarChar);
                para[13].Value = obj.country;
                para[14] = new SqlParameter("@dob", SqlDbType.VarChar);
                para[14].Value = obj.dob;
                para[15] = new SqlParameter("@placeofbirth", SqlDbType.VarChar);
                para[15].Value = obj.placeofbirth;
                para[16] = new SqlParameter("@countryofbirth", SqlDbType.VarChar);
                para[16].Value = obj.countryofbirth;
                para[17] = new SqlParameter("@streetNoName_primary", SqlDbType.VarChar);
                para[17].Value = obj.streetNoName_primary;
                para[18] = new SqlParameter("@IsDirector", SqlDbType.VarChar);
                para[18].Value = obj.IsDirector;
                para[19] = new SqlParameter("@IsSecretary", SqlDbType.VarChar);
                para[19].Value = obj.IsSecretary;
                para[20] = new SqlParameter("@IsPublicOfficer", SqlDbType.VarChar);
                para[20].Value = obj.IsPublicOfficer;
                para[21] = new SqlParameter("@id", SqlDbType.Int);
                para[21].Value = obj.id;
                ii = dal.executeprocedure("update_step3", para).ToString();
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }
        public DataTable get_step3(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                dt = dal.executedtprocedure("get_step3", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }
        public DataTable get_step3By_dirId(string companyid, string dirid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                para[1] = new SqlParameter("@dirid", SqlDbType.VarChar);
                para[1].Value = dirid;
                dt = dal.executedtprocedure("get_step3By_dirId", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }
        public string delete_step3_By_dirId(string companyid, string dirid)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                para[1] = new SqlParameter("@id", SqlDbType.VarChar);
                para[1].Value = dirid;
                ii = dal.executeprocedure("delete_step3By_dirId", para).ToString();
            }
            catch (Exception ex)
            {
            }
            return ii;
        }
        public string insert_Step4_AnotherShareHolder(Step4_AnotherShareHolder obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[36];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@shareholderdetails", SqlDbType.VarChar);
                para[1].Value = obj.shareholderdetails;
                para[2] = new SqlParameter("@shareclasstype_value", SqlDbType.VarChar);
                para[2].Value = obj.shareclasstype_value;
                para[3] = new SqlParameter("@shareclasstype_text", SqlDbType.VarChar);
                para[3].Value = obj.shareclasstype_text;
                para[4] = new SqlParameter("@no_of_shares", SqlDbType.VarChar);
                para[4].Value = obj.no_of_shares;
                para[5] = new SqlParameter("@amountpaidpershare", SqlDbType.VarChar);
                para[5].Value = obj.amountpaidpershare;
                para[6] = new SqlParameter("@amountremainingunpaidpershare", SqlDbType.VarChar);
                para[6].Value = obj.amountremainingunpaidpershare;
                para[7] = new SqlParameter("@isheldanotherorg", SqlDbType.VarChar);
                para[7].Value = obj.isheldanotherorg;
                para[8] = new SqlParameter("@beneficialownername", SqlDbType.VarChar);
                para[8].Value = obj.beneficialownername;
                para[9] = new SqlParameter("@step4ID", SqlDbType.VarChar);
                para[9].Value = obj.step4ID;
                para[10] = new SqlParameter("@individual_or_company", SqlDbType.VarChar);
                para[10].Value = obj.individual_or_company;
                para[11] = new SqlParameter("@individual_or_company_name", SqlDbType.VarChar);
                para[11].Value = obj.individual_or_company_name;
                para[12] = new SqlParameter("@individual_or_company_acn", SqlDbType.VarChar);
                para[12].Value = obj.individual_or_company_acn;
                para[13] = new SqlParameter("@individual_or_company_address", SqlDbType.VarChar);
                para[13].Value = obj.individual_or_company_address;

                para[14] = new SqlParameter("@individual_or_company_dob", SqlDbType.VarChar);
                para[14].Value = obj.individual_or_company_dob;
                para[15] = new SqlParameter("@individual_or_company_unit_level_suite", SqlDbType.VarChar);
                para[15].Value = obj.individual_or_company_unit_level_suite;
                para[16] = new SqlParameter("@individual_or_company_streetNoName", SqlDbType.VarChar);
                para[16].Value = obj.individual_or_company_streetNoName;
                para[17] = new SqlParameter("@individual_or_company_suburb_town_city", SqlDbType.VarChar);
                para[17].Value = obj.individual_or_company_suburb_town_city;
                para[18] = new SqlParameter("@individual_or_company_state", SqlDbType.VarChar);
                para[18].Value = obj.individual_or_company_state;
                para[19] = new SqlParameter("@individual_or_company_postcode", SqlDbType.VarChar);
                para[19].Value = obj.individual_or_company_postcode;
                para[20] = new SqlParameter("@individual_or_company_country", SqlDbType.VarChar);
                para[20].Value = obj.individual_or_company_country;

                para[21] = new SqlParameter("@individual_or_company_joint", SqlDbType.VarChar);
                para[21].Value = obj.individual_or_company_Joint;
                para[22] = new SqlParameter("@individual_or_company_name_joint", SqlDbType.VarChar);
                para[22].Value = obj.individual_or_company_name_Joint;
                para[23] = new SqlParameter("@individual_or_company_acn_joint", SqlDbType.VarChar);
                para[23].Value = obj.individual_or_company_acn_Joint;
                para[24] = new SqlParameter("@individual_or_company_address_joint", SqlDbType.VarChar);
                para[24].Value = obj.individual_or_company_address_Joint;

                para[25] = new SqlParameter("@individual_or_company_dob_joint", SqlDbType.VarChar);
                para[25].Value = obj.individual_or_company_dob_Joint;
                para[26] = new SqlParameter("@individual_or_company_unit_level_suite_joint", SqlDbType.VarChar);
                para[26].Value = obj.individual_or_company_unit_level_suite_Joint;
                para[27] = new SqlParameter("@individual_or_company_streetNoName_joint", SqlDbType.VarChar);
                para[27].Value = obj.individual_or_company_streetNoName_Joint;
                para[28] = new SqlParameter("@individual_or_company_suburb_town_city_joint", SqlDbType.VarChar);
                para[28].Value = obj.individual_or_company_suburb_town_city_Joint;
                para[29] = new SqlParameter("@individual_or_company_state_joint", SqlDbType.VarChar);
                para[29].Value = obj.individual_or_company_state_Joint;
                para[30] = new SqlParameter("@individual_or_company_postcode_joint", SqlDbType.VarChar);
                para[30].Value = obj.individual_or_company_postcode_Joint;
                para[31] = new SqlParameter("@individual_or_company_country_Joint", SqlDbType.VarChar);
                para[31].Value = obj.individual_or_company_country_Joint;
                para[32] = new SqlParameter("@ISJOINT", SqlDbType.VarChar);
                para[32].Value = obj.ISJOINT;
                para[33] = new SqlParameter("@dirid", SqlDbType.VarChar);
                para[33].Value = obj.dirid;
				////by praveen				
				para[34] = new SqlParameter("@shareoption", SqlDbType.VarChar);
				para[34].Value = obj.shareoption;				
				para[35] = new SqlParameter("@sharedetailsnotheldanotherorg", SqlDbType.VarChar);
				para[35].Value = obj.sharedetailsnotheldanotherorg;

				ii = dal.executeprocedure_returnid("insert_step4_anothershareholder", para);
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }
        public string update_step4_anothershareholder(Step4_AnotherShareHolder obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[38];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@shareholderdetails", SqlDbType.VarChar);
                para[1].Value = obj.shareholderdetails;
                para[2] = new SqlParameter("@shareclasstype_value", SqlDbType.VarChar);
                para[2].Value = obj.shareclasstype_value;
                para[3] = new SqlParameter("@shareclasstype_text", SqlDbType.VarChar);
                para[3].Value = obj.shareclasstype_text;
                para[4] = new SqlParameter("@no_of_shares", SqlDbType.VarChar);
                para[4].Value = obj.no_of_shares;
                para[5] = new SqlParameter("@amountpaidpershare", SqlDbType.VarChar);
                para[5].Value = obj.amountpaidpershare;
                para[6] = new SqlParameter("@amountremainingunpaidpershare", SqlDbType.VarChar);
                para[6].Value = obj.amountremainingunpaidpershare;
                para[7] = new SqlParameter("@isheldanotherorg", SqlDbType.VarChar);
                para[7].Value = obj.isheldanotherorg;
                para[8] = new SqlParameter("@beneficialownername", SqlDbType.VarChar);
                para[8].Value = obj.beneficialownername;
                para[9] = new SqlParameter("@step4ID", SqlDbType.VarChar);
                para[9].Value = obj.step4ID;
                para[10] = new SqlParameter("@individual_or_company", SqlDbType.VarChar);
                para[10].Value = obj.individual_or_company;
                para[11] = new SqlParameter("@individual_or_company_name", SqlDbType.VarChar);
                para[11].Value = obj.individual_or_company_name;
                para[12] = new SqlParameter("@individual_or_company_acn", SqlDbType.VarChar);
                para[12].Value = obj.individual_or_company_acn;
                para[13] = new SqlParameter("@individual_or_company_address", SqlDbType.VarChar);
                para[13].Value = obj.individual_or_company_address;

                para[14] = new SqlParameter("@individual_or_company_dob", SqlDbType.VarChar);
                para[14].Value = obj.individual_or_company_dob;
                para[15] = new SqlParameter("@individual_or_company_unit_level_suite", SqlDbType.VarChar);
                para[15].Value = obj.individual_or_company_unit_level_suite;
                para[16] = new SqlParameter("@individual_or_company_streetNoName", SqlDbType.VarChar);
                para[16].Value = obj.individual_or_company_streetNoName;
                para[17] = new SqlParameter("@individual_or_company_suburb_town_city", SqlDbType.VarChar);
                para[17].Value = obj.individual_or_company_suburb_town_city;
                para[18] = new SqlParameter("@individual_or_company_state", SqlDbType.VarChar);
                para[18].Value = obj.individual_or_company_state;
                para[19] = new SqlParameter("@individual_or_company_postcode", SqlDbType.VarChar);
                para[19].Value = obj.individual_or_company_postcode;
                para[20] = new SqlParameter("@individual_or_company_country", SqlDbType.VarChar);
                para[20].Value = obj.individual_or_company_country;

                para[21] = new SqlParameter("@individual_or_company_joint", SqlDbType.VarChar);
                para[21].Value = obj.individual_or_company_Joint;
                para[22] = new SqlParameter("@individual_or_company_name_joint", SqlDbType.VarChar);
                para[22].Value = obj.individual_or_company_name_Joint;
                para[23] = new SqlParameter("@individual_or_company_acn_joint", SqlDbType.VarChar);
                para[23].Value = obj.individual_or_company_acn_Joint;
                para[24] = new SqlParameter("@individual_or_company_address_joint", SqlDbType.VarChar);
                para[24].Value = obj.individual_or_company_address_Joint;

                para[25] = new SqlParameter("@individual_or_company_dob_joint", SqlDbType.VarChar);
                para[25].Value = obj.individual_or_company_dob_Joint;
                para[26] = new SqlParameter("@individual_or_company_unit_level_suite_joint", SqlDbType.VarChar);
                para[26].Value = obj.individual_or_company_unit_level_suite_Joint;
                para[27] = new SqlParameter("@individual_or_company_streetNoName_joint", SqlDbType.VarChar);
                para[27].Value = obj.individual_or_company_streetNoName_Joint;
                para[28] = new SqlParameter("@individual_or_company_suburb_town_city_joint", SqlDbType.VarChar);
                para[28].Value = obj.individual_or_company_suburb_town_city_Joint;
                para[29] = new SqlParameter("@individual_or_company_state_joint", SqlDbType.VarChar);
                para[29].Value = obj.individual_or_company_state_Joint;
                para[30] = new SqlParameter("@individual_or_company_postcode_joint", SqlDbType.VarChar);
                para[30].Value = obj.individual_or_company_postcode_Joint;
                para[31] = new SqlParameter("@individual_or_company_country_Joint", SqlDbType.VarChar);
                para[31].Value = obj.individual_or_company_country_Joint;
                para[32] = new SqlParameter("@ISJOINT", SqlDbType.VarChar);
                para[32].Value = obj.ISJOINT;
                para[33] = new SqlParameter("@dirid", SqlDbType.VarChar);
                para[33].Value = obj.dirid;
                para[34] = new SqlParameter("@id", SqlDbType.Int);
                para[34].Value = obj.id;
                para[35] = new SqlParameter("@placeofbirth", SqlDbType.VarChar);
                para[35].Value = obj.placeofbirth;
                ////by praveen

                para[36] = new SqlParameter("@shareoption", SqlDbType.VarChar);
                para[36].Value = obj.shareoption;
                para[37] = new SqlParameter("@sharedetailsnotheldanotherorg", SqlDbType.VarChar);
                para[37].Value = obj.sharedetailsnotheldanotherorg;

                //ii = dal.executeprocedure_returnid("update_step4_anothershareholder", para);
                int iii = dal.executeprocedure("update_step4_anothershareholder", para);
                ii = iii.ToString();
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }
        public string insert_step1_Share_distribute_grid(Step1_shares_distribute obj)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[15];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = obj.companyid;
                para[1] = new SqlParameter("@shareclass", SqlDbType.VarChar);
                para[1].Value = obj.shareclass;
                para[2] = new SqlParameter("@totalshares", SqlDbType.VarChar);
                para[2].Value = obj.totalshares;
                para[3] = new SqlParameter("@unitprice", SqlDbType.VarChar);
                para[3].Value = obj.unitprice;
                para[4] = new SqlParameter("@totalprice", SqlDbType.VarChar);
                para[4].Value = obj.totalprice;
                para[5] = new SqlParameter("@c_totalshares", SqlDbType.VarChar);
                para[5].Value = obj.c_totalshares;
                para[6] = new SqlParameter("@c_amountpaidpershare", SqlDbType.VarChar);
                para[6].Value = obj.c_amountpaidpershare;
                para[7] = new SqlParameter("@c_amountremaining_unpaidpershare", SqlDbType.VarChar);
                para[7].Value = obj.c_amountremaining_unpaidpershare;
                para[8] = new SqlParameter("@c_totalamountpaidpershare", SqlDbType.VarChar);
                para[8].Value = obj.c_totalamountpaidpershare;
                para[9] = new SqlParameter("@c_totalamountunpaidpershare", SqlDbType.VarChar);
                para[9].Value = obj.c_totalamountunpaidpershare;
                para[10] = new SqlParameter("@c_sharerange", SqlDbType.VarChar);
                para[10].Value = obj.c_sharerange;
                para[11] = new SqlParameter("@c_certificateno", SqlDbType.VarChar);
                para[11].Value = obj.c_certificateno;
                para[12] = new SqlParameter("@sno", SqlDbType.VarChar);
                para[12].Value = obj.sno;
                para[13] = new SqlParameter("@linkid", SqlDbType.VarChar);
                para[13].Value = obj.linkid;
                para[14] = new SqlParameter("@individual_or_company", SqlDbType.VarChar);
                para[14].Value = obj.individual_or_company;
                //ii = dal.executeprocedure_returnid("insert_step1", para);
                ii = dal.executeprocedure("insert_Share_distribute_grid1", para).ToString();
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }

        public string delete_step4_anothershareholder(string companyid)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                ii = dal.executeprocedure("delete_step4_anothershareholder", para).ToString();
            }
            catch (Exception ex)
            {

            }
            return ii;
        }

        public string delete_share_distribute_grid_step4(string companyid)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                ii = dal.executeprocedure("delete_Share_distribute_grid", para).ToString();
            }
            catch (Exception ex)
            {
               
            }
            return ii;
        }

        public string delete_share_distribute_grid_step4_ind(string companyid)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                ii = dal.executeprocedure("delete_share_distribute_grid_step4_ind", para).ToString();
            }
            catch (Exception ex)
            {

            }
            return ii;
        }

        public string delete_step4_anothershareholder_ind(string companyid)
        {
            string ii = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                ii = dal.executeprocedure("delete_step4_anothershareholder_ind", para).ToString();
            }
            catch (Exception ex)
            {

            }
            return ii;
        }


        public DataTable get_step4_anothershareholder(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                //para[1] = new SqlParameter("@step4ID", SqlDbType.VarChar);
                //para[1].Value = step4ID;
                dt = dal.executedtprocedure("get_step4_anothershareholder", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }


        public DataTable get_step4_anothershareholderALL(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                dt = dal.executedtprocedure("get_step4_anothershareholderALL", para, false);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        // Update 25 April 2018
        public DataTable get_step4_anothershareholder1(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                //para[1] = new SqlParameter("@step4ID", SqlDbType.VarChar);
                //para[1].Value = step4ID;
                dt = dal.executedtprocedure("get_step4_anothershareholder1", para, false);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable get_step4_get_share_distributegrid(string companyid, string linkid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                para[1] = new SqlParameter("@linkid", SqlDbType.VarChar);
                para[1].Value = linkid;
                dt = dal.executedtprocedure("get_share_distributegrid", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }

        public DataTable get_step4_get_share_distributegrid1(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                dt = dal.executedtprocedure("get_share_distributegrid1", para, false);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable get_step4_get_share_distributegrid12(string companyid, string company)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                para[1] = new SqlParameter("@company", SqlDbType.VarChar);
                para[1].Value = company;

                dt = dal.executedtprocedure("get_share_distributegrid12", para, false);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public string insertLBLmsg(string cid, string sms)
        {
            string reply = "0";
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = cid;
                para[1] = new SqlParameter("@sms", SqlDbType.VarChar);
                para[1].Value = sms;
                reply = dal.executeprocedure("insertLBLmsg", para).ToString();
            }
            catch (Exception ex) { }
            return reply;
        }
        #region insert ASIC
        public string insert_ra55(CompanyDataRA55 obj)
        {
            string ii = "";
            try
            {
                SqlParameter[] para = new SqlParameter[20];
                para[0] = new SqlParameter("@CompanyName", SqlDbType.VarChar);
                para[0].Value = obj.CompanyName;
                para[1] = new SqlParameter("@ACN", SqlDbType.VarChar);
                para[1].Value = obj.ACN;
                para[2] = new SqlParameter("@CompanyType", SqlDbType.VarChar);
                para[2].Value = obj.CompanyType;
                para[3] = new SqlParameter("@CompanyClass", SqlDbType.VarChar);
                para[3].Value = obj.CompanyClass;
                para[4] = new SqlParameter("@CertificatePrintOption", SqlDbType.VarChar);
                para[4].Value = obj.CertificatePrintOption;
                para[5] = new SqlParameter("@JurisdictionOfRegistration", SqlDbType.VarChar);
                para[5].Value = obj.JurisdictionOfRegistration;
                para[6] = new SqlParameter("@DateOfRegistration", SqlDbType.VarChar);
                para[6].Value = obj.DateOfRegistration;
                para[7] = new SqlParameter("@CompanySubclass", SqlDbType.VarChar);
                para[7].Value = obj.CompanySubclass;
                para[8] = new SqlParameter("@AccountNumber", SqlDbType.VarChar);
                para[8].Value = obj.AccountNumber;
                para[9] = new SqlParameter("@SupplierName", SqlDbType.VarChar);
                para[9].Value = obj.SupplierName;
                para[10] = new SqlParameter("@SupplierABN", SqlDbType.VarChar);
                para[10].Value = obj.SupplierABN;
                para[11] = new SqlParameter("@RegisteredAgentName", SqlDbType.VarChar);
                para[11].Value = obj.RegisteredAgentName;
                para[12] = new SqlParameter("@RegisteredAgentAddress", SqlDbType.VarChar);
                para[12].Value = obj.RegisteredAgentAddress;
                para[13] = new SqlParameter("@InvoiceDescription", SqlDbType.VarChar);
                para[13].Value = obj.InvoiceDescription;
                para[14] = new SqlParameter("@InvoiceAmmount", SqlDbType.VarChar);
                para[14].Value = obj.InvoiceAmmount;
                para[15] = new SqlParameter("@DocumentNumber", SqlDbType.VarChar);
                para[15].Value = obj.DocumentNumber;
                para[16] = new SqlParameter("@FormCode", SqlDbType.VarChar);
                para[16].Value = obj.FormCode;
                para[17] = new SqlParameter("@TaxInvoiceText", SqlDbType.VarChar);
                para[17].Value = obj.TaxInvoiceText;
                para[18] = new SqlParameter("@TaxCode", SqlDbType.VarChar);
                para[18].Value = obj.TaxCode;
                para[19] = new SqlParameter("@TaxAmmount", SqlDbType.VarChar);
                para[19].Value = obj.TaxAmmount;
                ii = dal.executeprocedure("insert_ra55", para).ToString();
            }
            catch (Exception ex)
            {

            }
            return ii;
        }
        public string AsicCompanyNameById(string cid)
        {
            string status = "";
            try
            {
                DataTable dt = new DataTable();
                dal.DataAccessLayer dal = new dal.DataAccessLayer();
                dt = dal.getdata("select top 1 fullname from companysearch where id='" + cid + "'");
                if (dt.Rows.Count > 0)
                {
                    status = dt.Rows[0]["fullname"].ToString();
                }
            }
            catch (Exception ex) { }
            return status;
        }
        public string AsicCompanyNameonlyById(string cid)
        {
            string status = "";
            try
            {
                DataTable dt = new DataTable();
                dal.DataAccessLayer dal = new dal.DataAccessLayer();
                dt = dal.getdata("select top 1 companyname from companysearch where id='" + cid + "'");
                if (dt.Rows.Count > 0)
                {
                    status = dt.Rows[0]["companyname"].ToString();
                }
            }
            catch (Exception ex) { }
            return status;
        }
        public string AsicStatus(string cid)
        {
            string status = "";
            try
            {
                DataTable dt = new DataTable();
                dal.DataAccessLayer dal = new dal.DataAccessLayer();
                dt = dal.getdata("select top 1 Asic_status from companysearch where id='" + cid + "'");
                if (dt.Rows.Count > 0)
                {
                    status = dt.Rows[0]["Asic_status"].ToString();
                }
            }
            catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
            return status;
        }
        public string DocumentNo(string cid)
        {
            string status = "";
            try
            {
                DataTable dt = new DataTable();
                dal.DataAccessLayer dal = new dal.DataAccessLayer();
                dt = dal.getdata("select top 1 Asic_DocNo from companysearch where id='" + cid + "'");
                if (dt.Rows.Count > 0)
                {
                    status = dt.Rows[0]["Asic_DocNo"].ToString();
                }
            }
            catch (Exception ex) { }
            return status;
        }
        public string AcnNo(string cid)
        {
            string status = "";
            try
            {
                DataTable dt = new DataTable();
                dal.DataAccessLayer dal = new dal.DataAccessLayer();
                dt = dal.getdata("select top 1 Asic_ACN from companysearch where id='" + cid + "'");
                if (dt.Rows.Count > 0)
                {
                    status = dt.Rows[0]["Asic_ACN"].ToString();
                }
            }
            catch (Exception ex) { }
            return status;
        }
        #endregion
        public DataTable getra55byID(string id)
        {
            DataTable dt = new DataTable();
            dt = dal.getdata("select * from ra55 where companyname=(select top 1 fullname from companysearch where id='" + id + "')");
            return dt;
        }
        public DataTable get_step1(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                dt = dal.executedtprocedure("get_step1", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }


        public DataTable show_Profile(string email)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@Email", SqlDbType.VarChar);
                para[0].Value = email;
                dt = dal.executedtprocedure("sp_get_Profile", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }
        public string insertRegistration(string GivenName, string FamilyName, string Email, string pass, string Phone)
        {
            string reply = "0";

            try
            {
                int i = dal.executesql("insert into Registration(GivenName,FamilyName,Email,pass,Phone) values('" + GivenName + "','" + FamilyName + "','" + Email + "','" + pass + "','" + Phone + "') SELECT SCOPE_IDENTITY() as regid");
                if(i>0)
                    {

                    DataTable ds = dal.getdata("select Max(sno) as regid from Registration with (nolock)");
                    if (ds.Rows.Count > 0)
                    {
                        reply = ds.Rows[0]["regid"].ToString();
                    }
                }
            
            }
            catch (Exception ex) { }
            return reply;
        }
        public DataTable get_registration(string email,long Regid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@email", SqlDbType.VarChar);
                para[0].Value = email;
                para[1] = new SqlParameter("@Regid", SqlDbType.BigInt);
                para[1].Value = Regid;
                dt = dal.executedtprocedure("get_registration", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }
        public DataTable getadminlogi( string email,string pwd) {
            DataTable dt = new DataTable();
            dt = dal.getdata("select * from tbl_user where Email='"+ email + "' and Password='"+ pwd + "'");
            return dt;
        }
        public string htmlEmailClient(string companyname) {
            string html = "";
            html += "Dear Customer,<br><br>";
            html += "Your company <u>" + companyname + "</u> has been been successfully registered with ASIC. <br>Congratulations!<br><br>";
            html += "Please use the following link to retrieve and print your company certificate and company incorporation documents. Please retain these documents for your records:<br>";
            html += "<ul><li>Company Certificate (A.C.N)</li><li>Company Constitution </li><li>Company Registry / Other documents </li><li>Minutes of a meeting of directors </li><li>Share Certificate(s) </li></ul><br>";
            html += "<b>Click here to go to your  Cheap Company Setup client account **</b><br><br>";
            html += "You will need to apply for an ABN (Australian Business Number) and Tax File Number. (TFN) in order for your business to operate.<br><br>";
            html += "In most cases, you also need to register the company for GST. Please visit Australian Business Register (ABR) website at <a href='#'>www.abr.gov.au</a><br><br>";
            html += "Please email us if you have any questions regarding the operation of your company<br><br>";
            html += "We thank you for choosing Cheap Company Setup<br><br>";
            html += "Kind regards,<br>";
            html += "<b>Cheap Company Setup</b><br><br><br>";
            html += "<b>E:</b> info@cheapcompanysetup.com.au<br>";
            html += "<b>P: 02 8011 0790</b><br>";
            html += "<b>W: <a href='www.CheapCompanySetup.com.au'>www.CheapCompanySetup.com.au</a></b><br>";
            html += "<br>";
         
            return html;
        }

        public bool isValidAddress(string unitOrOfficeNumber, string streetName, string locality, string postCode, string state)
        {
            bool isvalid = false;
            try
            {

                /*  state = STATE(state);
                 // string url = "https://onlydeeds.com.au/Deed_searchname/Deeds_queryNameAvailability.svc/addressAvailabilityCheck?unitOrOfficeNumber=" + unitOrOfficeNumber + "&streetName=" + streetName + "&locality=" + locality + "&postCode=" + postCode + "&state=" + state + "";


                  string url = "http://localhost:51697/Deeds_queryNameAvailability.svc/addressAvailabilityCheck?unitOrOfficeNumber=" + unitOrOfficeNumber + "&streetName=" + streetName + "&locality=" + locality + "&postCode=" + postCode + "&state=" + state + "";


                  var SYNCLIENT = new System.Net.WebClient();
                  var content = SYNCLIENT.DownloadString(url);
                  if (content.ToString().ToLower() == "true")
                  {
                      isvalid = true;
                  } */
                isvalid = true;
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());             
            }
          
            return isvalid;
        }
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
        #region 13Aug2017
        public string Fill1_forsummary(string companyid)
        {
            string html = "";
            try
            {

                DataTable dt = new DataTable();
                dal.Operation op = new dal.Operation();
                dt = op.get_step1(companyid);
                if (dt.Rows.Count > 0)
                {
                    html = "<table class='basic-table plain'>";

                    html += "<tr>";
                    html += "<td>Company Name</td>";
                    if (dt.Rows[0]["proposed_Name_Yes_No"].ToString() == "" || dt.Rows[0]["proposed_Name_Yes_No"].ToString().ToUpper() == "YES")
                    {
                        html += "<td>" + dt.Rows[0]["companyname"].ToString() + " " + dt.Rows[0]["companyname_ext"].ToString() + "</td>";
                    }
                    else
                    {
                        html += "<td>ACN will be the Company Name</td>";
                    }
                    html += "</tr>";
                    html += "<tr>";
                    html += "<td> State/Territory for Registration of your company</td>";
                    html += "<td>" + dt.Rows[0]["stateterritorry"].ToString() + "</td>";
                    html += "</tr>";

                    html += "<tr>";
                    html += "<td> Company Type</td>";
                    html += "<td>" + dt.Rows[0]["typeofcompany"].ToString() + "</td>";
                    html += "</tr>";
                    html += "<tr>";
                    html += "<td> Company Class</td>";

                    string classofcompany = "";
                    if (dt.Rows[0]["classofcompany"].ToString().ToLower().Contains("limited by shares"))
                    {
                        classofcompany = "Limited by Shares";
                    }
                    else if (dt.Rows[0]["classofcompany"].ToString().ToLower().Contains("unlimited with a share capital"))
                    {
                        classofcompany = "Unlimited with a Share Capital";
                    }
                    else if (dt.Rows[0]["classofcompany"].ToString().ToLower().Contains("limited by guarantee"))
                    {
                        classofcompany = "Limited By Guarantee";
                    }
                    else if (dt.Rows[0]["classofcompany"].ToString().ToLower().Contains("no liability"))
                    {
                        classofcompany = "No Liability";
                    }


                    html += "<td>" + classofcompany.ToString() + "</td>";
                    html += "</tr>";
                    html += "<tr>";
                    html += "<td> Company Subclass</td>";

                    string specialpurpose_ifapplicable = "";
                    if (dt.Rows[0]["specialpurpose_ifapplicable"].ToString().ToLower().Contains("home"))
                    {
                        specialpurpose_ifapplicable = "Home Unit (HUNT)";
                    }
                    else if (dt.Rows[0]["specialpurpose_ifapplicable"].ToString().ToLower().Contains("superannuation trustee"))
                    {
                        specialpurpose_ifapplicable = "Superannuation Trustee (PSTC)";
                    }
                    else if (dt.Rows[0]["specialpurpose_ifapplicable"].ToString().ToLower().Contains("charitable purposes only"))
                    {
                        specialpurpose_ifapplicable = "Charitable Purposes Only (PNPC)";
                    }
                    else if (dt.Rows[0]["specialpurpose_ifapplicable"].ToString().ToUpper().Contains("PROPRIETARY"))
                    {
                        specialpurpose_ifapplicable = "PROPRIETARY (PROP)";
                    }

                    html += "<td>" + specialpurpose_ifapplicable.ToString() + "</td>";
                    html += "</tr>";

                    /*html += "<tr>";
                    html += "<td>This Company is for Special Purpose</td>";
                    string isspecialpurpose = "";
                    if (dt.Rows[0]["isspecialpurpose"].ToString().ToLower() == "yes")
                    {
                        isspecialpurpose = "Yes";
                    }
                    else
                    {
                        isspecialpurpose = "No";
                    }
                    html += "<td>" + isspecialpurpose.ToString() + "</td>";
                    html += "</tr>";
                    if (dt.Rows[0]["isspecialpurpose"].ToString().ToUpper().Contains("N"))
                    {
                        //chkisspecialcompany.Visible = false;
                    }
                    html += "<tr>";
                    html += "<td>Have you reserved the company's name with ASIC Form 410?</td>";

                    string isreservecompany410 = "";
                    if (dt.Rows[0]["isreservecompany410"].ToString().ToLower() == "yes")
                    {
                        isreservecompany410 = "Yes";
                    }
                    else
                    {
                        isreservecompany410 = "No";
                    }
                    html += "<td>" + isreservecompany410.ToString();
                    if (dt.Rows[0]["isreservecompany410"].ToString().ToLower() == "yes")
                    {
                        html += "<p>*Asic Name Reservation number : " + dt.Rows[0]["reservecompany410_asicnamereservationnumber"].ToString() + "</p>";
                        if (dt.Rows[0]["Indivisual_Company_Asic_form401"].ToString().Trim() == "Individual")
                        {
                            html += "<p>*Full Legal name : " + dt.Rows[0]["reservecompany410_fulllegalname"].ToString() + "</p>";
                        }
                        else
                        {
                            html += "<p>*Company name : " + dt.Rows[0]["CompanyName_Asic_from401"].ToString() + "</p>";
                        }
                    }
                    html += "</td></tr>";

                    html += "<tr>";
                    html += "<td>Is the proposed name identical to a registered business name(s)?</td>";

                    string isproposeidentical = "";
                    if (dt.Rows[0]["isproposeidentical"].ToString().ToLower() == "yes")
                    {
                        isproposeidentical = "Yes";
                    }
                    else
                    {
                        isproposeidentical = "No";
                    }

                    html += "<td>" + isproposeidentical.ToString();
                    if (dt.Rows[0]["isproposeidentical"].ToString().ToLower() == "yes")
                    {
                        if (dt.Rows[0]["proposeidentical_before28may"].ToString().ToLower() != "")
                        {
                            string proposeidentical_before28may_totalstate = dt.Rows[0]["proposeidentical_before28may_totalstate"].ToString();
                            int totalstate = 0;
                            if (proposeidentical_before28may_totalstate != "")
                            {
                                totalstate = Convert.ToInt32(proposeidentical_before28may_totalstate);
                            }
                            html += "<p><b>**Before May 28th 2012</b><br>";
                            for (int iii = 0; iii < totalstate; iii++)
                            {
                                html += "**Previous business number : " + dt.Rows[0]["proposeidentical_before28may_previousbusinessno" + (iii + 1)].ToString() + "<br>**Previous state/territory of registration : " + dt.Rows[0]["proposeidentical_before28may_previousstateteritory" + (iii + 1)].ToString() + "<br>";
                            }
                            html += "</p>";
                        }

                        if (dt.Rows[0]["proposeidentical_after28may"].ToString().ToLower() != "")
                        {
                            html += "<p><b>***After May 28th 2012</b><br>***ABN Number : " + dt.Rows[0]["proposeidentical_after28may_abnnumber"].ToString() + "</p>";
                        }
                    }
                    if (dt.Rows[0]["isproposeidentical"].ToString().ToUpper().Contains("N"))
                    {
                        //chkisidentical.Visible = false;
                    }
                    html += "</td></tr>";

                    html += "<tr>";
                    html += "<td>Does this company have an Ultimate Holding Company?</td>";

                    string isultimateholdingcompany = "";
                    if (dt.Rows[0]["isultimateholdingcompany"].ToString().ToLower() == "yes")
                    {
                        isultimateholdingcompany = "Yes";
                    }
                    else
                    {
                        isultimateholdingcompany = "No";
                    }

                    html += "<td>" + isultimateholdingcompany.ToString();
                    if (dt.Rows[0]["isultimateholdingcompany"].ToString().ToLower() == "yes")
                    {
                        html += "<p><b>Full Legal Name of ultimate holding company : </b>" + dt.Rows[0]["ultimateholdingcompany_fulllegalname"].ToString() + "</p>";
                        html += "<p><b>Country of incorporation : </b>" + dt.Rows[0]["ultimateholdingcompany_country"].ToString() + "</p>";
                        html += "<p><b>ACN/ARBN of ultimate holding company (Only if country of incorporation is Australia) : </b>" + dt.Rows[0]["ultimateholdingcompany_ACN_ARBN"].ToString() + "</p>";
                        html += "<p><b>ABN of ultimate holding company  : </b>" + dt.Rows[0]["ultimateholdingcompany_ABN"].ToString() + "</p>";
                    }
                    html += "</td></tr>";*/

                    html += "</table>";

                }
            }
            catch (Exception ex) { }
            return html;
        }
        #endregion
        #region Sachin added on 11 Aug 2017

        public DataTable get_lodgement()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = dal.executedtprocedure("get_lodgement", null, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }


        public DataTable get_share_allocation_MainPdf(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;
                dt = dal.executedtprocedure("get_share_allocation_MainPdf", para, false);
            }
            catch (Exception ex)
            {
               
            }
            return dt;
        }

        public DataTable get_step4_minut(string companyid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@companyid", SqlDbType.VarChar);
                para[0].Value = companyid;

                dt = dal.executedtprocedure("get_step4_minut", para, false);
            }
            catch (Exception ex)
            {
                //
            }
            return dt;
        }

        #endregion
        #region 08Sept2017 
        public int updatePayment_companySearch(string companyid,string email)
        {
            int i = 0;
            i = dal.executesql("update companysearch set [status]='paid',[show_status]='1' where id='" + companyid + "' and userid='" + email + "'");
            return i;
        }
        #endregion

        #region update fees of constitutions 
        public int updateconstPayment_companySearch(long companyid, string email)
        {
            int i = 0;
            i = dal.executesql("update companysearch set [govofcomapany]='yes' where id='" + companyid + "' and userid='" + email + "'");
            return i;
        }

        //public string getconstPayment_companySearch(long companyid, string email)
        //{
        //    string msg = "";
        //    DataTable dtt = dal.getdata("select govofcomapany from companysearch  where id='" + companyid + "' and userid='" + email + "'");
        //    if (dtt.Rows.Count > 0)
        //    {
        //       msg = dtt.Rows[0]["govofcomapany"].ToString().Trim();
        //    }

        //    return msg;
        //}

        public string getconstPayment_companySearch(long companyid, string email)
        {
            string msg = "";
            DataSet dtt = dal.getdataset("select govofcomapany from companysearch  where id='" + companyid + "' and userid='" + email + "'; select companyusedfor from step1 where companyid='" + companyid + "';");
            if (dtt.Tables[0].Rows.Count > 0)
            {
                msg = dtt.Tables[0].Rows[0]["govofcomapany"].ToString().Trim();
            }
           // if (msg != "")
            {
                msg += ",";
            }
            if (dtt.Tables[1].Rows.Count > 0)
            {
                msg += dtt.Tables[1].Rows[0]["companyusedfor"].ToString().Trim();
            }

            return msg;
        }

        #endregion
    }
}
