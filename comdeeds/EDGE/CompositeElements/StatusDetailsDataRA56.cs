using System;
using System.Collections.Generic;

namespace comdeeds.EDGE.CompositeElements
	{
	public class StatusDetailsDataRA56
		{
		// Request document
		public string ProposedCompanyName { get; set; }
		public string RequestDocumentNumber { get; set; }
		public DateTime DateOfAdvice { get; set; }
		public string ASICAdviceType { get; set; }

		// Invoice subject
		public string AccountNumber { get; set; }
		public string SupplierName { get; set; }
		public string SupplierABN { get; set; }
		public string RegisteredAgentName { get; set; }
		public string RegisteredAgentAddress { get; set; }

		// Invoice details
		public string InvoiceDescription { get; set; }
		public int InvoiceAmmount { get; set; }
		public string DocumentNumber { get; set; }
		public string FormCode { get; set; }
		public string TaxInvoiceText { get; set; }
		public string TaxCode { get; set; }
		public int TaxAmmount { get; set; }
		public string ReferenceNumber { get; set; }
		public DateTime InvoiceIssueDate { get; set; }
		public string TreasurersDetermination { get; set; }

		// Status text
		public List <string> StatusText { get; set; }
		}
	}