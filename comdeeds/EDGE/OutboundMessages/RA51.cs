using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
{
    /// <summary>
    /// Form RA51 - Request reprint of registration certificate (version 8.00)
    /// </summary>
    public class RA51 : OutboundMessage
    {
        private int m_messageTraceNumber;
        private CompanyDataRA51 m_companydata;
        private string m_asicPin;
        private string m_certificateIdentifier;

        // IMPORTANT NOTE: for testing RA53 the ASIC pin can be the same as the test user id
        public RA51(int messageTraceNumber, string certificate, CompanyDataRA51 companydata, string asicPin,
                     X509Certificate2 x509)
        {
            m_messageTraceNumber = messageTraceNumber;
            m_certificate = certificate;
            m_companydata = companydata;
            m_asicPin = asicPin;
            m_x509 = x509;
            m_certificateIdentifier =  System.Configuration.ConfigurationManager.AppSettings["s_serialidentifier"].ToString();// m_x509.SerialNumber;
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

            string message = string.Format("ZHDASCRA51\t1000\t{0}\n", m_messageTraceNumber);
            message += string.Format("ZCO{0}\t{1}\n", m_companydata.CompanyName, m_companydata.acn);
            //message += string.Format("ZAM\t\t\t{0}\n", m_companydata.RegistrationCertificatedeliveryoption);
            message += string.Format("ZAM\t\t{0}\n", m_companydata.RegistrationCertificatedeliveryoption);
            message += string.Format("ZTRENDRA51\t{0}\n", message.Count(c => c == '\n') + 1);
            string messageToSign = message;
            return message;
        }
    }
}