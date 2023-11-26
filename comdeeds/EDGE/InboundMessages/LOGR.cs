using System;

namespace comdeeds.EDGE.InboundMessages
	{
	/// <summary>
	/// Client login reply from the ASIC EDGE server. Not to be instantiated directly.
	/// </summary>
	internal class LOGR : InboundMessage
		{
		public LOGR (string rawMessage) : base (rawMessage)
			{
			}
        public LOGR(string rawMessage, string companyId)
            : base(rawMessage, companyId)
        {
        }
		public override string MessageName { get { return "LOGR"; } }

		public static bool CanParse (string rawMessage)
			{
			return rawMessage.StartsWith ("XSVLOGR");
			}

		public override void Parse ()
			{
			try
				{
				ErrorMessage = m_rawMessage.Substring (8 , 3);
				LoginAccepted = ErrorMessage == "000";

				if (LoginAccepted)
					{
					Success = true;
					NextClientState = Convert.ToInt16 (m_rawMessage.Substring (11 , 1));
					switch (NextClientState)
						{
							case 0:
								NextClientStateDescription = "Disconnect";
								break;

							case 2:
								NextClientStateDescription = "Send command state message";
								break;

							case 3:
								NextClientStateDescription = "Receive reports";
								break;

							case 5:
								NextClientStateDescription = "Change password";
								break;
						}

					LastFileSent = m_rawMessage.Substring (12 , 12).Trim();
					}
				}
			catch (Exception e)
				{
				throw new Exception ("Unable to parse " + m_rawMessage + "\n" + e.Message);
				}
			}

		public bool LoginAccepted { get; private set; }
		public short NextClientState { get; private set; }
		public string NextClientStateDescription { get; private set; }
		public string LastFileSent { get; private set; }
		}
	}