using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static comdeeds.Models.BaseModel;

namespace comdeeds.App_Code
{
    public class UserMethods
    {
        public static ClassUserDetails GetUserById(long userid)
        {
            ClassUserDetails user = new ClassUserDetails();
            using (var db = new MyDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var u = db.TblUsers.AsNoTracking().Where(x => x.Del == false && x.Id == userid).FirstOrDefault();
                if (u != null)
                {
                    user.Firstname = u.FirstName;
                    user.Lastname = u.LastName;
                    user.Email = CryptoHelper.DecryptString(u.Email);
                    user.password = CryptoHelper.DecryptString(u.Password);
                    user.Phone = u.Phone;
                    user.Id = u.Id;
                    user.EmailVerified = Convert.ToBoolean(u.EmailVerified);
                    user.AddedDate = Convert.ToDateTime(u.AddedDate);
                    user.Lastlogin = u.LastLogIn;
                    user.Tuser = u.Tuser;
                    user._Role = u.Role;
                }
            }
            return user;
        }

        //public static string RegisterUserThruForm(ClassUserDetails form)
        //{
        //    string msg = "";
        //    if (!string.IsNullOrEmpty(form.Firstname))
        //    {
        //        if (!string.IsNullOrEmpty(form.Email) && !string.IsNullOrEmpty(form.password))
        //        {
        //            if (Helper.IsValidEmail(form.Email))
        //            {
        //                var user = new TblUser()
        //                {
        //                    FirstName = form.Firstname,
        //                    LastName = form.Lastname,
        //                    Email = CryptoHelper.EncryptData(form.Email),
        //                    Password = CryptoHelper.EncryptData(form.password),
        //                    Phone = form.Phone,
        //                    AddedDate = DateTime.Now,
        //                    UpdatedDate = DateTime.Now,
        //                    Role = "USER",
        //                    EmailVerified = true,
        //                    Del = false
        //                };

        //                using (var db = new MyDbContext())
        //                {
        //                    db.Configuration.AutoDetectChangesEnabled = false;
        //                    if (!db.TblUsers.AsNoTracking().Any(x => x.Email == user.Email && x.Del == false))
        //                    {
        //                        var uctx = db.TblUsers.Add(user);
        //                        if (db.SaveChanges() > 0)
        //                        {
        //                            var uData = new LoginUserData
        //                            {
        //                                email = form.Email,
        //                                IsFirstLogin = true,
        //                                LastLogin = DateTime.Now
        //                            };
        //                            var uDataJson = JsonConvert.SerializeObject(uData);
        //                            AuthHelper.SignIn(form.Firstname, true, new List<string>() { "USER" }, user.Id.ToString(), uDataJson);
        //                            //   msg = "success";
        //                        }

        //                    }
        //                    else
        //                    {
        //                        msg = Helper.CreateMessage("This email is already registered, please use different email id.", EnumMessageType.Error, "Error");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                msg = Helper.CreateMessage("This email is invalid , please use different email id.", EnumMessageType.Error, "Invalid email");
        //            }

        //        }
        //        else
        //        {
        //            msg = Helper.CreateMessage("Please provide email and password. ", EnumMessageType.Error, "Error");
        //        }
        //    }
        //    else
        //    {
        //        msg = Helper.CreateMessage("Please provide your name. ", EnumMessageType.Error, "Error");
        //    }

        //    return msg;
        //}

        public static string RegisterUserThruForm(ClassUserDetails form)
        {
            string Regid1 = "";
            string msg = "";
            if (!string.IsNullOrEmpty(form.Firstname))
            {
                if (!string.IsNullOrEmpty(form.Email) && !string.IsNullOrEmpty(form.password))
                {
                    if (Helper.IsValidEmail(form.Email))
                    {
                        var user = new TblUser()
                        {
                            FirstName = form.Firstname,
                            LastName = form.Lastname,
                            Email = CryptoHelper.EncryptData(form.Email),
                            Password = CryptoHelper.EncryptData(form.password),
                            Phone = form.Phone,
                            AddedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            Role = "USER",
                            EmailVerified = true,
                            Del = false,
                            Tuser = form.Tuser
                        };

                        using (var db = new MyDbContext())
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            if (!db.TblUsers.AsNoTracking().Any(x => x.Email == user.Email && x.Del == false))
                            {
                                /// insert registration
                                dal.Operation oper = new dal.Operation();
                                Regid1 = oper.insertRegistration(form.Firstname, form.Lastname, form.Email, form.password, form.Phone);

                                ///

                                var user1 = new TblUser()
                                {
                                    FirstName = form.Firstname,
                                    LastName = form.Lastname,
                                    Email = CryptoHelper.EncryptData(form.Email),
                                    Password = CryptoHelper.EncryptData(form.password),
                                    Phone = form.Phone,
                                    AddedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                    Role = "USER",
                                    EmailVerified = true,
                                    Del = false,
                                    Regid = Convert.ToInt64(Regid1),
                                    Tuser = form.Tuser
                                };

                                var uctx = db.TblUsers.Add(user1);
                                if (db.SaveChanges() > 0)
                                {
                                    var uData = new LoginUserData
                                    {
                                        email = form.Email,
                                        IsFirstLogin = true,
                                        LastLogin = DateTime.Now
                                    };
                                    var uDataJson = JsonConvert.SerializeObject(uData);
                                    AuthHelper.SignIn(form.Firstname, true, new List<string>() { "USER" }, user1.Id.ToString(), uDataJson);
                                    //   msg = "success";
                                }
                            }
                            else
                            {
                                msg = Helper.CreateNotification("This email is already registered, please use different email id.", EnumMessageType.Error, "Error");
                            }
                        }
                    }
                    else
                    {
                        msg = Helper.CreateNotification("This email is invalid , please use different email id.", EnumMessageType.Error, "Invalid email");
                    }
                }
                else
                {
                    msg = Helper.CreateNotification("Please provide email and password. ", EnumMessageType.Error, "Error");
                }
            }
            else
            {
                msg = Helper.CreateNotification("Please provide your name. ", EnumMessageType.Error, "Error");
            }

            return msg;
        }

        internal static TblUser GetUserLogin_New(string EncryptedEmail, string EncryptedPass)
        {
            var uData = new TblUser();
            try
            {  
                if (!string.IsNullOrEmpty(EncryptedEmail) && !string.IsNullOrEmpty(EncryptedPass))
                {
                    using (var db = new MyDbContext())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        var p = new DynamicParameters();
                        p.Add("@email", EncryptedEmail, dbType: System.Data.DbType.String);
                        p.Add("@pass", EncryptedPass, dbType: System.Data.DbType.String);
                        uData = db.Database.Connection.Query<TblUser>("userlogin_New", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog le = new ErrorLog();
                le.WriteErrorLog("UserMethods_LineNo_216_Start_"+ex.Message+"_End_Line_216");
            }
            return uData;
        }

        internal static TblUser GetUserLoginID(string EncryptedEmail)
        {
            var uData = new TblUser();
            if (!string.IsNullOrEmpty(EncryptedEmail))
            {
                using (var db = new MyDbContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    var p = new DynamicParameters();
                    p.Add("@email", EncryptedEmail, dbType: System.Data.DbType.String);
                    uData = db.Database.Connection.Query<TblUser>("userlogin_New1", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                }
            }
            return uData;
        }

        internal static TblUser GetUserLogin(string EncryptedEmail, string EncryptedPass, string regNo)
        {
            var uData = new TblUser();
            if (!string.IsNullOrEmpty(EncryptedEmail) && !string.IsNullOrEmpty(EncryptedPass) && !string.IsNullOrEmpty(regNo))
            {
                using (var db = new MyDbContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    var p = new DynamicParameters();
                    p.Add("@email", EncryptedEmail, dbType: System.Data.DbType.String);
                    p.Add("@pass", EncryptedPass, dbType: System.Data.DbType.String);
                    p.Add("@regno", regNo, dbType: System.Data.DbType.String);
                    uData = db.Database.Connection.Query<TblUser>("userlogin", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                }
            }
            return uData;
        }

        internal static TblUser GetUserLogin_Admin(string EncryptedEmail, string EncryptedPass, string regNo)
        {
            var uData = new TblUser();
            if (!string.IsNullOrEmpty(EncryptedEmail) && !string.IsNullOrEmpty(EncryptedPass) && !string.IsNullOrEmpty(regNo))
            {
                using (var db = new MyDbContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    var p = new DynamicParameters();
                    p.Add("@email", EncryptedEmail, dbType: System.Data.DbType.String);
                    p.Add("@pass", EncryptedPass, dbType: System.Data.DbType.String);
                    p.Add("@regno", regNo, dbType: System.Data.DbType.String);
                    uData = db.Database.Connection.Query<TblUser>("adminlogin_after", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                }
            }
            return uData;
        }

        public static string ChangePassword(string oldpass, string newpass, long uid)
        {
            string msg = "";
            using (var db = new MyDbContext())
            {
                var user = db.TblUsers.AsNoTracking().FirstOrDefault(x => x.Id == uid && x.Del == false);
                var Reg = db.Registrations.AsNoTracking().FirstOrDefault(x => x.Sno == user.Regid && x.Isactive == 1);
                if (CryptoHelper.EncryptData(oldpass) == user.Password)
                {
                    var u = new TblUser { Id = uid };
                    db.TblUsers.Attach(u);
                    u.Password = CryptoHelper.EncryptData(newpass);
                    var entry = db.Entry(u);
                    entry.Property(e => e.Password).IsModified = true;

                    var v = new Registration { Sno = Convert.ToInt32(user.Regid) };
                    db.Registrations.Attach(v);
                    v.Pass = newpass;
                    var entryR = db.Entry(v);
                    entryR.Property(e => e.Pass).IsModified = true;

                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        msg = Helper.CreateMessage("Password changed successfully !", EnumMessageType.Success, "Success");
                    }
                    else
                    {
                        msg = Helper.CreateMessage("Password did not changed , server error , please try again !", EnumMessageType.Error, "Error");
                    }
                }
                else
                {
                    msg = Helper.CreateMessage("Password is not correct , please enter a correct password !", EnumMessageType.Error, "Error");
                }
            }
            return msg;
        }

		public static string ResetUserPassword(string email, string newpass, long uid)
		{
			string msg = "";
			using (var db = new MyDbContext())
			{
				var user = db.TblUsers.AsNoTracking().FirstOrDefault(x => x.Id == uid && x.Del == false);
				var Reg = db.Registrations.AsNoTracking().FirstOrDefault(x => x.Sno == user.Regid && x.Isactive == 1);
				if (CryptoHelper.EncryptData(email) == user.Email)
				{
					var u = new TblUser { Id = uid };
					db.TblUsers.Attach(u);
					u.Password = CryptoHelper.EncryptData(newpass);
					var entry = db.Entry(u);
					entry.Property(e => e.Password).IsModified = true;

					var v = new Registration { Sno = Convert.ToInt32(user.Regid) };
					db.Registrations.Attach(v);
					v.Pass = newpass;
					var entryR = db.Entry(v);
					entryR.Property(e => e.Pass).IsModified = true;

					int i = db.SaveChanges();
					if (i > 0)
					{
						msg = Helper.CreateMessage("Password Reset successfully !", EnumMessageType.Success, "Success");
					}
					else
					{
						msg = Helper.CreateMessage("Password did not Reset , server error , please try again !", EnumMessageType.Error, "Error");
					}
				}
				else
				{
					msg = Helper.CreateMessage("Password is not correct , please enter a correct password !", EnumMessageType.Error, "Error");
				}
			}
			return msg;
		}
	}
}