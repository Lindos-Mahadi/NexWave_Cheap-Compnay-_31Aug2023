using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
{
    /// <summary>
    /// Form RA50 - Request withdrawal of form 201 (version 1.00)
    /// </summary>
    public class RA54 : OutboundMessage
    {
        private int m_messageTraceNumber;
        private PersonName m_signatoryName;
        private string m_asicPin;
        private string m_certificateIdentifier;

        // IMPORTANT NOTE: for testing RA53 the ASIC pin can be the same as the test user id
        public RA54(int messageTraceNumber, string certificate, PersonName datara54, string asicPin,
                     X509Certificate2 x509)
        {
            m_messageTraceNumber = messageTraceNumber;
            m_certificate = certificate;
            m_signatoryName = datara54;
            m_asicPin = asicPin;
            m_x509 = x509;
            m_certificateIdentifier = System.Configuration.ConfigurationManager.AppSettings["s_serialidentifier"].ToString();//m_x509.SerialNumber;
        }

        public override bool Validate()
        {
            //if (!m_registrationrequest.Validate())
            //{
            //    ErrorMsg = "Signatory name - " + m_signatoryName.ErrorMsg;
            //    return false;
            //}
            return true;
        }

        public override string MessageToSend(bool validateAndThrow = false)
        {
            if (validateAndThrow && !Validate())
                throw new Exception(ErrorMsg);

            string message = string.Format("ZHDASCRA54\t0800\t{0}\n", m_messageTraceNumber);
            message += string.Format("ZXI{0}\t\t\t\t\t\t\t{1}\n", m_x509.DistinguishedNameOpenSSLFormat(),System.Configuration.ConfigurationManager.AppSettings["s_serialidentifier"].ToString());// m_x509.SerialNumber);

            //message += string.Format("ZDC{0}\t\t{1}\t{2}\t{3}\n", m_datara54.SignatoryName, DateRep(m_datara54.DateSigned), BooleanRep(m_datara54.CanDigitallySignTransmission), BooleanRep(m_datara54.CanDigitallySignCompanyForms));

            message += string.Format("ZDC{0}\t{1}\t{2}\t{3}\t\t{4}\t{5}\t{6}\n", m_signatoryName.FamilyName.ToUpper(), m_signatoryName.GivenName1.ToUpper(),
               m_signatoryName.GivenName2.ToUpper(), m_signatoryName.GivenName3.ToUpper(), DateTime.Now.ToString("yyyyMMdd"), "Y", "Y");

            //message += string.Format("ZDC{0}\t{1}\t{2}\t{3}\t\t{4}\tY\tY\t\t\t\t\t\t{5}\n",
            //   m_signatoryName.FamilyName.ToUpper(), m_signatoryName.GivenName1.ToUpper(),
            //   m_signatoryName.GivenName2.ToUpper(), m_signatoryName.GivenName3.ToUpper(), DateTime.Now.ToString("yyyyMMdd"),
            //   m_asicPin);


            message += string.Format("ZTRENDRA54\t{0}\n", message.Count(c => c == '\n') + 1);
            //string messageToSign = message;
            //message += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", m_x509.DistinguishedNameOpenSSLFormat(), "0d");
            //message += Sign(messageToSign);
            //message += InsertCertificate();
            return message;
        }
        public string getCertificate()
        {
            string cert = InsertCertificate();
            return cert;
        }
    }
}