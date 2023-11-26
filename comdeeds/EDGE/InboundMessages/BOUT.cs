using System;
using System.Data;
using System.IO;

namespace comdeeds.EDGE.InboundMessages
	{
	/// <summary>
	/// Batch of Outbound Reports. Not to be instantiated directly.
	/// </summary>
	internal class BOUT : InboundMessage
		{
		public BOUT (string rawMessage) : base (rawMessage)
			{
			}
        public BOUT(string rawMessage,string companyId)
            : base(rawMessage, companyId)
        {
        }
		public override string MessageName { get { return "BOUT"; } }

		public static bool CanParse (string rawMessage)
			{
			return rawMessage.StartsWith ("XSVBOUT");
			}

		public override void Parse ()
			{
			try
				{
				ErrorMessage = m_rawMessage.Substring (8 , 3);
				int files = Convert.ToInt16 (ErrorMessage);

				if (files == 0)
					{
					NoPendingFiles = true;
					return;
					}

				files--;

				ReadFile ();

				NoPendingFiles = files == 0;

				Success = true;
				}
			catch (Exception e)
				{
				throw new Exception ("Unable to parse " + m_rawMessage + "\n" + e.Message);
				}
			}

        private void ReadFile()
        {
            ErrorLog errorLogi = new ErrorLog();
            string fileContent = m_rawMessage.Substring(34);

            string fileName = m_rawMessage.Substring(21, 12);
            //fileName = "C:/asicfiles/Logs/" + string.Format("{0}.{1}.txt", fileName.Substring(0, 8).Trim(), fileName.Substring(9, 3).Trim());

            if (fileName.ToUpper().Contains("OUT_TP"))
            {
                //fileName = "C:/asicfiles/Logs/" + m_companyId + "_" + string.Format("{0}.{1}.txt", fileName.Substring(0, 9).Trim(), fileName.Substring(9, 3).Trim());
                fileName = "C:/asicfiles/Logs/"  + string.Format("{0}.{1}.txt", fileName.Substring(0, 9).Trim(), fileName.Substring(9, 3).Trim());
            }
            else
            {
                //fileName = "C:/asicfiles/Logs/" + m_companyId + "_" + string.Format("{0}.{1}.txt", fileName.Substring(0, 8).Trim(), fileName.Substring(9, 3).Trim());
                fileName = "C:/asicfiles/Logs/"  + string.Format("{0}.{1}.txt", fileName.Substring(0, 8).Trim(), fileName.Substring(9, 3).Trim());
            }
            //fileName = m_companyId + "_" + fileName;
            

            if (!Directory.Exists("C:/Logs"))
             {
                 Directory.CreateDirectory("C:/Logs");
             }
             File.WriteAllText("C:/Logs/temp.log", fileContent);



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

           /////// earlier it was here ra55/ra56

             /* if (RA55.CanParse(m_rawMessage))
                new RA55(m_rawMessage);

                if (RA56.CanParse(m_rawMessage))
                new RA56(m_rawMessage); */

            DataEntities.ASICFILEREADER obc = new DataEntities.ASICFILEREADER();
            string cname = "";
            cname = obc.companyname(fileContent);
            m_companyId = cname;
            dal.DataAccessLayer dal = new dal.DataAccessLayer();
            string finename = cname.Replace(" ","_") + "_Only201.txt";
            string data201 = gettext(finename);//.Replace("\r\n", "\n");

           ///  m_companyId = "SUPER INDIA FUND PTY LTD";

            string companyFullName = m_companyId.Replace("PTY LTD", "").Trim();
            if(companyFullName=="" || companyFullName==null)
            {
                companyFullName = m_companyId.Substring(0,m_companyId.Length-7);
            }
           // companyFullName = companyFullName.Replace("  "," ");
            errorLogi.WriteErrorLog(" ---START ROW Message-- "+m_rawMessage+ " --END ROW Message--");

            //  T11 Transmission was successfully completed
            if (m_rawMessage.ToUpper().Contains("Transmission was successfully completed".ToUpper()))
            {
                dal.executesql("insert into ASIC_TXN_File(companyid,formdata,asicresponse,TRANS_STATUS) values('" + m_companyId + "','" + data201 + "','" + m_rawMessage.Replace("'", "''") + "','TRANSMIT SUCCESS')");
                dal.executesql("update companysearch set Asic_File='" + fileName + "',Asic_Error='" + m_rawMessage.Replace("'", "''") + "' where companyname='" + companyFullName + "'");
            }
            else if (m_rawMessage.ToUpper().Contains("OUT_TP")) // addad date April-14-2018 
            {
                dal.executesql("update companysearch set Asic_status='DOCUMENTS ACCEPTED',Asic_OUT_File='" + fileName + "' where CompamyName='" + companyFullName + "'");
            }
            else if (m_rawMessage.ToUpper().Contains("DOCUMENTS REJECTED"))
            {
                dal.executesql("update companysearch set Asic_status='DOCUMENTS REJECTED',Asic_File='" + fileName + "',Asic_Error='" + m_rawMessage.Replace("'", "''") + "' where CompamyName='" + companyFullName + "' and Asic_status='DOCUMENTS ACCEPTED'");
                dal.executesql("insert into ASIC_TXN_File(companyid,formdata,asicresponse,TRANS_STATUS) values('" + m_companyId + "','" + data201 + "','" + m_rawMessage.Replace("'", "''") + "','DOCUMENTS REJECTED')");
            }
            else if (m_rawMessage.ToUpper().Contains("DOCUMENTS ACCEPTED") || m_rawMessage.ToUpper().Contains("1DOCUMENTS ACCEPTED") || m_rawMessage.ToUpper().Contains("1ACCEPTED   1DOCUMENTS ACCEPTED001") || m_rawMessage.ToUpper().Contains("1ACCEPTED"))
            {
                dal.executesql("update companysearch set Asic_status='DOCUMENTS ACCEPTED',Asic_File='" + fileName + "',Asic_Error='" + m_rawMessage.Replace("'", "''") + "' where CompanyName='" + companyFullName + "'");
                dal.executesql("insert into ASIC_TXN_File(companyid,formdata,asicresponse,TRANS_STATUS) values('" + m_companyId + "','" + data201 + "','" + m_rawMessage.Replace("'", "''") + "','DOCUMENTS ACCEPTED')");
            }
            else if (m_rawMessage.ToUpper().Contains("ZHDASCRA56"))
            {
                string DocumentNo = obc.getDocumentNo_ra56(fileContent);
                dal.executesql("update companysearch set Asic_status='DOCUMENTS ACCEPTED',Asic_DocNo='" + DocumentNo + "',Asic_ResType='RA56',Asic_File='" + fileName + "',Asic_Error='" + m_rawMessage.Replace("'", "''") + "' where companyname='" + m_companyId + "'");
                dal.executesql("update [dbo].[ASIC_TXN_File] set ra56='" + m_rawMessage.Replace("'", "''") + "' where companyid='" + m_companyId + "' and TRANS_STATUS='DOCUMENTS ACCEPTED'");
            }
            else if (m_rawMessage.ToUpper().Contains("ZHDASCRA55"))
            {
                string acn = obc.getACN_ra55(fileContent);
                dal.executesql("update companysearch set Asic_status='DOCUMENTS ACCEPTED',Asic_acn='" + acn + "',Asic_ResType='RA55',Asic_File='" + fileName + "',Asic_Error='" + m_rawMessage.Replace("'", "''") + "' where Companyname='" + companyFullName + "'");
                string DocumentNo = obc.getDocumentNo_ra55(fileContent);
                dal.executesql("update companysearch set Asic_status='DOCUMENTS ACCEPTED',Asic_DocNo='" + DocumentNo + "',Asic_ResType='RA55',Asic_File='" + fileName + "',Asic_Error='" + m_rawMessage.Replace("'", "''") + "' Where Companyname='" + companyFullName + "'");
                dal.executesql("update [dbo].[ASIC_TXN_File] set ra55='" + m_rawMessage.Replace("'", "''") + "' where companyid='" + m_companyId + "' and TRANS_STATUS='DOCUMENTS ACCEPTED'");
            }
            else
            {
                dal.executesql("insert into ASIC_TXN_File(companyid,formdata,asicresponse,TRANS_STATUS) values('" + m_companyId + "','" + data201 + "','" + m_rawMessage.Replace("'", "''") + "','')");
                dal.executesql("update companysearch set Asic_File='" + fileName + "',Asic_Error='" + m_rawMessage.Replace("'", "''") + "' where Companyname='" + companyFullName + "'and Asic_status='DOCUMENTS ACCEPTED'");
            }

            if (RA55.CanParse(m_rawMessage))
                new RA55(m_rawMessage, m_companyId);

            if (RA56.CanParse(m_rawMessage))
                new RA56(m_rawMessage, m_companyId);
        }
        private string gettext(string filename)
        {
            string dta = "";
            try
            {
                string textme = System.IO.File.ReadAllText(@"C:/asicfiles/" + filename, System.Text.Encoding.UTF8);//.Replace("\r\n", "\n");
                dta = textme;
            }
            catch(Exception ex){}
            return dta;
        }
		public bool NoPendingFiles { get; private set; }
		}
	}