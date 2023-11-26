using System;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
	{
	/// <summary>
	/// Base class for all outbound messages. Not usable directly.
	/// </summary>
	public class OutboundMessage
		{
		/// <summary>
		/// Validate the message acording to the rules from ASIC EDGE.
		/// </summary>
		/// <remarks>
		/// If an error is found the ErrorMsg should be set to reflect it.
		/// </remarks>
		/// <returns>Implement so that it returns true if the generated message is valid, false otherwise.</returns>
		public virtual bool Validate ()
			{
			return false;
			}

		/// <summary>
		/// Implement so that it packages the message to send through the protocol.
		/// </summary>
		/// <param name="validateAndThrow">
		/// If true the Validate () method will be invoked prior to sending the messege and if there are errors an exception is thrown.
		/// </param>
		/// <returns></returns>
		public virtual string MessageToSend (bool validateAndThrow = false)
			{
			return "Not implemented...";
			}

		/// <summary>
		/// If an error is found when Validate () is invoked this should contain the description of the error.
		/// </summary>
		public string ErrorMsg { get; set; }

		// --------------------------------------------------------------------
		// Methods used by messages that need signing (currently TXID and RA53)
		protected string Sign (string message)
			{
			message = m_x509.Sign (message);
			string signature = "";

			for (int i = 0 ; i < 200 ; i++)
				{
				if (message.Length < 64)
					{
                        signature += string.Format("ZXS{0}\n", message);
					break;
					}

				signature += string.Format ("ZXS{0}\n" , message.Substring (0 , 64));
				message = message.Substring (64);
				}

			return signature;
			}

		protected string InsertCertificate ()
			{
			string processedCertificate = "";
			string certificate = m_certificate;

			for (int i = 0 ; i < 200 ; i++)
				{
				if (certificate.Length < 64)
					{
					processedCertificate += string.Format ("ZXC{0}\n" , certificate);
					break;
					}

				processedCertificate += string.Format ("ZXC{0}\n" , certificate.Substring (0 , 64));
				certificate = certificate.Substring (64);
				}

			return processedCertificate;
			}

		protected string BooleanRep (bool boolValue)
			{
			return boolValue ? "Y" : "N";
			}

		protected string DateRep (DateTime dateValue)
			{
			return dateValue.ToString ("yyyyMMdd");
			}

		protected string m_certificate;
		protected X509Certificate2 m_x509;
		}
	}