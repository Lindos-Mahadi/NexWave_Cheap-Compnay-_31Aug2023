using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Globalization;
//using onDeedServices.ServiceReference1_ASIC1;

namespace comdeeds
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Deeds_queryNameAvailability" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Deeds_queryNameAvailability.svc or Deeds_queryNameAvailability.svc.cs at the Solution Explorer and start debugging.
    public class Deeds_queryNameAvailability : IDeeds_queryNameAvailability
    {
        public void DoWork()
        {
        }

        ErrorLog oErrorLog = new ErrorLog();

        //public bool addressAvailabilityCheck(string unitOrOfficeNumber, string streetName, string locality, string postCode, string state)
        //{
        //    bool val = false;
        //    try
        //    {
        //        state = state.ToUpper();
        //        locality = locality.ToUpper();
        //        locality = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(locality.ToLower());
        //        streetName = streetName.Replace(",", " ");
        //        #region Header
        //        onDeedServices.ServiceReference1_ASIC2.ExternalQueryAddressClient proxy = new ServiceReference1_ASIC2.ExternalQueryAddressClient();
        //        proxy.ClientCredentials.UserName.UserName = "ASICM2MRA@ONLYDEEDS.COM";
        //        proxy.ClientCredentials.UserName.Password = "T0day1234";
        //        proxy.Open();
        //        onDeedServices.ServiceReference1_ASIC2.businessDocumentHeaderType objHeader = new onDeedServices.ServiceReference1_ASIC2.businessDocumentHeaderType(); ///create DocumentHeader
        //        objHeader.messageType = "queryAddress";
        //        objHeader.messageReferenceNumber = "1234566";
        //        objHeader.asicReferenceNumber = "?";
        //        objHeader.messageVersion = 1;
        //        objHeader.senderId = "000040125";
        //        objHeader.senderType = "REGA";
        //        #endregion
        //        onDeedServices.ServiceReference1_ASIC2.queryAddressReplyType repltype = new ServiceReference1_ASIC2.queryAddressReplyType();
        //        repltype.businessDocumentHeader = new ServiceReference1_ASIC2.businessDocumentHeaderType();
        //        repltype.businessDocumentBody = new ServiceReference1_ASIC2.replyDataType();
        //        onDeedServices.ServiceReference1_ASIC2.requestDataType objBody = new ServiceReference1_ASIC2.requestDataType();
        //        objBody.address = new ServiceReference1_ASIC2.addressLodgeType();
        //        onDeedServices.ServiceReference1_ASIC2.addressLodgeType adty = new onDeedServices.ServiceReference1_ASIC2.addressLodgeType();
        //        onDeedServices.ServiceReference1_ASIC2.addressLodgeTypePhysicalAddress adphyadd = new onDeedServices.ServiceReference1_ASIC2.addressLodgeTypePhysicalAddress();
        //        //adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.NSW;
        //        #region state Code
        //        if (state == "ACT")
        //        {
        //            adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.ACT;
        //        }
        //        if (state == "NSW")
        //        {
        //            adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.NSW;
        //        }
        //        if (state == "NT")
        //        {
        //            adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.NT;
        //        }
        //        if (state == "QLD")
        //        {
        //            adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.QLD;
        //        }
        //        if (state == "SA")
        //        {
        //            adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.SA;
        //        }
        //        if (state == "TAS")
        //        {
        //            adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.TAS;
        //        }
        //        if (state == "VIC")
        //        {
        //            adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.VIC;
        //        }
        //        if (state == "WA")
        //        {
        //            adphyadd.state = onDeedServices.ServiceReference1_ASIC2.stateTerritoryCodeType.WA;
        //        }
        //        #endregion
        //        adphyadd.postCode = postCode;
        //        adphyadd.streetName = streetName;
        //        adphyadd.locality = locality;
        //        adphyadd.unitOrOfficeNumber = unitOrOfficeNumber;
        //        adty.Item = adphyadd;
        //        objBody.address = adty;
        //        onDeedServices.ServiceReference1_ASIC2.queryAddressRequestType reqtype = new ServiceReference1_ASIC2.queryAddressRequestType();
        //        reqtype.businessDocumentHeader = objHeader;
        //        reqtype.businessDocumentBody = objBody;
        //        repltype = proxy.externalQueryAddress(reqtype);
        //        val = repltype.businessDocumentBody.valid;
        //    }
        //    catch (Exception ex)
        //    {
        //        oErrorLog.WriteErrorLog(ex.ToString());
        //        //Basic: realm  like problem is related to web.config
        //        //throw ex;
        //    }
        //    return val;
        //}

    }
}
