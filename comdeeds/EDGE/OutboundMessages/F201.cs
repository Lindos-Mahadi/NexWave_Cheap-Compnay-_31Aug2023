using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.Utils;

namespace comdeeds.EDGE.OutboundMessages
	{
	public class F201 : OutboundMessage
		{
		private int m_messageTraceNumber;
		private Form201Data m_data;
		private string m_certificateIdentifier;
       
		public F201 (Form201Data data , int messageTraceNumber , string certificate , 
					 X509Certificate2 x509)
			{
			m_data = data;
			m_messageTraceNumber = messageTraceNumber;
			m_certificate = certificate;
			m_x509 = x509;
            m_certificateIdentifier = System.Configuration.ConfigurationManager.AppSettings["s_serialidentifier"].ToString();//m_x509.SerialNumber;
			}

		public override bool Validate ()
			{
			return m_data.Validate ();
			}

		public override string MessageToSend (bool validateAndThrow = false)
			{
			if (validateAndThrow && !Validate ())
				throw new Exception (ErrorMsg);
            
			string message = string.Format ("ZHDASC201\t1000\t{0}\n" , m_messageTraceNumber);
            message += string.Format("ZCO{0}\t\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t\t{7}\t{8}\t{9}\t{10}\t\t{11}\t\t\t\t{12}\n",
                m_data.CompanyName, m_data.CompanyType, m_data.CompanyClass, m_data.CompanySubclass,
                BooleanRep(m_data.UseACNAsCompanyName), m_data.LegalTerms, BooleanRep(m_data.CompanyGovernedByConstituion),
                BooleanRep(m_data.WillSharesBeIssuedForNonCashConsideration),
                BooleanRep(m_data.HasNameBeenReservedForThisBobyByForm401),
                BooleanRep(m_data.IsProposedNameIdenticalToRegisteredBusinessName), m_data.Jurisdition,
                BooleanRep(m_data.AreAllOfficeholderAddressesTheUsualResidentialAddressOfOfficeholder), m_data.ABNBusinessName);

			/*message += string.Format ("ZCO{0}\t\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t\t{7}\t{8}\t{9}\t{10}\t\t{11}\t\t\t\t{12}\n" ,
				m_data.CompanyName , m_data.CompanyType , m_data.CompanyClass , m_data.CompanySubclass ,
				BooleanRep (m_data.UseACNAsCompanyName) , m_data.LegalTerms , BooleanRep (m_data.CompanyGovernedByConstituion) ,
				BooleanRep (m_data.WillSharesBeIssuedForNonCashConsideration) ,
				BooleanRep (m_data.HasNameBeenReservedForThisBobyByForm401) ,
				BooleanRep (m_data.IsProposedNameIdenticalToRegisteredBusinessName) , m_data.Jurisdition ,
				BooleanRep (m_data.AreAllOfficeholderAddressesTheUsualResidentialAddressOfOfficeholder) , m_data.ABNBusinessName);
			message += string.Format ("ZNR{0}\t{1}\t{2}\n" , m_data.ApplicantNameIfPerson , m_data.ApplicantNameIfOrganization ,
				m_data.ReservationDocumentNumber);

            if (m_data.BusinessNamesOwned != null)
            {
                foreach (BusinessNamesOwnedData businessNamesOwnedData in m_data.BusinessNamesOwned)
                {
                    message += string.Format("ZPR{0}\t{1}\n", businessNamesOwnedData.PlaceOfRegistrationOfBusinessName,
                        businessNamesOwnedData.RegistrationNumberOfBusinessName);
                }
            }
			if (m_data.AmountOfMembersGuarantee > 0)
				message += string.Format ("ZSC\t\t{0}\n" , m_data.AmountOfMembersGuarantee);

			message += string.Format ("ZRG\t\t{0}\t{1}\t" , m_data.PublicOfficeAddress ,
				BooleanRep (m_data.WillTheCompanyOccupyTheAddress));
			if (!m_data.WillTheCompanyOccupyTheAddress)
				message += string.Format ("{0}\t{1}\t{2}\n" , m_data.IfTheCompanyWillNotOccupyRegisteredOfficeNameOfOccupier ,
					BooleanRep (m_data.HasOccupantsConsentBeenObtained) , BooleanRep (m_data.AddressOverriden));
			else
				message += string.Format ("\t\t{0}\n" , BooleanRep (m_data.AddressOverriden));

			message += string.Format ("ZFY\t\t\t{0}\n" , BooleanRep (m_data.StandardHours));

			foreach (OfficeHoursData officeHoursData in m_data.OfficeHours)
				{
				message += string.Format ("ZOH{0}\t{1}\n" , officeHoursData.OfficeOpeningTime , officeHoursData.OfficeClosingTime);
				}


			message += string.Format ("ZRP\t\t{0}\t{1}\n" , BooleanRep (m_data.AddressOverridenIfInAustralia) ,
				m_data.AddressOfPrincipalPlaceOfBusiness);

			if (!string.IsNullOrEmpty (m_data.NameOfUltimateHoldingCompany))
				message += string.Format ("ZUH{0}\t{1}\t{2}\t{3}\n" , m_data.NameOfUltimateHoldingCompany ,
					m_data.ACNOfUltimateHoldingCompany , m_data.PlaceOfIncorporation , m_data.ABNOfUltimateHoldingCompany);

			foreach (OfficersData officer in m_data.Officers)
				{
				message += string.Format ("ZSD\t\t{0}\t{1}\t\t\t{2}\t{3}\n" , officer.NameOfOfficer , DateRep (officer.BirthOfOfficer) ,
					officer.OfficerAddress , BooleanRep (officer.AddressOverriden));

				foreach (string formerOfficerName in officer.FormerOfficerNames)
					{
					message += string.Format ("ZFN{0}\n" , formerOfficerName);
					}

				foreach (string officesHeld in officer.OfficesHeldInCompany)
					{
					message += string.Format ("ZOF{0}\n" , officesHeld);
					}
				}

			foreach (ShareClassesData shareClassesData in m_data.ShareClasses)
				{
				message += string.Format ("ZSC{0}\t{1}\t\t{2}\t\t{3}\t{4}\n" , shareClassesData.ShareClassCode ,
					shareClassesData.FullTitleOfShare , shareClassesData.TotalNumberIssued , shareClassesData.TotalAmountPaid ,
					shareClassesData.TotalAmountUnpaid);
				}

			foreach (ShareMembersData shareMembersData in m_data.ShareMembers)
				{
				foreach (HoldingData holdingData in shareMembersData.Holding)
					{
					message += string.Format ("ZHH{0}\t{1}\t{2}\t{3}\t\t{4}\t{5}\t\t\t\t\t\t{6}\t{7}\n" , holdingData.ShareClassCode ,
						holdingData.NumberAgreedToBeTakenUp , BooleanRep (holdingData.AreTheSharesFullyPaid) ,
						BooleanRep (holdingData.IsMemberTheBeneficialOwner) , holdingData.TotalAmountPaid , holdingData.TotalAmountUnpaid ,
						holdingData.AmountPaidPerShare , holdingData.AmountDueAndPayablePerShare);
					}

				foreach (MemberData memberData in shareMembersData.Member)
					{
					message += string.Format ("ZSH{0}\t{1}\t{2}\t{3}\t{4}\t\t{5}\n" , memberData.MemberNamePerson ,
						memberData.MemberNameOrganization , memberData.MemberACN , memberData.MemberAddress ,
						BooleanRep (memberData.DoesMemberHaveAnACN) , BooleanRep (memberData.AddressOverriden));
					}
				}
            if (m_data.NonShareMembers != null)
            {
                foreach (MemberData nonShareMember in m_data.NonShareMembers)
                {
                    message += string.Format("ZNS{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", nonShareMember.MemberNamePerson,
                        nonShareMember.MemberNameOrganization, nonShareMember.MemberACN, nonShareMember.MemberAddress,
                        BooleanRep(nonShareMember.DoesMemberHaveAnACN), BooleanRep(nonShareMember.AddressOverriden));
                }
            }
			message += string.Format ("ZCG{0}\t{1}\t{2}\t{3}\n" , m_data.NameOfApplicantIfPerson ,
				m_data.NameOfApplicantIfOrganization , m_data.ACN_ARBNOfApplicantIfOrganization , m_data.ApplicantAddress);
			message += string.Format ("ZDC{0}\t{1}\t{2}\t{3}\n" , m_data.SignatoryName , m_data.SignatoryRole ,
				DateRep (m_data.DateSigned) , BooleanRep (m_data.DeclarationInN126HasBeenAssentedTo));
			message += string.Format ("ZAM{0}\t{1}\t{2}\n" , BooleanRep (m_data.RequestApplicationBeManuallyReviewed) ,
				BooleanRep (m_data.HasASICConsentBeenGranted) , m_data.RegistrationCertificateDeliveryOption);

			foreach (string text in m_data.Text)
				{
				message += string.Format ("ZTE{0}\n" , text);
				}
			
			message += string.Format ("ZTREND201\t{0}\n" , message.Count (c => c == '\n') + 1);
            */
			string messageToSign = message;

            //message += string.Format ("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n" , m_x509.DistinguishedNameOpenSSLFormat () ,
            //    m_certificateIdentifier);
			//message += Sign (messageToSign);









            //message = "ZHDASC201\t800\t8\nZCOCISCO 32 PTY\t\tAPTY\tLMSH\tHUNT\tN\tPTY\tN\t\tN\tY\tN\tNSW\t\tN\t\t\t\t\nZNR\t\t0\nZSC\t\t3\nZRG\t\t37 SHEDWORTH ST MARAYONG NSW 2148\tY\t\t\tN\nZFY\t\t\tY\nZOH0800\t1800\nZRP\t\tN\t\nZSD\t\tOfficer1 kumar sanu\t19780105\t\t\t59a marion st 40 third avn Blacktown NSW 2148\tN\nZFNFormer1 Officer name\nZOFDIR\nZSD\t\tOfficer2 kumar sanu\t19850105\t\t\t59a marion st 40 third avn Blacktown NSW 2148\tN\nZFNFormer2 Officer name\nZOFDIR\nZSD\t\tOfficer3 kumar sanu\t19800405\t\t\t59a marion st 40 third avn Blacktown NSW 2148\tN\nZFNFormer3 Officer name\nZOFSEC\nZSCA\tA\t\t10\t\t25\t0\nZHHA\t6\tY\tY\t\t250\t0\t\t\t\t\t\t150\t0\nZHHA\t2\tY\tY\t\t50\t0\t\t\t\t\t\t25\t0\nZHHA\t2\tY\tY\t\t50\t0\t\t\t\t\t\t25\t0\nZSHOfficer1 kumar sanu\tatal corp\t123456789\t59a marion st 40 third avn Blacktown NSW 2148\tN\t\tN\nZSHOfficer2 kumar sanu\tatal corp\t123456789\t59a marion st 40 third avn Blacktown NSW 2148\tN\t\tN\nZSHOfficer3 kumar sanu\tatal corp\t123456789\t59a marion st 40 third avn Blacktown NSW 2148\tN\t\tN\nZCGBhupendra Parihar\tDSYS PVT\t123456789\t37 SHEDWORTH ST MARAYONG NSW 2148\nZDCSignatoryName\t\t20160105\tY\nZAMY\tY\t\nZTEHello this is ASIC Testing.\nZTREND201\t29\nZXI/C=AU/ST=NSW/L=Sydney/O=ASIC/OU=ITSB eBusiness Systems Team/CN=eBusiness Development CA\t\tMD5\tRSA\t\t\t\t0D\nZXSbZB3flILc5zecZujeK3oo/f5txU4MPFwuXqDm2HN6M0vq7f5E1WgoV7q+ijEwa5D\nZXSccjgpwd5OwR8znd0sGQXLXYnFutcMBHHHR3Lef4HQVZGRUcI5Y1LMcCqTr+V+moJ\nZXSLEiUMFFu+R/AD8rysHPFZAWYfNGUBXAGpBeIHvyr0Cw=";
            string textme = System.IO.File.ReadAllText(@"C:\sample2.txt", System.Text.Encoding.UTF8);
            message = textme; 
			return message;
			}
		}
	}						