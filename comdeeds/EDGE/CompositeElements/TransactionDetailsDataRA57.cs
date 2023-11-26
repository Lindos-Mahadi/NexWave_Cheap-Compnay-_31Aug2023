using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comdeeds.EDGE.CompositeElements
{
    public class TransactionDetailsDataRA57
    {
        // Status text
        public List<string> StatusText { get; set; }
        public List<TransactionsRa57> TXNdetails { get; set; }

    }
    public class TransactionsRa57
    {
        public DateTime Transactiondate { get; set; }
        public string Transactionlegend { get; set; }
        public string Transactionstatus { get; set; }
        public string Transactionvalue { get; set; }
        public string Transactionoutstandingvalue { get; set; }
        public string Transactionreference { get; set; }
        public string Transactionallocationreference { get; set; }
        public string ACN { get; set; }
    }
}