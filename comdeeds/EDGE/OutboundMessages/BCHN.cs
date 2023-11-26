using System;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
	{
	public class BCHN : OutboundMessage
		{
		private string m_file;
		private string m_extension;

		public BCHN (string file , string extension)
			{
			m_file = file;
			m_extension = extension;
			}

		public override bool Validate ()
			{
			return true;
			}

		public override string MessageToSend (bool validateAndThrow = false)
			{
			if (validateAndThrow && !Validate ())
				throw new Exception (ErrorMsg);

			string message = "XSCBCHN\n";
            //message += string.Format ("{0,-8}.{1,-3}\n" , m_file , m_extension);
            message += m_file+"."+m_extension+"\n";
			return message;
			}
		}
	}