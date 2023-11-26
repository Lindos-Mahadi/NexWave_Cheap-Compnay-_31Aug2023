using comdeeds.App_Code;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static comdeeds.Models.BaseModel;

namespace comdeeds.Areas.User.Controllers
{
    public class OrderController : comdeeds.Controllers.BaseController
    {
        // GET: User/Order
        public ActionResult trust()
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            return View();
        }

        #region Trust PDF Download

        public ActionResult downloadtrust(int id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            var trustData = new ClassFullTrust();
            if (id > 0)
            {
                trustData = TrustMethods.GetFullTrustDetails(id);
            }
            if (trustData.TransactionDetail.TransactionStatus == true)
            {
                string htmlPath = string.Empty, body = "";
                if (trustData.trust != null)
                {
                    switch (trustData.trust.TrustType.ToLower())
                    {
                        case "bare trust":
                            htmlPath = Server.MapPath("~/Content/deedhtml/property_bare_trust.html");
                            break;

                        case "super fund trust":
                            htmlPath = Server.MapPath("~/Content/deedhtml/sftd.html");
                            break;
                    }
                    using (StreamReader red = new StreamReader(htmlPath))
                    {
                        body = red.ReadToEnd();
                    }

                    var t = trustData.trust;
                    var a = trustData.appointers;
                    var capitName = new CultureInfo("en-US").TextInfo.ToTitleCase(t.TrustName);
                    switch (trustData.trust.TrustType.ToLower())
                    {
                        case "bare trust":

                            body = body.Replace("{trustdate}", t.TrustSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            body = body.Replace("{trustname}", t.TrustName.ToUpper());
                            body = body.Replace("{trustnamelower}", capitName);
                            body = body.Replace("{propertyaddress}", t.PropertyAddress);
                            body = body.Replace("{companywithacn}", t.PropertyTrusteeName + ", ACN: " + t.PropertyTrusteeAcn);
                            body = body.Replace("{smsfcompanyname}", t.SmsfCompanyName);
                            body = body.Replace("{smsfacn}", t.Smsfacn);
                            body = body.Replace("{smsf}", t.Smsf);
                            body = body.Replace("{abn}", t.Abn);
                            body = body.Replace("{lendername}", t.LenderName);
                            body = body.Replace("{state}", t.TrustState);
                            body = body.Replace("{propertytrustee}", t.PropertyTrusteeName);
                            body = body.Replace("{acn}", t.PropertyTrusteeAcn);
                            string td = "";
                            switch (a.Where(x => x.HolderType == "member").Count())
                            {
                                case 1:
                                    td = $"<tr><td><span class='c16'>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 1)}</b></span></td></tr>";
                                    break;

                                case 2:
                                    td = $"<tr><td><span class='c16'>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 1)}</b></span></td>" +
                                        $"<td><span class='c16'>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 2)}</b></span></td></tr>";
                                    break;

                                case 3:
                                    td = $"<tr><td ><span class='c16'>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 1)}</b></span></td>" +
                                        $"<td><span class='c16'>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 2)}</b></span></td></tr>" +
                                        $"<tr><td><span class='c16'><br/><br/>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 3)}</b></span></td></tr>";
                                    break;

                                case 4:
                                    td = $"<tr><td><span class='c16'>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 1)}</b></span></td>" +
                                        $"<td><span class='c16'>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 2)}</b></span></td></tr>" +
                                        $"<tr><td><span class='c16'><br/><br/>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 3)}</b></span></td>" +
                                        $"<td><span class='c16'><br/><br/>______________________________<br/><b style='text-align: left;margin-top: 5px;display: inline-block;'>{GetMemberName(a, 4)}</b></span></td></tr>";
                                    break;
                            }
                            body = body.Replace("{memberslist}", td);
                            break;

                        case "super fund trust":

                            var firstMember = a.OrderBy(x => x.Id).FirstOrDefault();

                            string memberdetails = string.Empty;
                            string memberdetailsfooter = string.Empty;

                            foreach (var m in a.OrderBy(x => x.Id))
                            {
                                memberdetails += string.Format("<p>{0} <br/> {1} <br/> Date of Birth: {2} </p>",
                                    m.FirstName + " " + m.LastName,
                                    m.UnitLevel + " " + m.Street + " " + m.Suburb + " " + m.State + " " + m.PostCode + " " + m.Country,
                                    m.Dob.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                                    );

                                memberdetailsfooter += string.Format(@"<br/><br/><br/><p>{0}– Director/Member
            <br />in the presence of:
            </p>
            <table border='0' cellpadding='0'>
                <tr>
                    <td style='border:0'>Witness signature</td>
                    <td style='border:0' width='350'><br /><hr /></td>
                </tr>
                <tr>
                    <td style='border:0'>Name of witness</td>
                    <td style='border:0' width='350'><br /><hr /></td>
                </tr>
                <tr>
                    <td style='border:0'>Address</td>
                    <td style='border:0' width='350'><br /><hr /></td>
                </tr>
                <tr>
                    <td style='border:0'></td>
                    <td style='border:0' width='350'><br /><hr /></td>
                </tr>
                <tr>
                    <td style='border:0'></td>
                    <td style='border:0' width='350'><br /><hr /></td>
                </tr>
                <tr>
                    <td style='border:0'>Occupation</td>
                    <td style='border:0' width='350'><br /><hr /></td>
                </tr>

            </table>", m.FirstName + "  " + m.LastName);
                            }
                            body = body.Replace("{trustname}", capitName);
                            body = body.Replace("{trustdate}", t.TrustSetupDate.Value.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            body = body.Replace("{trustcompanyname}", t.SmsfCompanyName);
                            body = body.Replace("{trustcompanyacn}", t.Smsfacn);
                            body = body.Replace("{member1add}", firstMember.UnitLevel + " " + firstMember.Street + " " + firstMember.Suburb + " " + firstMember.State + " " + firstMember.PostCode + " " + firstMember.Country);
                            body = body.Replace("{memberdetails}", memberdetails);
                            body = body.Replace("{memberdetailsfooter}", memberdetailsfooter);
                            break;
                    }

                    var f = trustData.trust.TrustName.Replace(" ", "-");
                    createpdf(body, f + "-trust-deed.pdf");
                }
            }
            return View();
        }


        public ActionResult downloadsmsf(int id)
        {
            ErrorLog oe = new ErrorLog();
            try
            {
                var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/admin/signin"));
                var trustData = new ClassFullTrust();
                if (id > 0)
                {
                    trustData = TrustMethods.GetFullTrustDetails(id);
                }
                if (trustData.TransactionDetail.TransactionStatus == true)
                {
                    string defaultPath = string.Empty; string filenamee = string.Empty;
                    if (trustData.trust.ToString().ToLower() != null)
                    {
                        switch (trustData.trust.TrustType.ToLower())
                        {
                            case "bare trust":
                                defaultPath = Server.MapPath("~/Content/deedhtml/Property_Trust_Deed_.pdf");
                                filenamee = "-Property-Trust-Deed";
                                break;

                            case "super fund trust":
                                defaultPath = Server.MapPath("~/Content/deedhtml/SuperFundTrustDeed_Final_New.pdf");
                                filenamee = "-SMSF-DEED";
                                break;

                            default:
                                defaultPath = Server.MapPath("~/Content/deedhtml/Super_Fund_Trust_Deed_Update_Deed_N.pdf");
                                filenamee = "-SMSF-UPDATE-DEED";
                                break;
                        }

                        if (filenamee == "Property-Trust-Deed")
                        {
                            var t = trustData.trust;
                            var a = trustData.appointers;
                            var capitName = new CultureInfo("en-US").TextInfo.ToTitleCase(t.TrustName);
                            var membs = string.Empty;
                            var tbl = string.Empty;
                            var memberswithaddress = string.Empty;
                            var m = a.OrderBy(x => x.Id).FirstOrDefault();
                            foreach (var mr in a.Where(x => x.HolderType == "member").OrderBy(x => x.Id))
                            {
                                membs += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + ", ";
                                memberswithaddress += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + " of " + mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";
                            }

                            CreateCustomerFolder(id);
                            string pdfTemplate = ""; pdfTemplate = defaultPath;
                            string exportPath = Server.MapPath("../../../ExportedFiles\\" + id + "\\Property_Trust_Deed_.pdf");
                            string newFile = exportPath;
                            memberswithaddress = memberswithaddress.TrimEnd(',');
                            membs = membs.TrimEnd(',');
                            string[] membs1 = membs.Split(',');

                            PdfReader pdfReader = new PdfReader(pdfTemplate);
                            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                            AcroFields pdfFormFields = pdfStamper.AcroFields;
                            pdfFormFields.SetField("txtDeedDate", "( " + t.TrustSetupDate.Value.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture) + " )");
                            pdfFormFields.SetField("txtProName", t.SmsfCompanyName);
                            pdfFormFields.SetField("txtPropAdd", t.PropertyAddress);
                            pdfFormFields.SetField("txtProLagalName", t.PropertyTrusteeName + ", ACN:" + t.PropertyTrusteeAcn);
                            pdfFormFields.SetField("txtSMSFBene", t.SmsfCompanyName + ", ACN:" + t.Smsfacn + " ATF " + t.SmsfCompanyName);
                            pdfFormFields.SetField("txtABN_S", "ABN : " + (t.Abn == null ? "" : t.Abn));
                            pdfFormFields.SetField("txtState", "(NSW)");
                            pdfFormFields.SetField("txtProtrustee1", t.PropertyTrusteeName);
                            pdfFormFields.SetField("txtProtrustee", t.PropertyTrusteeName + "(ACN:" + t.PropertyTrusteeAcn + ")");

                            for (int i = 1; i <= membs1.Length; i++)
                            {
                                pdfFormFields.SetField("txtDirSign" + i, membs1[i - 1]);
                                pdfFormFields.SetField("txtDirSign" + i + "1", membs1[i - 1]);
                            }

                            pdfFormFields.SetField("txtSMSFtrustee", t.PropertyTrusteeName);
                            pdfFormFields.SetField("txtSMSFtrustee1", t.PropertyTrusteeName + "(ACN: " + t.PropertyTrusteeAcn + ")");

                            pdfFormFields.SetField("txtDate_S", t.TrustSetupDate.Value.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            pdfFormFields.SetField("txtPropryadd", t.PropertyAddress);
                            pdfFormFields.SetField("txtLegalOwner", t.PropertyTrusteeName + "(ACN:" + t.PropertyTrusteeAcn + ")");
                            pdfFormFields.SetField("txtLegalOwner1", t.PropertyTrusteeName);
                            pdfFormFields.SetField("txtBeneficiary", t.PropertyTrusteeName + "(ACN:" + t.PropertyTrusteeAcn + ")");
                            pdfFormFields.SetField("txtBeneficiary1", t.PropertyTrusteeName);
                            pdfStamper.FormFlattening = false;
                            pdfStamper.Close();
                            var f = trustData.trust.TrustName.Replace(" ", "-");
                            createpdfNew(exportPath, f + "property-trust-deed");
                        }
                        else if (filenamee == "-SMSF-DEED")
                        {
                            var t = trustData.trust;
                            var a = trustData.appointers;
                            var capitName = new CultureInfo("en-US").TextInfo.ToTitleCase(t.TrustName);
                            var membs = string.Empty; var tbl = string.Empty; var memberswithaddress = string.Empty; var DOB = string.Empty;

                            var m = a.OrderBy(x => x.Id).FirstOrDefault();
                            foreach (var mr in a.Where(x => x.HolderType == "member").OrderBy(x => x.Id))
                            {
                                membs += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + ", ";
                                memberswithaddress += mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";
                                // DOB += mr.Dob + ", ";
                            }

                            CreateCustomerFolder(id);
                            string pdfTemplate = "";
                            pdfTemplate = defaultPath;
                            string exportPath = Server.MapPath("../../../ExportedFiles\\" + id + "\\SMSFNew.pdf");
                            string newFile = exportPath;
                            string[] membsNew = membs.Trim().TrimEnd(',').Split(',');
                            string[] memberswithaddressNew = memberswithaddress.Split(',');
                            string[] DOBNew = DOB.Trim().TrimEnd(',').Split(',');

                            PdfReader pdfReader = new PdfReader(pdfTemplate);
                            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                            AcroFields pdfFormFields = pdfStamper.AcroFields;
                            pdfFormFields.SetField("txtFundName", capitName);
                            pdfFormFields.SetField("dofEstlish", t.SmsfTrusteeSetupDate.Value.ToString("dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            pdfFormFields.SetField("txtSMSFwopty3", capitName);
                            pdfFormFields.SetField("txtSMSFwopty4", capitName);
                            pdfFormFields.SetField("txtSMSFwopty5", capitName);
                            if (t.PropertyAddress == string.Empty || t.PropertyAddress == null)
                            { pdfFormFields.SetField("txtAddSMSF", memberswithaddressNew[0]); }
                            else { pdfFormFields.SetField("txtAddSMSF", t.PropertyAddress); }

                            pdfFormFields.SetField("txtSMSF3", t.SmsfCompanyName);
                            pdfFormFields.SetField("txtACNN", "A.C.N: " + t.Smsfacn);

                            for (int i = 1; i <= membsNew.Length; i++)
                            {
                                pdfFormFields.SetField("txtMemm," + i, membsNew[i - 1]);
                                pdfFormFields.SetField("txtAddMem" + i, "Address : " + memberswithaddressNew[i - 1]);
                                pdfFormFields.SetField("txtSMSFName" + i, t.SmsfCompanyName);
                                pdfFormFields.SetField("txtACN" + i, "A.C.N: " + t.Smsfacn);
                                pdfFormFields.SetField("txtSMSF_wopty" + i, "ATF " + capitName);
                                pdfFormFields.SetField("txtDirMem" + i, membsNew[0] + " - Director/Member");
                            }

                            int n = 4;//fill members details
                            for (int i = 1; i <= membsNew.Length; i++)
                            {
                                pdfFormFields.SetField("txtSMSFName_" + i, capitName.ToUpper());
                                pdfFormFields.SetField("txtmem" + i, "I, " + membsNew[i - 1] + " hereby apply to become a member of the " + capitName.ToUpper() + " and submit the following details for the benefit of the Trustee:");
                                pdfFormFields.SetField("txtFName_" + i, membsNew[i - 1]);
                                pdfFormFields.SetField("txtAddMem_" + i, memberswithaddressNew[i - 1]);
                                pdfFormFields.SetField("txtSignMem_" + i, membsNew[i - 1]);
                            }

                            int memcount = n - membsNew.Length;
                            for (int i = (membsNew.Length) + 1; i <= n; i++)
                            {
                                pdfFormFields.SetField("txtmem" + i, "I, ............. hereby apply to become a member of the ................... and submit the following details for the benefit of the Trustee:");
                            }

                            // meeting chair person details
                            pdfFormFields.SetField("txtSMSFName_M", capitName);
                            pdfFormFields.SetField("txtDate_M", t.SmsfTrusteeSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            pdfFormFields.SetField("txtPlace", memberswithaddressNew[0]);
                            pdfFormFields.SetField("txtChairby", membsNew[0]);
                            pdfFormFields.SetField("txtAttendees", membs.Trim().Trim(','));

                            pdfStamper.FormFlattening = false;
                            pdfStamper.Close();
                            var f = trustData.trust.TrustName.Replace(" ", "-").ToUpper();
                            createpdfNew(exportPath, f + filenamee);
                        }
                        else
                        {
                            var t = trustData.trust;
                            var a = trustData.appointers;
                            var capitName = new CultureInfo("en-US").TextInfo.ToTitleCase(t.TrustName);

                            var membs = string.Empty; var tbl = string.Empty; var memberswithaddress = string.Empty;

                            var m = a.OrderBy(x => x.Id).FirstOrDefault();
                            foreach (var mr in a.Where(x => x.HolderType == "member").OrderBy(x => x.Id))
                            {
                                membs += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + ", ";
                                memberswithaddress += mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";
                            }

                            CreateCustomerFolder(id);
                            string pdfTemplate = "";
                            pdfTemplate = defaultPath;
                            string exportPath = Server.MapPath("../../../ExportedFiles\\" + id + "\\SMSFNew.pdf");
                            string newFile = exportPath;
                            string[] membsNew = membs.Trim().TrimEnd(',').Split(',');
                            string[] memberswithaddressNew = memberswithaddress.Split(',');

                            PdfReader pdfReader = new PdfReader(pdfTemplate);
                            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                            AcroFields pdfFormFields = pdfStamper.AcroFields;
                            pdfFormFields.SetField("txtFundName", capitName);
                            pdfFormFields.SetField("dofEstlish", t.SmsfTrusteeSetupDate.Value.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            pdfFormFields.SetField("txtSMSFwopty3", capitName + " of " + memberswithaddressNew[0]);
                            pdfFormFields.SetField("txtSMSFwopty4", capitName);
                            pdfFormFields.SetField("txtSMSFwopty5", capitName);
                            //pdfFormFields.SetField("txtAddSMSF", memberswithaddressNew[0]);
                            pdfFormFields.SetField("txtSMSF3", t.SmsfCompanyName + " (A.C.N: " + t.Smsfacn + ")");
                            // pdfFormFields.SetField("txtACN3", "A.C.N: " + t.Smsfacn);
                            pdfFormFields.SetField("txtSMSFName", capitName);
                            pdfFormFields.SetField("txtABN", "ABN" + (t.Abn == null ? "" : t.Abn));
                            pdfFormFields.SetField("txtSMSFName1", capitName);
                            pdfFormFields.SetField("txtPlace", memberswithaddressNew[0]);
                            pdfFormFields.SetField("txtPresent", membs.Trim().Trim(','));
                            pdfFormFields.SetField("txtChair", membsNew[0] + " elected Chair of the meeting");
                            pdfFormFields.SetField("txtDate", t.SmsfTrusteeSetupDate.Value.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            pdfFormFields.SetField("txtTDate", t.SmsfTrusteeSetupDate.Value.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture));

                            for (int i = 1; i <= membsNew.Length; i++)
                            {
                                pdfFormFields.SetField("txtAddMem" + i, membsNew[i - 1] + " Of " + memberswithaddressNew[i - 1]);
                                // pdfFormFields.SetField("txtAddMem" + i, memberswithaddressNew[i - 1]);
                                pdfFormFields.SetField("txtMem1" + i, membsNew[i - 1] + " - Member " + i + "  " + memberswithaddressNew[i - 1]);
                            }
                            pdfStamper.FormFlattening = false;
                            pdfStamper.Close();
                            var f = trustData.trust.TrustName.Replace(" ", "-").ToUpper();
                            createpdfNew(exportPath, f + filenamee);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oe.WriteErrorLog("downloadconstitution_LineNo_931_" + ex.ToString());
            }
            return View();
        }


        public ActionResult downloadtrustminuteNew(int id)
        {
            ErrorLog oe = new ErrorLog();
            try
            {
                var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/admin/signin"));
                var trustData = new ClassFullTrust();
                if (id > 0)
                {
                    trustData = TrustMethods.GetFullTrustDetails(id);
                }
                if (trustData.TransactionDetail.TransactionStatus == true)
                {
                    string defaultPath = string.Empty, body = ""; string htmlPath = string.Empty;
                    if (trustData.trust != null)
                    {
                        switch (trustData.trust.TrustType.ToLower())
                        {
                            case "bare trust":
                                defaultPath = Server.MapPath("~/Content/deedhtml/Property_Trust_Minute.pdf");
                                break;
                            case "super fund trust":
                                defaultPath = Server.MapPath("~/Content/deedhtml/Supplementary_Super_Fund_Documents_N.pdf");// superfund_minute.html");
                                break;
                            case "super fund trust - update":
                                defaultPath = Server.MapPath("~/Content/deedhtml/SMSF_DEED_OF_VARIATION_N_N.pdf");
                                break;
                            default:
                                defaultPath = Server.MapPath("~/Content/deedhtml/SMSF_DEED_OF_VARIATION_N_N.pdf");
                                break;
                        }

                        switch (trustData.trust.TrustType.ToLower())
                        {
                            #region bare trust

                            case "bare trust":

                                var t = trustData.trust;
                                var a = trustData.appointers;
                                var capitName = new CultureInfo("en-US").TextInfo.ToTitleCase(t.TrustName);
                                var membs = string.Empty;
                                var tbl = string.Empty;
                                var memberswithaddress = string.Empty;
                                var onlyaddress = string.Empty;
                                var m = a.OrderBy(x => x.Id).FirstOrDefault();
                                foreach (var mr in a.Where(x => x.HolderType == "member").OrderBy(x => x.Id))
                                {
                                    membs += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + ", ";
                                    memberswithaddress += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + " of " + mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";
                                    onlyaddress += mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";
                                }

                                CreateCustomerFolder(id);
                                string pdfTemplate = ""; pdfTemplate = defaultPath;
                                string exportPath = Server.MapPath("../../../ExportedFiles\\" + id + "\\Property_Trust_Minute.pdf");
                                string newFile = exportPath;
                                memberswithaddress = memberswithaddress.TrimEnd(',');
                                // memberswithaddress = memberswithaddress.Replace(","," and ");
                                string[] memberswithaddressNew = memberswithaddress.Split(',');
                                string[] onlyaddressNew = onlyaddress.Split(',');

                                membs = membs.TrimEnd(',');
                                string[] membs1 = membs.Split(',');

                                PdfReader pdfReader = new PdfReader(pdfTemplate);
                                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                                AcroFields pdfFormFields = pdfStamper.AcroFields;

                                pdfFormFields.SetField("txtPty", t.SmsfCompanyName);
                                pdfFormFields.SetField("txtACN", "A.C.N." + t.Smsfacn);
                                pdfFormFields.SetField("txtSMSFName", t.Smsf);

                                pdfFormFields.SetField("txtMeetaddrs", m.UnitLevel + " " + m.Street + " " + m.Suburb + " " + m.State + " " + m.PostCode);
                                pdfFormFields.SetField("txtMeeDate", t.TrustSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                pdfFormFields.SetField("txtMeetPerson", membs);
                                pdfFormFields.SetField("txtProaddress", t.PropertyAddress);
                                pdfFormFields.SetField("txtpropty", t.PropertyTrusteeName);
                                pdfFormFields.SetField("txtProptrust", t.TrustName);
                                pdfFormFields.SetField("txtSMSFName1", t.Smsf);
                                pdfFormFields.SetField("txtSMSFName3", t.Smsf);

                                pdfFormFields.SetField("txtDDDate", t.TrustSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                //fill memberlist
                                for (int i = 1; i <= membs1.Length; i++)
                                { pdfFormFields.SetField("txtsmem" + i, membs1[i - 1]); }

                                pdfFormFields.SetField("txtFadd1", memberswithaddressNew[0], true);
                                if (membs1.Length == 2)
                                {
                                    pdfFormFields.SetField("txtFadd2", membs1[1]);
                                    pdfFormFields.SetField("txtFadd12", onlyaddressNew[1]);
                                }

                                pdfFormFields.SetField("txtPropacn", t.PropertyTrusteeName + " ACN " + t.PropertyTrusteeAcn);
                                pdfFormFields.SetField("txtDDate", t.PropertyTrusteeSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                pdfFormFields.SetField("txtPropacn1", t.PropertyTrusteeName + " ACN " + t.PropertyTrusteeAcn);
                                pdfFormFields.SetField("txtUadd", t.PropertyAddress);
                                pdfFormFields.SetField("txtDDate1", t.PropertyTrusteeSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                pdfFormFields.SetField("txtPAcn", "A.C.N: " + t.Smsfacn);
                                pdfFormFields.SetField("txtPPty", t.SmsfCompanyName);
                                pdfFormFields.SetField("txtSDatee", t.PropertyTrusteeSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                pdfFormFields.SetField("txtProptrust1", t.TrustName);
                                pdfFormFields.SetField("txtProptrust2", t.TrustName);

                                pdfStamper.FormFlattening = false;
                                pdfStamper.Close();
                                var f = trustData.trust.TrustName.Replace(" ", "-");
                                createpdfNew(exportPath, f + "-property-trust-minute");

                                break;

                            #endregion bare trust

                            #region super fund trust

                            case "super fund trust":

                                var t1 = trustData.trust;
                                var a1 = trustData.appointers;
                                var capitName1 = new CultureInfo("en-US").TextInfo.ToTitleCase(t1.TrustName);

                                var membs2 = string.Empty;
                                var tbl1 = string.Empty;
                                var memberswithaddress1 = string.Empty;
                                var memDOB = string.Empty;

                                var m1 = a1.OrderBy(x => x.Id).FirstOrDefault();
                                foreach (var mr in a1.Where(x => x.HolderType == "member").OrderBy(x => x.Id))
                                {
                                    membs2 += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + ", ";
                                    memberswithaddress1 += mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";
                                    memDOB += mr.Dob.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) + ",";
                                }

                                CreateCustomerFolder(id);
                                string pdfTemplate1 = "";
                                pdfTemplate1 = defaultPath;
                                string exportPath1 = Server.MapPath("../../../ExportedFiles\\" + id + "\\Supplementary_Super_Fund_Documents_New.pdf");
                                string newFile1 = exportPath1;
                                string[] membsNew = membs2.Trim().TrimEnd(',').Split(',');
                                string[] memberswithaddressNew1 = memberswithaddress1.Split(',');
                                string[] memDOBNew = memDOB.Split(',');

                                PdfReader pdfReader1 = new PdfReader(pdfTemplate1);
                                PdfStamper pdfStamper1 = new PdfStamper(pdfReader1, new FileStream(newFile1, FileMode.OpenOrCreate));
                                AcroFields pdfFormFields1 = pdfStamper1.AcroFields;
                                pdfFormFields1.SetField("txtFundName", capitName1.ToUpper().Trim());
                                pdfFormFields1.SetField("txtFundNamee", capitName1);

                                int n = 4;//fill members details
                                for (int i = 1; i <= membsNew.Length; i++)
                                {
                                    pdfFormFields1.SetField("txtSMSFName_" + i, capitName1);
                                    pdfFormFields1.SetField("txtmem" + i, "I, " + membsNew[i - 1] + " hereby apply to become a member of the " + capitName1.ToUpper() + " and submit the following details for the benefit of the Trustee:");
                                    pdfFormFields1.SetField("txtFName_" + i, membsNew[i - 1]);
                                    pdfFormFields1.SetField("txtAddMem_" + i, memberswithaddressNew1[i - 1]);
                                    pdfFormFields1.SetField("txtSignMem_" + i, membsNew[i - 1]);
                                }

                                int memcount = n - membsNew.Length;
                                for (int i = (membsNew.Length) + 1; i <= n; i++)
                                {
                                    pdfFormFields1.SetField("txtmem" + i, "I, ............. hereby apply to become a member of the ................... and submit the following details for the benefit of the Trustee:");
                                }

                                // meeting chair person details
                                pdfFormFields1.SetField("txtSMSFName_M", capitName1);
                                pdfFormFields1.SetField("txtDate_M", t1.SmsfTrusteeSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                pdfFormFields1.SetField("txtPlace", memberswithaddressNew1[0]);
                                pdfFormFields1.SetField("txtChairby", membsNew[0]);
                                pdfFormFields1.SetField("txtAttendees", membs2.Trim().Trim(','));
                                pdfStamper1.FormFlattening = false;
                                pdfStamper1.Close();
                                var f1 = trustData.trust.TrustName.Replace(" ", "-").ToUpper();
                                createpdfNew(exportPath1, f1 + "SMSF-SUPPLEMENTARY");
                                break;

                            #endregion super fund trust

                            #region super fund trust - update

                            case "super fund trust - update":

                                //defaultPath = Server.MapPath("~/Content/deedhtml/SMSF_DEED_OF_VARIATION_N.pdf");
                                var t2 = trustData.trust;
                                var a2 = trustData.appointers;
                                var capitName2 = new CultureInfo("en-US").TextInfo.ToTitleCase(t2.TrustName);
                                var membs3 = string.Empty;
                                var tbl3 = string.Empty;
                                var memberswithaddress2 = string.Empty;

                                var m2 = a2.OrderBy(x => x.Id).FirstOrDefault();
                                foreach (var mr in a2.Where(x => x.HolderType == "member").OrderBy(x => x.Id))
                                {
                                    membs3 += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + ", ";
                                    //  memberswithaddress += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + " of " + mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";
                                    memberswithaddress2 += mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";
                                }

                                CreateCustomerFolder(id);
                                string pdfTemplate2 = "";
                                pdfTemplate = defaultPath;
                                string exportPath2 = Server.MapPath("../../../ExportedFiles\\" + id + "\\SMSF_DEED_OF_VARIATION__New.pdf");
                                string newFile2 = exportPath2;
                                string[] membsNew3 = membs3.Trim().TrimEnd(',').Split(',');
                                string[] memberswithaddressNew2 = memberswithaddress2.Split(',');

                                PdfReader pdfReader2 = new PdfReader(pdfTemplate);
                                PdfStamper pdfStamper2 = new PdfStamper(pdfReader2, new FileStream(newFile2, FileMode.OpenOrCreate));
                                AcroFields pdfFormFields3 = pdfStamper2.AcroFields;

                                pdfFormFields3.SetField("txtTrustName", capitName2.ToUpper().Trim());
                                pdfFormFields3.SetField("txtTrustee", t2.SmsfCompanyName + " ( ACN : " + t2.Smsfacn + ") of " + memberswithaddressNew2[0] + "(Trustees)");
                                //  pdfFormFields3.SetField("txtRegOfc", memberswithaddressNew2[0]);
                                // pdfFormFields3.SetField("txtMemO", membs3.Trim().TrimEnd(',').Replace(",", System.Environment.NewLine));
                                pdfFormFields3.SetField("txtRegOfc1", memberswithaddressNew2[0]);
                                pdfFormFields3.SetField("txtTrustWithAdd", capitName2 + "  of " + memberswithaddressNew2[0] + "  (\"Trustee\") ");
                                pdfFormFields3.SetField("txtMemWithAdd", membs3.Trim().TrimEnd(',') + " of " + memberswithaddressNew2[0] + " (\"Member\") ");
                                pdfFormFields3.SetField("txtTrustName1", capitName2);
                                pdfFormFields3.SetField("txtEst", t2.SmsfTrusteeSetupDate.Value.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                //if (!(t2.ExistingSetupDate == null))
                                //    pdfFormFields3.SetField("txtSupdate", t2.ExistingSetupDate.Value.ToShortDateString());
                                if (!(t2.ClauseNumber == null))
                                {
                                    pdfFormFields3.SetField("txtCauseNo", "{ " + t2.ClauseNumber + " }");
                                    pdfFormFields3.SetField("txtCauseNo1", "{ " + t2.ClauseNumber + " }");
                                }

                                for (int i = 1; i <= membsNew3.Length; i++)
                                {
                                    pdfFormFields3.SetField("txtmem" + i, membsNew3[i - 1]);
                                    pdfFormFields3.SetField("txtAddMem" + i, membsNew3[i - 1] + " of " + memberswithaddressNew2[i - 1]);
                                    pdfFormFields3.SetField("txtmem" + i + "_1", membsNew3[i - 1]);
                                    pdfFormFields3.SetField("txtTrustCom" + i, t2.SmsfCompanyName);
                                }
                                pdfFormFields3.SetField("txtDate", t2.SmsfTrusteeSetupDate.Value.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                // pdfFormFields3.SetField("txtDate1", t2.SmsfTrusteeSetupDate.Value.ToString("dd/MMM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                pdfStamper2.FormFlattening = false;
                                pdfStamper2.Close();
                                var f2 = trustData.trust.TrustName.Replace(" ", "-").ToUpper();
                                createpdfNew(exportPath2, f2 + "SMSF-DEED-OF-VARIATION");

                                break;

                                #endregion super fund trust - update
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oe.WriteErrorLog("DownloadControoler_LineNo_383_" + ex.ToString());
            }

            return View();
        }



        private void CreateCustomerFolder(int id)
        {
            ErrorLog oe = new ErrorLog();
            try
            {
                string directoryPath = Server.MapPath("../../../ExportedFiles\\" + id);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            catch (Exception ex)
            {
                oe.WriteErrorLog(ex.ToString());
            }
        }


        public void createpdfNew(string path, string filename)
        {
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");
            Response.TransmitFile(path);
            Response.End();
        }

        public ActionResult downloadtrustminute(int id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            var trustData = new ClassFullTrust();
            if (id > 0)
            {
                trustData = TrustMethods.GetFullTrustDetails(id);
            }
            if (trustData.TransactionDetail.TransactionStatus == true)
            {
                string htmlPath = string.Empty, body = "";
                if (trustData.trust != null)
                {
                    switch (trustData.trust.TrustType.ToLower())
                    {
                        case "bare trust":
                            htmlPath = Server.MapPath("~/Content/deedhtml/property_minute.html");
                            break;

                        case "super fund trust":
                            htmlPath = Server.MapPath("~/Content/deedhtml/superfund_minute.html");
                            break;
                    }
                    using (StreamReader red = new StreamReader(htmlPath))
                    {
                        body = red.ReadToEnd();
                    }

                    var t = trustData.trust;
                    var a = trustData.appointers;
                    var capitName = new CultureInfo("en-US").TextInfo.ToTitleCase(t.TrustName);
                    switch (trustData.trust.TrustType.ToLower())
                    {
                        case "bare trust":
                            var membs = string.Empty;
                            var tbl = string.Empty;
                            var memberswithaddress = string.Empty;
                            var m = a.OrderBy(x => x.Id).FirstOrDefault();
                            foreach (var mr in a.Where(x => x.HolderType == "member").OrderBy(x => x.Id))
                            {
                                membs += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + ", ";

                                memberswithaddress += mr.FirstName + " " + mr.MiddleName + " " + mr.LastName + " of " + mr.UnitLevel + " " + mr.Street + " " + mr.Suburb.ToUpper() + " " + mr.State.ToUpper() + " " + mr.PostCode + ", ";

                                tbl += string.Format(@"<table class='c22' style='margin-bottom:30px'>
        <tbody>
            <tr class='c19'>
                <td class='c15' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>.........................................................</span></p>
                </td>
                <td class='c9' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>&nbsp;</span></p>
                </td>
                <td class='c14' colspan='2' rowspan='1'>
                    <p class='c3 c5'><span class='c13 c12'></span></p>
                </td>
                <td class='c32' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>...........................................................................................</span></p>
                </td>
            </tr>
            <tr class='c19'>
                <td class='c15' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>Signature of person before whom the declaration is made</span></p>
                </td>
                <td class='c9' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>&nbsp;</span></p>
                </td>
                <td class='c4' colspan='1' rowspan='1'>
                    <p class='c3 c5'><span class='c13 c12'></span></p>
                </td>
                <td class='c28' colspan='2' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>Signature of person making the declaration</span></p>
                </td>
            </tr>
            <tr class='c19'>
                <td class='c15' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>&nbsp;</span></p>
                </td>
                <td class='c9' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>&nbsp;</span></p>
                </td>
                <td class='c4' colspan='1' rowspan='1'>
                    <p class='c3 c5'><span class='c13 c12'></span></p>
                </td>
                <td class='c31' colspan='2' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>&nbsp;</span></p>
                </td>
            </tr>
            <tr class='c40'>
                <td class='c15' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>.........................................................</span></p>
                </td>
                <td class='c9' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>&nbsp;</span></p>
                </td>
                <td class='c4' colspan='1' rowspan='1'>
                    <p class='c3 c5'><span class='c13 c12'></span></p>
                </td>
                <td class='c31' colspan='2' rowspan='1'>
                    <p class='c3 c5'><span class='c12 c13'></span></p>
                </td>
            </tr>
            <tr class='c19'>
                <td class='c15' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>Title of person before whom the declaration is made</span></p>
                </td>
                <td class='c9' colspan='1' rowspan='1'>
                    <p class='c3'><span class='c13 c12'>&nbsp;</span></p>
                </td>
                <td class='c4' colspan='1' rowspan='1'>
                    <p class='c3 c5'><span class='c13 c2'></span></p>
                </td>
                <td class='c28' colspan='2' rowspan='1'>
                    <p class='c3'><span class='c6 c2'>{0}</span></p>
                </td>
            </tr>
        </tbody>
    </table>", mr.FirstName + " " + mr.MiddleName + " " + mr.LastName);
                            }

                            memberswithaddress = memberswithaddress.TrimEnd(',');
                            membs = membs.TrimEnd(',');
                            body = body.Replace("{trustsetupdate}", t.TrustSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            body = body.Replace("{smsftrustee}", t.SmsfCompanyName);
                            body = body.Replace("{smsftrusteeacn}", t.Smsfacn);
                            body = body.Replace("{member1address}", m.UnitLevel + " " + m.Street + " " + m.Suburb.ToUpper() + " " + m.State.ToUpper() + " " + m.PostCode);
                            body = body.Replace("{smsftrusteesetupdate}", t.SmsfTrusteeSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            body = body.Replace("{smsfacn}", t.Smsfacn);
                            body = body.Replace("{smsf}", t.Smsf);
                            body = body.Replace("{meetingtime}", "12:00 PM");
                            body = body.Replace("{members}", membs);
                            body = body.Replace("{propertyaddress}", t.PropertyAddress);
                            body = body.Replace("{propertytrusteeCompany}", t.PropertyTrusteeName);
                            body = body.Replace("{propertycompanyacn}", t.PropertyTrusteeAcn);
                            body = body.Replace("{propertytrusteesetupdate}", t.PropertyTrusteeSetupDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));

                            body = body.Replace("{trustname}", t.TrustName);
                            body = body.Replace("{lender}", t.LenderName);

                            //body = body.Replace("{member1}", m.FirstName + " " + m.MiddleName + " " + m.LastName);
                            //body = body.Replace("{member1address}", m.UnitLevel + " " + m.Street + " " + m.Suburb.ToUpper() + " " + m.State.ToUpper() + " " + m.PostCode);
                            body = body.Replace("{memberswithaddress}", memberswithaddress);
                            body = body.Replace("{memtbl}", tbl);
                            break;

                        case "super fund trust":

                            var firstMember = a.OrderBy(x => x.Id).FirstOrDefault();
                            string membernames = string.Empty;
                            string memberssign = string.Empty;
                            string trustnamewithmemberage = string.Empty;
                            string memberfooter = string.Empty;

                            foreach (var d in a.OrderBy(x => x.Id))
                            {
                                membernames += d.FirstName + " " + d.LastName + "<br/>";
                                memberssign += string.Format(@"<table class='tbl'>
                <tbody>
                    <tr>
                        <td>
                            Signature of member and Director of the Trustee Company<br />
                            …………………………………………………………………<br />
                            {0}
                        </td>
                        <td width='150' align='center'>
                            Date<br />
                            &nbsp;&nbsp;&nbsp;/ &nbsp;&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                </tbody>
            </table>", d.FirstName + " " + d.LastName);
                                trustnamewithmemberage += CalculateAge(d.Dob.Value) + " &";

                                memberfooter += string.Format(@"<p>
                ____________________________________<br />
                Member / Director of {0}<br />
                Name: {1}<br />
            </p>", capitName, d.FirstName + " " + d.LastName);
                            }
                            trustnamewithmemberage = capitName + " " + trustnamewithmemberage.TrimEnd('&');
                            body = body.Replace("{trustname}", capitName);
                            body = body.Replace("{meetingdate}", (t.TrustSetupDate.Value.AddDays(1)).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                            body = body.Replace("{membersname}", membernames);
                            body = body.Replace("{chairperson}", firstMember.FirstName + " " + firstMember.LastName);
                            body = body.Replace("{member1add}", firstMember.UnitLevel + " " + firstMember.Street + " " + firstMember.Suburb + " " + firstMember.State + " " + firstMember.PostCode + " " + firstMember.Country);
                            body = body.Replace("{memberssign}", memberssign);
                            body = body.Replace("{trustnamewithmemberage}", trustnamewithmemberage);
                            body = body.Replace("{membersfooter}", memberfooter);
                            break;
                    }

                    var f = trustData.trust.TrustName.Replace(" ", "-");
                    createpdf(body, f + "-trust-minutes-of-meeting.pdf");
                }
            }
            return View();
        }

        public ActionResult downloadtrustinvoice(int id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            var trustData = new ClassFullTrust();
            if (id > 0)
            {
                trustData = TrustMethods.GetFullTrustDetails(id);
            }
            if (trustData.TransactionDetail.TransactionStatus == true)
            {
                string htmlPath = string.Empty, body = "";
                if (trustData.trust != null)
                {
                    htmlPath = Server.MapPath("~/Content/deedhtml/deedinvoice.html");
                    //switch (trustData.trust.TrustType.ToLower())
                    //{
                    //    case "bare trust":

                    //        break;
                    //}
                    using (StreamReader red = new StreamReader(htmlPath))
                    {
                        body = red.ReadToEnd();
                    }
                    var tr = trustData.TransactionDetail;
                    var t = trustData.trust;
                    var member = trustData.appointers.OrderBy(x => x.Id).FirstOrDefault();
                    var ccf = ((trustData.Cost.SetupCost + trustData.Cost.SetupGST) * trustData.Cost.CreditCardFee) / 100; // Credit card fees
                    var m = $"{member.FirstName} {member.MiddleName} {member.LastName}";
                    var ma = $"{member.UnitLevel} {member.Street} <br/> {member.Suburb.ToUpper()} {member.State.ToUpper()} {member.PostCode} <br/> {member.Country}";
                    switch (trustData.trust.TrustType.ToLower())
                    {
                        case "bare trust":
                            body = body.Replace("{invoiceno}", tr.Id.ToString());
                            body = body.Replace("{date}", tr.AddedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            body = body.Replace("{username}", m);
                            body = body.Replace("{address}", ma);
                            body = body.Replace("{deedname}", $"Trust deed ({t.TrustName}) - Setup Fee");
                            body = body.Replace("{unitcost}", "$" + trustData.Cost.SetupCost);
                            body = body.Replace("{unittotal}", "$" + trustData.Cost.SetupCost);
                            body = body.Replace("{subtotal}", "$" + trustData.Cost.SetupCost);
                            body = body.Replace("{gst}", "$" + trustData.Cost.SetupGST);
                            body = body.Replace("{total}", "$" + trustData.Cost.TotalCost);

                            break;

                        case "super fund trust":
                            body = body.Replace("{invoiceno}", tr.Id.ToString());
                            body = body.Replace("{date}", tr.AddedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            body = body.Replace("{username}", m);
                            body = body.Replace("{address}", ma);
                            body = body.Replace("{deedname}", $"Trust deed ({t.TrustName}) - Setup Fee");
                            body = body.Replace("{unitcost}", "$" + trustData.Cost.SetupCost);
                            body = body.Replace("{unittotal}", "$" + trustData.Cost.SetupCost);
                            body = body.Replace("{subtotal}", "$" + trustData.Cost.SetupCost);
                            body = body.Replace("{gst}", "$" + trustData.Cost.SetupGST);
                            body = body.Replace("{total}", "$" + trustData.Cost.TotalCost);
                            break;
                    }

                    // Common for both
                    body = body.Replace("{creditcardfeesp}", trustData.Cost.CreditCardFee + "%");
                    body = body.Replace("{creditcardfees}", "$" + ccf);
                    body = body.Replace("{processingfees}", trustData.Cost.ProcessingFee + "cents");
                    var f = trustData.trust.TrustName.Replace(" ", "-");
                    createpdf(body, f + "-trust-invoice.pdf");
                }
            }
            return View();
        }

        public void createpdf(string body, string filename)
        {
            StringReader sr = new StringReader(body);
            Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                Response.Buffer = true;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
                Response.Close();
            }
        }

        public string GetMemberName(List<TblTrustAppointer> members, int index)
        {
            string name = "";
            try
            {
                index = index - 1;
                var m = members.OrderBy(x => x.Id).ToList()[index];
                name = m.FirstName + " " + m.MiddleName + " " + m.LastName;
            }
            catch (Exception ex)
            {
            }
            return name;
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;
            return age;
        }

        #endregion Trust PDF Download

        public ActionResult company()
        {
            //Sachin change
            //var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER" }, "/user/signin"));
            if (TempData["c_error"] != null)
            {
                ViewBag.msg = TempData["c_error"];
                TempData["c_error"] = null;
            }
            return View();
        }

        public ActionResult downloadCertificate(Int64 id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            // var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER" }, "/admin/signin"));
            var companyData = new ClassFullCompany();
            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;
                    //if (companyData.TransactionDetail.TransactionStatus == true) // Changing this to ASIC status
                    //if(companyData.CompanyMeta.ASICStatus.ToUpper()== "DOCUMENTS ACCEPTED"
                    //    && companyData.CompanyMeta.BillStatus.ToLower()=="paid"
                    //    )
                    if (companyData.TransactionDetail != null)
                    {
                        if (companyData.CompanyMeta.BillStatus.ToLower() == "paid")
                        {
                            string htmlPath = string.Empty, body = "";
                            if (companyData.Company != null)
                            {
                                htmlPath = Server.MapPath("~/Content/deedhtml/company.html");
                                using (StreamReader red = new StreamReader(htmlPath))
                                {
                                    body = red.ReadToEnd();
                                }
                                body = BuildCompanyPDF.BuildCertPDF(body, companyData);
                            }
                            var f = companyData.Company.CompanyName.Replace(" ", "-");

                            StringReader sr = new StringReader(body);
                            Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                                pdfDoc.Open();
                                string imageFilePath = Server.MapPath("~/Content/deedhtml/comdeeds.png");
                                //string imageFilePath = "";
                                Image jpg = Image.GetInstance(imageFilePath);
                                //jpg.ScaleToFit();
                                jpg.ScaleAbsolute(pdfDoc.PageSize.Width - 20, pdfDoc.PageSize.Height - 20);
                                jpg.Alignment = Image.UNDERLYING;
                                jpg.Border = 0;
                                jpg.SetAbsolutePosition(10, 10);
                                jpg.PaddingTop = 0;
                                writer.PageEvent = new ImageBackgroundHelper(jpg);
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                                pdfDoc.Close();
                                byte[] bytes = memoryStream.ToArray();
                                memoryStream.Close();
                                Response.Clear();
                                Response.ContentType = "application/pdf";
                                Response.AddHeader("Content-Disposition", "attachment; filename=" + f + "-Certificate.pdf");
                                Response.Buffer = true;
                                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                Response.BinaryWrite(bytes);
                                Response.End();
                                Response.Close();
                            }
                        }
                        else
                        {
                            TempData["c_error"] = Helper.CreateNotification("Sorry, there is some problem while downloading the document.", EnumMessageType.Warning, "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("company");
        }

        public ActionResult downloadconstitution(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN", "SUBADMIN", "USER" }, "/admin/signin"));
            string htmlPath = string.Empty, filename = string.Empty;
            var companyData = new ClassFullCompany();

            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;

                    if (companyData.TransactionDetail != null)
                    {
                        if (companyData.TransactionDetail.TransactionStatus == true)
                        {
                            if (companyData.Company.CompanyUseFor == "A company to operate business" || companyData.Company.CompanyUseFor.Contains("A company to operate business"))
                            {
                                string defaultPath = string.Empty;
                                defaultPath = Server.MapPath("~/Content/deedhtml/standard_company_constitution.pdf");
                                string pdfTemplate = ""; pdfTemplate = defaultPath;
                                string exportPath = Server.MapPath("../../../ExportedFiles\\" + id + "\\constitution_newone.pdf");
                                string newFile = exportPath;
                                filename = companyData.Company.CompanyName.Replace(" ", "-") + " - constitution";
                                PdfReader pdfReader = new PdfReader(pdfTemplate);
                                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                                AcroFields pdfFormFields = pdfStamper.AcroFields;

                                pdfFormFields.SetField("txtcompanyname", companyData.Company.CompanyName.ToUpper());
                                pdfFormFields.SetField("txtacn", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtacn1", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtType", companyData.Company.CompanyPurpose);
                                pdfFormFields.SetField("txtName", companyData.Company.CompanyName.ToUpper());

                                pdfStamper.FormFlattening = false;
                                pdfStamper.Close();

                                Response.ContentType = "Application/pdf";
                                Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");
                                Response.TransmitFile(exportPath);
                                Response.End();
                            }
                            else
                            {
                                string defaultPath = string.Empty;
                                defaultPath = Server.MapPath("~/Content/deedhtml/SMSF_Trustee_Constitution.pdf");
                                string pdfTemplate = ""; pdfTemplate = defaultPath;
                                string exportPath = Server.MapPath("../../../ ExportedFiles\\" + id + "\\smsf_constitution_newone.pdf");
                                string newFile = exportPath;
                                filename = companyData.Company.CompanyName.Replace(" ", "-") + "-smsf-constitution";
                                PdfReader pdfReader = new PdfReader(pdfTemplate);
                                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.OpenOrCreate));
                                AcroFields pdfFormFields = pdfStamper.AcroFields;

                                pdfFormFields.SetField("txtCompnayName", companyData.Company.CompanyName.ToUpper());
                                pdfFormFields.SetField("txtACN", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtAcn1", companyData.CompanyMeta.CompanyACN);
                                pdfFormFields.SetField("txtDate", companyData.TransactionDetail.AddedDate.Value.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture));
                                pdfFormFields.SetField("txtName", companyData.Company.CompanyName.ToUpper());

                                pdfStamper.FormFlattening = false;
                                pdfStamper.Close();

                                Response.ContentType = "Application/pdf";
                                Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");
                                Response.TransmitFile(exportPath);
                                Response.End();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        public ActionResult downloadconstitution_old(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            string htmlPath = string.Empty, body = "";
            var companyData = new ClassFullCompany();
            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    companyData = CompanyMethods.GetFullCompanyData(companyId);
                    var dt = AuthHelper.GetUserData();
                    var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                    companyData.user = u;
                    if (companyData.TransactionDetail.TransactionStatus == true)
                    {
                        htmlPath = Server.MapPath("~/Content/deedhtml/constitution.html");
                        using (StreamReader red = new StreamReader(htmlPath))
                        {
                            body = red.ReadToEnd();
                        }

                        var RegOfcAddModel = companyData.Address.Where(x => x.IsRegisteredAddress == true).FirstOrDefault();
                        string RegOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                            RegOfcAddModel.UnitLevel,
                            RegOfcAddModel.Street,
                            RegOfcAddModel.Suburb,
                            RegOfcAddModel.State,
                            RegOfcAddModel.PostCode);
                        string dirsign = string.Empty;
                        foreach (var d in companyData.Directors)
                        {
                            dirsign += string.Format("<p style='margin-bottom:0;'>............................................. <br />Name - {0} <br /> Date - {1}</p><br/>", d.FirstName + " " + d.LastName, DateTime.Now.ToString("dd-MM-yyyy"));
                        }
                        // replace names
                        body = body.Replace("{companyname}", companyData.Company.CompanyName.ToUpper());
                        body = body.Replace("{acn}", companyData.CompanyMeta.CompanyACN); // Please insert ACN here
                        body = body.Replace("{companyaddress}", RegOfcAdd);
                        body = body.Replace("{directorsign}", dirsign);

                        var f = companyData.Company.CompanyName + "-constitution.pdf";
                        StringReader sr = new StringReader(body);
                        Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                            writer.PageEvent = new ITextEvents();
                            pdfDoc.Open();
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                            pdfDoc.Close();
                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();
                            Response.Clear();
                            Response.ContentType = "application/pdf";
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + f);
                            Response.Buffer = true;
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.BinaryWrite(bytes);
                            Response.End();
                            Response.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ex.Message; // title
                //ex.InnerException.Message; // message
                //ex.
            }

            return RedirectToAction("company");
        }

        public ActionResult downloadreginvoice(int id)
        {
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER", "SUBUSER" }, "/user/signin"));
            var companyData = new ClassFullCompany();
            if (id > 0)
            {
                companyData = CompanyMethods.GetFullCompanyData(id);
                var dt = AuthHelper.GetUserData();
                var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                companyData.user = u;
            }
            if (companyData.TransactionDetail != null)
            {
                if (companyData.TransactionDetail.TransactionStatus == true)
                {
                    string htmlPath = string.Empty, body = "";
                    if (companyData.Company != null)
                    {
                        htmlPath = Server.MapPath("~/Content/deedhtml/companyinvoice.html");
                        using (StreamReader red = new StreamReader(htmlPath))
                        {
                            body = red.ReadToEnd();
                        }
                        var cr = companyData.TransactionDetail;
                        var c = companyData.Company;

                        var member = companyData.Applicant;
                        var ccf = ((companyData.Cost.AsicFee + companyData.Cost.SetupCost + companyData.Cost.SetupGST) * companyData.Cost.CreditCardFee) / 100; // Credit card fees
                        var m = $"{member.GivenName} {member.FamilyName}";
                        var RegOfcAddModel = companyData.Address.Where(x => x.IsRegisteredAddress == true).FirstOrDefault();
                        string RegOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                            RegOfcAddModel.UnitLevel,
                            RegOfcAddModel.Street,
                            RegOfcAddModel.Suburb,
                            RegOfcAddModel.State,
                            RegOfcAddModel.PostCode);

                        body = body.Replace("{invoiceno}", cr.Id.ToString());
                        body = body.Replace("{date}", cr.AddedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                        body = body.Replace("{username}", m);
                        body = body.Replace("{address}", RegOfcAdd);

                        body = body.Replace("{asicfee}", "$" + companyData.Cost.AsicFee);
                        body = body.Replace("{asictotal}", "$" + companyData.Cost.AsicFee);

                        body = body.Replace("{deedname}", $"Company ({companyData.Company.CompanyName.ToUpper()}) - Setup Fee");
                        body = body.Replace("{unitcost}", "$" + companyData.Cost.SetupCost);
                        body = body.Replace("{unittotal}", "$" + companyData.Cost.SetupCost);

                        //if (companyData.Company.CompanyUseFor == "smsf")
                        //{
                        //    body = body.Replace("{smsffee}", "$" + 48);
                        //    body = body.Replace("{smsftotel}", "$" + 48);
                        //}
                        //else
                        //{
                        //    body = body.Replace("{smsffee}", "$" + 0);
                        //    body = body.Replace("{smsftotel}", "$" + 0);
                        //}

                        if (companyData.companysearch.govofcomapany == "yes")
                        {
                            body = body.Replace("{constdfees}", "$" + 10);
                            body = body.Replace("{consttotel}", "$" + 10);
                        }
                        else
                        {
                            body = body.Replace("{constdfees}", "$" + 0);
                            body = body.Replace("{consttotel}", "$" + 0);
                        }
                        //if (companyData.Company.CompanyUseFor == "smsf" && companyData.companysearch.govofcomapany == "yes")
                        //{
                        //    body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee + 48 + 10));
                        //}
                        //else if (companyData.Company.CompanyUseFor == "smsf")
                        //{
                        //    body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee + 48));
                        //}
                        if (companyData.Company.CompanyUseFor == "smsf")
                        {
                            body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee + 10));
                        }
                        else
                        {
                            body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee));
                        }

                        body = body.Replace("{subtotal}", "$" + (companyData.Cost.SetupCost + companyData.Cost.AsicFee));
                        body = body.Replace("{gst}", "$" + companyData.Cost.SetupGST);
                        body = body.Replace("{total}", "$" + companyData.Cost.TotalCost);
                        body = body.Replace("{creditcardfeesp}", companyData.Cost.CreditCardFee + "%");
                        body = body.Replace("{creditcardfees}", "$" + ccf);
                        body = body.Replace("{processingfees}", companyData.Cost.ProcessingFee + "cents");
                        var f = companyData.Company.CompanyName.Replace(" ", "-");
                        createpdf(body, f + "-setup-invoice.pdf");
                    }
                }
            }

            return RedirectToAction("company");
        }

        public ActionResult downloadregAsic(int id)
        {
            ErrorLog objErrorLog = new ErrorLog();
            var uid = Convert.ToInt32(AuthHelper.IsValidRequest(new List<string> { "USER", "ADMIN", "SUBUSER" }, "/user/signin"));
            var companyData = new ClassFullCompany();
            if (id > 0)
            {
                companyData = CompanyMethods.GetFullCompanyData(id);
                var dt = AuthHelper.GetUserData();
                var u = JsonConvert.DeserializeObject<LoginUserData>(dt);
                companyData.user = u;
                try
                {
                    if (companyData.TransactionDetail != null)
                    {
                        if (companyData.TransactionDetail.TransactionStatus == true)
                        {
                            var AsicStatus = new ClassAsicSetup();
                            AsicStatus = CompanyMethods.getAsicDetails(id.ToString());
                            //string lbldocno = "3E9568948"; testing purpose only
                            string lbldocno = "";
                            string lblresponseType = "";
                            string lblpath = "";

                            lbldocno = AsicStatus.Asic_DocNo.ToString();
                            lblresponseType = AsicStatus.Asic_ResType.ToString();

                            if (System.IO.File.Exists("C:/asicfiles/Logs/" + lbldocno + ".pdf") && (lblresponseType.Trim() == "RA55" || lblresponseType.Trim() == ""))
                            {
                                lblpath = "C:/asicfiles/Logs/" + lbldocno + ".pdf";
                                System.IO.FileInfo _file = new System.IO.FileInfo(lblpath);
                                if (_file.Exists)
                                {
                                    Response.Clear();
                                    Response.AddHeader("Content-Disposition", "attachment; filename=" + _file.Name);
                                    Response.AddHeader("Content-Length", _file.Length.ToString());
                                    Response.ContentType = "application/octet-stream";
                                    Response.WriteFile(_file.FullName);
                                    Response.Flush();
                                    return View();

                                    // Response.End();
                                }
                                else
                                {
                                    ViewBag.type = "c";
                                    return View("waitforAisc");
                                }
                            }
                            else
                            {
                                ViewBag.type = "c";
                                return View("waitforAisc");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    objErrorLog.WriteErrorLog(ex.ToString());
                }
            }
            return RedirectToAction("company");
        }

        public ActionResult waitforAisc()
        {
            return View();
        }

        #region Pdf Events

        private class ImageBackgroundHelper : PdfPageEventHelper
        {
            private Image img;

            public ImageBackgroundHelper(Image img)
            {
                this.img = img;
            }

            /**
             * @see com.itextpdf.text.pdf.PdfPageEventHelper#onEndPage(
             *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
             */

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                writer.DirectContentUnder.AddImage(img);
            }
        }

        private class ITextEvents : PdfPageEventHelper
        {
            // This is the contentbyte object of the writer
            private PdfContentByte cb;

            // we will put the final number of pages in a template
            private PdfTemplate footerTemplate;

            // this is the BaseFont we are going to use for the header / footer
            private BaseFont bf = null;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    footerTemplate = cb.CreateTemplate(50, 50);
                }
                catch (DocumentException de)
                {
                    //handle exception here
                }
                catch (System.IO.IOException ioe)
                {
                    //handle exception here
                }
            }

            public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);
                if (writer.PageNumber > 1)
                {
                    iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                    String text = "Page " + (writer.PageNumber - 1) + " of ";
                    //Add paging to footer
                    {
                        cb.BeginText();
                        cb.SetFontAndSize(bf, 10);
                        cb.SetTextMatrix(document.PageSize.GetRight(150), document.PageSize.GetBottom(10));
                        cb.ShowText(text);
                        cb.EndText();
                        float len = bf.GetWidthPoint(text, 10);
                        cb.AddTemplate(footerTemplate, document.PageSize.GetRight(150) + len, document.PageSize.GetBottom(10));
                        //Move the pointer and draw line to separate footer section from rest of page
                        cb.MoveTo(40, document.PageSize.GetBottom(25));
                        cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(25));
                        cb.Stroke();
                        BaseColor b = new BaseColor(9, 9, 9);
                        cb.SetColorStroke(b);
                    }
                }
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                if (writer.PageNumber > 1)
                {
                    footerTemplate.BeginText();
                    footerTemplate.SetFontAndSize(bf, 10);
                    footerTemplate.SetTextMatrix(0, 0);
                    footerTemplate.ShowText((writer.PageNumber - 1).ToString());
                    footerTemplate.EndText();
                }
            }
        }

        #endregion Pdf Events
    }
}