using System;

namespace comdeeds.EDGE.CompositeElements
	{
	public class CompanyDataRA55
		{
		// Company ID
		public string CompanyName { get; set; }
		public string ACN { get; set; }
		public string CompanyType { get; set; }
		public string CompanyClass { get; set; }
		public string CertificatePrintOption { get; set; }
		public string JurisdictionOfRegistration { get; set; }
		public DateTime DateOfRegistration { get; set; }
		public string CompanySubclass { get; set; }

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
		}
	}