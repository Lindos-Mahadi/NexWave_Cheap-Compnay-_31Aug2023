using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace comdeeds.App_Code
{
    public enum EnumMessageType : int
    {
        Success = 1,
        Error = 2,
        Info = 3,
        Warning = 4
    }

    public class Helper
    {
        /// <summary>
        /// Admin Site notification
        /// </summary>
        public static string CreateMessage(string Message, EnumMessageType MessageType, string Title = "")
        {
            string Style = "";
            switch (MessageType)
            {
                case EnumMessageType.Success:
                    Style = "success";
                    break;

                case EnumMessageType.Error:
                    Style = "danger";
                    break;

                case EnumMessageType.Info:
                    Style = "info";
                    break;

                case EnumMessageType.Warning:
                    Style = "warning";
                    break;
            }

            if (string.IsNullOrEmpty(Title))
            {
                return string.Format("<div class='alert alert-dismissible alert-{0}'><button type='button' data-dismiss='alert' aria-label='Close' class='close'><span aria-hidden='true' class='mdi mdi-close'></span></button><p>{1}</p></div>", Style, Message);
            }
            else
            {
                string _title = string.Format("<h4>{0}</h4>", Title);
                return string.Format("<div class='alert alert-dismissible alert-{0}' ><button type='button' data-dismiss='alert' aria-label='Close' class='close'><span aria-hidden='true' class='mdi mdi-close'></span></button><h4>{1}</h4><p>{2}</p></div>", Style, string.IsNullOrEmpty(Title) ? string.Empty : _title, Message);
            }
        }


        /// <summary>
        /// Front end Site notification 
        /// </summary>
        public static string CreateNotification(string Message, EnumMessageType MessageType, string Title = "")
        {
            string Style = "";
            switch (MessageType)
            {
                case EnumMessageType.Success:
                    Style = "success";
                    break;

                case EnumMessageType.Error:
                    Style = "error";
                    break;

                case EnumMessageType.Info:
                    Style = "notice";
                    break;

                case EnumMessageType.Warning:
                    Style = "warning";
                    break;
            }

            if (string.IsNullOrEmpty(Title))
            {
                return string.Format("<div class='notification closeable {0}' style='margin-top: 20px'><p>{1}</p><a class='close'></a></div>", Style, Message);
            }
            else
            {
                string _title = string.Format("<h4>{0}</h4>", Title);
                return string.Format("<div class='notification closeable {0}' style='margin-top: 20px'><h4>{1}</h4><p>{2}</p><a class='close'></a></div>", Style, string.IsNullOrEmpty(Title) ? string.Empty : _title, Message);
            }
        }






        public static string GenerateSlug(string phrase)
        {
            string str = RemoveAccent(phrase).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it
            str = Regex.Replace(str, @"\s", "-"); // hyphens

            if (str.Length > 120)
            {
                str = str.Substring(0, 120);
            }
            return str;
        }

        public static string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string RandomString(int length)
        {
            int PasswordLength = length;
            string _allowedChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            string Text = new string(chars);
            return Text;
        }


        public static string Randomnumber(int length)
        {
            int PasswordLength = length;
            string _allowedChars = "0123456789";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            string Text = new string(chars);
            return Text;
        }



        public static string JoinURL(string ParentURL, string ChildURL)
        {
            string sJoined = string.Empty;
            if (ParentURL.EndsWith("/"))
            {
                ParentURL = ParentURL.TrimEnd('/');
            }
            if (ChildURL.StartsWith("/"))
            {
                ChildURL = ChildURL.TrimStart('/');
            }

            sJoined = string.Format("{0}/{1}", ParentURL, ChildURL);

            return sJoined;
        }

        public static string GetBaseURL()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

        public static string GetWebConfig_Appsetting_Value(string key)
        {
            return WebConfigurationManager.AppSettings[key];
        }


        public static string getdateMothPath(HttpContext context, string BasePath)
        {
            string _path = BasePath;
            if (!string.IsNullOrWhiteSpace(_path))
            {
                bool pexists = System.IO.Directory.Exists(context.Server.MapPath("~/" + _path));
                if (!pexists)
                {
                    System.IO.Directory.CreateDirectory(context.Server.MapPath("~/" + _path));
                }
                string month = DateTime.Now.ToString("MM");
                string year = DateTime.Now.ToString("yyyy");
                bool yexists = System.IO.Directory.Exists(context.Server.MapPath(Path.Combine("/" + _path + "/" + year)));
                bool mexists = System.IO.Directory.Exists(context.Server.MapPath(Path.Combine("/" + _path + "/" + year + "/" + month)));
                if (!yexists)
                {
                    System.IO.Directory.CreateDirectory(context.Server.MapPath(Path.Combine("/" + _path + "/" + year)));
                }
                if (!mexists)
                {
                    System.IO.Directory.CreateDirectory(context.Server.MapPath(Path.Combine("/" + _path + "/" + year + "/" + month)));
                }

                return Path.Combine(_path + "/" + year + "/" + month);
            }
            else
            {
                return "";
            }
        }


        public static string getFileType(string ext)
        {
            string ret = "img";
            if (ext == ".png" ||
                    ext == ".jpg" ||
                    ext == ".jpeg" ||
                    ext == ".bmp" ||
                    ext == ".gif" ||
                    ext == ".svg")
            {
                ret = "img";
            }
            else if (ext == ".doc" || ext == ".docx") { ret = "doc"; }
            else if (ext == ".xls" || ext == ".xl") { ret = "xl"; }
            else if (ext == ".pdf") { ret = "pdf"; }
            else if (ext == ".mp3") { ret = "mp3"; }
            else if (ext == ".mp4" || ext == ".mpeg" || ext == ".avi" || ext == ".3gp" || ext == ".ogg" || ext == ".flv") { ret = "video"; }
            return ret;
        }


        //for client side validation
        public const string MulitEmails = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*([,;]\\s*\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)*";

        public const string SingleEmail = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        /// <summary>
        /// Check email is valid format
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsValidEmail(string email)
        {
            bool valid = false;
            if (!string.IsNullOrEmpty(email))
            {
                valid = Regex.Match(email, Helper.SingleEmail).Success;
            }
            return valid;
        }

        /// <summary>
        /// Check multiple emails are valid 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsValidMultiEmails(string email)
        {
            bool valid = false;
            if (!string.IsNullOrEmpty(email))
            {
                valid = Regex.Match(email, Helper.MulitEmails).Success;
            }
            return valid;
        }

    }
}