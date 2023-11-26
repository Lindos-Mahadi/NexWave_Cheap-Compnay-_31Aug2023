using comdeeds.Models;


namespace comdeeds.App_Code.PinPayments
{
    public class Response
    {
        public string error { get; set; }
        public string error_description { get; set; }
        public string charge_token { get; set; }
        public Message[] messages { get; set; }
        
    }
}
