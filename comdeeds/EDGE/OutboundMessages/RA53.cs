using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
{
    /// <summary>
    /// Form RA53 - Authorise X.509 certificate (version 8.00)
    /// </summary>
    public class RA53 : OutboundMessage
    {
        private int m_messageTraceNumber;
        private PersonName m_signatoryName;
        private string m_asicPin;
        private string m_certificateIdentifier;

        // IMPORTANT NOTE: for testing RA53 the ASIC pin can be the same as the test user id
        public RA53(int messageTraceNumber, string certificate, PersonName signatoryName, string asicPin,
                     X509Certificate2 x509)
        {
            m_messageTraceNumber = messageTraceNumber;
            m_certificate = certificate;
            m_signatoryName = signatoryName;
            m_asicPin = asicPin;
            m_x509 = x509;
            m_certificateIdentifier = System.Configuration.ConfigurationManager.AppSettings["s_serialidentifier"].ToString();// m_x509.SerialNumber;
        }

        public override bool Validate()
        {
            if (!m_signatoryName.Validate())
            {
                ErrorMsg = "Signatory name - " + m_signatoryName.ErrorMsg;
                return false;
            }

            return true;
        }

        public override string MessageToSend(bool validateAndThrow = false)
        {
            if (validateAndThrow && !Validate())
                throw new Exception(ErrorMsg);

            string message = string.Format("ZHDASCRA53\t0800\t{0}\n", m_messageTraceNumber);
            message += InsertCertificate();
            //message += string.Format("ZDC{0}\t{1}\t{2}\t{3}\t\t{4}\tY\tY\t\t\t\t\t\t{5}\n",
            //    m_signatoryName.FamilyName.ToUpper(), m_signatoryName.GivenName1.ToUpper(),
            //    m_signatoryName.GivenName2.ToUpper(), m_signatoryName.GivenName3.ToUpper(), DateTime.Now.ToString("yyyyMMdd"),
            //    m_asicPin);
            message += string.Format("ZDC{0}\t{1}\t{2}\t{3}\t\t{4}\tY\tY\t\t\t\t\t\t\t{5}\n",
              m_signatoryName.FamilyName.ToUpper(), m_signatoryName.GivenName1.ToUpper(),
              m_signatoryName.GivenName2.ToUpper(), m_signatoryName.GivenName3.ToUpper(), DateTime.Now.ToString("yyyyMMdd"),
              m_asicPin);
            //message += string.Format("ZDC{0}\t{1}\t{2}\t{3}\t\t{4}\tY\tY\n",
            //    m_signatoryName.FamilyName.ToUpper(), m_signatoryName.GivenName1.ToUpper(),
            //    m_signatoryName.GivenName2.ToUpper(), m_signatoryName.GivenName3.ToUpper(), DateTime.Now.ToString("yyyyMMdd"));

            message += string.Format("ZTRENDRA53\t{0}\n", message.Count(c => c == '\n') + 1);

            string messageToSign = message;

            //message += InsertCertificate();
            //message += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", m_x509.DistinguishedNameOpenSSLFormat(), m_certificateIdentifier);
            //message += Sign(messageToSign);
            //message += Sign(message);
            return message;
        }

        public string MessageToSend_withoutSign(bool validateAndThrow = false)
        {
            if (validateAndThrow && !Validate())
                throw new Exception(ErrorMsg);

            string message = string.Format("ZHDASCRA53\t800\t{0}\n", m_messageTraceNumber);
            message += InsertCertificate();
            message += string.Format("ZDC{0}\t{1}\t{2}\t{3}\t\t{4}\tY\tY\t\t\t\t\t\t{5}\n",
                m_signatoryName.FamilyName.ToUpper(), m_signatoryName.GivenName1.ToUpper(),
                m_signatoryName.GivenName2.ToUpper(), m_signatoryName.GivenName3.ToUpper(), DateTime.Now.ToString("yyyyMMdd"),
                m_asicPin);
            message += string.Format("ZTRENDRA53\t{0}\n", message.Count(c => c == '\n') + 1);

            string messageToSign = message;

            message += InsertCertificate();
            message += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", m_x509.DistinguishedNameOpenSSLFormat(),
                m_certificateIdentifier);
            message += Sign(messageToSign) + "\n";

            return message;
        }
        public string getCertificate()
        {
            string cert = InsertCertificate();
            return cert;
        }
    }
}