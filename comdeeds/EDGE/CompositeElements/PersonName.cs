namespace comdeeds.EDGE.CompositeElements
	{
	public class PersonName
		{
		public PersonName ()
			{
			FamilyName = "";
			GivenName1 = "";
			GivenName2 = "";
			GivenName3 = "";
			}

		public string FamilyName { get; set; }
		public string GivenName1 { get; set; }
		public string GivenName2 { get; set; }
		public string GivenName3 { get; set; }

		public bool Validate ()
			{
			if (string.IsNullOrEmpty (FamilyName) || string.IsNullOrEmpty (GivenName1))
				{
				ErrorMsg = "FamilyName and GiveName1 are mandatory.";
				return false;
				}

			if (FamilyName.Length < 2 || FamilyName.Length > 30)
				{
				ErrorMsg = "FamilyName must be between 2 and 30 characters";
				return false;
				}

			if (GivenName1.Length > 20)
				{
				ErrorMsg = "GivenName1 must be at most 20 characters long";
				return false;
				}

			if (!string.IsNullOrEmpty(GivenName2) && GivenName2.Length > 20)
				{
				ErrorMsg = "GivenName2 must be null or at most 20 characters long";
				return false;
				}

			if (!string.IsNullOrEmpty (GivenName3) && GivenName3.Length > 20)
				{
				ErrorMsg = "GivenName3 must be null or at most 20 characters long";
				return false;
				}

			return true;
			}

		public string ErrorMsg { get; set; }
		}
	}