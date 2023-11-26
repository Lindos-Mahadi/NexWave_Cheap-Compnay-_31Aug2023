using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace comdeeds.EDGE
	{
	public class EDGETransport
		{
        ErrorLog oErrorLog = new ErrorLog();

        private readonly TcpClient m_client;
		private NetworkStream m_networkStream;
		private SslStream m_stream;

		private static int s_bufferSize = 102400;
		private bool m_validateServerCertificate;

		public EDGETransport (bool validateServerCertificate = true)
			{
			m_client = new TcpClient ();
			m_validateServerCertificate = validateServerCertificate;
			}

		public bool Connect (String server , int port)
			{
            try
            {
                m_client.Connect (server , port);

                if (m_client.Connected == true)
                {
                    m_networkStream = m_client.GetStream();

                    if (m_validateServerCertificate)
                        m_stream = new SslStream(m_networkStream, true);
                    else
                        m_stream = new SslStream(m_networkStream, true, ValidateCertificate);

                    m_stream.AuthenticateAsClient(server);
                    return true;
                }
                else
                { return false; }

				}
			catch (Exception ex)
				{
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return true;
        }

		// TODO: review - workaround for expired certificate
		private bool ValidateCertificate (object sender , X509Certificate certificate , X509Chain chain , SslPolicyErrors sslpolicyerrors)
			{
			return true;
			}

		public void Disconnect ()
			{
			m_stream.Close ();
			m_client.Close ();
			}

		public void Send (string msg , bool last = false)
			{
                try
                {
                    string message = "BDAT " + msg.Length + (last ? " LAST" : "") + "\r\n" + msg;

                    Write(message);
                    Debug.Print(string.Format("\r\n-----------------------------------------------\r\n{0}: request:\r\n", DateTime.Now));
                    Debug.Print(string.Format("[{0}]\r\n", message));
                }
                catch (Exception ex)
                {
                    oErrorLog.WriteErrorLog(ex.ToString());
                }	
			}

		public string Receive ()
			{
            string respTail = "";
            try
            {
                string resp = Response();

                if (string.IsNullOrEmpty(resp))
                    return "ERROR: empty response";     // TODO: throw exception

                Debug.Print(string.Format("\r\n-----------------------------------------------\r\n{0}: response:\r\n", DateTime.Now));
                Debug.Print(string.Format("[{0}", resp));

                if (resp != "")
                {
                    // parse the BDAT <length> LAST message
                    resp = resp.Substring(5);
                    resp = resp.Substring(0, resp.IndexOf(" "));

                    int responseLength = Convert.ToInt32(resp);
                    respTail = Response(responseLength);
                    Debug.Print(string.Format("{0}]\r\n", respTail));
                    EdgeLog oedgeLog = new EdgeLog();
                    oedgeLog.WriteErrorLog(string.Format("{0}]\r\n", respTail));
                }
                  
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString());
            }
            return respTail;

        }
		#region Internals

		// TODO: review & refactor - see send/receive
		private void Write (String str)
			{

			ASCIIEncoding encoding = new ASCIIEncoding ();

			byte [] bufferBytes = encoding.GetBytes (str);

			try
				{
				m_stream.Write (bufferBytes , 0 , bufferBytes.Length);
				}
			catch (Exception ex)
				{
                oErrorLog.WriteErrorLog(ex.ToString());
				}
			}

		private string Response ()
			{
			ASCIIEncoding encoding = new ASCIIEncoding ();
			
			byte [] bufferBytes = new byte [s_bufferSize];
			int count = 0;

			while (true)
				{
				byte [] receiveBuffer = new byte [2];
				int byteCount = 0;

				try
					{
					byteCount = m_stream.Read (receiveBuffer , 0 , 1);
					}
				catch (Exception ex)
					{
					throw ex;
					}

				if (byteCount == 1)
					{
					bufferBytes [count] = receiveBuffer [0];
					count++;

					if (receiveBuffer [0] == '\n')
						{
						break;
						}
					}
				else
					{
					break;
					}

				if (encoding.GetString (bufferBytes , 0 , count) == "\u0017\u0003\u0001\0@")
					{
					count = 0;
					break;
					}
				}

			return encoding.GetString (bufferBytes , 0 , count);
			}

		private string Response (int readLength)
			{
			ASCIIEncoding encoding = new ASCIIEncoding ();

			byte [] bufferBytes = new byte [s_bufferSize];

           //long byte[] bufferByteslong = new long byte[s_bufferSize];

			int count = 0;

			while (count < readLength)
				{
				byte [] receiveBuffer = new byte [2];
				int byteCount = m_stream.Read (receiveBuffer , 0 , 1);
                if (byteCount == 1)
                {
                    try
                    {
                        bufferBytes[count] = receiveBuffer[0];
                    }
                    catch (Exception ex) { }

                    count++;
                }
                else
                {
                    break;
                }

				if (encoding.GetString (bufferBytes , 0 , count) == "\u0017\u0003\u0001\0@")
					{
					count = 0;
					break;
					}
				}

			return encoding.GetString (bufferBytes , 0 , count);
			}

		#endregion
		}
	}
