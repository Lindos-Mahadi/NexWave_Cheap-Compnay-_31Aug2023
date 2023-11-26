using System;

namespace comdeeds.EDGE.OutboundMessages
	{
	/// <summary>
	/// Client password change request; the server responds with a PWCH or a SVER with the corresponding error inside.
	/// </summary>
	public class PSWD : OutboundMessage
		{
		private string m_oldPassword;
		private string m_newPassword;

		/// <summary>
		/// Old and new password
		/// </summary>
		/// <param name="oldPassword">Old password, the one currently in use</param>
		/// <param name="newPassword">New password, the one that will be in use once the operation completes</param>
		public PSWD (string oldPassword , string newPassword)
			{
			m_oldPassword = oldPassword;
			m_newPassword = newPassword;
			}

		public override bool Validate ()
			{
			if (string.IsNullOrEmpty (m_oldPassword) || m_oldPassword.Length > 16)
				{
				ErrorMsg = "Please check the old password. It can't be empty and it can't be more than 16 characters.";
				return false;
				}

			if (string.IsNullOrEmpty (m_newPassword) || m_newPassword.Length > 16)
				{
				ErrorMsg = "Please check the new password. It can't be empty and it can't be more than 16 characters.";
				return false;
				}

			return true;
			}

		public override string MessageToSend (bool validateAndThrow = false)
			{
			if (validateAndThrow && !Validate ())
				throw new Exception (ErrorMsg);

			return string.Format ("XSCPSWD\n{0,-16}{1,-16}" , m_oldPassword , m_newPassword);
			}
		}
	}