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
    public class RA50 : OutboundMessage
    {
        private int m_messageTraceNumber;
        private RegistrationRequest m_registrationrequest;
        private string m_asicPin;
        private string m_certificateIdentifier;

        // IMPORTANT NOTE: for testing RA53 the ASIC pin can be the same as the test user id
        public RA50(int messageTraceNumber, string certificate, RegistrationRequest registrtionrequest, string asicPin,
                     X509Certificate2 x509)
        {
            m_messageTraceNumber = messageTraceNumber;
            m_certificate = certificate;
            m_registrationrequest = registrtionrequest;
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

            string message = string.Format("ZHDASCRA50\t1000\t{0}\n", m_messageTraceNumber);
            message += string.Format("ZNR\t\t\t\t{0}\t{1}\n", m_registrationrequest.ProposedCompanyName, m_registrationrequest.DocumentNumber);
            message += string.Format("ZDC{0}\t\t\t{1}\t{2}\n", m_registrationrequest.SignatoryName, DateRep(m_registrationrequest.DateSigned), BooleanRep(m_registrationrequest.DeclarationInN126HasBeenAssentedTo));
            message += string.Format("ZTRENDRA50\t{0}\n", message.Count(c => c == '\n') + 1);
            string messageToSign = message;
            //message += InsertCertificate();
            //message += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", m_x509.DistinguishedNameOpenSSLFormat(),"0d");
            //message += Sign(messageToSign );

            return message;
        }
    }
}