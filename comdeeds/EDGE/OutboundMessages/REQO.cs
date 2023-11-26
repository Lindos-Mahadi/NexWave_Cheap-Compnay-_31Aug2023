using System;

namespace comdeeds.EDGE.OutboundMessages
	{
	/// <summary>
	/// Client request DIS Output.
	/// Gets the DIS Output and returns any waiting files (through BOUT).
	/// </summary>
	public class REQO : OutboundMessage
		{
		public override bool Validate ()
			{
			return true;
			}

		public override string MessageToSend (bool validateAndThrow = false)
			{
			if (validateAndThrow && !Validate ())
				throw new Exception (ErrorMsg);

			return "XSCREQO\n";
			}
		}
	}