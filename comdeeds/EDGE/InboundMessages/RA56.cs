using System;
using System.Collections.Generic;
using System.IO;
using comdeeds.EDGE.CompositeElements;

namespace comdeeds.EDGE.InboundMessages
{
    /// <summary>
    /// Advice of ASIC processing status 
    /// </summary>
    internal class RA56 : InboundMessage
    {
        public RA56(string rawMessage)
            : base(rawMessage)
        {
        }
        public RA56(string rawMessage, string companyId)
            : base(rawMessage, companyId)
        {
        }
        public override string MessageName { get { return "RA56"; } }

        public static bool CanParse(string rawMessage)
        {
           //return rawMessage.StartsWith("ZHDASCRA56");
            return rawMessage.Contains("ZHDASCRA56\t");
        }

        public override void Parse()
        {
            try
            {
                dal.DataAccessLayer dal = new dal.DataAccessLayer();
                string text_ = "";
                //dal.DataAccessLayer dal = new dal.DataAccessLayer();
                //string finename = m_companyId + "_Only201.txt";
                //string data201 = gettext(finename);//.Replace("\r\n", "\n");
                //dal.executesql("insert into ASIC_TXN_File(companyid,formdata,asicresponse) values('" + m_companyId + "','" + data201 + "','" + m_rawMessage + "')");
                string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                StatusDetailsDataRA56 companyData = new StatusDetailsDataRA56();
                string[] fieldData = segments[1].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                /*if (segments[3].Substring(3) == "ZNR")
                {
                    companyData.ProposedCompanyName = fieldData[0];
                    companyData.RequestDocumentNumber = fieldData[1];
                    companyData.DateOfAdvice = ParseDate(fieldData[2]);
                    companyData.ASICAdviceType = fieldData[3];
                    dal.executesql("update [dbo].[ASIC_TXN_File] set DOCUMENT_NUMBER='" + companyData.RequestDocumentNumber + "',DATERA56='" + companyData.DateOfAdvice + "' where companyid='" + m_companyId + "' and TRANS_STATUS='DOCUMENTS ACCEPTED'");
                }*/


                /*if (segments.Length > 2)
                {
                    fieldData = segments[2].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                    companyData.AccountNumber = fieldData[4];
                    companyData.SupplierName = fieldData[6];
                    companyData.SupplierABN = fieldData[7];
                    companyData.RegisteredAgentName = fieldData[8];
                    companyData.RegisteredAgentAddress = fieldData[9];
                }
                if (segments.Length > 3)
                {
                    fieldData = segments[3].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                    companyData.InvoiceDescription = fieldData[0];
                    companyData.InvoiceAmmount = Convert.ToInt16(fieldData[1]);
                    companyData.DocumentNumber = fieldData[2];
                    companyData.FormCode = fieldData[3];
                    companyData.TaxInvoiceText = fieldData[5];
                    companyData.TaxCode = fieldData[6];
                    companyData.TaxAmmount = Convert.ToInt16(fieldData[7]);

                    companyData.StatusText = new List<string>();

                    //int ndx = 4;
                    //while (segments[ndx].StartsWith("ZTE"))
                    //{
                    //    companyData.StatusText.Add(segments[ndx].Substring(3));
                    //    text_ += segments[ndx].Substring(3) + "<br>";
                    //    ndx++;
                    //}
                }*/

                string companyFullName = m_companyId.ToUpper().Replace("PTY LTD", "").Trim();

                if (companyFullName == "" || companyFullName == null)
                {
                    companyFullName = m_companyId.Trim();
                }



                for (int i = 0; i < segments.Length; i++) {
                    if (segments[i].StartsWith("ZNR"))
                    {
                        fieldData = segments[i].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                        companyData.ProposedCompanyName = fieldData[4];
                        companyData.RequestDocumentNumber = fieldData[5];
                        companyData.DateOfAdvice = ParseDate(fieldData[6]);
                        companyData.ASICAdviceType = fieldData[7];
                        dal.executesql("update [dbo].[ASIC_TXN_File] set DOCUMENT_NUMBER='" + companyData.RequestDocumentNumber + "',DATERA56='" + companyData.DateOfAdvice + "' where companyid='" + m_companyId.ToUpper() + "' and TRANS_STATUS='DOCUMENTS ACCEPTED'");
                        dal.executesql("update companysearch set Asic_DocNo='" + companyData.RequestDocumentNumber + "',Asic_ResType='RA56',Asic_status='DOCUMENTS ACCEPTED' where CompanyName='" + companyFullName + "'");

                    }
                    if (segments[i].StartsWith("ZIL")) {
                        fieldData = segments[i].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                        companyData.AccountNumber = fieldData[4];
                        companyData.SupplierName = fieldData[6];
                        companyData.SupplierABN = fieldData[7];
                        companyData.RegisteredAgentName = fieldData[8];
                        companyData.RegisteredAgentAddress = fieldData[9];
                    }
                    if (segments[i].StartsWith("ZIA"))
                    {
                        fieldData = segments[i].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                        companyData.InvoiceDescription = fieldData[0];
                        companyData.InvoiceAmmount = Convert.ToInt16(fieldData[1]);
                        companyData.DocumentNumber = fieldData[2];
                        companyData.FormCode = fieldData[3];
                        companyData.TaxInvoiceText = fieldData[5];
                        companyData.TaxCode = fieldData[6];
                        companyData.TaxAmmount = Convert.ToInt16(fieldData[7]);
                    }
                    if (segments[i].StartsWith("ZTE"))
                    {
                        text_ += segments[i].Substring(3) + "<br>";
                    }
                }

                //int ndx = 4;
                //while (segments[ndx].StartsWith("ZTE"))
                //{
                //    companyData.StatusText.Add(segments[ndx].Substring(3));
                //    text_ += segments[ndx].Substring(3) + "<br>";
                //    ndx++;
                //}
                // TODO: implement whatever needs to be done with this data, if anything
                //string DocumentNo = companyData.DocumentNumber;
                //dal.executesql("update companysearch set Asic_status='DOCUMENTS ACCEPTED',Asic_DocNo='" + DocumentNo + "',Asic_ResType='RA56',Asic_File='" + fileName + "',Asic_Error='" + m_rawMessage.Replace("'", "''") + "' where fullname='" + m_companyId + "'");
                int iin = dal.executesql("insert into RA56(ProposedCompanyName ,RequestDocumentNumber ,DateOfAdvice ,ASICAdviceType ,AccountNumber ,SupplierName ,SupplierABN ,RegisteredAgentName ,RegisteredAgentAddress ,InvoiceDescription ,InvoiceAmmount ,DocumentNumber ,FormCode ,TaxInvoiceText ,TaxCode ,TaxAmmount ,StatusText) values('" + companyData.ProposedCompanyName + "' ,'" + companyData.RequestDocumentNumber + "' ,'" + companyData.DateOfAdvice + "' ,'" + companyData.ASICAdviceType + "' ,'" + companyData.AccountNumber + "' ,'" + companyData.SupplierName + "' ,'" + companyData.SupplierABN + "' ,'" + companyData.RegisteredAgentName + "' ,'" + companyData.RegisteredAgentAddress + "' ,'" + companyData.InvoiceDescription + "' ,'" + companyData.InvoiceAmmount + "' ,'" + companyData.DocumentNumber + "' ,'" + companyData.FormCode + "' ,'" + companyData.TaxInvoiceText + "' ,'" + companyData.TaxCode + "' ,'" + companyData.TaxAmmount + "' ,'" + text_ + "')");
                Success = true;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to parse " + m_rawMessage + "\n" + e.Message);
            }
        }

        private string gettext(string filename)
        {
            string textme = System.IO.File.ReadAllText(@"C:/asicfiles/" + filename, System.Text.Encoding.UTF8);//.Replace("\r\n", "\n");
            return textme;
        }
    }
}