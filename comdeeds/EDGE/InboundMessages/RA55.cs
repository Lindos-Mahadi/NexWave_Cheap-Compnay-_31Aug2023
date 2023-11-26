using System;
using System.IO;
using comdeeds.EDGE.CompositeElements;
using comdeeds.dal;

namespace comdeeds.EDGE.InboundMessages
{
    /// <summary>
    /// Company Registration Advice
    /// </summary>
    internal class RA55 : InboundMessage
    {
        ErrorLog objerro = new ErrorLog();
        public RA55(string rawMessage)
            : base(rawMessage)
        {
        }
        public RA55(string rawMessage, string companyId)
            : base(rawMessage, companyId)
        {
        }
        public override string MessageName { get { return "RA55"; } }

        public static bool CanParse(string rawMessage)
        {
            //return rawMessage.StartsWith ("ZHDASCRA55");//commented by bhupi, because this condition always coming false
            return rawMessage.Contains("ZHDASCRA55\t");
        }

        public override void Parse()
        {
            try
            {
                string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                CompanyDataRA55 companyData = new CompanyDataRA55();

                //string [] fieldData = segments [1].Substring (3).Split (new [] {'\t'} , StringSplitOptions.None);
                string[] fieldData = segments[3].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                companyData.CompanyName = fieldData[0].Replace("*** TEST DATABASE ***","");
                companyData.ACN = fieldData[1];
                companyData.CompanyType = fieldData[2];
                companyData.CompanyClass = fieldData[3];
                companyData.CertificatePrintOption = fieldData[4];
                companyData.JurisdictionOfRegistration = fieldData[12];
                companyData.DateOfRegistration = ParseDate(fieldData[13]);
                companyData.CompanySubclass = fieldData[17];

                //fieldData = segments [2].Substring (3).Split (new [] { '\t' } , StringSplitOptions.None);
                fieldData = segments[4].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                companyData.AccountNumber = fieldData[4];
                companyData.SupplierName = fieldData[6];
                companyData.SupplierABN = fieldData[7];
                companyData.RegisteredAgentName = fieldData[8];
                companyData.RegisteredAgentAddress = fieldData[9];

                //fieldData = segments [3].Substring (3).Split (new [] { '\t' } , StringSplitOptions.None);
                fieldData = segments[5].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                companyData.InvoiceDescription = fieldData[0];
                companyData.InvoiceAmmount = Convert.ToInt32(fieldData[1]);
                companyData.DocumentNumber = fieldData[2];
                companyData.FormCode = fieldData[3];
                companyData.TaxInvoiceText = fieldData[5];
                companyData.TaxCode = fieldData[6];
                companyData.TaxAmmount = Convert.ToInt16(fieldData[7]);

                // TODO: implement whatever needs to be done with this data, if anything
                Operation oper = new Operation();
                string retid = oper.insert_ra55(companyData);



                ReadFile(companyData.DocumentNumber, (segments[3] + segments[4] + segments[5]));
                
                dal.DataAccessLayer dal = new dal.DataAccessLayer();

                string companyFullName = companyData.CompanyName.ToUpper().Replace("PTY LTD","").Trim();

                if (companyFullName == "" || companyFullName==null)
                {
                    companyFullName=companyData.CompanyName.Trim();
                }

                string acn = companyData.ACN;
                string fileName = "C:/asicfiles/Logs/" + companyData.DocumentNumber + "_RA55.txt";
                // dal.executesql("update companysearch set Asic_status='DOCUMENTS ACCEPTED',Asic_acn='" + acn + "',Asic_ResType='RA55',Asic_File='" + fileName + "' where fullname='" + companyData.CompanyName + "'");
                 dal.executesql("update companysearch set Asic_status='DOCUMENTS ACCEPTED',Asic_acn='" + acn + "',Asic_ResType='RA55',Asic_File='" + fileName + "' where CompanyName='" + companyFullName + "'");

                string DocumentNo = companyData.DocumentNumber;

                // dal.executesql("update companysearch set Asic_DocNo='" + DocumentNo + "' where fullname='" + companyData.CompanyName + "'");
                dal.executesql("update companysearch set Asic_DocNo='" + DocumentNo + "' where CompanyName='" + companyFullName + "'");

                dal.executesql("update [dbo].[ASIC_TXN_File] set ACN='" + companyData.ACN + "',DOCUMENT_NUMBER='" + companyData.DocumentNumber + "',DATERA55='" + companyData.DateOfRegistration + "' where companyid='" + companyData.CompanyName.ToUpper() + "' and TRANS_STATUS='DOCUMENTS ACCEPTED'");

               // dal.executesql("update companysearch set Asic_Error='" + (segments[3] + segments[4] + segments[5]).Replace("'", "''") + "' where fullname='" + companyData.CompanyName + "'");
                dal.executesql("update companysearch set Asic_Error='" + (segments[3] + segments[4] + segments[5]).Replace("'", "''") + "' where CompanyName='" + companyFullName + "'");

                objerro.WriteErrorLog("start of RA55.cs - " + companyData.CompanyName + " end of RA55.cs");

                Success = true;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to parse " + m_rawMessage + "\n" + e.Message);
            }
        }

        private void ReadFile(string documentNumber, string ra55data)
        {
            string fileContent = m_rawMessage.Substring(m_rawMessage.IndexOf("ZTRENDRA55") + 13);
            string fileName = "C:/asicfiles/Logs/" + documentNumber.Trim() + (fileContent.StartsWith("%PDF") ? ".pdf" : ".ps");
            //fileName = m_companyId + "_" + fileName;
            if (!Directory.Exists("C:/asicfiles/Logs"))
            {
                Directory.CreateDirectory("C:/asicfiles/Logs");
            }
            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
                fs.Dispose();
            }
            File.WriteAllText(fileName, fileContent);

            ///Save RA55Now
            fileName = "C:/asicfiles/Logs/" + documentNumber + "_RA55.txt";
            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
                fs.Dispose();
            }
            File.WriteAllText(fileName, ra55data);
            //Directory.CreateDirectory ("Logs");
            //File.WriteAllText (string.Format ("Logs\\{0}" , fileName) , fileContent);
        }
    }
}