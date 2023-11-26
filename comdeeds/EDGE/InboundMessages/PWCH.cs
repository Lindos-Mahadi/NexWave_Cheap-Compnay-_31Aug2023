using System;

namespace comdeeds.EDGE.InboundMessages
	{
	/// <summary>
	/// Server response for password change request from the ASIC EDGE server. Not to be instantiated directly.
	/// </summary>
	internal class PWCH : InboundMessage
		{
		public PWCH (string rawMessage) : base (rawMessage)
			{
			}
        public PWCH(string rawMessage, string companyId)
            : base(rawMessage, companyId)
        {
        }
		public override string MessageName { get { return "PWCH"; } }

		public static bool CanParse (string rawMessage)
			{
			return rawMessage.StartsWith ("XSVPWCH");
			}

		public override void Parse ()
			{
			try
				{
				ErrorMessage = m_rawMessage.Substring (8 , 3);
				Success = ErrorMessage == "000";
				}
			catch (Exception e)
				{
				throw new Exception ("Unable to parse " + m_rawMessage + "\n" + e.Message);
				}
			}
		}
	}