using System;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
	{
	public class TXID : OutboundMessage
		{
		private string m_certificateIdentifier;
		private bool m_selfSigned;
		private int m_registeredAgentNo;
		private bool m_testTransmission;
		private string m_softwareVersion;
		private string m_packageSoftwareVersion;
		private int m_numberOfDocumentsInTransmission;
		private string m_innerMsg;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerMsg">Message to send that will be signed by TXID</param>
		/// <param name="softwareVersion">Software registration number. Use 99999 for testing purposes</param>
		/// <param name="packageSoftwareVersion"></param>
		/// <param name="numberOfDocumentsInTransmission"></param>
		/// <param name="registeredAgentNo"></param>
		/// <param name="testTransmission"></param>
		/// <param name="selfSigned"></param>
		/// <param name="certificate"></param>
		/// <param name="x509"></param>
		public TXID (string innerMsg , string softwareVersion , string packageSoftwareVersion , int numberOfDocumentsInTransmission , int registeredAgentNo , bool testTransmission = false , bool selfSigned = false , string certificate = "" , X509Certificate2 x509 = null)
			{
			m_innerMsg = innerMsg;
			m_softwareVersion = softwareVersion;
			m_packageSoftwareVersion = packageSoftwareVersion;
			m_numberOfDocumentsInTransmission = numberOfDocumentsInTransmission;
			m_testTransmission = testTransmission;
			m_registeredAgentNo = registeredAgentNo;
			m_selfSigned = selfSigned;
			m_certificate = certificate;
			m_x509 = x509;
            // m_certificateIdentifier = System.Configuration.ConfigurationManager.AppSettings["s_serialidentifier"].ToString();// m_x509.SerialNumber;
            m_certificateIdentifier= m_x509.SerialNumber;

        }

		public override bool Validate ()
			{
			return true;
			}

		public override string MessageToSend (bool validateAndThrow = false)
			{
			if (validateAndThrow && !Validate ())
				throw new Exception (ErrorMsg);

			string message = m_innerMsg;

			message += "ZHDASCTXID\t0500\n";
            //message += string.Format ("ZTX{0}\t{1}\t{2}\t{3}\t{4}\t0500\t{5}\n" , m_registeredAgentNo ,
            //    m_testTransmission ? "Y" : "N" , DateTime.Now.ToString ("yyyyMMdd") , m_softwareVersion , m_packageSoftwareVersion ,
            //    m_numberOfDocumentsInTransmission);
            message += string.Format("ZTX{0}\t{1}\t{2}\t{3}\t{4}\t0500\t{5}\n", m_registeredAgentNo,
                m_testTransmission ? "Y" : "N", DateRep(DateTime.Now), m_softwareVersion, "1.0.0",
                m_numberOfDocumentsInTransmission);
			message += "ZTRENDTXID\t3\n";









           /* if (m_selfSigned)
            {
                string toSign = message;

              message += InsertCertificate();// uncomment for RSA53
              message += string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", m_x509.DistinguishedNameOpenSSLFormat(),
                  m_certificateIdentifier);
             
                //message += string.Format("ZXI{0}\t\tMD5\tRSA\n", m_x509.DistinguishedNameOpenSSLFormat());

              message += Sign(toSign);
              //message += Sign(message);
            }*/
           
			return message;
			}
           ///do not sign for RA52
		}
	}