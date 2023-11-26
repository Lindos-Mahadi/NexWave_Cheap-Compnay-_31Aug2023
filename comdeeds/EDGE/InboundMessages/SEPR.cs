using System;

namespace comdeeds.EDGE.InboundMessages
	{
	/// <summary>
	/// BOUT separator; used in conjunction with BOUT. Not to be instantiated directly.
	/// </summary>
	internal class SEPR : InboundMessage
		{
		public SEPR (string rawMessage) : base (rawMessage)
			{
			}
        public SEPR(string rawMessage, string companyId)
            : base(rawMessage, companyId)
        {
        }
		public override string MessageName { get { return "SEPR"; } }

		public static bool CanParse (string rawMessage)
			{
			return rawMessage.StartsWith ("XSVSEPR");
			}

		public override void Parse ()
			{
			try
				{
				Filename = m_rawMessage.Substring (10 , 12);
				}
			catch (Exception e)
				{
				throw new Exception ("Unable to parse " + m_rawMessage + "\n" + e.Message);
				}
			}

		public string Filename { get; set; }
		}
	}