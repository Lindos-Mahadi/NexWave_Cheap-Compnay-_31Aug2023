using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.dal
{
    public class pdfForm201
    {
        #region lodgement
        public string asic_registered_agentNo { get; set; }
        public string firm { get; set; }
        public string contactname { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string postaladdress { get; set; }
        public string suburb { get; set; }
        public string stateterritory { get; set; }
        public string postcode { get; set; }
        #endregion
        #region Company_Address
        public string Company_Address_contactperson { get; set; }
        public string Company_Address_unit_level_suite { get; set; }
        public string Company_Address_streetNoName { get; set; }
        public string Company_Address_suburb_town_city { get; set; }
        public string Company_Address_state { get; set; }
        public string Company_Address_postcode { get; set; }

        public string Company_Address_contactperson_primary { get; set; }
        public string Company_Address_unit_level_suite_primary { get; set; }
        public string Company_Address_streetNoName_primary { get; set; }
        public string Company_Address_suburb_town_city_primary { get; set; }
        public string Company_Address_state_primary { get; set; }
        public string Company_Address_postcode_primary { get; set; }

        public string isoccupier { get; set; }
        public string occupiername { get; set; }
        #endregion
        public string lbl1 { get; set; }
        public string lbl2 { get; set; }
        public string lbl3 { get; set; }
        public string lbl4 { get; set; }
        public string lbl5 { get; set; }
        public string lbl6 { get; set; }
        public string lbl7 { get; set; }
        public string lbl8 { get; set; }
        public string lbl9 { get; set; }
        public string lbl10 { get; set; }

        public string chk1 { get; set; }
        public string chk2 { get; set; }
        //  public string chk3 { get; set; }
        public string lbl11 { get; set; }
        public string lbl12 { get; set; }
        // public string chk4 { get; set; }
        // public string chk5 { get; set; }
        // public string chk6 { get; set; }
        //  public string chk7 { get; set; }
        // public string chk8 { get; set; }
        public string chk9 { get; set; }
        public string chk10 { get; set; }

        public string chktype1 { get; set; }
        public string chktype2 { get; set; }
        public string chktype3 { get; set; }
        public string chktype4 { get; set; }
        public string chktype5 { get; set; }
        public string chktype6 { get; set; }
        public string chktype7 { get; set; }
        public string chktype8 { get; set; }
        public string chktype9 { get; set; }

        public string lbl13 { get; set; }





        //further details of company   2 chek box  yes,no
        public string lbl14 { get; set; }
        public string lbl15 { get; set; }
        public string lbl16 { get; set; }
        public string lbl17 { get; set; }
        public string lbl18 { get; set; }
        public string lbl19 { get; set; }
        public string lbl20 { get; set; }
        public string lbl21 { get; set; }

        //further details of compnay chek box
        public string chk11 { get; set; }
        public string chk12 { get; set; }
        public string chk13 { get; set; }
        public string chk14 { get; set; }
        public string chk15 { get; set; }
        public string chk16 { get; set; }

        public string chk17 { get; set; }
        public string chk18 { get; set; }
        public string chk19 { get; set; }
        public string chk20 { get; set; }
        public string chk21 { get; set; }
        public string chk22 { get; set; }
        public string chk23 { get; set; }
        public string chk24 { get; set; }
        public string chk25 { get; set; }
        public string chk26 { get; set; }

        //public string lbl21 { get; set; }
        public string lbl22 { get; set; }
        public string lbl23 { get; set; }
        public string lbl24 { get; set; }
        public string lbl25 { get; set; }
        public string lbl26 { get; set; }
        public string lbl27 { get; set; }

        public string chk27 { get; set; }
        public string chk28 { get; set; }
        public string chk29 { get; set; }
        public string If_no_name_of_occupier { get; set; }
        public string chk30 { get; set; }
        public string chk30_1 { get; set; }
        //further details of company (Does the company occupy the premises?) before textbox16 and 17
        public string Text16 { get; set; }
        public string Text17 { get; set; }
        public string Office_unit_level_2 { get; set; }
        public string Street_number_and_sttreet_name_2 { get; set; }
        public string SubrubCity_3 { get; set; }

        public string StateTerritory_3 { get; set; }
        public string Postcode_3 { get; set; }
        public string Country_if_not_Australia { get; set; }
        public string chkultimateyes { get; set; }
        public string txtultimatecompanyname { get; set; }
        public string txtultimateacnabn { get; set; }
        public string txtultimatecompanycountry { get; set; }
        public string chkultimateno { get; set; }

        public string Text_sco1 { get; set; }
        public string Text_head1 { get; set; }
        public string Text_totalshare1 { get; set; }
        public string Text_totalpaid1 { get; set; }
        public string Text_totalunpaid1 { get; set; }

        public string Text_sco2 { get; set; }
        public string Text_head2 { get; set; }
        public string Text_totalshare2 { get; set; }
        public string Text_totalpaid2 { get; set; }
        public string Text_totalunpaid2 { get; set; }

        public string Text_sco3 { get; set; }
        public string Text_head3 { get; set; }
        public string Text_totalshare3 { get; set; }
        public string Text_totalpaid3 { get; set; }
        public string Text_totalunpaid3 { get; set; }

        public string Text_sco4 { get; set; }
        public string Text_head4 { get; set; }
        public string Text_totalshare4 { get; set; }
        public string Text_totalpaid4 { get; set; }
        public string Text_totalunpaid4 { get; set; }

        public string Text_sco5 { get; set; }
        public string Text_head5 { get; set; }
        public string Text_totalshare5 { get; set; }
        public string Text_totalpaid5 { get; set; }
        public string Text_totalunpaid5 { get; set; }

        public string Text_sco6 { get; set; }
        public string Text_head6 { get; set; }
        public string Text_totalshare6 { get; set; }
        public string Text_totalpaid6 { get; set; }
        public string Text_totalunpaid6 { get; set; }

        public string Text_sco7 { get; set; }
        public string Text_head7 { get; set; }
        public string Text_totalshare7 { get; set; }
        public string Text_totalpaid7 { get; set; }
        public string Text_totalunpaid7 { get; set; }

        public string Text_sco8 { get; set; }
        public string Text_head8 { get; set; }
        public string Text_totalshare8 { get; set; }
        public string Text_totalpaid8 { get; set; }
        public string Text_totalunpaid8 { get; set; }

        public string Text_sco9 { get; set; }
        public string Text_head9 { get; set; }
        public string Text_totalshare9 { get; set; }
        public string Text_totalpaid9 { get; set; }
        public string Text_totalunpaid9 { get; set; }

        public string Text_sco10 { get; set; }
        public string Text_head10 { get; set; }
        public string Text_totalshare10 { get; set; }
        public string Text_totalpaid10 { get; set; }
        public string Text_totalunpaid10 { get; set; }

        public string Text_sco11 { get; set; }
        public string Text_head11 { get; set; }
        public string Text_totalshare11 { get; set; }
        public string Text_totalpaid11 { get; set; }
        public string Text_totalunpaid11 { get; set; }

        public string Text_sco12 { get; set; }
        public string Text_head12 { get; set; }
        public string Text_totalshare12 { get; set; }
        public string Text_totalpaid12 { get; set; }
        public string Text_totalunpaid12 { get; set; }

        public string chk31 { get; set; }
        public string chk32 { get; set; }
        public string chk33 { get; set; }
        public string chk34 { get; set; }

        public string chksignature1 { get; set; }
        public string signaturenameofapplicant { get; set; }
        public string chksignature2 { get; set; }
        public string chksignature3 { get; set; }
        public string Name_of_officeholder { get; set; }
        public string chksignature4 { get; set; }
        public string Name_of_agent { get; set; }
        public string Signature_of_applicant_2 { get; set; }

        public string sd1 { get; set; }
        public string sd2 { get; set; }
        public string sm1 { get; set; }
        public string sm2 { get; set; }
        public string sy1 { get; set; }
        public string sy2 { get; set; }


    }
}