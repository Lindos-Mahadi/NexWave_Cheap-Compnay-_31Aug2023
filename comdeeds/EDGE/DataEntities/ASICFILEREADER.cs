using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using comdeeds.EDGE.CompositeElements;

namespace comdeeds.EDGE.DataEntities
{
    public class ASICFILEREADER
    {   protected string m_rawMessage_content;
        protected string m_filename;
        public ASICFILEREADER()
        {

        }
       
        public ASICFILEREADER(string rawMessage, string fileName)
        {
            m_rawMessage_content = rawMessage;
            m_filename = fileName;
            Parse();
        }

        public string RawMessage { get { return m_rawMessage_content; } }
        public string FileName { get { return m_filename; } }

        public void Parse()
        {
            try {
                if (m_filename.ToUpper().Contains("VALID_TP")) { 
                
                }
            }
            catch(Exception ex){}
        }
        public string companyname(string RawMessage)
        {
            string name = "";
            try
            {
                if (RawMessage.ToUpper().Contains("DOCUMENTS REJECTED") || RawMessage.ToUpper().Contains("DOCUMENTS ACCEPTED"))
                {
                    //string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    //string[] fieldData = segments[1].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                    //name = fieldData[0].ToUpper();
                    string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] fieldData = segments[10].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                    name = fieldData[0].ToUpper().Replace("//","_");
                    string[] fd = name.Split('_');
                    name = fd[fd.Length - 2];
                }
                if (RawMessage.ToUpper().Contains("ZHDASCRA56"))
                {
                    string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    StatusDetailsDataRA56 companyData = new StatusDetailsDataRA56();
                    string[] fieldData = segments[1].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                    name = fieldData[4];
                }
                if (RawMessage.ToUpper().Contains("ZHDASCRA55"))
                {
                    string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    CompanyDataRA55 companyData = new CompanyDataRA55();
                    string[] fieldData = segments[3].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                    name = fieldData[0];
                }
            }
            catch(Exception ex){}
            return name;
        }
        public string getACN_ra55(string RawMessage)
        {
            string name = "";
            if (RawMessage.ToUpper().Contains("ZHDASCRA55"))
            {
                string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                CompanyDataRA55 companyData = new CompanyDataRA55();
                string[] fieldData = segments[3].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                name = fieldData[1];
            }
            return name;
        }
        public string getDocumentNo_ra55(string RawMessage)
        {
            string name = "";
            if (RawMessage.ToUpper().Contains("ZHDASCRA55"))
            {
                string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] fieldData = segments[5].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                name = fieldData[2];
            }
            return name;
        }
        public string getDocumentNo_ra56(string RawMessage)
        {
            string name = "";
            if (RawMessage.ToUpper().Contains("ZHDASCRA56"))
            {
                string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                StatusDetailsDataRA56 companyData = new StatusDetailsDataRA56();
                string[] fieldData = segments[1].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                name = fieldData[5];
            }
            return name;
        }
    }
}