using System;
using System.Collections.Generic;
using System.IO;
using comdeeds.EDGE.CompositeElements;

namespace comdeeds.EDGE.InboundMessages
{
    /// <summary>
    /// Created by BHU Advice of ASIC processing status 
    /// </summary>
    internal class RA57 : InboundMessage
    {
        public RA57(string rawMessage)
            : base(rawMessage)
        {
        }
        public RA57(string rawMessage, string companyId)
            : base(rawMessage, companyId)
        {
        }
        public override string MessageName { get { return "RA57"; } }

        public static bool CanParse(string rawMessage)
        {
            return rawMessage.StartsWith("ZHDASCRA57");
        }

        public override void Parse()
        {
            try
            {
                string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                TransactionDetailsDataRA57 companyData = new TransactionDetailsDataRA57();

                companyData.StatusText = new List<string>();
                int ndx = 0;
                while (segments[ndx].StartsWith("ZTE"))
                {
                    companyData.StatusText.Add(segments[ndx].Substring(3));
                    ndx++;
                }

                companyData.TXNdetails = new List<TransactionsRa57>();
                ndx = 1;
                while (segments[ndx].StartsWith("ZOL"))
                {
                    string[] fieldData = segments[ndx].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                    TransactionsRa57 obj = new TransactionsRa57();
                    obj.Transactiondate = Convert.ToDateTime(fieldData[0]);
                    obj.Transactionlegend = fieldData[1];
                    obj.Transactionstatus = fieldData[2];
                    obj.Transactionvalue = fieldData[3];
                    obj.Transactionoutstandingvalue = fieldData[4];
                    obj.Transactionreference = fieldData[5];
                    obj.Transactionallocationreference = fieldData[6];
                    obj.ACN = fieldData[13];
                    companyData.TXNdetails.Add(obj);
                    ndx++;
                }             
                // TODO: implement whatever needs to be done with this data, if anything
                Success = true;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to parse " + m_rawMessage + "\n" + e.Message);
            }
        }
    }
}