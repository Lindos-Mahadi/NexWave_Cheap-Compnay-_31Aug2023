using System;

namespace comdeeds.EDGE.OutboundMessages
	{
	/// <summary>
	/// Client logout.
	/// Perform prior to closing the connection.
	/// </summary>
	public class LOUT : OutboundMessage
		{
		public override bool Validate ()
			{
			return true;
			}

		public override string MessageToSend (bool validateAndThrow = false)
			{
			if (validateAndThrow && !Validate ())
				throw new Exception (ErrorMsg);

			return "XSCLOUT\n";
			}
		}
	}