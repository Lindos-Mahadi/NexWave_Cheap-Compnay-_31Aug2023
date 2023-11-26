using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comdeeds.EDGE.InboundMessages
	{
	class UnknownMessage : InboundMessage
		{
		public UnknownMessage (string rawMessage) : base (rawMessage)
			{
			
			}

		public override string MessageName { get { return "Unknown message"; } }

		public static bool CanParse (string rawMessage)
			{
			return true;
			}

		public override void Parse ()
			{
                //("Unknown message. Unable to parse " + m_rawMessage);
			//throw new Exception ("Unknown message. Unable to parse " + m_rawMessage);
			}
		}
	}
