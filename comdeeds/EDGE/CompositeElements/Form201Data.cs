using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comdeeds.EDGE.CompositeElements
	{
	public class Form201Data
		{
        
		public string CompanyName { get; set; }
		public string CompanyType { get; set; }
		public string CompanyClass { get; set; }
		public string CompanySubclass { get; set; }
		public bool UseACNAsCompanyName { get; set; }
		public string LegalTerms { get; set; }
		public bool CompanyGovernedByConstituion { get; set; }
		public bool WillSharesBeIssuedForNonCashConsideration { get; set; }
		public bool HasNameBeenReservedForThisBobyByForm401 { get; set; }
		public bool IsProposedNameIdenticalToRegisteredBusinessName { get; set; }
		public string Jurisdition { get; set; }
		public bool AreAllOfficeholderAddressesTheUsualResidentialAddressOfOfficeholder { get; set; }
		public string ABNBusinessName { get; set; }
		
		// Details of name reservation if name has been reserved by form 410
		public string ApplicantNameIfPerson { get; set; }
		public string ApplicantNameIfOrganization { get; set; }
		public int ReservationDocumentNumber { get; set; }

		// 	Details of identical business names of which all the proprietors are the members listed on this application
		public List <BusinessNamesOwnedData> BusinessNamesOwned { get; set; }

		public int AmountOfMembersGuarantee { get; set; }

		public string PublicOfficeAddress { get; set; }
		public bool WillTheCompanyOccupyTheAddress { get; set; }
		public string IfTheCompanyWillNotOccupyRegisteredOfficeNameOfOccupier { get; set; }
		public bool HasOccupantsConsentBeenObtained { get; set; }
		public bool AddressOverriden { get; set; }

		public bool StandardHours { get; set; }
		public List <OfficeHoursData> OfficeHours { get; set; }

		// Principal place of business address in Australia
		public bool AddressOverridenIfInAustralia { get; set; }
		public string AddressOfPrincipalPlaceOfBusiness { get; set; }

		public string NameOfUltimateHoldingCompany { get; set; }
		public string ACNOfUltimateHoldingCompany { get; set; }
		public string PlaceOfIncorporation        { get; set; }
		public string ABNOfUltimateHoldingCompany { get; set; }
	    
		// Officers
		public List <OfficersData> Officers { get; set; } 

		// Share structure
		public List <ShareClassesData> ShareClasses { get; set; }

		// Members
		public List <ShareMembersData> ShareMembers { get; set; }

		// Non-share members
		public List <MemberData> NonShareMembers { get; set; }

		public string NameOfApplicantIfPerson { get; set; }
		public string NameOfApplicantIfOrganization { get; set; }
		public string ACN_ARBNOfApplicantIfOrganization { get; set; }
		public string ApplicantAddress { get; set; }

		public string SignatoryName { get; set; }
		public string SignatoryRole { get; set; }
		public DateTime DateSigned { get; set; }
		public bool DeclarationInN126HasBeenAssentedTo { get; set; }

		public bool RequestApplicationBeManuallyReviewed { get; set; }
		public bool HasASICConsentBeenGranted { get; set; }
		public string RegistrationCertificateDeliveryOption { get; set; }

		public List <string> Text { get; set; }

		public bool Validate ()
			{
			if (string.IsNullOrEmpty (CompanyName))
				{
				ErrorMsg = "CompanyName is mandatory.";
				return false;
				}

			// It is important to implement the other business rules; I don't have the necessary knowledge to do
			// it in a meaningfull way

			return true;
			}

		public string ErrorMsg { get; set; }
		}

	public class ShareMembersData
		{
		public List <HoldingData> Holding { get; set; }
		public List <MemberData> Member { get; set; }
		}

	public class MemberData
		{
		public string MemberNamePerson { get; set; }
		public string MemberNameOrganization { get; set; }
		public string MemberACN { get; set; }
		public string MemberAddress { get; set; }
		public bool DoesMemberHaveAnACN { get; set; }
		public bool AddressOverriden { get; set; }
		}

	public class HoldingData
		{
		public string ShareClassCode { get; set; }
		public int NumberAgreedToBeTakenUp { get; set; }
		public bool AreTheSharesFullyPaid { get; set; }
		public bool IsMemberTheBeneficialOwner { get; set; }
		public int TotalAmountPaid { get; set; }
		public int TotalAmountUnpaid { get; set; }
		public int AmountPaidPerShare { get; set; }
		public int AmountDueAndPayablePerShare { get; set; }
		}

	public class ShareClassesData
		{
		public string ShareClassCode { get; set; }
		public string FullTitleOfShare { get; set; }
		public int TotalNumberIssued { get; set; }
		public int TotalAmountPaid { get; set; }
		public int TotalAmountUnpaid { get; set; }
		}

	public class OfficersData
		{
		public string NameOfOfficer { get; set; }
		public DateTime BirthOfOfficer { get; set; }
		public string OfficerAddress { get; set; }
		public bool AddressOverriden { get; set; }

		public List <string> FormerOfficerNames { get; set; }
		public List <string> OfficesHeldInCompany { get; set; }
		}

	public class OfficeHoursData
		{
		public string OfficeOpeningTime { get; set; }
		public string OfficeClosingTime { get; set; }
		}

	public class BusinessNamesOwnedData
		{
		public string PlaceOfRegistrationOfBusinessName { get; set; }
		public int RegistrationNumberOfBusinessName { get; set; }
		}
	}
