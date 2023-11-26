using comdeeds.App_Code;
using Comdeeds;
using comdeeds.EDGE;
using comdeeds.EDGE.CompositeElements;
using comdeeds.EDGE.InboundMessages;
using comdeeds.EDGE.OutboundMessages;
using comdeeds.EDGE.Utils;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;

namespace comdeeds.Controllers
{
    public class Lodge201Controller : Controller
    {



        ErrorLog oErrorLog = new ErrorLog();
        #region Connection Parameters
        private static string s_certificateFileName = @"ComDeeds.pem";
        //private static string s_server = "edge1.asic.gov.au";
        //private static int s_port = 5608;
        private static string s_server = "edge1.uat.asic.gov.au";
        private static int s_port = 5610;

        private static int s_agentNumber = 40125;
        private static string s_userId = "S00190";
        private static string s_userPassword = "D00194";
        private static string s_newPassword = "D00194";
        private static bool s_validateServerCertificate = false;
        private static string s_serialidentifier = "1d2879d2595322234de0ef169fc5545e";
        #endregion
        private static int cou = 0;



        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            try
            {
                s_userId = ConfigurationManager.AppSettings["userid"].ToString();
                s_userPassword = ConfigurationManager.AppSettings["oldpass"].ToString();
                s_newPassword = ConfigurationManager.AppSettings["newpass"].ToString();
                s_server = ConfigurationManager.AppSettings["asicserver"].ToString();
                s_agentNumber = Convert.ToInt32(ConfigurationManager.AppSettings["agentno"].ToString());
                s_serialidentifier = ConfigurationManager.AppSettings["s_serialidentifier"].ToString();
            }
            catch (Exception ex) { }

        }









        public ActionResult Index()
        {

            if (Session["201formId"] != null)
            {
                var id = Convert.ToInt64(Session["201formId"]);
                var c = App_Code.CompanyMethods.GetFullCompanyData(id);
                var path = Build201FormData(c);
                ViewBag.path = path;
                sendform201toasic(path, id);
            }
            return View();
        }



        public string Build201FormData(Models.BaseModel.ClassFullCompany companydata) 
        {
            string p = "";
            if (Session["201formId"] != null)
            {
                var id = Convert.ToInt64(Session["201formId"]);
                string dirdetail = "";
                string sharedetails = "";
                string compSubClass = "";
                compSubClass = companydata.Company.CompanyPurpose == "public" ? "ULSN" : compSubClass;
                compSubClass = companydata.Company.CompanyPurpose == "proprietary" ? "PROP" : compSubClass;

                var regAdd = "";
                var prncadd = "";
                if (companydata.Address.Any(x => x.IsRegisteredAddress == true))
                {
                    var a = companydata.Address.FirstOrDefault(x => x.IsRegisteredAddress == true);
                    regAdd = string.Format(@"{0} {1} {2} {3} {4}", a.UnitLevel.ToUpper(), a.Street.ToUpper(), a.Suburb.ToUpper(), a.State.ToUpper(), a.PostCode);
                }
                if (companydata.Address.Any(x => x.IsPrincipleAddress == true))
                {
                    var a = companydata.Address.FirstOrDefault(x => x.IsPrincipleAddress == true);
                    prncadd = string.Format(@"{0} {1} {2} {3} {4}", a.UnitLevel.ToUpper(), a.Street.ToUpper(), a.Suburb.ToUpper(), a.State.ToUpper(), a.PostCode);
                }


                foreach (var d in companydata.Directors)
                {
                    var dob = Convert.ToString(d.DoByear) + Convert.ToString(d.DoBmonth) + Convert.ToString(d.DoBday);
                    dirdetail += string.Format(@"ZSD  {0} {1} {2} {3} {4} {5}   {6}{7}", d.FirstName.ToUpper(), d.LastName.ToUpper(), dob, d.DoBcity.ToUpper(), d.DoBstate.ToUpper(), d.DoBcountry.ToUpper(), d.DoBaddress.ToUpper(),Environment.NewLine);
                    dirdetail += string.Format(@"ZOFDIR{0}",Environment.NewLine);
                }

                foreach (var s in companydata.Shares)
                {
                    var paidamount = (s.ShareAmount * s.NoOfShare);
                    var unpaidamount = (100 * s.ShareAmount) - paidamount;
                    sharedetails += string.Format(@"ZSC{0} {1}  {2} {3} {4}{5}", "ORD", "ORD", s.NoOfShare, paidamount, unpaidamount,Environment.NewLine);
                    sharedetails += string.Format(@"ZHH{0} {1} {2} {3}  {4} {5}     {6} {7}{8}", "ORD", "100", "Y", "N", paidamount, unpaidamount, s.ShareAmount, "0",Environment.NewLine);
                }

                var body = "";                
                body = System.IO.File.ReadAllText(Server.MapPath("~/Content/201template/temp.txt"), System.Text.Encoding.UTF8);
                body = body.Replace("{companyname}", companydata.Company.CompanyName.ToUpper());
                body = body.Replace("{companytype}", "APTY");
                body = body.Replace("{companyclass}", "LMSH");
                body = body.Replace("{companysubclass}", compSubClass.ToUpper());
                body = body.Replace("{companystate}", companydata.Company.RegistrationState.ToUpper());
                body = body.Replace("{companyregaddress}", regAdd);
                body = body.Replace("{companyprincipaladdress}", prncadd);
                body = body.Replace("{dirdetail}", dirdetail);
                body = body.Replace("{sharedetail}", sharedetails);
                body = body.Replace("{applicantname}", companydata.Applicant.LastName.ToUpper() + " " + companydata.Applicant.FirstName.ToUpper());
                body = body.Replace("{applicantaddress}", (CryptoHelper.DecryptString(companydata.Applicant.Email)).ToUpper());
                body = body.Replace("{datesign}", DateTime.Now.ToString("yyyyMMdd"));


                //byte[] bytes = Encoding.Default.GetBytes(body);
                //body = Encoding.UTF8.GetString(bytes);


                try
                {                    
                    var path = App_Code.Helper.getdateMothPath(System.Web.HttpContext.Current, "/Logs/FORM201/");
                    var fileName = Server.MapPath("~" + path + "/FORM201_" + companydata.Company.Id + ".txt");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (!System.IO.File.Exists(fileName))
                    {
                        FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                        fs.Close();
                        fs.Dispose();
                    }

                    System.IO.File.WriteAllText(fileName, body);
                    p = fileName;
                }
                catch (Exception ex) { }
            }

            return p;

        }



        private void sendform201toasic(string txtPath,long compid)
        {
            bool flag = false;
            string Errormsg = "";
            cou++;
            EDGETransport edgeTransportLayer = new EDGETransport(s_validateServerCertificate);
            edgeTransportLayer.Connect(s_server, s_port);
            //lblmsg.Text = cou + " : Connected to " + s_server + "</br>";
            // login
            edgeTransportLayer.Send(new LOGN(s_userId, s_userPassword).MessageToSend(true), true);
            InboundMessage msg = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), compid.ToString());

            if (msg.MessageName == "SVER")
            {
                SVER sver = (SVER)msg;
                flag = false;
                Errormsg = (sver.ErrorMessage);
            }
            else if (msg.MessageName == "LOGR")
            {
                LOGR logr = (LOGR)msg;

                if (!logr.Success)
                {
                    Errormsg = ("Login not accepted");
                    flag = false;
                }
                else
                {
                    callform201(edgeTransportLayer,txtPath,compid);
                    //lblmsg.Text = ("LOGR next client state: {REQO Calling..}");
                    Step3ReadLastFiles(edgeTransportLayer,compid);
                    //lblmsg.Text = "Response : </br>";
                    //string textme = System.IO.File.ReadAllText(@"C:\\Logs\\temp.log", System.Text.Encoding.UTF8);
                    //lblmsg.Text = textme + "<br><hr>";
                }
            }
            else
            {
                Errormsg = ("Unexpected reply to login: " + msg.RawMessage);
                flag = false;
            }

            // logout
            edgeTransportLayer.Send(new LOUT().MessageToSend(), true);
            edgeTransportLayer.Disconnect();
            if (!flag)
            {
                Response.Redirect("/setuperror");
            }
            //lblmsg.Text = ("Done");
        }




        #region ASIC
        private void callform201(EDGETransport edgeTransportLayer,string filepath,long compid)
        {
            s_certificateFileName = @"ComDeeds.pem";
            s_certificateFileName = Server.MapPath(s_certificateFileName);
            InboundMessage msgcp;
            if (true)
            {
                X509Certificate2 x509 = new X509Certificate2();
                string certificate = x509.FromPemFile(s_certificateFileName);
                string data = "";
                string finename = filepath;//txtcompanyid.Text + "_Only201.txt";
                data = gettext(finename).Replace("\r\n", "\n");

                BCHN bchn = new BCHN("TESTING8", "TXT");
                String bchnreply = bchn.MessageToSend();

                String f201reply = data;
                String Signature_f201 = Sign(f201reply, x509);
                String dataZXI = string.Format("ZXI{0}\t\tMD5\tRSA\t\t\t\t{1}\n", x509.DistinguishedNameOpenSSLFormat(), s_serialidentifier);
                f201reply = data + dataZXI + Signature_f201;

                TXID txid = new TXID("", "99999", "1", 1, s_agentNumber, false, true, certificate, x509);
                String txidreply = txid.MessageToSend(true);

                String ToSign201_TXID = f201reply + txidreply;
                String Signature_ToSign201_TXID = Sign(ToSign201_TXID, x509);
                String All = bchnreply + ToSign201_TXID + dataZXI + Signature_ToSign201_TXID;

                edgeTransportLayer.Send(All, true);
                msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), compid.ToString());

                // = (msgcp.Success + " - " + msgcp.ErrorMessage);

                if (msgcp.Success)
                    Step3ReadLastFiles(edgeTransportLayer,compid);
            }
        }
        private void Step3ReadLastFiles(EDGETransport edgeTransportLayer,long compid)
        {
            var flag = false;
            edgeTransportLayer.Send(new REQO().MessageToSend(true), true);

            while (true)
            {
                InboundMessage msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), compid.ToString());

                if (msgcp.Success)
                {
                    if (msgcp.MessageName.ToUpper() == "RA55")
                    {
                        //RA55 bout = (RA55)msgcp;
                        //lblmsg.Text = "Transaction Success, Certificate Generated Successfully.";
                        flag = true;
                        break;
                    }
                    else if (msgcp.MessageName.ToUpper() == "BOUT")
                    {
                        BOUT bout = (BOUT)msgcp;

                        if (bout.NoPendingFiles)
                            break;

                        msgcp = InboundMessage.InboundMessageFactory(edgeTransportLayer.Receive(), compid.ToString());
                        if (msgcp.Success)
                        {
                            SEPR sepr = (SEPR)msgcp;
                            flag = true;
                            //  lblmsg.Text = (sepr.Filename);
                        }
                    }
                }
                else
                    break;
            }
            if (flag)
            {
                Response.Redirect("/setupcomplete");
            }else
            {
                Response.Redirect("/setuperror");
            }
        }
        private string gettext(string filename)
        {
            string textme = System.IO.File.ReadAllText(filename, System.Text.Encoding.UTF8).Replace("\r\n", "\n");
            return textme;
        }
        protected string Sign(string message, X509Certificate2 x509)
        {
            message = x509.Sign(message);
            string signature = "";

            for (int i = 0; i < 200; i++)
            {
                if (message.Length < 64)
                {
                    signature += string.Format("ZXS{0}\n", message);
                    break;
                }

                signature += string.Format("ZXS{0}\n", message.Substring(0, 64));
                message = message.Substring(64);
            }

            return signature;
        }
        #endregion






    }
}