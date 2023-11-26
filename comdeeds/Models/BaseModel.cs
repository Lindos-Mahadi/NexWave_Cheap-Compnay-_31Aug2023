using comdeeds.App_Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace comdeeds.Models
{
    public class BaseModel
    {
        public class ClassUserDetails
        {
            public long Id { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string password { get; set; }
            public bool EmailVerified { get; set; }
            public DateTime AddedDate { get; set; }
            public DateTime? Lastlogin { get; set; }
            public string _Role { get; set; }
            public string Tuser { get; set; }
        }

        public class ClassCompanyModel
        {
            public string companyName { get; set; }
            public bool? isNameReserve { get; set; }
            public string abn { get; set; }
            public string companyPurpose { get; set; }
            public string UseOfCompany { get; set; }
            public string regState { get; set; }
            public bool? decl { get; set; }
            public long tid { get; set; }
            public string trustName { get; set; }
            public string trustAbn { get; set; }
            public string trustTfn { get; set; }
            public string trustAddress { get; set; }
            public string trustCountry { get; set; }
            public bool? AcnasName { get; set; }
            public string tuser { get; set; }
            public string UlimateHoldingCompany { get; set; }
            public string ucompanyname { get; set; }
            public string acnarbnabn { get; set; }
            public string countryIcor { get; set; }

        }

        public class ClassCompanyAddressModel
        {
            public long id { get; set; }
            public string unit { get; set; }
            public string street { get; set; }
            public string state { get; set; }
            public string suburb { get; set; }
            public string postcode { get; set; }
            public string type { get; set; }
            public string sameadd { get; set; }
        }

        public class ClassDirectorModel
        {
            public long id { get; set; }
            public string fname { get; set; }
            public string lname { get; set; }
            public int? dobday { get; set; }
            public int? dobmonth { get; set; }
            public int? dobyear { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string stateraw { get; set; }
            public string country { get; set; }

            public string dirunit { get; set; }
            public string dirstreet { get; set; }
            public string dirsuburb { get; set; }
            public string dirpostcode { get; set; }
            public string dirstate { get; set; }
        }

        public class ClassShareModel
        {
            public long Id { get; set; }
            public string dirName { get; set; }
            public long? dId { get; set; }
            public string shareclass { get; set; }

            // public int? noofshare { get; set; }
            public double? noofshare { get; set; }

            public double? sharecost { get; set; }
            public bool other { get; set; }
            public string ownername { get; set; }

            public string rdshare_individual { get; set; }
            public string rdshare_company { get; set; }
            public string share_indname { get; set; }
            public string share_comname { get; set; }
            public string share_comacn { get; set; }
            public string shareaddunit { get; set; }
            public string shareaddstreet { get; set; }
            public string ddshareaddstate { get; set; }
            public string shareaddsuburb { get; set; }
            public string shareaddpostcode { get; set; }
            public string share_sharetype { get; set; }
            public string share_noofshare { get; set; }
            public string share_amountopaidshare { get; set; }
            public string share_shareownername { get; set; }

			public string isheldanotherorg { get; set; }			
			public string shareoption { get; set; }			
			public string sharedetailsnotheldanotherorg { get; set; }
	}

        public class ClassIndShareModel
        {
            public long Id { get; set; }

            public long sharedirectorsCounter { get; set; }
            public string fullname { get; set; }
            public string ind_dobday { get; set; }
            public string ind_dobmonth { get; set; }
            public string ind_dobyear { get; set; }

            public string share_placeofbirth { get; set; }
            public string rdshare_individual { get; set; }
            public string rdshare_company { get; set; }
            public string share_indname { get; set; }
            public string share_comname { get; set; }
            public string shareaddunit { get; set; }
            public string shareaddstreet { get; set; }
            public string ddshareaddstate { get; set; }
            public string shareaddsuburb { get; set; }
            public string shareaddpostcode { get; set; }
            public string share_sharetype { get; set; }
            public double? share_noofshare { get; set; }
            public double? share_amountopaidshare { get; set; }
            public string share_indshareownername { get; set; }

            // company share parameter

            public string share_compORtrtname { get; set; }
            public string share_compORtrtABN { get; set; }
            public string sharecompunit { get; set; }
            public string sharecompstreet { get; set; }
            public string ddsharecompstate { get; set; }
            public string sharecompsuburb { get; set; }
            public string sharecomppostcode { get; set; }
            public string share_compsharetype { get; set; }
            public double? share_noofcompshare { get; set; }
            public double? share_amountopaidcompshare { get; set; }
            public string share_compshareownername { get; set; }

			public string isheldanotherorg { get; set; }			
			public string shareoption { get; set; }			
			public string sharedetailsnotheldanotherorg { get; set; }
		}

        public class govofcompany
        {
            public string rdogovcheck { get; set; }
        }

        public class ClassUserLoginForm
        {
            public string loginemail { get; set; }
            public string loginemailnew { get; set; }
            public string loginpassword { get; set; }
            public bool rememberme { get; set; }
        }

        public class ClassTrustDetails
        {
            public long Id { get; set; }
            public string TrustType { get; set; }
            public string TrustName { get; set; }
            public DateTime Trust_Date { get; set; }
            public string TrustDate { get; set; }
            public string TrustState { get; set; }
            public string SmsfCompanyName { get; set; }
            public string SmsfAcn { get; set; }
            public DateTime SmsfCompanySetupDate { get; set; }
            public string Smsf { get; set; }
            public string Abn { get; set; }
            public string PropertyTrusteeName { get; set; }
            public string PropertyTrusteeAcn { get; set; }
            public string PropertyAddress { get; set; }
            public DateTime PropertyTrusteeDate { get; set; }
            public string LenderName { get; set; }
            public DateTime ExistingSetupDate { get; set; }
            public string ClauseNumber { get; set; }
        }

        public class ClassTrustAppointer
        {
            public long Id { get; set; } // Id (Primary key)
            public string HolderType { get; set; } // HolderType (length: 50)
            public string FirstName { get; set; } // FirstName (length: 500)
            public string MiddleName { get; set; } // MiddleName (length: 500)
            public string LastName { get; set; } // LastName (length: 500)
            public string CompanyName { get; set; } // CompanyName (length: 500)
            public string CompanyAcn { get; set; } // CompanyACN (length: 500)
            public bool CommanSeal { get; set; } // CommanSeal
            public string UnitLevel { get; set; } // UnitLevel (length: 2000)
            public string Street { get; set; } // Street (length: 2000)
            public string State { get; set; } // State (length: 500)
            public string Suburb { get; set; } // Suburb (length: 500)
            public string PostCode { get; set; } // PostCode (length: 500)
            public string Country { get; set; } // Country (length: 500)
            public bool IsTrustee { get; set; } // IsTrustee
            public long TrustId { get; set; } // TrustId
            public string OrdinaryPrice { get; set; }
            public string TotalUnitHolders { get; set; }
            public string UnitType { get; set; } // UnitType (length: 500)
            public int UnitNumber { get; set; } // UnitNumber
            public double UnitTotalAmount { get; set; } // UnitTotalAmount
            public double UnitAmountOwing { get; set; } // UnitAmountOwing
            public DateTime dob { get; set; }
        }

        public class ClassTrustAppointerform
        {
            public List<ClassTrustAppointer> appointer { get; set; }
            public string OrdinaryPrice { get; set; }
            public string TotalUnitHolders { get; set; }
        }

        public class ClassTrustees
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public bool istrustee { get; set; }
        }

        public class ClassBeneficiaryCompany
        {
            public long Id { get; set; }
            public string CompanyName { get; set; }
            public string CompanyACN { get; set; }
            public DateTime RegDate { get; set; }
            public string ContactPerson { get; set; }
        }

        public class ClassBeneficiary
        {
            public List<ClassTrustees> Members { get; set; }
            public List<ClassBeneficiaryCompany> Company { get; set; }
            public string bType { get; set; }
        }

        public class ClassTrusteesFromTbl
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public bool istrustee { get; set; }
            public string CompanyName { get; set; }
            public string HolderType { get; set; }
        }

        public class ClassSetupPrice
        {
            public double AsicFee { get; set; }
            public double SetupCost { get; set; }
            public double SetupGST { get; set; }
            public double TotalCost { get; set; }
            public double CreditCardFee { get; set; }
            public double ProcessingFee { get; set; }
        }

        public class ClassAsicSetup
        {
            public double id { get; set; }
            public string CompanyName { get; set; }
            public string userid { get; set; }
            public double status { get; set; }
            public string Fullname { get; set; }
            public string AsicFee { get; set; }
            public string Asic_status { get; set; }
            public string Asic_Error { get; set; }
            public string Asic_File { get; set; }
            public string Asic_ACN { get; set; }
            public string Asic_DocNo { get; set; }
            public string Asic_ResType { get; set; }
        }

        public class ClassTrustCheckout
        {
            public List<ClassBeneficiaryList> BeneficiariesMembers { get; set; }
            public List<ClassBeneficiaryList> BeneficiariesCompany { get; set; }
            public ClassSetupPrice Cost { get; set; }
            public int total { get; set; }
            public string bType { get; set; }
        }

        public class ClassBeneficiaryList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string CompanyName { get; set; }
            public string CompanyACN { get; set; }
            public DateTime RegDate { get; set; }
            public string ContactPerson { get; set; }
            public string HolderType { get; set; }
            public bool istrustee { get; set; }
        }

        public class ClassFullTrust
        {
            public TblTrust trust { get; set; }
            public List<TblTrustAppointer> appointers { get; set; }
            public ClassSetupPrice Cost { get; set; }
            public TblTransaction TransactionDetail { get; set; }
        }

        public class ClassTrustOption
        {
            public bool chkquotefortax { get; set; }
            public bool chklegalassesment { get; set; }
            public bool chkborrowingreview { get; set; }
            public bool chkagreement { get; set; }

			//by praveen
			public string CompanySecretary { get; set; }
			public string PublicOfficerOfCompany { get; set; }
			public string HowfstmeetingOfDirheld { get; set; }
			
			public string DateOfIncorporation { get; set; }			
		}

        public class ClassPaymentFormData
        {
            public long ID { get; set; }
            public string Name { get; set; }
            public double Cost { get; set; }
            public string Email { get; set; }
            public string type { get; set; }
            public string TrustType { get; set; }
            public string CustomerName { get; set; }
        }

        public class ClassPaymentform
        {
            public string desc { get; set; }
            public string deedname { get; set; }
            public string txtcost { get; set; }
            public string txtcno { get; set; }
            public string txtcvc { get; set; }
            public string txtem { get; set; }
            public string txtey { get; set; }
            public string txtname { get; set; }
            public string txtadd1 { get; set; }
            public string txtadd2 { get; set; }
            public string txtcity { get; set; }
            public string txtpostcode { get; set; }
            public string txtstate { get; set; }
            public string txtcountry { get; set; }
            public string hftype { get; set; }
            public string txtemail { get; set; }
            public long formid { get; set; }
        }

        public class ClassFullCompany
        {
            public LoginUserData user { get; set; }
            public Registration Applicant { get; set; }
            public TblCompany Company { get; set; }
            public TblCompanyTrust CompanyTrust { get; set; }
            public List<TblCompanyAddress> Address { get; set; }
            public List<TblCompanyDirector> Directors { get; set; }
            public List<TblCompanyShare> Shares { get; set; }
            public List<TblCompanyShare> indShares { get; set; }
            public ClassSetupPrice Cost { get; set; }
            public TblTransaction TransactionDetail { get; set; }
            public ClassCompanyShortDetail CompanyMeta { get; set; }
            public Companysearch companysearch { get; set; }
        }

        public class LoginUserData
        {
            public string email { get; set; }
            public DateTime LastLogin { get; set; }
            public bool IsFirstLogin { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
        }

        public class RequestGridParam
        {
            public string direction { get; set; }
            public int page { get; set; }
            public int limit { get; set; }
            public string sortBy { get; set; }
            public int _parent { get; set; }
            public string search { get; set; }
        }

        public class ClassSqlGridParam
        {
            public int startLength { get; set; }
            public int length { get; set; }
            public string orderBy { get; set; }
            public int userid { get; set; }
        }

        public class ClassGridTrustDetails
        {
            public long Id { get; set; }
            public string TrustType { get; set; }
            public string TrustName { get; set; }
            public string SetupDate { get; set; }
            public string AddedDate { get; set; }
            public string TrustState { get; set; }
            public bool Paid { get; set; }
            public string txnId { get; set; }
        }

        public class ClassGridTrustResult
        {
            public long Total { get; set; }
            public List<ClassGridTrustDetails> data { get; set; }
        }

        public class ClassGridCompanyDetails
        {
            public long Id { get; set; }
            public string CompanyName { get; set; }
            public string AddedDate { get; set; }
            public bool Paid { get; set; }
            public string txnId { get; set; }
        }

        public class ClassGridCompanyResult
        {
            public long Total { get; set; }
            public List<ClassGridCompanyDetails> data { get; set; }
        }

        public class ClassGridUserResult
        {
            public long Total { get; set; }
            public List<ClassUserDetails> users { get; set; }
        }

        public class ClassadminOptions
        {
            public int ID { get; set; }
            public string Key { get; set; }
            public string value { get; set; }
            public string type { get; set; }
        }

        public class Class_mailer
        {
            public string fromEmail { get; set; }
            public string fromName { get; set; }
            public string subject { get; set; }
            public string toMail { get; set; }
            public string HtmlBody { get; set; }
        }

        public class ClassUserPasswordData
        {
            public string oldpass { get; set; }
            public string newpass { get; set; }
            public string confirmpass { get; set; }
        }
		public class ClassResetUserPasswordData
		{
			public string useremail { get; set; }
			public string newpass { get; set; }
			public string confirmpass { get; set; }
		}

		public class ClassPaymentList
        {
            public long Id { get; set; }
            public bool TransactionStatus { get; set; }
            public double Amount { get; set; }
            public long TrustCompanyId { get; set; }
            public string FormType { get; set; }
            public DateTime AddedDate { get; set; }
            public string adate { get; set; }
            public string AddedBy { get; set; }
            public string TxnId { get; set; }
            public string formname { get; set; }
        }

        public class ClassGridPaymentList
        {
            public long Total { get; set; }
            public List<ClassPaymentList> Payment { get; set; }
        }

        public class ClassCompanyShortDetail
        {
            public int Id { get; set; }
            public string CompanyACN { get; set; }
            public string userEmail { get; set; }
            public string BillStatus { get; set; }
            public string ASICStatus { get; set; }
        }

        public class ClassUserForm
        {
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public string confpassword { get; set; }
            public string phone { get; set; }
            public string userRole { get; set; }
            public bool emailoption { get; set; }
        }

        public class Classduser
        {
            public long id { get; set; }
            public string userrole { get; set; }
        }

        public class Classdcompany
        {
            public long Id { get; set; }
            public bool TransactionStatus { get; set; }
        }

        public class Classdtrust
        {
            public long Id { get; set; }
            public string TrustType { get; set; }
        }

        public class Classdcontact
        {
            public long Id { get; set; }
            public bool Status { get; set; }
        }

        public class ClassDashboardCounters
        {
            public List<Classduser> users { get; set; }
            public List<Classdcompany> company { get; set; }
            public List<Classdtrust> trust { get; set; }
            public List<Classdcontact> contact { get; set; }
        }

        public class ClassChartData
        {
            public long Id { get; set; }
            public string dates { get; set; }
        }

        public class ClassReport
        {
            public List<ClassChartData> company { get; set; }
            public List<ClassChartData> users { get; set; }
            public List<ClassChartData> trusts { get; set; }
        }
    }
}