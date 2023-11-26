using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.EDGE.CompositeElements
{
    public class CommonAddress
    {
        private AddressClass m_data;
        public CommonAddress(AddressClass data)
        {
            m_data = data;
        }
        public string formatAddress()
        {
            string add = "";
            
            if (m_data.country == null || m_data.country.ToUpper()=="AUSTRALIA")
            {
                m_data.country = "\t";
            }
            else {
                m_data.country = "\t" + m_data.country;
            }
            if (m_data.careof == null && m_data.unitlevel_line2 == null)
            {
                add = string.Format("\t\t{0}\t{1}\t{2}\t{3}{4}", m_data.street, m_data.locality, m_data.state, m_data.pcode, m_data.country);
            }
            else if (m_data.careof == null && m_data.unitlevel_line2 != null)
            {
                add = string.Format("\t{0}\t{1}\t{2}\t{3}\t{4}{5}", m_data.unitlevel_line2, m_data.street, m_data.locality, m_data.state, m_data.pcode, m_data.country);
            }
            else if (m_data.careof != null && m_data.unitlevel_line2 == null)
            {
                add = string.Format("{0}\t\t{1}\t{2}\t{3}\t{4}{5}", m_data.careof, m_data.street, m_data.locality, m_data.state, m_data.pcode, m_data.country);
            }
            else if (m_data.careof != null && m_data.unitlevel_line2 != null)
            {
                add = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}{6}", m_data.careof,m_data.unitlevel_line2, m_data.street, m_data.locality, m_data.state, m_data.pcode, m_data.country);
            }

            if (m_data.country.ToUpper().Trim() != "AUSTRALIA" && m_data.country.ToUpper().Trim()!="")
            {
                add = string.Format("\t\t{0}\t{1}\t{2}\t{3}\t{4}", "", (m_data.unitlevel_line2 + " " + m_data.street + " " + m_data.locality + " " + m_data.state).Trim(), "", "", m_data.country.Trim());
            }
            return add;
        }
    }
    public class AddressClass
    {
        public string careof { get; set; }
        public string unitlevel_line2 { get; set; }
        public string street { get; set; }
        public string locality { get; set; }
        public string state { get; set; }
        public string pcode { get; set; }
        public string country { get; set; }
    }
}