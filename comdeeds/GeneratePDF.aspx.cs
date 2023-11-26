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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Ionic.Zip;
using System.Net.Mail;
using System.Configuration;
using comdeeds.App_Code;
using iTextSharp.tool.xml;
using static comdeeds.Models.BaseModel;
using Newtonsoft.Json;
using comdeeds;
using System.Globalization;
using iTextSharp.text.html.simpleparser;

namespace comdeeds
{
    public partial class GeneratePDF : System.Web.UI.Page
    {
        ErrorLog oErrorLog = new ErrorLog();
        string dtallmember_ = "";
        List<String> DynamicPdfName = new List<string>();
        dal.Operation op = new dal.Operation();
        dal.DataAccessLayer dal = new DataAccessLayer();
        long companyID = 0;
        protected void Page_Load1(object sender, EventArgs e)
        {
            Response.Redirect("/ThankYou?utm_t=c",false);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Email"] != null && Request.QueryString["CompanyID"] != null)
                {
                    string val = Request.QueryString["Email"].ToString();
                    hdnemail.Value  = val;

                    string id = Request.QueryString["CompanyID"].ToString();
                    hdncompanyid.Value = id;
                    companyID = Convert.ToInt64(id);
                }
                else
                {                 
                    oErrorLog.WriteErrorLog("querystring shows empty in Generate PDF page line 48 ");
                }

                //hdncompanyid.Value = "2192";
                //hdnemail.Value = "teach.msp@gmail.com";
                //companyID = 2192;

                DataTable dtcompanyname = dal.getdata("select FULLNAME,Asic_ACN from companysearch where id='" + hdncompanyid.Value.ToString() + "'");
                if (dtcompanyname.Rows.Count > 0)
                {
                    hdncompanyname.Value = dtcompanyname.Rows[0]["FULLNAME"].ToString();
                    hdnacn.Value = dtcompanyname.Rows[0]["Asic_ACN"].ToString();
                }
                
                CreateCustomerFolder();
                DownloadCertificate(companyID);
                downloadconstitution(companyID);
              
                mergeallpdf();
                createpdfZip();
                Response.Redirect("/ThankYou?utm_t=c", false);
            }
        }

        #region Create Folder
        private void CreateCustomerFolder()
        {
            try
            {
                string directoryPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string directoryPath2 = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Final");
                if (!Directory.Exists(directoryPath2))
                {
                    Directory.CreateDirectory(directoryPath2);
                }
            }
            catch (Exception ex) { }
        }
        #endregion

        #region Create PDF by through controller 

        public void DownloadCertificate(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN","USER", "SUBUSER" }, "/admin/signin"));
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

                    if (companyData.CompanyMeta != null)
                    {
                        if (companyData.CompanyMeta.BillStatus.ToLower() == "paid")
                        {
                            FileStream file = new FileStream(Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\") + "company-certificate.PDF", FileMode.Create, System.IO.FileAccess.Write);
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
                            //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                            using (MemoryStream memoryStream = new MemoryStream())
                           {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, file);
                           
                            pdfDoc.Open();
                            //htmlparser.Parse(sr);

                            string imageFilePath = Server.MapPath("~/Content/deedhtml/comdeeds.png");
                                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageFilePath);
                                jpg.ScaleAbsolute(pdfDoc.PageSize.Width - 20, pdfDoc.PageSize.Height - 20);
                                jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
                                jpg.Border = 0;
                                jpg.SetAbsolutePosition(10, 10);
                                jpg.PaddingTop = 0;
                                writer.PageEvent = new ImageBackgroundHelper(jpg);
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                               
                                byte[] bytes = memoryStream.ToArray();

                               

                          }
                            
                            //PdfWriter.GetInstance(pdfDoc, file);
                            pdfDoc.Close();
                            file.Close();

                        }
                        else
                        {
                            ViewState["c_error"] = Helper.CreateNotification("Sorry, there is some problem while downloading the document.", EnumMessageType.Warning, "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            if (ViewState["c_error"] != null)
            {
                string message = Helper.CreateNotification(ViewState["c_error"].ToString(), EnumMessageType.Error, "");
                errormsg.InnerHtml = message.Replace("notification closeable error", "");
            }
            else
            {

            }

        }
        class ImageBackgroundHelper : PdfPageEventHelper
        {
            private iTextSharp.text.Image img;
            public ImageBackgroundHelper(iTextSharp.text.Image img)
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

        public void downloadconstitution(long id)
        {
            var uid = Convert.ToInt64(AuthHelper.IsValidRequest(new List<string> { "ADMIN","USER", "SUBUSER" }, "/admin/signin"));
            string htmlPath = string.Empty, body = "";
            var companyData = new ClassFullCompany();
            try
            {
                var companyId = id;
                if (companyId > 0)
                {
                    FileStream file = new FileStream(Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\") + "company-constitution.PDF", FileMode.Create, System.IO.FileAccess.Write);

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
                            //dirsign += string.Format(@"<p style='margin-bottom:0;'>............................................. <br />[Name], [Signature], Member[Date]</p><br/>");
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
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, file);
                            writer.PageEvent = new ITextEvents();
                            pdfDoc.Open();
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                            
                            pdfDoc.Close();
                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();                
                        }
                        file.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }

        }

        #region Pdf Events


        class ITextEvents : PdfPageEventHelper
        {
            ErrorLog oErrorLog = new ErrorLog();

            // This is the contentbyte object of the writer
            PdfContentByte cb;
            // we will put the final number of pages in a template
            PdfTemplate footerTemplate;
            // this is the BaseFont we are going to use for the header / footer
            BaseFont bf = null;


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
                    oErrorLog.WriteErrorLog(de.ToString());
                }
                catch (System.IO.IOException ioe)
                {
                    oErrorLog.WriteErrorLog(ioe.ToString());
                }
            }

            public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                try
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
                catch (Exception ex)
                {
                    oErrorLog.WriteErrorLog(ex.ToString());
                }

            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                try
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
                catch (Exception ex)
                {

                    oErrorLog.WriteErrorLog(ex.ToString());
                }

            }
        }

        #endregion

        #endregion
     
        #region Merge PDF

        private void Merge_Certificate_Members(string[] lstFiles)
        {

            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = @"E:/pdf/HolderFinal.pdf";
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Final\\"+ "ShareCertificateFinal.pdf");
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
                oErrorLog.WriteErrorLog(ex.ToString());;
            }
        }

        private void MergeAllocateShare(string[] lstFiles)
        {
           
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = @"E:/pdf/HolderFinal.pdf";
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Final\\RegisterofAllottedSharesFinal.pdf");
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
               oErrorLog.WriteErrorLog(ex.ToString());;
            }
        }

        private void MergeMembers(string[] lstFiles)
        {

            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = @"E:/pdf/HolderFinal.pdf";
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Final\\ApplicationForShareFinal.pdf");
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
                 oErrorLog.WriteErrorLog(ex.ToString());;
            }
        }
        private void MergeDirector(string[] lstFiles)
        {

            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = @"E:/pdf/HolderFinal.pdf";
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Final\\consentdirectorFinal.pdf");
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
                // oErrorLog.WriteErrorLog(ex.ToString());;
            }
        }
        private void MergeSecratory(string[] lstFiles)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = @"E:/pdf/HolderFinal.pdf";
            string exportPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Final\\consentsecratoryFinal.pdf");
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
                // oErrorLog.WriteErrorLog(ex.ToString());;
            }

        }
        private int get_pageCcount(string file)
        {
            PdfReader pdfReader = new PdfReader(file);
            int numberOfPages = pdfReader.NumberOfPages;
            return numberOfPages;
        }
       
        #endregion

        #region MErge & zip all files

        private void mergeallpdf()
        {
            try
            {
                string directoryPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\");
                int totalcreatedfiles = 0;

                ArrayList arrpdfs = new ArrayList();
                bool companyconstitution = false;
                if (File.Exists(directoryPath + "company-constitution.pdf"))
                {
                    totalcreatedfiles = totalcreatedfiles + 1;
                    companyconstitution = true;
                    arrpdfs.Add(directoryPath + "company-constitution.pdf");
                }
                bool companycertificate = false;
                if (File.Exists(directoryPath + "company-certificate.pdf"))
                {
                    totalcreatedfiles = totalcreatedfiles + 1;
                    companycertificate = true;
                    arrpdfs.Add(directoryPath + "company-certificate.pdf");
                }
                
                string[] lstFiles = new string[arrpdfs.Count];
                for (int ii = 0; ii < arrpdfs.Count; ii++)
                {
                    lstFiles[ii] = arrpdfs[ii].ToString();
                    DynamicPdfName.Add(arrpdfs[ii].ToString());
                }
             
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
        }
        private void createpdfZip()
        {
            try
            {
                string directoryPath = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\");
                string[] filename = Directory.GetFiles(directoryPath);
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddFiles(filename, "file");
                    zip.Save(Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Doc_" + hdncompanyid.Value + ".zip"));

                    string username = "";
                    string Password = "";
                    string Mailserver = "";
                    string Port = "";
                    bool ssl = true;
                    string apiurl = "";
                    string Message = "";
                    string path_Form201 = Server.MapPath("ExportedFiles\\" + hdncompanyid.Value + "\\Form201.pdf");
                    username = ConfigurationManager.AppSettings["FromMail"].ToString();
                    Password = ConfigurationManager.AppSettings["Password"].ToString();
                    Mailserver = ConfigurationManager.AppSettings["Host"].ToString();
                    Port = "587";
                    MailMessage Msg = new MailMessage();
                    string fromeamil1 = username.ToString();
                    Msg.From = new System.Net.Mail.MailAddress(fromeamil1);
                    Msg.To.Add(new MailAddress(hdnemail.Value.ToString()));
                    Msg.Subject = "Company Documents";
                    Msg.Body = Message.ToString();
                    for (int File = 0; File < DynamicPdfName.Count; File++)
                    {
                        if (DynamicPdfName.Count > 0)
                        {
                            Msg.Attachments.Add(new Attachment(DynamicPdfName[File]));
                        }
                    }
                    Msg.Attachments.Add(new Attachment(path_Form201));
                    Msg.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = Mailserver;
                    smtp.Port = Convert.ToInt32(Port);
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new System.Net.NetworkCredential(fromeamil1.ToString(), Password.ToString());
                    smtp.Timeout = 600000;
                    smtp.EnableSsl = ssl;
                   // smtp.Send(Msg);
                    
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());;
            }
        }
        #endregion
    }
}