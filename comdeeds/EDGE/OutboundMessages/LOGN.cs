using System;

namespace comdeeds.EDGE.OutboundMessages
	{
	/// <summary>
	/// Client login; the server responds with a LOGR or a SVER with the corresponding error inside.
	/// </summary>
	public class LOGN : OutboundMessage
		{
		private string m_user;
		private string m_password;

		/// <summary>
		/// User credentials
		/// </summary>
		/// <param name="user">EDGE user id</param>
		/// <param name="password">EDGE user password</param>
		public LOGN (string user , string password)
			{
			m_user = user;
			m_password = password;
			}

		public override bool Validate ()
			{
			if (string.IsNullOrEmpty (m_user) || m_user.Length > 6)
				{
				ErrorMsg = "Please check the username. It can't be empty and it can't be more than 6 characters.";
				return false;
				}

			if (string.IsNullOrEmpty (m_password) || m_password.Length > 16)
				{
				ErrorMsg = "Please check the password. It can't be empty and it can't be more than 16 characters.";
				return false;
				}

			return true;
			}

		public override string MessageToSend (bool validateAndThrow = false)
			{
			if (validateAndThrow && !Validate ())
				throw new Exception (ErrorMsg);

			return string.Format ("XSCLOGN\n{0,-6}{1,-16}" , m_user , m_password);
			}
		}
	}