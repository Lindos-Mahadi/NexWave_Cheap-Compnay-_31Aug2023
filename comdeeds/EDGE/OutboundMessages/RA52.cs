using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
{
    /// <summary>
    /// Form RA52 - Request account transaction listing (version 3.00)
    /// </summary>
    public class RA52 : OutboundMessage
    {
        private int m_messageTraceNumber;
        private AccountTransactionRA52 m_accounttransaction;
        private string m_asicPin;
        private string m_certificateIdentifier;

        // IMPORTANT NOTE: for testing RA53 the ASIC pin can be the same as the test user id
        public RA52(int messageTraceNumber, string certificate, AccountTransactionRA52 accounttransaction, string asicPin,
                     X509Certificate2 x509)
        {
            m_messageTraceNumber = messageTraceNumber;
            m_certificate = certificate;
            m_accounttransaction = accounttransaction;
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

            string message = string.Format("ZHDASCRA52\t0300\t{0}\n", m_messageTraceNumber);
            message += string.Format("ZAT\t\t\t\t{0}\t{1}\n", Convert.ToDateTime(m_accounttransaction.startdate).ToString("yyyyMMdd"), Convert.ToDateTime(m_accounttransaction.enddate).ToString("yyyyMMdd"));
            message += string.Format("ZTRENDRA52\t{0}\n", message.Count(c => c == '\n') + 1);
            string messageToSign = message;
            return message;
        }
    }
}