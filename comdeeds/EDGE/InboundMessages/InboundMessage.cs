using System;

namespace comdeeds.EDGE.InboundMessages
	{
	/// <summary>
	/// Base class for all inbound messages. Not usable directly. Please use the InboundMessageFactory to insantiate the right message class.
	/// </summary>
	public class InboundMessage
		{
		protected string m_rawMessage;
        protected string m_companyId;
		public InboundMessage (string rawMessage)
			{
			m_rawMessage = rawMessage;
			Parse ();
			}

        public InboundMessage(string rawMessage,string companyId)
        {
            m_rawMessage = rawMessage;
            m_companyId = companyId;
            Parse();
        }
		/// <summary>
		/// Parses an ASIC date (string formatted as YYYYMMDDD) into a DateTime
		/// </summary>
		/// <param name="date">The string date</param>
		/// <returns></returns>
		public DateTime ParseDate (string date)
			{
			return new DateTime(Convert.ToInt16(date.Substring (0 , 4)) , Convert.ToInt16 (date.Substring (4 , 2)) , Convert.ToInt16 (date.Substring (6 , 2)));
			}

		/// <summary>
		/// The textual name of the messsage
		/// </summary>
		public virtual string MessageName { get { return "Unknown"; } }

		public string RawMessage { get { return m_rawMessage; } }
        public string CompanyId { get { return m_companyId; } }

		/// <summary>
		/// Instantiate the InboundMessage derived class that can parse the rawMessage.
		/// </summary>
		/// <param name="rawMessage">Raw ASIC EDGE server message.</param>
		/// <returns></returns>
		public static InboundMessage InboundMessageFactory (string rawMessage)
			{
			if (LOGR.CanParse (rawMessage))
				return new LOGR (rawMessage);

			if (SVER.CanParse (rawMessage))
				return new SVER (rawMessage);

			if (PWCH.CanParse (rawMessage))
				return new PWCH (rawMessage);

            if (RA55.CanParse(rawMessage))
                return new RA55(rawMessage);

			if (BOUT.CanParse (rawMessage))
				return new BOUT (rawMessage);

			if (SEPR.CanParse (rawMessage))
				return new SEPR (rawMessage);

            //if (RA55.CanParse(rawMessage)) //position changed by bhupi,because Bout always true
            //    return new RA55(rawMessage);

			return new UnknownMessage (rawMessage);
			}
        public static InboundMessage InboundMessageFactory(string rawMessage,string companyId)
        {
            if (LOGR.CanParse(rawMessage))
                return new LOGR(rawMessage,companyId);

            if (SVER.CanParse(rawMessage))
                return new SVER(rawMessage,companyId);

            if (PWCH.CanParse(rawMessage))
                return new PWCH(rawMessage,companyId);

            if (RA55.CanParse(rawMessage))
                return new RA55(rawMessage,companyId);

            if (BOUT.CanParse(rawMessage))
                return new BOUT(rawMessage,companyId);

            if (SEPR.CanParse(rawMessage))
                return new SEPR(rawMessage,companyId);

            //if (RA55.CanParse(rawMessage)) //position changed by bhupi,because Bout always true
            //    return new RA55(rawMessage);

            return new UnknownMessage(rawMessage);
        }
		/// <summary>
		/// Check to see if this class can parse the ASIC EDGE message received. Reimplement on all messages.
		/// </summary>
		/// <param name="rawMessage">Raw ASIC EDGE server message.</param>
		/// <returns></returns>
		public static bool CanParse (string rawMessage)
			{
			return false;
			}

		/// <summary>
		/// Parse the message received from the ASIC EDGE server.
		/// </summary>
		public virtual void Parse ()
			{
			throw new Exception ("Unable to parse " + m_rawMessage);
			}

		/// <summary>
		/// If true then the incoming message is a success message
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// Check to get the error code if the login was not accepted.
		/// </summary>
		public string ErrorMessage { get; protected set; }
		}
	}