using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using comdeeds.App_Code;

namespace comdeeds.App_Code.PinPayments.Actions
{
    internal static class Urls
    {
        public static string Card
        {
            get { return BaseUrl() + "/1/cards"; }
        }

        public static string ChargesSearch
        {
            get { return BaseUrl() + "/1/charges/search"; }
        }
        
        public static string Charge
        {
            get { return BaseUrl() + "/1/charges"; }
        }

        public static string Charges
        {
            get { return BaseUrl() + "/1/charges/"; }
        }

        public static string CustomerAdd
        {
            get { return BaseUrl() + "/1/customers"; }
        }

        public static string Customers
        {
            get { return BaseUrl() + "/1/customers"; }
        }

        public static string CustomerCharges
        {
            get { return BaseUrl() + "/1/customers/{token}/charges"; }
        }

        public static string Refund
        {
            get { return BaseUrl() + "/1/charges/{token}/refunds"; }
        }

        private static string BaseUrl()
        {
            string url = "https://test-api.pin.net.au";
            var opt = OptionMethods.GetAllOptions();
            if (opt.Any(x => x.OptionName.ToLower() == "paymentmode" && x.Type=="setting"))
            {
                var mode = opt.Where(x => x.OptionName.ToLower() == "paymentmode" && x.Type == "setting").FirstOrDefault().OptionValue;
                if(mode=="live")
                {
                    url = "https://api.pin.net.au";
                }
            }
            return url;
        }
    }
}
