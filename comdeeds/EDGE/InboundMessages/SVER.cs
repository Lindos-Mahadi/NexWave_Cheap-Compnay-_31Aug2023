using System;

namespace comdeeds.EDGE.InboundMessages
	{
	/// <summary>
	/// Server error message. Not to be instantiated directly.
	/// </summary>
	internal class SVER : InboundMessage
		{
		public SVER (string rawMessage) : base (rawMessage)
			{
			}
        public SVER(string rawMessage, string companyId)
            : base(rawMessage, companyId)
        {
        }
		public override string MessageName { get { return "SVER"; } }

		public static bool CanParse (string rawMessage)
			{
			return rawMessage.StartsWith ("XSVSVER");
			}

		public override void Parse ()
			{
			try
				{
				ErrorCode = m_rawMessage.Substring (8 , 3);
				ErrorMessage = ErrorCode + " - " + m_rawMessage.Substring (11);
				}
			catch (Exception e)
				{
				throw new Exception ("Unable to parse " + m_rawMessage + "\n" + e.Message);
				}
			}

		public string ErrorCode { get; set; }
		}
	}