using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
{
    public class F410: OutboundMessage
		{
		private int m_messageTraceNumber;
		private Form410Data m_data;
		private string m_certificateIdentifier;

        public F410(Form410Data data, int messageTraceNumber, string certificate, 
					 X509Certificate2 x509)
			{
			m_data = data;
			m_messageTraceNumber = messageTraceNumber;
			m_certificate = certificate;
			m_x509 = x509;
            m_certificateIdentifier = System.Configuration.ConfigurationManager.AppSettings["s_serialidentifier"].ToString();// m_x509.SerialNumber;
			}

		public override bool Validate ()
			{
			return m_data.Validate ();
			}

		public override string MessageToSend (bool validateAndThrow = false)
			{
			if (validateAndThrow && !Validate ())
				throw new Exception (ErrorMsg);

			string message = string.Format ("ZHDASC410\t800\t{0}\n" , m_messageTraceNumber);
            message += string.Format("ZCO{0}\t\t{1}\t{2}\t\t{3}\t\t\t\t\t\t\t\t\t\t\t\t\t{4}\t\n",
                m_data.NameReserved, m_data.Companytype, m_data.Companyclass,BooleanRep ( m_data.Istheproposednameidenticaltoaregisteredbusinessname), m_data.ABNofbusinessname);

            message += string.Format("ZPR{0}\t{1}\t{2}\n", m_data.Identicalbusinessnames, m_data.Placeofregistrationofbusinessname, m_data.Registrationnumberofbusinessname);

            message += string.Format("ZUH\t\t\t\t{0}\n", m_data.PartandDivision);

            message += string.Format("ZDO\t\t\t\t{0}\t{1}\n", m_data.Currentregisteredcompanyname,m_data.ACNorARBN);

            message += string.Format("ZNR\t\t{0}\t{1}\t{2}\n", m_data.Reasonforextension, m_data.Reservationdocumentnumber, DateRep(m_data.Existingexpirydate));
            message += string.Format("ZCG{0}\t{1}\t{2}\n", m_data.Nameofapplicant_inv, m_data.Nameofapplicant_org, m_data.ACN_ARBNofapplicant);
            message += string.Format("ZDC{0}\t{1}\t{2}\t{3}\n", m_data.SignatoryName, m_data.SignatoryRole,
                DateRep(m_data.DateSigned), BooleanRep(m_data.isDeclarationisTrue));
            message += string.Format("ZAM{0}\t{1}\n", BooleanRep(m_data.Requestapplicationbemanuallyreviewed));
            foreach (string text in m_data.Text)
            {
                message += string.Format("ZTE{0}\n", text);
            }
            message += string.Format("ZTREND410\t{0}\n", message.Count(c => c == '\n') + 1);
            string messageToSign = message;
            message += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", m_x509.DistinguishedNameOpenSSLFormat(),
                m_certificateIdentifier);
            message += Sign(messageToSign);

			return message;
			}
		}
}