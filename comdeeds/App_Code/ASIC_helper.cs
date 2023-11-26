using comdeeds.QueryNameService;
using System;
using System.Web.Configuration;

namespace comdeeds.App_Code
{
    public class ASIC_helper
    {
        
        public class ASIC_credential
        {
            public string username { get; }
            public string password { get; }
            public string senderId { get; }
            public string senderType { get; }

            public ASIC_credential()
            {
                this.username = WebConfigurationManager.AppSettings["username_asic"];
                this.password = WebConfigurationManager.AppSettings["password_asic"];
                this.senderId = WebConfigurationManager.AppSettings["senderId"];
                this.senderType = WebConfigurationManager.AppSettings["senderType"];
            }
        }
        
        public static queryNameAvailabilityReplyType CheckCompanyName(string CompanyNameQuery, string MessageRefNumber)
        {
            ASIC_credential u = new ASIC_credential();
            try
            {
                using (ExternalQueryNameAvailabilityClient client = new ExternalQueryNameAvailabilityClient())
                {

                    client.ClientCredentials.UserName.UserName = u.username;
                    client.ClientCredentials.UserName.Password = u.password;

                    queryNameAvailabilityRequestType request =
                    new queryNameAvailabilityRequestType();

                    request.businessDocumentHeader = new businessDocumentHeaderType
                    {
                        messageType = "queryNameAvailability",
                        senderId = u.senderId,
                        senderType = u.senderType,
                        messageReferenceNumber = MessageRefNumber,
                        messageVersion = 2
                    };
                    request.businessDocumentBody = new queryNniNameType
                    {
                        proposedName = CompanyNameQuery.ToUpper(),
                        Item = true,
                        ItemElementName = ItemChoiceType3.companyNameAvailabilityCheck
                    };

                    return client.externalQueryNameAvailability(request);
                }
            }
            catch (Exception ex)
            {
                ErrorLog objerrorlog = new ErrorLog();
                objerrorlog.WriteErrorLog(ex.ToString());
                return new queryNameAvailabilityReplyType
                {
                    businessDocumentBody = new nameAvailabilityResponseType
                    {
                        shortDescription = "error",
                        longDescription = "company search service is down, please try after sometimes."
                        
                        // longDescription = ex.Message
                    }

                };
                
            }
        }
    }
}