using System;
using System.Collections.Generic;
using System.IO;
using comdeeds.EDGE.CompositeElements;

namespace comdeeds.EDGE.InboundMessages
{
    /// <summary>
    /// Created by BHU Advice of ASIC processing status 
    /// </summary>
    internal class RA59 : InboundMessage
    {
        public RA59(string rawMessage)
            : base(rawMessage)
        {
        }
        public RA59(string rawMessage, string companyId)
            : base(rawMessage, companyId)
        {
        }
        public override string MessageName { get { return "RA59"; } }

        public static bool CanParse(string rawMessage)
        {
            return rawMessage.StartsWith("ZHDASCRA59");
        }

        public override void Parse()
        {
            try
            {
                string[] segments = RawMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                CertificateidentifierDataRA59 companyData = new CertificateidentifierDataRA59();

                companyData.StatusText = new List<string>();
                int ndx = 0;
                while (segments[ndx].StartsWith("ZTE"))
                {
                    companyData.StatusText.Add(segments[ndx].Substring(3));
                    ndx++;
                }

                companyData.CIdetails = new List<CertificateidentifierRa59>();
                ndx = 1;
                while (segments[ndx].StartsWith("ZXI"))
                {
                    string[] fieldData = segments[ndx].Substring(3).Split(new[] { '\t' }, StringSplitOptions.None);
                    CertificateidentifierRa59 obj = new CertificateidentifierRa59();
                    obj.Distinguishednameofcertificateauthority = fieldData[0];
                    obj.Distinguishednameofcertificatesubject = fieldData[4];
                    obj.Authorisedtosigntransmissions = fieldData[5];
                    obj.Authorisedtosigndocuments = fieldData[6];
                    obj.Serialidentifier200 = fieldData[7];
                    companyData.CIdetails.Add(obj);
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