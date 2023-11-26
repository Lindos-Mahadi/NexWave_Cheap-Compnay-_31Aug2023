using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using comdeeds.dal;
using System.Collections;
using static comdeeds.Models.BaseModel;
using comdeeds.App_Code;

namespace comdeeds
{
    public partial class CreateForm201 : System.Web.UI.Page
    {
        private ErrorLog oErrorLog = new ErrorLog();
        private string email = "";
        private string emaile = "";
        private string Role = "";
        private dal.Operation op = new dal.Operation();
        private DataAccessLayer dal = new DataAccessLayer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Email"] != null && Request.QueryString["companyid"] != null)
            {
                string val = Request.QueryString["Email"].ToString();
                hdnemail.Value = val;
                string id = Request.QueryString["companyid"].ToString();
                hdncompanyid.Value = id;
                emaile = CryptoHelper.EncryptData(hdnemail.Value);
                DataTable ds = dal.getdata("select _Role from Tbl_User where Email='" + emaile + "'");
                if (ds.Rows.Count > 0)
                {  Role = ds.Rows[0][0].ToString();}
            }
            else { }

            if (!IsPostBack)
            {
                CreateCustomerFolder();
                try
                {
                    Call201();///create Base of Pdf with company details
                    createHolderPdf();///generate pdf for all holder details
                    createMemberPdf();///generate pdf for all member details
                    string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\");
                    string FirstPart = System.IO.Path.Combine(exportPath, "FirstPart.pdf");
                    string HolderFinal = System.IO.Path.Combine(exportPath, "HolderFinal.pdf");
                    string MemberFinal = System.IO.Path.Combine(exportPath, "MemberFinal.pdf");
                    string LastPart = System.IO.Path.Combine(exportPath, "LastPart.pdf");
                    string[] lstFiles = new string[5];
                    lstFiles[0] = FirstPart;
                    lstFiles[1] = HolderFinal;
                    lstFiles[2] = MemberFinal;
                    lstFiles[3] = LastPart;
                    MergeAll(lstFiles);
                }
                catch (Exception ex)
                {
                    oErrorLog.WriteErrorLog(ex.ToString());

                    if (Role.ToLower() == "admin")
                    {
                        Response.Redirect("/ThankYou?utm_t=a", false);
                    }
                    else if (Role.ToLower() == "subadmin")
                    {
                        Response.Redirect("/ThankYou?utm_t=a", false);
                    }
                    else
                    {
                        Response.Redirect("/ThankYou?utm_t=c", false);
                    }
                }
            }
        }

        private void CreateCustomerFolder()
        {
            try
            {
                string directoryPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value);
                if (!Directory.Exists(directoryPath))
                {  Directory.CreateDirectory(directoryPath); }
            }
            catch (Exception ex)
            {   oErrorLog.WriteErrorLog(ex.ToString()); }
        }

        private void createHolderPdf()
        {
            try
            {
                string defaultPath = Server.MapPath("DefaultDocuments\\Holder\\");
                string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\");
                DataTable dt = new DataTable();

                dt = op.get_step3(hdncompanyid.Value);
                if (dt.Rows.Count > 0)
                {
                    string[] lstFiles = new string[dt.Rows.Count + 1];
                    for (int m = 0; m < dt.Rows.Count; m++)
                    {
                        string designation = "";
                        string pdfTemplate = @defaultPath + "Holder_" + (m + 1) + ".pdf";
                        string newFile = @exportPath + "Holder_" + (m + 1) + ".pdf";
                        lstFiles[m] = newFile;
                        PdfReader pdfReader = new PdfReader(pdfTemplate);
                        PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                        AcroFields pdfFormFields = pdfStamper.AcroFields;
                        //pdfFormFields.SetField("Form 201", "1");
                        designation = dt.Rows[m]["designation"].ToString();
                        pdfFormFields.SetField("holder_txtfamilyname" + (m + 1), dt.Rows[m]["familyname"].ToString());
                        pdfFormFields.SetField("holder_txtgivenname" + (m + 1), dt.Rows[m]["firstname"].ToString() + " " + dt.Rows[m]["middlename"].ToString());
                        //pdfFormFields.SetFieldProperty("lbl2", "Dsys Data ", PdfFormField.FF_READ_ONLY, null);
                        pdfFormFields.SetField("holder_txtformername" + (m + 1), dt.Rows[m]["anyformername"].ToString());
                        if (dt.Rows[m]["anyformername"].ToString().Trim() == "no")
                        {
                            pdfFormFields.SetField("holder_txtcareof" + (m + 1), "NA");
                        }
                        else
                        {
                            pdfFormFields.SetField("holder_txtcareof" + (m + 1), dt.Rows[m]["firstname_former"].ToString() + " " + dt.Rows[m]["middlename_former"].ToString() + " " + dt.Rows[m]["familyname_former"].ToString());
                        }
                        pdfFormFields.SetField("holder_txtunit" + (m + 1), dt.Rows[m]["unit_level_suite_primary"].ToString());
                        pdfFormFields.SetField("holder_txtstreetname" + (m + 1), dt.Rows[m]["streetNoName_primary"].ToString());
                        pdfFormFields.SetField("holder_txtcity" + (m + 1), dt.Rows[m]["suburb_town_city_primary"].ToString());
                        pdfFormFields.SetField("holder_txtstate" + (m + 1), dt.Rows[m]["state_primary"].ToString());
                        pdfFormFields.SetField("holder_txtpostcode" + (m + 1), dt.Rows[m]["postcode_primary"].ToString());
                        pdfFormFields.SetField("holder_txtcountry" + (m + 1), dt.Rows[m]["country"].ToString());
                        string str = "";// Convert.ToDateTime(dt.Rows[m]["dob"].ToString()).ToString("dd/MM/yyyy");
                        string dob__ = dt.Rows[m]["dob"].ToString();
                        if (dob__ != "" && dob__ != "0-0-0")
                        {
                            dob__ = Convert.ToDateTime(dob__).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            dob__ = "";
                        }
                        str = dob__;
                        pdfFormFields.SetField("holder_txtdob" + (m + 1), str);
                        pdfFormFields.SetField("holder_txtplacebirth" + (m + 1), dt.Rows[m]["placeofbirth"].ToString());
                        pdfFormFields.SetField("holder_txtstatecountry" + (m + 1), dt.Rows[m]["countryofbirth"].ToString());
                        if (designation == "director")
                        {
                            pdfFormFields.SetField("holder_chkdirector" + (m + 1), "Yes");
                        }
                        else if (designation == "secretary")
                        {
                            pdfFormFields.SetField("holder_chksecretory" + (m + 1), "Yes");
                        }
                        pdfStamper.Close();
                    }
                    MergeHolders(lstFiles);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void MergeHolders(string[] lstFiles)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = @"E:/pdf/HolderFinal.pdf";
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\HolderFinal.pdf");
            outputPdfPath = exportPath;
            sourceDocument = new Document();
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));
            sourceDocument.Open();

            try
            {
                //Loop through the files list
                for (int f = 0; f < lstFiles.Length - 1; f++)
                {
                    int pages = get_pageCcount(lstFiles[f]);

                    reader = new PdfReader(lstFiles[f]);
                    //Add pages of current file
                    for (int i = 1; i <= pages; i++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }

                    reader.Close();
                }
                //At the end save the output file
                sourceDocument.Close();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void MergeMembers(string[] lstFiles)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = @"E:/pdf/HolderFinal.pdf";
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\MemberFinal.pdf");
            outputPdfPath = exportPath;
            sourceDocument = new Document();
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));
            sourceDocument.Open();

            try
            {
                //Loop through the files list
                for (int f = 0; f < lstFiles.Length - 1; f++)
                {
                    int pages = get_pageCcount(lstFiles[f]);

                    reader = new PdfReader(lstFiles[f]);
                    //Add pages of current file
                    for (int i = 1; i <= pages; i++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }

                    reader.Close();
                }
                //At the end save the output file
                sourceDocument.Close();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void MergeAll(string[] lstFiles)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = @"E:/pdf/HolderFinal.pdf";
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Form201.pdf");
            outputPdfPath = exportPath;
            sourceDocument = new Document();
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));
            sourceDocument.Open();
            try
            {
                //Loop through the files list
                for (int f = 0; f < lstFiles.Length - 1; f++)
                {
                    int pages = get_pageCcount(lstFiles[f]);

                    reader = new PdfReader(lstFiles[f]);
                    //Add pages of current file
                    for (int i = 1; i <= pages; i++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }
                    reader.Close();
                }
                //At the end save the output file
                pdfCopyProvider.Close();
                pdfCopyProvider.Dispose();
                sourceDocument.Close();
                sourceDocument.Dispose();
                //Response.Redirect("../createforms/CreateCompanyConstitutionPart1.aspx?Email=" + email + "&CompanyID=" + hdncompanyid.Value, false);

                try
                {
                    string docn_ = op.DocumentNo(hdncompanyid.Value.ToString());
                    string myfilepath = Server.MapPath("ExportedFiles/" + hdncompanyid.Value + "/Final/" + docn_ + ".pdf");
                    string url = "ExportedFiles/" + hdncompanyid.Value + "/Final/" + docn_ + ".pdf";
                    if (File.Exists("C:/asicfiles/Logs/" + docn_ + ".pdf"))
                    {
                        if (!File.Exists(myfilepath))
                        {
                            System.IO.File.Copy("C:/asicfiles/Logs/" + docn_ + ".pdf", Server.MapPath("ExportedFiles/" + hdncompanyid.Value + "/Final/" + docn_ + ".pdf"));
                        }
                    }
                }
                catch (Exception ex) { oErrorLog.WriteErrorLog(ex.ToString()); }
                Response.Redirect("GeneratePDF.aspx?Email=" + hdnemail.Value + "&CompanyID=" + hdncompanyid.Value, false);

                Button1.Text = "Done!!!!";

                //Response.Redirect("../ExportedFiles\\" + hdncompanyid.Value + "\\Form201.pdf", false);
            }
            catch (Exception ex)
            {
                sourceDocument.Close();
                sourceDocument.Dispose();
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private int get_pageCcount(string file)
        {
            PdfReader pdfReader = new PdfReader(file);
            int numberOfPages = pdfReader.NumberOfPages;
            return numberOfPages;
            //using (StreamReader sr = new StreamReader(File.OpenRead(file)))
            //{
            //    Regex regex = new Regex(@"/Type\s*/Page[^s]");
            //    MatchCollection matches = regex.Matches(sr.ReadToEnd());

            //    return matches.Count;
            //}
        }

        private void createMemberPdf()
        {
            DataAccessLayer daldir = new DataAccessLayer();
            try
            {
                string defaultPath = Server.MapPath("DefaultDocuments\\Member\\");
                string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\");

                DataTable dt_othermember = new DataTable();
                dt_othermember = op.get_step4_anothershareholderALL(hdncompanyid.Value);
                if (dt_othermember.Rows.Count > 0)
                {
                    string[] lstFiles = new string[10];

                    #region others

                    if (dt_othermember.Rows.Count > 0)
                    {
                        for (int member = 0; member < dt_othermember.Rows.Count; member++)
                        {
                            string shareholderdetails_ = dt_othermember.Rows[member]["shareholderdetails"].ToString();
                            string indivisual_company = dt_othermember.Rows[member]["individual_or_company"].ToString().ToLower();
                            string individual_or_company_name = dt_othermember.Rows[member]["individual_or_company_name"].ToString();

                            #region Details

                            lstFiles[(member)] = exportPath + "Member_" + (member + 1) + ".pdf";
                            PdfReader pdfReader_ = new PdfReader(defaultPath + "Member_" + (member + 1) + ".pdf");
                            PdfStamper pdfStamper_ = new PdfStamper(pdfReader_, new FileStream(exportPath + "Member_" + (member + 1) + ".pdf", FileMode.Create));
                            AcroFields pdfFormFields_ = pdfStamper_.AcroFields;
                            int pdfpage_ = member + 1;
                            string address = "";
                            string individual_or_company_unit_level_suite = "";
                            string individual_or_company_streetNoName = "";
                            string individual_or_company_suburb_town_city = "";
                            string individual_or_company_state = "";
                            string individual_or_company_postcode = "";
                            string individual_or_company_country = "";
                            //if( individual_or_company_name.Trim() != "")
                            //{
                            individual_or_company_unit_level_suite = dt_othermember.Rows[member]["individual_or_company_unit_level_suite"].ToString();
                            individual_or_company_streetNoName = dt_othermember.Rows[member]["individual_or_company_streetNoName"].ToString();
                            individual_or_company_suburb_town_city = dt_othermember.Rows[member]["individual_or_company_suburb_town_city"].ToString();
                            individual_or_company_state = dt_othermember.Rows[member]["individual_or_company_state"].ToString();
                            individual_or_company_postcode = dt_othermember.Rows[member]["individual_or_company_postcode"].ToString();
                            individual_or_company_country = dt_othermember.Rows[member]["individual_or_company_country"].ToString();
                            address = (individual_or_company_unit_level_suite + " " + individual_or_company_streetNoName + " " + individual_or_company_suburb_town_city + " " + individual_or_company_state + " " + individual_or_company_postcode + " " + individual_or_company_country).Trim();

                            string beneficialownername_YN = "";
                            string beneficialownername = dt_othermember.Rows[member]["beneficialownername"].ToString();
                            if (beneficialownername != null)
                            {
                                beneficialownername_YN = "Y";
                            }
                            else
                            {
                                beneficialownername_YN = "N";
                            }
                            string individual_or_company = dt_othermember.Rows[member]["individual_or_company"].ToString();
                            string dirid = dt_othermember.Rows[member]["dirid"].ToString();
                            if (individual_or_company.Trim() != "")
                            {
                                shareholderdetails_ = dt_othermember.Rows[member]["individual_or_company_name"].ToString();
                            }
                            string givenname_ = "";
                            string familyname_ = "";
                            string companyname = dt_othermember.Rows[member]["individual_or_company_name"].ToString();
                            string companyacn = dt_othermember.Rows[member]["individual_or_company_acn"].ToString();
                            if (shareholderdetails_.Contains(" "))
                            {
                                int lastindex = shareholderdetails_.Trim().Split(' ').Length - 1;
                                familyname_ = shareholderdetails_.Split(' ')[lastindex];
                                givenname_ = shareholderdetails_.Replace(familyname_, "").Trim();
                            }
                            else
                            {
                                givenname_ = shareholderdetails_;
                            }
                            if (dirid != "" && dirid != "0")
                            {
                                DataTable dtdird = daldir.getdata("select top 1 * from step3 where id='" + dirid + "'");
                                if (dtdird.Rows.Count > 0)
                                {
                                    string fname = dtdird.Rows[0]["familyname"].ToString();
                                    string gname = dtdird.Rows[0]["firstname"].ToString() + " " + dtdird.Rows[0]["middlename"].ToString();
                                    familyname_ = fname.Trim();
                                    givenname_ = gname.Trim();
                                }
                            }

                            if (indivisual_company.Trim() == "Individual" || indivisual_company.Trim() == "" || indivisual_company.Trim() == "Individual1" || indivisual_company.Trim() == "individual" || indivisual_company.Trim() == "individual1")
                            {
                                pdfFormFields_.SetField("chkcompanyname" + pdfpage_, "No");
                                pdfFormFields_.SetField("txtfamilyname" + pdfpage_, familyname_);
                                pdfFormFields_.SetField("txtgivenname" + pdfpage_, givenname_);
                            }

                            if (indivisual_company.Trim() == "company" || indivisual_company.Trim() == "Company")
                            {
                                pdfFormFields_.SetField("chkfamilyname" + pdfpage_, "No");
                                pdfFormFields_.SetField("txtcompanyname" + pdfpage_, companyname);
                                pdfFormFields_.SetField("txtacn" + pdfpage_, companyacn);
                            }

                            pdfFormFields_.SetField("txtcareof" + pdfpage_, "NA");
                            pdfFormFields_.SetField("txtunit" + pdfpage_, individual_or_company_unit_level_suite);
                            pdfFormFields_.SetField("txtstreetname" + pdfpage_, individual_or_company_streetNoName);
                            pdfFormFields_.SetField("txtcity" + pdfpage_, individual_or_company_suburb_town_city);
                            pdfFormFields_.SetField("txtstate" + pdfpage_, individual_or_company_state);
                            pdfFormFields_.SetField("txtpostcode" + pdfpage_, individual_or_company_postcode);
                            pdfFormFields_.SetField("txtcountry" + pdfpage_, individual_or_company_country);

                            if (indivisual_company == "Individual" || indivisual_company.Trim() == "" || indivisual_company == "individual")
                            {
                                DataTable dtsharedetails_ = daldir.getdata("select top 100 *,(c_amountpaidpershare+c_amountremaining_unpaidpershare) as shareRate,(case c_totalamountunpaidpershare when 0 then 'Y' else 'N' end) as FullyPaid from Share_distribute_grid where companyid='" + hdncompanyid.Value.ToString() + "' and linkid='" + dt_othermember.Rows[member]["dirid"].ToString() + "'");
                                if (dtsharedetails_.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtsharedetails_.Rows.Count; i++)
                                    {
                                        string classcode = dtsharedetails_.Rows[i]["shareclass"].ToString();
                                        if (classcode == "Redeemable Preference[REDP]")
                                        {
                                            classcode = "REDP";
                                        }
                                        if (classcode == "Ordinary [ORD]")
                                        {
                                            classcode = "ORD";
                                        }

                                        string noofshares = dtsharedetails_.Rows[i]["c_totalshares"].ToString();
                                        string shareRate = dtsharedetails_.Rows[i]["shareRate"].ToString();
                                        string totalpaid = dtsharedetails_.Rows[i]["c_totalamountpaidpershare"].ToString();
                                        string totalunpaid = dtsharedetails_.Rows[i]["c_totalamountunpaidpershare"].ToString();
                                        string isFullyPaid = dtsharedetails_.Rows[i]["FullyPaid"].ToString();
                                        string amountunpaid = dtsharedetails_.Rows[i]["c_amountremaining_unpaidpershare"].ToString();
                                        pdfFormFields_.SetField("txtclass" + (i + 1) + "_" + pdfpage_, classcode);
                                        pdfFormFields_.SetField("txttaken" + (i + 1) + "_" + pdfpage_, noofshares);
                                        pdfFormFields_.SetField("txtshareamt" + (i + 1) + "_" + pdfpage_, shareRate);
                                        pdfFormFields_.SetField("txtpaid" + (i + 1) + "_" + pdfpage_, totalpaid);
                                        pdfFormFields_.SetField("txtunpaid" + (i + 1) + "_" + pdfpage_, amountunpaid);
                                        pdfFormFields_.SetField("txttotalunpaid" + (i + 1) + "_" + pdfpage_, totalunpaid);
                                        pdfFormFields_.SetField("txtfullypaid" + (i + 1) + "_" + pdfpage_, isFullyPaid);
                                        pdfFormFields_.SetField("txtheld" + (i + 1) + "_" + pdfpage_, beneficialownername_YN);
                                    }
                                }
                                pdfStamper_.Close();
                            }
                            else if (indivisual_company == "Individual1" || indivisual_company == "individual1")
                            {
                                DataTable dtsharedetails_ = daldir.getdata("select top 100 *,(c_amountpaidpershare+c_amountremaining_unpaidpershare) as shareRate,(case c_totalamountunpaidpershare when 0 then 'Y' else 'N' end) as FullyPaid from Share_distribute_grid where companyid='" + hdncompanyid.Value.ToString() + "' and individual_or_company='Individual1'");
                                if (dtsharedetails_.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtsharedetails_.Rows.Count; i++)
                                    {
                                        string classcode = dtsharedetails_.Rows[i]["shareclass"].ToString();
                                        if (classcode == "Redeemable Preference[REDP]")
                                        {
                                            classcode = "REDP";
                                        }
                                        if (classcode == "Ordinary [ORD]")
                                        {
                                            classcode = "ORD";
                                        }

                                        string noofshares = dtsharedetails_.Rows[i]["c_totalshares"].ToString();
                                        string shareRate = dtsharedetails_.Rows[i]["shareRate"].ToString();
                                        string totalpaid = dtsharedetails_.Rows[i]["c_totalamountpaidpershare"].ToString();
                                        string totalunpaid = dtsharedetails_.Rows[i]["c_totalamountunpaidpershare"].ToString();
                                        string isFullyPaid = dtsharedetails_.Rows[i]["FullyPaid"].ToString();
                                        string amountunpaid = dtsharedetails_.Rows[i]["c_amountremaining_unpaidpershare"].ToString();
                                        pdfFormFields_.SetField("txtclass" + (i + 1) + "_" + pdfpage_, classcode);
                                        pdfFormFields_.SetField("txttaken" + (i + 1) + "_" + pdfpage_, noofshares);
                                        pdfFormFields_.SetField("txtshareamt" + (i + 1) + "_" + pdfpage_, shareRate);
                                        pdfFormFields_.SetField("txtpaid" + (i + 1) + "_" + pdfpage_, totalpaid);
                                        pdfFormFields_.SetField("txtunpaid" + (i + 1) + "_" + pdfpage_, amountunpaid);
                                        pdfFormFields_.SetField("txttotalunpaid" + (i + 1) + "_" + pdfpage_, totalunpaid);
                                        pdfFormFields_.SetField("txtfullypaid" + (i + 1) + "_" + pdfpage_, isFullyPaid);
                                        pdfFormFields_.SetField("txtheld" + (i + 1) + "_" + pdfpage_, beneficialownername_YN);
                                    }
                                }
                                pdfStamper_.Close();
                            }
                            else if (indivisual_company == "company" || indivisual_company == "company")
                            {
                                DataTable dtsharedetails_ = daldir.getdata("select top 100 *,(c_amountpaidpershare+c_amountremaining_unpaidpershare) as shareRate,(case c_totalamountunpaidpershare when 0 then 'Y' else 'N' end) as FullyPaid from Share_distribute_grid where companyid='" + hdncompanyid.Value.ToString() + "' and individual_or_company='company'");
                                if (dtsharedetails_.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtsharedetails_.Rows.Count; i++)
                                    {
                                        string classcode = dtsharedetails_.Rows[i]["shareclass"].ToString();
                                        if (classcode == "Redeemable Preference[REDP]")
                                        {
                                            classcode = "REDP";
                                        }
                                        if (classcode == "Ordinary [ORD]")
                                        {
                                            classcode = "ORD";
                                        }

                                        string noofshares = dtsharedetails_.Rows[i]["c_totalshares"].ToString();
                                        string shareRate = dtsharedetails_.Rows[i]["shareRate"].ToString();
                                        string totalpaid = dtsharedetails_.Rows[i]["c_totalamountpaidpershare"].ToString();
                                        string totalunpaid = dtsharedetails_.Rows[i]["c_totalamountunpaidpershare"].ToString();
                                        string isFullyPaid = dtsharedetails_.Rows[i]["FullyPaid"].ToString();
                                        string amountunpaid = dtsharedetails_.Rows[i]["c_amountremaining_unpaidpershare"].ToString();
                                        pdfFormFields_.SetField("txtclass" + (i + 1) + "_" + pdfpage_, classcode);
                                        pdfFormFields_.SetField("txttaken" + (i + 1) + "_" + pdfpage_, noofshares);
                                        pdfFormFields_.SetField("txtshareamt" + (i + 1) + "_" + pdfpage_, shareRate);
                                        pdfFormFields_.SetField("txtpaid" + (i + 1) + "_" + pdfpage_, totalpaid);
                                        pdfFormFields_.SetField("txtunpaid" + (i + 1) + "_" + pdfpage_, amountunpaid);
                                        pdfFormFields_.SetField("txttotalunpaid" + (i + 1) + "_" + pdfpage_, totalunpaid);
                                        pdfFormFields_.SetField("txtfullypaid" + (i + 1) + "_" + pdfpage_, isFullyPaid);
                                        pdfFormFields_.SetField("txtheld" + (i + 1) + "_" + pdfpage_, beneficialownername_YN);
                                    }
                                }
                                pdfStamper_.Close();
                            }

                            #endregion Details
                        }
                    }

                    #endregion others

                    int coun = 0;
                    for (int i = 0; i < lstFiles.Length; i++)
                    {
                        if (lstFiles[i] != null)
                        {
                            if (lstFiles[i].Trim() != "")
                            {
                                coun = coun + 1;
                            }
                        }
                    }
                    ArrayList arr = new ArrayList();
                    string[] lstFilesNULL = new string[coun];
                    for (int k = 0; k < lstFiles.Length; k++)
                    {
                        if (lstFiles[k] != null)
                        {
                            arr.Add(lstFiles[k].ToString());
                        }
                    }

                    string[] lstFilesnew = new string[coun + 1];
                    for (int ii = 0; ii < coun; ii++)
                    {
                        lstFilesnew[ii] = arr[ii].ToString();
                    }

                    MergeMembers(lstFilesnew);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void Call201()
        {
            try
            {
                DataTable dt = new DataTable();

                dal.pdfForm201 obj = new dal.pdfForm201();

                #region initialize Checkbox

                obj.chk1 = "No";
                obj.chk2 = "No";
                // obj.chk3 = "No";
                // obj.chk4 = "No";
                // obj.chk5 = "No";
                // obj.chk6 = "No";
                // obj.chk7 = "No";
                // obj.chk8 = "No";
                obj.chktype1 = "No";
                obj.chktype2 = "No";
                obj.chktype3 = "No";
                obj.chktype4 = "No";
                obj.chktype5 = "No";
                obj.chktype6 = "No";
                obj.chktype7 = "No";
                obj.chktype8 = "No";
                obj.chktype9 = "No";
                obj.chk9 = "No";
                obj.chk10 = "No";
                obj.chk11 = "No";
                obj.chk12 = "No";
                obj.chk13 = "No";
                obj.chk14 = "No";
                obj.chk15 = "No";
                obj.chk16 = "No";
                obj.chk17 = "No";
                obj.chk18 = "No";
                obj.chk19 = "No";
                obj.chk20 = "No";
                obj.chk21 = "No";
                obj.chk22 = "No";
                obj.chk23 = "No";
                obj.chk24 = "No";
                obj.chk25 = "No";
                //obj.chk26 = "No";
                obj.chk26 = "No"; // according to Rob, changes has been done 1 JUn 2018 // changes rollback 04-06-2018
                obj.chk27 = "No";
                obj.chk28 = "No";
                obj.chk29 = "No";
                obj.chk30 = "No";
                obj.chk30_1 = "No";
                obj.chk31 = "No";
                obj.chk32 = "No";
                obj.chk33 = "No";
                obj.chk34 = "No";
                obj.chksignature1 = "No";
                obj.chksignature2 = "No";
                obj.chksignature3 = "No";
                obj.chksignature4 = "No";
                obj.chkultimateyes = "No";
                obj.chkultimateno = "No";

                #endregion initialize Checkbox

                #region Lodgement

                DataTable dtlodge = op.get_lodgement();
                if (dtlodge.Rows.Count > 0)
                {
                    obj.asic_registered_agentNo = dtlodge.Rows[0]["asic_registered_agentNo"].ToString();
                    obj.firm = dtlodge.Rows[0]["firm"].ToString();
                    obj.contactname = dtlodge.Rows[0]["contactname"].ToString();
                    obj.telephone = dtlodge.Rows[0]["telephone"].ToString();
                    obj.email = dtlodge.Rows[0]["email"].ToString();
                    obj.postaladdress = dtlodge.Rows[0]["postaladdress"].ToString();
                    obj.suburb = dtlodge.Rows[0]["suburb"].ToString();
                    obj.stateterritory = dtlodge.Rows[0]["stateterritory"].ToString();
                    obj.postcode = dtlodge.Rows[0]["postcode"].ToString();
                }

                #endregion Lodgement

                #region CompanyAddress

                DataTable dtCompanyAddress = op.getStep2_bycid(hdncompanyid.Value);
                if (dtCompanyAddress.Rows.Count > 0)
                {
                    obj.Company_Address_contactperson = dtCompanyAddress.Rows[0]["contactperson"].ToString();
                    obj.Company_Address_unit_level_suite = dtCompanyAddress.Rows[0]["unit_level_suite"].ToString();
                    obj.Company_Address_streetNoName = dtCompanyAddress.Rows[0]["streetNoName"].ToString();
                    obj.Company_Address_suburb_town_city = dtCompanyAddress.Rows[0]["suburb_town_city"].ToString();
                    obj.Company_Address_state = dtCompanyAddress.Rows[0]["state"].ToString();
                    obj.Company_Address_postcode = dtCompanyAddress.Rows[0]["postcode"].ToString();
                    string isoccupier = dtCompanyAddress.Rows[0]["iscompanylocatedaboveaddress"].ToString().ToLower();

                    string occupiername = dtCompanyAddress.Rows[0]["occupiername"].ToString();

                    if (isoccupier == "no")
                    {
                        obj.isoccupier = "Yes";
                        obj.occupiername = occupiername;
                        obj.chk29 = "Yes";
                    }
                    else
                    {
                        obj.isoccupier = "No";
                        obj.occupiername = "";
                        obj.chk29 = "No";
                        obj.chk27 = "Yes";
                    }
                    string isprimaryaddress = dtCompanyAddress.Rows[0]["isprimaryaddress"].ToString().ToLower();
                    if (isprimaryaddress == "yes")
                    {
                        obj.Company_Address_contactperson_primary = dtCompanyAddress.Rows[0]["contactperson"].ToString();
                        obj.Company_Address_unit_level_suite_primary = dtCompanyAddress.Rows[0]["unit_level_suite"].ToString();
                        obj.Company_Address_streetNoName_primary = dtCompanyAddress.Rows[0]["streetNoName"].ToString();
                        obj.Company_Address_suburb_town_city_primary = dtCompanyAddress.Rows[0]["suburb_town_city"].ToString();
                        obj.Company_Address_state_primary = dtCompanyAddress.Rows[0]["state"].ToString();
                        obj.Company_Address_postcode_primary = dtCompanyAddress.Rows[0]["postcode"].ToString();
                    }
                    else
                    {
                        obj.Company_Address_contactperson_primary = dtCompanyAddress.Rows[0]["contactperson_primary"].ToString();
                        obj.Company_Address_unit_level_suite_primary = dtCompanyAddress.Rows[0]["unit_level_suite_primary"].ToString();
                        obj.Company_Address_streetNoName_primary = dtCompanyAddress.Rows[0]["streetNoName_primary"].ToString();
                        obj.Company_Address_suburb_town_city_primary = dtCompanyAddress.Rows[0]["suburb_town_city_primary"].ToString();
                        obj.Company_Address_state_primary = dtCompanyAddress.Rows[0]["state_primary"].ToString();
                        obj.Company_Address_postcode_primary = dtCompanyAddress.Rows[0]["postcode_primary"].ToString();
                    }
                }

                #endregion CompanyAddress

                // ths option is added after disucss with Rob. on 04-06-2018

                #region constitution cost

                dt = op.getcompanysearchbyid(hdncompanyid.Value);
                if (dt.Rows.Count > 0)
                {
                    string govofcompany = "";
                    govofcompany = dt.Rows[0]["govofcomapany"].ToString();
                    if (govofcompany != "")
                    {
                        if (govofcompany == "yes")
                        {
                            obj.chk26 = "Yes";
                        }
                        else
                        {
                            obj.chk25 = "Yes";
                        }
                    }
                    else
                    {
                        obj.chk25 = "Yes";
                    }
                }

                #endregion constitution cost

                dt = op.get_step1(hdncompanyid.Value);
                if (dt.Rows.Count > 0)
                {
                    obj.lbl11 = dt.Rows[0]["companyname"].ToString();
                    //txtacn.Text = dt.Rows[0]["acn"].ToString();
                    string companyextension = dt.Rows[0]["companyname_ext"].ToString();
                    if (companyextension.Trim().ToUpper().Contains("PTY LTD"))
                    {
                        obj.chktype1 = "Yes";
                    }
                    if (companyextension.Trim().ToUpper() == "PTY. LTD.")
                    {
                        obj.chktype2 = "Yes";
                    }
                    if (companyextension.Trim().ToUpper() == "PTY. LTD")
                    {
                        obj.chktype3 = "Yes";
                    }
                    if (companyextension.Trim().ToUpper().Contains("PTY LTD."))
                    {
                        obj.chktype4 = "Yes";
                    }
                    if (companyextension.Trim().ToUpper().Contains("PTY. LIMITED"))
                    {
                        obj.chktype5 = "Yes";
                    }
                    if (companyextension.Trim().ToUpper().Contains("PTY LIMITED"))
                    {
                        obj.chktype6 = "Yes";
                    }
                    if (companyextension.Trim().ToUpper() == "PROPRIETARY LTD")
                    {
                        obj.chktype7 = "Yes";
                    }
                    if (companyextension.Trim().ToUpper() == "PROPRIETARY LTD.")
                    {
                        obj.chktype8 = "Yes";
                    }
                    if (companyextension.Trim().ToUpper().Contains("PROPRIETARY LIMITED"))
                    {
                        obj.chktype9 = "Yes";
                    }

                    obj.lbl10 = dt.Rows[0]["stateterritorry"].ToString();
                    //string sp1 = dt.Rows[0]["isspecialpurpose"].ToString();
                    //if (sp1.ToLower() == "true")
                    //{ obj.chk24 = "Yes"; }
                    //else { obj.chk24 = "no"; }
                    //obj.chk24 = "no";   // according to Rob, ths changes has been done...  // //changes Rollback 06042018

                    string sp2 = dt.Rows[0]["isreservecompany410"].ToString();
                    if (sp2.ToLower() == "yes")
                    { obj.chk1 = "Yes"; }
                    else
                    { //obj.chk2 = "Yes";
                    }

                    //ddlreservedcompname.SelectedValue=dt.Rows[0]["isreservecompany410"].ToString();
                    if (dt.Rows[0]["isreservecompany410"].ToString().ToLower() == "yes")
                    {
                        obj.lbl12 = dt.Rows[0]["reservecompany410_asicnamereservationnumber"].ToString();
                        //txtpnl_fulllegalname.Text = dt.Rows[0]["reservecompany410_fulllegalname"].ToString();
                    }
                    obj.chk1 = "Yes";
                    string sp3 = dt.Rows[0]["isproposeidentical"].ToString();
                    if (sp3.ToLower() == "yes")
                    { obj.chk9 = "Yes"; }
                    else
                    { obj.chk10 = "Yes"; }
                    //ddlproposedidentical.SelectedValue=dt.Rows[0]["isproposeidentical"].ToString();
                    if (dt.Rows[0]["isproposeidentical"].ToString().ToLower() == "yes")
                    {
                        if (dt.Rows[0]["proposeidentical_before28may"].ToString().ToLower() != "")
                        {
                            obj.lbl14 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno1"].ToString();
                            obj.lbl15 = dt.Rows[0]["proposeidentical_before28may_previousstateteritory1"].ToString();

                            obj.lbl16 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno2"].ToString();
                            obj.lbl17 = dt.Rows[0]["proposeidentical_before28may_previousstateteritory2"].ToString();

                            obj.lbl18 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno3"].ToString();
                            obj.lbl19 = dt.Rows[0]["proposeidentical_before28may_previousstateteritory3"].ToString();

                            obj.lbl20 = dt.Rows[0]["proposeidentical_before28may_previousbusinessno4"].ToString();
                            obj.lbl21 = dt.Rows[0]["proposeidentical_before28may_previousstateteritory4"].ToString();
                        }
                        if (dt.Rows[0]["proposeidentical_after28may"].ToString().ToLower() != "")
                        {
                            obj.lbl13 = dt.Rows[0]["proposeidentical_after28may_abnnumber"].ToString();
                        }
                    }
                    string sp4 = dt.Rows[0]["isultimateholdingcompany"].ToString();
                    //if (sp4.ToLower() == "yes") 
                    //{ obj.chkultimateyes = "Yes"; }
                    //else
                    //{ obj.chkultimateno = "Yes"; }

                    //ddlultimateholding.SelectedValue=dt.Rows[0]["isultimateholdingcompany"].ToString();

                    if (dt.Rows[0]["isultimateholdingcompany"].ToString().ToLower() == "yes")
                    {
                        obj.txtultimatecompanyname = dt.Rows[0]["ultimateholdingcompany_fulllegalname"].ToString();
                        obj.txtultimatecompanycountry = dt.Rows[0]["ultimateholdingcompany_country"].ToString();
                        //obj.txtultimateacnabn = dt.Rows[0]["ultimateholdingcompany_ACN_ARBN"].ToString();
                        //txtabnultimate.Text = dt.Rows[0]["ultimateholdingcompany_ABN"].ToString();

                        string acn_ = dt.Rows[0]["ultimateholdingcompany_ACN_ARBN"].ToString();
                        string abn_ = dt.Rows[0]["ultimateholdingcompany_ABN"].ToString();
                        obj.txtultimateacnabn = acn_ + abn_;
                    }

                    if(dt.Rows[0]["ulimateHoldingCompany"].ToString().ToLower() =="true")
                    {
                        obj.chkultimateyes = "Yes";
                        obj.chkultimateno = "No";
                        obj.txtultimatecompanyname = dt.Rows[0]["ucompanyname"].ToString();
                        obj.txtultimatecompanycountry = dt.Rows[0]["countryIcor"].ToString(); 
                        string acn_ = dt.Rows[0]["acnarbnabn"].ToString();
                        //string abn_ = dt.Rows[0]["ultimateholdingcompany_ABN"].ToString();
                        obj.txtultimateacnabn = acn_;
                    }
                    else
                    {
                        obj.chkultimateyes = "No";
                        obj.chkultimateno = "Yes";
                    }

                    string typeofcompany = dt.Rows[0]["typeofcompany"].ToString().ToLower();
                    if (typeofcompany != "")
                    {
                        string classofcompany = dt.Rows[0]["classofcompany"].ToString();
                        string specialpurpose_ifapplicable = dt.Rows[0]["specialpurpose_ifapplicable"].ToString();
                        if (typeofcompany == "proprietary company".ToLower() || typeofcompany == "proprietary (prop)")
                        {
                            obj.chk11 = "Yes";
                            if (classofcompany.ToLower() == "limited by shares")
                            {
                                obj.chk12 = "Yes";
                                //  obj.chk15 = "Yes";  //accoding to Rob, ths changes has been done. //changes Rollback 06042018
                            }
                            if (classofcompany.ToLower() == "unlimited with a share capital")
                            {
                                obj.chk13 = "Yes";
                            }

                            if (specialpurpose_ifapplicable == "home unit (HUNT)" || specialpurpose_ifapplicable.Contains("home unit "))
                            {
                                obj.chk14 = "Yes";
                            }
                            if (specialpurpose_ifapplicable == "superannuation trustee (PSTC)" || specialpurpose_ifapplicable.Contains("superannuation trustee"))
                            {
                                obj.chk15 = "Yes"; // hide //changes Rollback 06042018
                               // obj.chk25 = "Yes"; // hide -2-dec-2018
                            }
                            if (specialpurpose_ifapplicable == "charitable purposes only (PNPC)" || specialpurpose_ifapplicable.Contains("charitable purposes only"))
                            {
                                obj.chk16 = "Yes";
                            }
                            //if (specialpurpose_ifapplicable == "")
                            //{
                            //    obj.chk14 = "Yes";
                            //}

                        }
                        if (typeofcompany == "public company".ToLower())
                        {
                            obj.chk17 = "Yes";
                            if (classofcompany.ToLower() == "limited by shares")
                            {
                                obj.chk18 = "Yes";
                            }
                            if (classofcompany.ToLower() == "limited by guarantee")
                            {
                                obj.chk19 = "Yes";
                            }
                            if (classofcompany.ToLower() == "unlimited with a share capital")
                            {
                                obj.chk20 = "Yes";
                            }
                            if (classofcompany.ToLower() == "no liability")
                            {
                                obj.chk21 = "Yes";
                            }

                            if (specialpurpose_ifapplicable == "superannuation trustee (ULSS)" || specialpurpose_ifapplicable.Contains("superannuation trustee"))
                            {
                                obj.chk22 = "Yes";
                            }
                            if (specialpurpose_ifapplicable == "charitable purposes only (ULSN)" || specialpurpose_ifapplicable.Contains("charitable purposes only"))
                            {
                                obj.chk23 = "Yes";
                            }
                        }
                    }
                    string compacompanyusedfor = dt.Rows[0]["companyusedfor"].ToString();

                    // this option is added after meeting with Rob 04-06-2018
                    if (compacompanyusedfor != "")
                    {
                        if (compacompanyusedfor == "smsf")
                        {
                            obj.chk15 = "Yes";
                           // obj.chk25 = "Yes";
                            obj.chk24 = "Yes";
                        }
                        else
                        {
                            obj.chk15 = "No";
                            obj.chk24 = "No";
                        }
                    }

					//string cash = dt.Rows[0]["cash"].ToString().ToLower();
					//string writtencontact = dt.Rows[0]["writtencontact"].ToString().ToLower();
					//if (cash != "")
					//{
					//    if (cash == "yes")
					//    {
					//        obj.chk31 = "Yes";
					//        if ("yes" == writtencontact)
					//        {
					//            obj.chk33 = "Yes";
					//        }
					//        if ("no" == writtencontact)
					//        {
					//            obj.chk34 = "Yes";
					//        }
					//    }
					//    if (cash == "no")
					//    {
					//        obj.chk32 = "Yes";
					//    }
					//}

					DataTable dtanother = new DataTable();
					dtanother = op.get_step4_anothershareholder(hdncompanyid.Value);
					if (dtanother.Rows.Count > 0)
					{
						string shareoption = "";
						for (int i = 0; i <= dtanother.Rows.Count; i++)
						{
							shareoption = dtanother.Rows[0]["shareoption"].ToString().ToLower();
							if (shareoption.Trim() == "paid")
							{
								obj.chk31 = "Yes";
								obj.chk33 = "Yes";
								break;
							}
							else
							{
								obj.chk32 = "Yes";
								obj.chk34 = "Yes";
							}
						}
						
					}

					Form201Part1(obj);
                    Form201PartLast(obj);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void Form201Part1(pdfForm201 obj)
        {
            try
            {
                string defaultPath = Server.MapPath("DefaultDocuments\\FirstPart.pdf");
                string sourcePath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\FirstPart.pdf");
                string pdfTemplate = @"d://FORM201_MODIFIED.pdf";
                pdfTemplate = defaultPath;

                string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\FirstPart.pdf");
                string newFile = exportPath;
                PdfReader pdfReader = new PdfReader(pdfTemplate);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                AcroFields pdfFormFields = pdfStamper.AcroFields;
                pdfFormFields.SetField("Form 201", "");
                pdfFormFields.SetField("lbl1", obj.asic_registered_agentNo);
                pdfFormFields.SetField("lbl2", obj.firm);

                //string []Tel = (obj.telephone).Split(' ');

                pdfFormFields.SetField("lbl3", obj.contactname);
                pdfFormFields.SetField("lbl4", obj.telephone);
                // pdfFormFields.SetField("lbl4_1", Tel[0]);
                pdfFormFields.SetField("lbl5", obj.email);
                pdfFormFields.SetField("lbl6", obj.postaladdress);
                pdfFormFields.SetField("lbl7", obj.suburb);
                pdfFormFields.SetField("lbl8", obj.stateterritory);
                pdfFormFields.SetField("lbl9", obj.postcode);
                pdfFormFields.SetField("lbl10", obj.lbl10);
                pdfFormFields.SetField("chk1", obj.chk1);
                pdfFormFields.SetField("chk2", obj.chk2);

                // pdfFormFields.SetField("chktype1", obj.chktype1);
                if (obj.chktype1 == "Yes")
                {
                    //  pdfFormFields.SetField("chktype1", "Yes", true);
                    pdfFormFields.SetField("chktype1", "Yes"); // according to ROB, change 6 jun 2018
                }
                if (obj.chktype2 == "Yes")
                {
                    //pdfFormFields.SetField("chktype2", "Yes", true);
                    pdfFormFields.SetField("chktype2", "Yes");
                }

                if (obj.chktype3 == "Yes")
                {
                    // pdfFormFields.SetField("chktype3", "Yes", true);
                    pdfFormFields.SetField("chktype3", "Yes");
                }
                if (obj.chktype4 == "Yes")
                {
                    //pdfFormFields.SetField("chktype4", "Yes", true);
                    pdfFormFields.SetField("chktype4", "Yes");
                }
                if (obj.chktype5 == "Yes")
                {
                    //pdfFormFields.SetField("chktype5", "Yes", true);
                    pdfFormFields.SetField("chktype5", "Yes");
                }
                if (obj.chktype6 == "Yes")
                {
                    // pdfFormFields.SetField("chktype6", "Yes", true);
                    pdfFormFields.SetField("chktype6", "Yes");
                }
                if (obj.chktype7 == "Yes")
                {
                    // pdfFormFields.SetField("chktype7", "Yes", true);
                    pdfFormFields.SetField("chktype7", "Yes");
                }
                if (obj.chktype8 == "Yes")
                {
                    // pdfFormFields.SetField("chktype8", "Yes", true);
                    pdfFormFields.SetField("chktype8", "Yes");
                }
                if (obj.chktype9 == "Yes")
                {
                    // pdfFormFields.SetField("chktype9", "Yes", true);
                    pdfFormFields.SetField("chktype8", "Yes");
                }

                pdfFormFields.SetField("lbl11", obj.lbl11);
                pdfFormFields.SetField("lbl12", obj.lbl12);

                //pdfFormFields.SetField("chktype2", obj.chktype2);
                //pdfFormFields.SetField("chktype3", obj.chktype3);
                //pdfFormFields.SetField("chktype4", obj.chktype4);
                //pdfFormFields.SetField("chktype5", obj.chktype5);
                //pdfFormFields.SetField("chktype6", obj.chktype6);
                //pdfFormFields.SetField("chktype7", obj.chktype7);
                //pdfFormFields.SetField("chktype8", obj.chktype8);
                //pdfFormFields.SetField("chktype9", obj.chktype9);

                pdfFormFields.SetField("chk9", obj.chk9);
                pdfFormFields.SetField("chk10", obj.chk10);
                pdfFormFields.SetField("lbl13", obj.lbl13);

                pdfFormFields.SetField("lbl14", obj.lbl14);
                pdfFormFields.SetField("lbl15", obj.lbl15);
                pdfFormFields.SetField("lbl16", obj.lbl16);
                pdfFormFields.SetField("lbl17", obj.lbl17);
                pdfFormFields.SetField("lbl18", obj.lbl18);
                pdfFormFields.SetField("lbl19", obj.lbl19);
                pdfFormFields.SetField("lbl20", obj.lbl20);
                pdfFormFields.SetField("lbl21", obj.lbl21);

                pdfFormFields.SetField("chk11", obj.chk11);
                pdfFormFields.SetField("chk12", obj.chk12);
                pdfFormFields.SetField("chk13", obj.chk13);
                pdfFormFields.SetField("chk14", obj.chk14);
                pdfFormFields.SetField("chk15", obj.chk15);
                pdfFormFields.SetField("chk16", obj.chk16);

                pdfFormFields.SetField("chk17", obj.chk17);
                pdfFormFields.SetField("chk18", obj.chk18);
                pdfFormFields.SetField("chk19", obj.chk19);
                pdfFormFields.SetField("chk20", obj.chk20);
                pdfFormFields.SetField("chk21", obj.chk21);
                pdfFormFields.SetField("chk22", obj.chk22);
                pdfFormFields.SetField("chk23", obj.chk23);
                pdfFormFields.SetField("chk24", obj.chk24);
                pdfFormFields.SetField("chk25", obj.chk25);
                pdfFormFields.SetField("chk26", obj.chk26);

                pdfFormFields.SetField("lbl22", obj.Company_Address_contactperson);
                pdfFormFields.SetField("lbl23", obj.Company_Address_unit_level_suite);
                pdfFormFields.SetField("lbl24", obj.Company_Address_streetNoName);
                pdfFormFields.SetField("lbl25", obj.Company_Address_suburb_town_city);
                pdfFormFields.SetField("lbl26", obj.Company_Address_state);
                pdfFormFields.SetField("lbl27", obj.Company_Address_postcode);

                pdfFormFields.SetField("chk27", obj.chk27);
                pdfFormFields.SetField("chk28", obj.isoccupier);
                pdfFormFields.SetField("If no name of occupier", obj.occupiername);
                pdfFormFields.SetField("chk29", obj.chk29);
                pdfFormFields.SetField("chk30", obj.chk30);
                pdfFormFields.SetField("chk30_1", obj.chk30_1);
                //further details of company (Does the company occupy the premises?) before textbox16 and 17
                pdfFormFields.SetField("Text16", obj.Text16);
                pdfFormFields.SetField("Text17", obj.Text17);
                pdfFormFields.SetField("Company_Address_unit_level_suite_primary", obj.Company_Address_unit_level_suite_primary);
                pdfFormFields.SetField("Company_Address_streetNoName_primary", obj.Company_Address_streetNoName_primary);
                pdfFormFields.SetField("Company_Address_suburb_town_city_primary", obj.Company_Address_suburb_town_city_primary);
                pdfFormFields.SetField("Company_Address_state_primary", obj.Company_Address_state_primary);
                pdfFormFields.SetField("Company_Address_postcode_primary", obj.Company_Address_postcode_primary);
                pdfFormFields.SetField("Country_if_not_Australia", obj.Country_if_not_Australia);
                pdfFormFields.SetField("chkultimateyes", obj.chkultimateyes);
                pdfFormFields.SetField("txtultimatecompanyname", obj.txtultimatecompanyname);
                pdfFormFields.SetField("txtultimateacnabn", obj.txtultimateacnabn);
                pdfFormFields.SetField("txtultimatecompanycountry", obj.txtultimatecompanycountry);
                pdfFormFields.SetField("chkultimateno", obj.chkultimateno);

                pdfStamper.FormFlattening = false;

                // close the pdf
                pdfStamper.Close();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }

        private void Form201PartLast(pdfForm201 obj)
        {
            string defaultPath = Server.MapPath("DefaultDocuments\\LastPart.pdf");
            string sourcePath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\LastPart.pdf");
            string pdfTemplate = @"d://FORM201_MODIFIED.pdf";

            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\LastPart.pdf");
            string newFile = exportPath;
            pdfTemplate = defaultPath;
            PdfReader pdfReader = new PdfReader(pdfTemplate);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            #region FillMainShare

            DataTable dt_gr = op.get_share_allocation_MainPdf(hdncompanyid.Value.ToString());

            if (dt_gr.Rows.Count > 0)
            {
                for (int i = 0; i < dt_gr.Rows.Count; i++)
                {
                    string sharecode = dt_gr.Rows[i]["shareclass"].ToString();
                    if (sharecode.Trim() == "ordinary")
                    {
                        sharecode = "[ORD]";
                    }

                    if (sharecode.Trim() == "Redeemable Preference[REDP]")
                    {
                        sharecode = "[REDP]";
                    }

                    string totalshare = dt_gr.Rows[i]["c_totalshares"].ToString();
                    string paid = dt_gr.Rows[i]["c_totalamountpaidpershare"].ToString();
                    string unpaid = dt_gr.Rows[i]["c_totalamountunpaidpershare"].ToString();
                    string shareclass = dt_gr.Rows[i]["shareclass"].ToString();
                    pdfFormFields.SetField("Text_sco" + (i + 1), sharecode);
                    pdfFormFields.SetField("Text_head" + (i + 1), shareclass);
                    pdfFormFields.SetField("Text_totalshare" + (i + 1), totalshare);
                    pdfFormFields.SetField("Text_totalpaid" + (i + 1), paid);
                    pdfFormFields.SetField("Text_totalunpaid" + (i + 1), unpaid);
                    if (i == 11)
                        break;
                }
            }

            #endregion FillMainShare

            #region FillRA55 Registration Date

            DataTable dtdate = new DataTable();
            dtdate = op.getra55byID("239");
            if (dtdate.Rows.Count > 0)
            {
                string date = dtdate.Rows[0]["DateOfRegistration"].ToString();
                if (date.Trim() != "")
                {
                    if (date.Contains("/"))
                    {
                        string[] dat_ = date.Split('/');
                        obj.sm1 = dat_[0].PadLeft(2, '0').Substring(0, 1);
                        obj.sm2 = dat_[0].PadLeft(2, '0').Substring(1, 1);
                        obj.sd1 = dat_[1].PadLeft(2, '0').Substring(0, 1);
                        obj.sd2 = dat_[1].PadLeft(2, '0').Substring(1, 1);
                        obj.sy1 = dat_[2].Substring(2, 1);
                        obj.sy2 = dat_[2].Substring(3, 1);
                    }
                }
            }

            #endregion FillRA55 Registration Date

            pdfFormFields.SetField("chk31", obj.chk31);
            pdfFormFields.SetField("chk32", obj.chk32);
            pdfFormFields.SetField("chk33", obj.chk33);
            pdfFormFields.SetField("chk34", obj.chk34);
            pdfFormFields.SetField("chksignature1", obj.chksignature1);
            pdfFormFields.SetField("signaturenameofapplicant", " Comdeeds");
            pdfFormFields.SetField("chksignature2", obj.chksignature2);
            pdfFormFields.SetField("chksignature3", obj.chksignature3);
            pdfFormFields.SetField("Name_of_officeholder", obj.Name_of_officeholder);
            pdfFormFields.SetField("chksignature4", "Yes");
            pdfFormFields.SetField("Name of agent", "comdeeds ABN 85927028979");
            pdfFormFields.SetField("Signature_of_applicant_2", obj.Signature_of_applicant_2);
            pdfFormFields.SetField("sd1", obj.sd1);
            pdfFormFields.SetField("sd2", obj.sd2);
            pdfFormFields.SetField("sm1", obj.sm1);
            pdfFormFields.SetField("sm2", obj.sm2);
            pdfFormFields.SetField("sy1", obj.sy1);
            pdfFormFields.SetField("sy2", obj.sy2);

            pdfStamper.FormFlattening = false;

            // close the pdf
            pdfStamper.Close();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Call201();///create Base of Pdf with company details
            createHolderPdf();///generate pdf for all holder details
            createMemberPdf();///generate pdf for all member details
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\");
            string FirstPart = System.IO.Path.Combine(exportPath, "FirstPart.pdf");
            string HolderFinal = System.IO.Path.Combine(exportPath, "HolderFinal.pdf");
            string MemberFinal = System.IO.Path.Combine(exportPath, "MemberFinal.pdf");
            string LastPart = System.IO.Path.Combine(exportPath, "LastPart.pdf");
            string[] lstFiles = new string[5];
            lstFiles[0] = FirstPart;
            lstFiles[1] = HolderFinal;
            lstFiles[2] = MemberFinal;
            lstFiles[3] = LastPart;
            MergeAll(lstFiles);
            //dal.PdfMethodsPartitionPlus partiti = new PdfMethodsPartitionPlus();
            //string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\");
            //string sourceFile = System.IO.Path.Combine(exportPath, "BasicPart.pdf");
            //string destPart1 = System.IO.Path.Combine(exportPath, "Part1.pdf");
            //string destPart2 = System.IO.Path.Combine(exportPath, "Part2.pdf");
            //partiti.removePagesFromPdf(sourceFile, destPart1, 1, 2, 3);
            //partiti.removePagesFromPdf(sourceFile, destPart2, 4, 5, 6, 7);

            //string destHolder = System.IO.Path.Combine(exportPath, "HolderFinal.pdf");
            //string destMember = System.IO.Path.Combine(exportPath, "MemberFinal.pdf");
            //string[] lstFiles = new string[5];
            //lstFiles[0] = destPart1;
            //lstFiles[1] = destHolder;
            //lstFiles[2] = destMember;
            //lstFiles[3] = destPart2;
            //MergeAll(lstFiles);
            //dal.PdfMethodsPartitionPlus partiti = new PdfMethodsPartitionPlus();

            //string sourceFile = System.IO.Path.Combine(exportPath, "LastPart.pdf");

            //string destPart1 = System.IO.Path.Combine(exportPath, "Part1.pdf");
            //string destPart2 = System.IO.Path.Combine(exportPath, "Part2.pdf");
            //partiti.removePagesFromPdf(sourceFile, destPart1, 1, 2, 3);
            //partiti.removePagesFromPdf(sourceFile, destPart2, 4, 5, 6, 7);

            //string destHolder = System.IO.Path.Combine(exportPath, "HolderFinal.pdf");
            //string destMember = System.IO.Path.Combine(exportPath, "MemberFinal.pdf");
        }
    }
}