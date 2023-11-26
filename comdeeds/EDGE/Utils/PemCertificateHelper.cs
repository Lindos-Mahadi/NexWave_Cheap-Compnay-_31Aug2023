using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Configuration;

namespace comdeeds.EDGE.Utils
{
    #region X509PemCertificateHelper

    public static class X509PemCertificateHelper
    {
        public static string FromPemFile(this X509Certificate2 x509, string fileName)
        {
            x509.Import(fileName);
            string certText = ReadCertFile(fileName);

            string b64privateKey = "";

            b64privateKey = ReadPrivateKey1(certText);//production server
          // b64privateKey = ReadPrivateKey(certText);  //testing server

            if (b64privateKey != "")
            {
                byte[] binkey = Convert.FromBase64String(b64privateKey);

                string dd = "";
                for (int i = 0; i < binkey.Length; i++)
                {
                    dd += Char.ConvertFromUtf32(binkey[i]);
                }
                MemoryStream mem = new MemoryStream(binkey);
                BinaryReader binr = new BinaryReader(mem);
                string unsig = "";
                for (int i = 0; i < binkey.Length; i++)
                {
                    try
                    {
                        unsig += Char.ConvertFromUtf32(binr.ReadUInt16());
                    }
                    catch (Exception ex) { }
                    //unsig += Char.ConvertFromUtf32(binkey[i]);
                }

               // x509.PrivateKey = DecodeRSAPrivateKeyReal(binkey);  // testing server
               x509.PrivateKey = DecodeRSAPrivateKey(binkey);  // production server
            }

            return ReadCertificate(certText);
        }

        /// <summary>
        /// Signin certifiacte CODE HERE
        /// </summary>

        public static string Sign(this X509Certificate2 x509, string message)
        {
            //byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] data = Encoding.ASCII.GetBytes(message);
            byte[] signedData;

            using (MD5 hasher = MD5.Create())
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509.PrivateKey;
                signedData = rsa.SignData(data, hasher);
            }

            return Convert.ToBase64String(signedData);// +Environment.NewLine + Environment.NewLine;
            //return ByteArrayToString(signedData); //Convert.ToBase64String(signedData);
        }

        //public static string ByteArrayToString(byte[] signedBytes)
        //{
        //    StringBuilder hex = new StringBuilder(signedBytes.Length * 2);
        //    foreach (byte b in signedBytes)
        //        hex.AppendFormat("{0:x2}", b);
        //    return hex.ToString();
        //}
        /// <summary>
        /// END  Signin certifiacte CODE
        /// </summary>

        public static string Sign_old(this X509Certificate2 x509, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] signedData;

            using (SHA1 hasher = SHA1.Create())
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509.PrivateKey;
                signedData = rsa.SignData(data, hasher);
            }

            return Convert.ToBase64String(signedData);
        }

        public static string DistinguishedNameOpenSSLFormat(this X509Certificate2 x509)
        {
            return "/" + x509.IssuerName.Decode(X500DistinguishedNameFlags.UseUTF8Encoding).Replace(", ", "/").Replace("/S=", "/ST=");
        }

        private static string ReadPrivateKey(string certText)  // teating server code
        {
            try
            {
                if (string.IsNullOrEmpty(certText))
                    return "";

                int start = certText.IndexOf("-----BEGIN RSA PRIVATE KEY-----");
                int end = certText.IndexOf("-----END RSA PRIVATE KEY-----");
                return certText.Substring(start, end - start).Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("\n", "").Replace("\r", "");
            }
            catch (Exception)
            {
            }

            return "";
        }

        private static string ReadPrivateKey1(string certText) // production server code
        {
            try
            {
                if (string.IsNullOrEmpty(certText))
                    return "";

                int start = certText.IndexOf("-----BEGIN PRIVATE KEY-----");
                int end = certText.IndexOf("-----END PRIVATE KEY-----");
                return certText.Substring(start, end - start).Replace("-----BEGIN PRIVATE KEY-----", "").Replace("\n", "").Replace("\r", "");
            }
            catch (Exception)
            {
            }

            return "";
        }

        private static string ReadCertificate(string certText)
        {
            try
            {
                if (string.IsNullOrEmpty(certText))
                    return "";

                int start = certText.IndexOf("-----BEGIN CERTIFICATE-----");
                int end = certText.IndexOf("-----END CERTIFICATE-----");
                return certText.Substring(start, end - start).Replace("-----BEGIN CERTIFICATE-----", "").Replace("\n", "").Replace("\r", "");
            }
            catch (Exception)
            {
            }

            return "";
        }

        private static string ReadCertFile(string fileName)
        {
            try
            {
                return File.ReadAllText(fileName);
            }
            catch (Exception)
            {
            }

            return "";
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            // this byte[] includes the sequence byte and terminal encoded null
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(privkey);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);		//read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)	//expect an Octet string
                    return null;

                bt = binr.ReadByte();		//read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();
                //------ at this stage, the remaining sequence should be the RSA private key

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKeyReal(rsaprivkey);
                return rsacsp;
                // return null;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        //------- Parses binary ans.1 RSA private key; returns RSACryptoServiceProvider  ---
        public static RSACryptoServiceProvider DecodeRSAPrivateKeyReal(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)	//version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey___old(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);     //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }

    #endregion X509PemCertificateHelper
}