using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.CompositeElements
{
    public class AddSign:OutboundMessages.OutboundMessage
    {
        public AddSign(X509Certificate2 x509)
        {
            m_x509 = x509;
        }
        public string AddCertificate(string message)
        {
            message += InsertCertificate();
            return message;
        }
        public string AddZXS(string message)
        {
            message = Sign(message);
            return message;
        }
    }
}