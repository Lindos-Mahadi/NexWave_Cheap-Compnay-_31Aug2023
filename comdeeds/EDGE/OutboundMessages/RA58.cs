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
    public class RA58 : OutboundMessage
    {
        private int m_messageTraceNumber;
        private DATA_RA58 m_datara58;
        private string m_asicPin;
        private string m_certificateIdentifier;

        // IMPORTANT NOTE: for testing RA53 the ASIC pin can be the same as the test user id
        public RA58(int messageTraceNumber, string certificate, DATA_RA58 datara58, string asicPin,
                     X509Certificate2 x509)
        {
            m_messageTraceNumber = messageTraceNumber;
            m_certificate = certificate;
            m_datara58 = datara58;
            m_asicPin = asicPin;
            m_x509 = x509;
            m_certificateIdentifier = System.Configuration.ConfigurationManager.AppSettings["s_serialidentifier"].ToString();// m_x509.SerialNumber;
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

            string message = string.Format("ZHDASCRA58\t0100\t{0}\n", m_messageTraceNumber);
            message += string.Format("ZDC{0}\t{1}\n", m_datara58.SignatoryName, DateRep(m_datara58.DateSigned));
            message += string.Format("ZTRENDRA58\t{0}\n", message.Count(c => c == '\n') + 1);
            string messageToSign = message;
            message += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", m_x509.DistinguishedNameOpenSSLFormat(), m_x509);
            message += Sign(messageToSign);
            //message += InsertCertificate();
            return message;
        }
    }
}