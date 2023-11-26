using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static comdeeds.Models.BaseModel;

namespace comdeeds.App_Code
{
    public class BuildCompanyPDF
    {
        //  --------------------    KEYS     --------------------
        //  {companyname} ->        company name
        //  {acn } ->               ACN
        //  {directorshare} ->      Share cert of other directors
        //  {regofficeadd} ->       Registered office address
        //  {principaloffadd} ->    Principle place of company
        //  {director1 } ->         first director
        //  {director1add } ->      first director address
        //  {director1dob} ->       first director date of birth
        //  {director1pob} ->       first director place of birth
        //  {dir1noofshare}->       First Director share no
        //  {dir1sharecost}->       First Director share amount including $  sign
        //  {companyexecdate} ->    Company registration Execution/reg date
        //  {companyregistryrows}   tables rows for company registry
        //  {totaldirectors}->      Total no of rows

        // {totalpaidshare} -> total paid share amount
        // {totalunpaidshare} -> total unpaid share amount
        //  {alldiretorsname } ->   all director's name followed by <br/>
        //  {applicationshare} ->   share application

        //  {sharenowithdir} -> share cert of all director like below example
        //  ----------------
        //  Share Certificate No. 1 JAMES RAYMOND MCKECHNIE<br />
        //  Share Certificate No. 2 MARIA CONCETTA CRISANTE
        //  ---------

        #region Certification

        public static string BuildCertPDF(string html, ClassFullCompany companyData)
        {
            string str = string.Empty;

            // CHANGE THIS ACCORDING TO DATA
            // HERE ASSUMING TRANSACTION DATE WOULD BE THE DATE WHEN COMPANY REGISTERED
            string companyDate = companyData.TransactionDetail.UpdatedDate.Value.ToString("dd MMMM yyyy");
            string companyACN = companyData.CompanyMeta.CompanyACN; // PLEASE ENTER ACN NO HERE
            string directorshare = string.Empty;
            string companyregistryRows = string.Empty;
            // First Directors details
            string Dir1Name = "",
                   Dir1Add = "",
                   Dir1DobPlace = "",
                   Dir1Dob = "",
                   Dir1shareno = "",
                   Dir1isbeneficial = "",
                   Dir1shareCost = "";
            string DirectorConsent = "";
            string shareapplication = string.Empty;
            string alldiretorsname = string.Empty;
            string shareNoWithDir = string.Empty;

            decimal totalPaidshareAmt = 0;
            decimal totalUnPaidshareAmt = 0;
            string RegOfcAdd = "";
            string principalOfcAdd = "";

            var RegOfcAddModel = companyData.Address.Where(x => x.IsRegisteredAddress == true).FirstOrDefault();
            var principalOfcAddModel = companyData.Address.Where(x => x.IsPrincipleAddress == true).FirstOrDefault();

            if (RegOfcAddModel.UnitLevel != "" && RegOfcAddModel.UnitLevel.Trim().IndexOf(" ") > -1)
            {
                RegOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                RegOfcAddModel.UnitLevel.Trim().Replace(" ", "/"),
                RegOfcAddModel.Street,
                RegOfcAddModel.Suburb,
                RegOfcAddModel.State,
                RegOfcAddModel.PostCode);
            }
            else
            {
                RegOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                RegOfcAddModel.UnitLevel,
                RegOfcAddModel.Street,
                RegOfcAddModel.Suburb,
                RegOfcAddModel.State,
                RegOfcAddModel.PostCode);
            }

            if (principalOfcAddModel.UnitLevel != "" && principalOfcAddModel.UnitLevel.Trim().IndexOf(" ") > -1)
            {
                principalOfcAdd = string.Format("{0} {1} {2} {3} {4}",
                principalOfcAddModel.UnitLevel.Trim().Replace(" ", "/"),
                principalOfcAddModel.Street,
                principalOfcAddModel.Suburb,
                principalOfcAddModel.State,
                principalOfcAddModel.PostCode);
            }
            else
            {
                principalOfcAdd = string.Format("{0}  {1} {2} {3} {4}",
                principalOfcAddModel.UnitLevel,
                principalOfcAddModel.Street,
                principalOfcAddModel.Suburb,
                principalOfcAddModel.State,
                principalOfcAddModel.PostCode);
            }

            //  ------------  for first director

            var Dir1 = companyData.Directors.OrderBy(x => x.Id).FirstOrDefault();
            long dir1ID = Dir1.Id;

            Dir1Name = string.Format("{0} {1}", Dir1.FirstName, Dir1.LastName);

            Dir1.DoBaddress = Dir1.DoBaddress.Replace("/",",");
            int index = Dir1.DoBaddress.IndexOf(",");
            if (index == 0)
            { Dir1.DoBaddress = Dir1.DoBaddress.Substring(1); }
            else { Dir1.DoBaddress = "Unit " + Dir1.DoBaddress; }

            Dir1Add = Dir1.DoBaddress;
            Dir1Dob = Dir1.DoBday + "/" + Dir1.DoBmonth + "/" + Dir1.DoByear;
            Dir1DobPlace = Dir1.DoBcity +" "+ Dir1.DoBstate +" "+ Dir1.DoBcountry;
            Dir1shareno = companyData.Shares.Where(x => x.DirectorId == dir1ID).FirstOrDefault().NoOfShare.ToString();
            Dir1shareCost = companyData.Shares.Where(x => x.DirectorId == dir1ID).FirstOrDefault().ShareAmount.ToString();
            Dir1isbeneficial = companyData.Shares.Where(x => x.DirectorId == dir1ID).FirstOrDefault().ShareBehalf.ToString();
            alldiretorsname = Dir1Name + "(Secretary) <br/>";
            string Dir1sharetype = companyData.Shares.Where(x => x.DirectorId == dir1ID).FirstOrDefault().ShareClass;
            Dir1sharetype = Dir1sharetype == "ordinary" ? "ORD" : Dir1sharetype;
            shareNoWithDir = "Share Certificate No. 1 " + Dir1Name + " <br />";

            totalPaidshareAmt += Convert.ToDecimal(Dir1shareCost);
            //totalUnPaidshareAmt += Convert.ToDecimal(Dir1shareCost);

            if (Dir1shareCost.Contains("."))
            {
                companyregistryRows = string.Format(@"<tr>
                        <td>{0} </td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                    </tr>",
                   Dir1.LastName,
                   Dir1.FirstName,
                   "", // no middle name in db
                   Dir1.DoBaddress,
                   Dir1shareno + " " + Dir1sharetype,
                   Dir1isbeneficial == "True" ? "Y" : "N",
                   "$" + Dir1shareCost,
                   "$0.00"
                   );

                shareapplication += getshareapplication(
              companyData.Company.CompanyName.ToUpper(),
              companyACN,
              RegOfcAdd,
              Dir1Name,
              Dir1.DoBaddress,
              companyDate,
              Convert.ToString(Dir1shareno),
              "$" + Dir1shareCost,
              Dir1sharetype,
              Dir1isbeneficial == "True" ? "Y" : "N"
              );
            }
            else
            {
                companyregistryRows = string.Format(@"<tr>
                        <td>{0} </td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                    </tr>",
                       Dir1.LastName,
                       Dir1.FirstName,
                       "", // no middle name in db
                       Dir1.DoBaddress,
                       Dir1shareno + " " + Dir1sharetype,
                       Dir1isbeneficial == "True" ? "Y" : "N",
                       "$" + Dir1shareCost + ".00",
                       "$0.00"
                       );

                shareapplication += getshareapplication(
              companyData.Company.CompanyName.ToUpper(),
              companyACN,
              RegOfcAdd,
              Dir1Name,
              Dir1.DoBaddress,
              companyDate,
              Convert.ToString(Dir1shareno),
              "$" + Dir1shareCost + ".00",
              Dir1sharetype,
              Dir1isbeneficial == "True" ? "Y" : "N"
              );
            }

            //  ------------  for first director end

            var secondarydirectors = companyData.Directors.Where(x => x.Id != dir1ID).ToList();
            int i = 2;

            foreach (var d in secondarydirectors.OrderBy(x => x.Id))
            {
                double? Dirshareno = companyData.Shares.Where(x => x.DirectorId == d.Id).FirstOrDefault().NoOfShare;
                double? DirshareCost = companyData.Shares.Where(x => x.DirectorId == d.Id).FirstOrDefault().ShareAmount;
                string sharetype = companyData.Shares.Where(x => x.DirectorId == d.Id).FirstOrDefault().ShareClass;
                string Dirisbeneficial = companyData.Shares.Where(x => x.DirectorId == d.Id).FirstOrDefault().ShareBehalf.ToString();

                totalPaidshareAmt += Convert.ToDecimal(DirshareCost);
                //totalUnPaidshareAmt += Convert.ToDecimal(Dir1shareCost);
                sharetype = sharetype == "ordinary" ? "ORD" : sharetype;

                d.DoBaddress = d.DoBaddress.Replace("/", ",");
                int index1 = d.DoBaddress.IndexOf(",");
                if (index1 == 0)
                { d.DoBaddress = d.DoBaddress.Substring(1); }
                else { d.DoBaddress = "Unit " + d.DoBaddress; }

                if (DirshareCost.ToString().Contains("."))
                {
                    directorshare += getshareswithF(companyData.Company.CompanyName.ToUpper(),
                  companyACN,
                  RegOfcAdd,
                  d.FirstName + " " + d.LastName,
                  d.DoBaddress,
                  companyDate,
                  Convert.ToInt32(Dirshareno),
                  Convert.ToDouble(DirshareCost),
                  i + " of " + (secondarydirectors.Count + 1) + " inclusive ",
                  sharetype
                  );

                    companyregistryRows += string.Format(@"<tr>
                        <td>{0} </td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                    </tr>",
                   d.LastName,
                   d.FirstName,
                   "", // no middle name in db
                   d.DoBaddress,
                   Dirshareno + " " + sharetype,
                   Dirisbeneficial == "True" ? "Y" : "N",
                   "$" + DirshareCost,
                   "$0.00"
                   );

                    DirectorConsent += getconsent(companyData.Company.CompanyName.ToUpper(),
                companyACN,
                d.FirstName + " " + d.LastName,
                d.DoBday + "/" + d.DoBmonth + "/" + d.DoByear,
                d.DoBaddress,
                d.DoBcity + " " + d.DoBstate + " " + d.DoBcountry,
                companyDate
                );

                    shareapplication += getshareapplication(
                        companyData.Company.CompanyName.ToUpper(),
                        companyACN,
                        RegOfcAdd,
                        d.FirstName + " " + d.LastName,
                        d.DoBaddress,
                        companyDate,
                        Convert.ToString(Dirshareno),
                        "$" + DirshareCost,
                        sharetype,
                        Dirisbeneficial == "True" ? "Y" : "N"
                        );
                }
                else
                {
                    directorshare += getshares(companyData.Company.CompanyName.ToUpper(),
                  companyACN,
                  RegOfcAdd,
                  d.FirstName + " " + d.LastName,
                  d.DoBaddress,
                  companyDate,
                  Convert.ToInt32(Dirshareno),
                  Convert.ToDouble(DirshareCost),
                  i + " of " + (secondarydirectors.Count + 1) + " inclusive "
                  );

                    companyregistryRows += string.Format(@"<tr>
                        <td>{0} </td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                    </tr>",
                   d.LastName,
                   d.FirstName,
                   "", // no middle name in db
                   d.DoBaddress,
                   Dirshareno + " " + sharetype,
                    Dirisbeneficial == "True" ? "Y" : "N",
                   "$" + DirshareCost + ".00",
                   "$0.00"
                   );

                    DirectorConsent += getconsent(companyData.Company.CompanyName.ToUpper(),
                companyACN,
                d.FirstName + " " + d.LastName,
                d.DoBday + "/" + d.DoBmonth + "/" + d.DoByear,
                d.DoBaddress,
                d.DoBcity + " " + d.DoBstate,
                companyDate
                );

                    shareapplication += getshareapplication(
                        companyData.Company.CompanyName.ToUpper(),
                        companyACN,
                        RegOfcAdd,
                        d.FirstName + " " + d.LastName,
                        d.DoBaddress,
                        companyDate,
                        Convert.ToString(Dirshareno),
                        "$" + DirshareCost + ".00",
                        sharetype,
                         Dirisbeneficial == "True" ? "Y" : "N"
                        );
                }

                alldiretorsname += d.FirstName + " " + d.LastName + "<br/>";
                shareNoWithDir += "Share Certificate No. " + i + " " + d.FirstName + " " + d.LastName + " <br />";
                i++;
            }

            var individual_or_companys = companyData.indShares.Where(x => x.Id != dir1ID).ToList();
            int aa = 3;

            foreach (var d in individual_or_companys.OrderBy(x => x.Id))
            {
                if (d.individual_or_company == "Individual1" || d.individual_or_company == "individual1")
                {
                    double? Dirshareno = d.NoOfShare;
                    double? DirshareCost = d.ShareAmount;
                    string sharetype = d.ShareClass;
                    sharetype = sharetype == "ordinary" ? "ORD" : sharetype;
                    string isbeneficial = d.ShareBehalf.ToString();

                    string[] shareholdernamee = null;
                    string fiName = string.Empty;
                    string miName = string.Empty;
                    string liName = string.Empty;

                    totalPaidshareAmt += Convert.ToDecimal(DirshareCost);
                    //totalUnPaidshareAmt += Convert.ToDecimal(Dir1shareCost);

                    if (d.shareholderdetails != "")
                    {
                        shareholdernamee = d.shareholderdetails.Split(' ');
                        if (shareholdernamee.Length == 3 || shareholdernamee.Length > 2)
                        {
                            liName = shareholdernamee[2];
                            miName = shareholdernamee[1];
                            fiName = shareholdernamee[0];
                        }
                        else if (shareholdernamee.Length == 2 || shareholdernamee.Length > 1)
                        {
                            liName = shareholdernamee[1];
                            fiName = shareholdernamee[0];
                        }
                        else
                        {
                            fiName = d.shareholderdetails;
                        }
                    }

                    d.individual_or_company_address = d.individual_or_company_address.Replace("/", ",");
                    int index1 = d.individual_or_company_address.IndexOf(",");
                    if (index1 == 0)
                    { d.individual_or_company_address = d.individual_or_company_address.Substring(1); }
                    else { d.individual_or_company_address = "Unit " + d.individual_or_company_address; }

                    if (DirshareCost.ToString().Contains("."))
                    {
                        directorshare += getshareswithF(companyData.Company.CompanyName.ToUpper(),
                     companyACN,
                     RegOfcAdd,
                     d.shareholderdetails,
                     d.individual_or_company_address,
                     companyDate,
                     Convert.ToInt32(Dirshareno),
                     Convert.ToDouble(DirshareCost),
                     i + " of " + (individual_or_companys.Count + secondarydirectors.Count + 1) + " inclusive ", sharetype
                     );

                        companyregistryRows += string.Format(@"<tr>
                        <td>{0} </td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                    </tr>",
                      liName,
                      fiName,
                      miName, // no middle name in db
                      d.individual_or_company_address,
                      Dirshareno + " " + sharetype,
                      isbeneficial == "True" ? "Y" : "N",
                      "$" + DirshareCost,
                      "$0.00"
                      );

                        DirectorConsent += getconsent_d(companyData.Company.CompanyName.ToUpper(),
                       companyACN,
                       d.shareholderdetails + " ",
                       DOB(d.individual_or_company_dob.ToString()),
                       d.individual_or_company_address,
                       d.placeofbirth + " ",
                       companyDate
                       );

                        shareapplication += getshareapplication(
                      companyData.Company.CompanyName.ToUpper(),
                      companyACN,
                      RegOfcAdd,
                      d.shareholderdetails,
                      d.individual_or_company_address,
                      companyDate,
                      Convert.ToString(Dirshareno),
                      "$" + DirshareCost,
                      sharetype,
                      isbeneficial == "True" ? "Y" : "N"
                      );
                    }
                    else
                    {
                        directorshare += getshares(companyData.Company.CompanyName.ToUpper(),
                                             companyACN,
                                             RegOfcAdd,
                                             d.shareholderdetails,
                                             d.individual_or_company_address,
                                             companyDate,
                                             Convert.ToInt32(Dirshareno),
                                             Convert.ToDouble(DirshareCost),
                                             i + " of " + (individual_or_companys.Count + secondarydirectors.Count + 1)
                                             );

                        companyregistryRows += string.Format(@"<tr>
                        <td>{0} </td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                    </tr>",
                      liName,
                      fiName,
                      miName, // no middle name in db
                      d.individual_or_company_address,
                      Dirshareno + " " + sharetype,
                      isbeneficial == "True" ? "Y" : "N",
                      "$" + DirshareCost + ".00",
                      "$0.00"
                      );

                        DirectorConsent += getconsent_d(companyData.Company.CompanyName.ToUpper(),
                       companyACN,
                       d.shareholderdetails + " ",
                       DOB(d.individual_or_company_dob.ToString()),
                       d.individual_or_company_address,
                       d.placeofbirth + " ",
                       companyDate
                       );

                        shareapplication += getshareapplication(
                      companyData.Company.CompanyName.ToUpper(),
                      companyACN,
                      RegOfcAdd,
                      d.shareholderdetails,
                      d.individual_or_company_address,
                      companyDate,
                      Convert.ToString(Dirshareno),
                      "$" + DirshareCost + ".00",
                      sharetype,
                      isbeneficial == "True" ? "Y" : "N"
                      );
                    }

                    alldiretorsname += d.shareholderdetails + "<br/>";
                    shareNoWithDir += "Share Certificate No. " + i + " " + d.shareholderdetails + " <br />";
                    i++;
                }
                else
                {
                    double? Dirshareno = d.NoOfShare;
                    double? DirshareCost = d.ShareAmount;
                    string sharetype = d.ShareClass;
                    sharetype = sharetype == "ordinary" ? "ORD" : sharetype;
                    string isbeneficial = d.ShareBehalf.ToString();
                    string abn = d.individual_or_company_acn;

                    totalPaidshareAmt += Convert.ToDecimal(DirshareCost);
                    //totalUnPaidshareAmt += Convert.ToDecimal(Dir1shareCost);

                    if (DirshareCost.ToString().Contains("."))
                    {
                        directorshare += getshareswithF(companyData.Company.CompanyName.ToUpper(),
                  companyACN,
                  RegOfcAdd,
                  d.shareholderdetails,
                  d.individual_or_company_address,
                  companyDate,
                  Convert.ToInt32(Dirshareno),
                  Convert.ToDouble(DirshareCost),
                  i + " of " + (individual_or_companys.Count + secondarydirectors.Count + 1) + " inclusive ", sharetype
                  );

                        companyregistryRows += string.Format(@"<tr>
                        <td>{0} </td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                    </tr>",
                         d.shareholderdetails,
                         "",
                         "", // no middle name in db
                         d.individual_or_company_address,
                         Dirshareno + " " + sharetype,
                         isbeneficial == "True" ? "Y" : "N",
                         "$" + DirshareCost,
                         "$0.00"
                         );
                        DirectorConsent += getconsent_c(companyData.Company.CompanyName.ToUpper(),
                         companyACN,
                         d.shareholderdetails + " ",
                         abn,
                         d.individual_or_company_address,
                         d.placeofbirth + " ",
                         companyDate
                         );

                        shareapplication += getshareapplication(
                         companyData.Company.CompanyName.ToUpper(),
                         companyACN,
                         RegOfcAdd,
                         d.shareholderdetails,
                         d.individual_or_company_address,
                         companyDate,
                         Convert.ToString(Dirshareno),
                         "$" + DirshareCost,
                         sharetype,
                         isbeneficial == "True" ? "Y" : "N"
                         );
                    }
                    else
                    {
                        directorshare += getshares(companyData.Company.CompanyName.ToUpper(),
                   companyACN,
                   RegOfcAdd,
                   d.shareholderdetails,
                   d.individual_or_company_address,
                   companyDate,
                   Convert.ToInt32(Dirshareno),
                   Convert.ToDouble(DirshareCost),
                   i + " of " + (individual_or_companys.Count + secondarydirectors.Count + 1) + " inclusive "
                   );

                        companyregistryRows += string.Format(@"<tr>
                        <td>{0} </td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                    </tr>",
                           d.shareholderdetails,
                           "",
                           "", // no middle name in db
                           d.individual_or_company_address,
                           Dirshareno + " " + sharetype,
                           isbeneficial == "True" ? "Y" : "N",
                           "$" + DirshareCost + ".00",
                           "$0.00"
                           );

                        DirectorConsent += getconsent_c(companyData.Company.CompanyName.ToUpper(),
                            companyACN,
                            d.shareholderdetails + " ",
                            abn,
                            d.individual_or_company_address,
                            d.placeofbirth + " ",
                            companyDate
                            );

                        shareapplication += getshareapplication(
                           companyData.Company.CompanyName.ToUpper(),
                           companyACN,
                           RegOfcAdd,
                           d.shareholderdetails,
                           d.individual_or_company_address,
                           companyDate,
                           Convert.ToString(Dirshareno),
                           "$" + DirshareCost + ".00",
                           sharetype,
                           isbeneficial == "True" ? "Y" : "N"
                           );
                    }

                    alldiretorsname += d.shareholderdetails + "<br/>";
                    shareNoWithDir += "Share Certificate No. " + i + " " + d.shareholderdetails + " <br />";
                    i++;
                }
            }

            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace("{companyname}", companyData.Company.CompanyName.ToUpper());
                html = html.Replace("{acn}", companyACN);
                html = html.Replace("{directorshare}", directorshare);
                html = html.Replace("{regofficeadd}", RegOfcAdd);
                html = html.Replace("{principaloffadd}", principalOfcAdd);
                html = html.Replace("{director1}", Dir1Name);
                html = html.Replace("{director1add}", Dir1Add);
                html = html.Replace("{director1dob}", Dir1Dob);
                html = html.Replace("{director1pob}", Dir1DobPlace);
                html = html.Replace("{dir1noofshare}", Dir1shareno);
                html = html.Replace("{shareTtype}", getFullShareType(Dir1sharetype));
                if (Dir1shareCost.Contains(".")) { html = html.Replace("{dir1sharecost}", "$" + Dir1shareCost); }
                else { html = html.Replace("{dir1sharecost}", "$" + Dir1shareCost + ".00"); }
                html = html.Replace("{companyexecdate}", companyDate);
                html = html.Replace("{companyregistryrows}", companyregistryRows);
                html = html.Replace("{totaldirectors}", (i - 1).ToString());
                html = html.Replace("{totalpaidshare}", totalPaidshareAmt.ToString());
                html = html.Replace("{totalunpaidshare}", totalUnPaidshareAmt.ToString().Replace("0", "0.00"));
                html = html.Replace("{directorconsent}", DirectorConsent);
                html = html.Replace("{applicationshare}", shareapplication);
                html = html.Replace("{alldiretorsname}", alldiretorsname);
                html = html.Replace("{sharenowithdir}", shareNoWithDir);
            }

            return html;
        }

        public static string DOB(string dob)
        {
            try
            {
                if (dob != "")
                {
                    string[] dob_ = dob.ToString().Split('-');
                    return dob_[2] + "/" + dob_[1] + "/" + dob_[0];
                }
                else
                {
                    return dob;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return dob;
        }

        public static string shareName(string shareholdername)
        {
            string[] shareholdernamee = null;
            try
            {
                if (shareholdername != "")
                {
                    shareholdernamee = shareholdername.Split(' ');
                    return shareholdernamee[1] + " " + shareholdernamee[0];
                }
                else
                {
                    return shareholdername;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return shareholdername;
        }

        private static string getshares(string companyname, string acn, string address, string directorname,
            string directoradd, string companydate, int noofshare, double shareamount, string inclof)
        {
            string str = string.Format(@"<span style='display:block;page-break-after:always'></span>
    <div class='page'>
        <div style='padding-top:200px'>
            <p style='text-align:center;font-size:24px;color:#0f243e'>
                <strong style='font-family:Times New Roman'> {0}</strong><br />
                <strong style='font-family:Times New Roman'> A.C.N {1} </strong><br />
                <span style='font-family:Arial;font-size:12px'> (Registered under the Corporations Act 2001)</span>
            </p>

            <p style='text-align:center;color:#0f243e'>
                <span style='font-family:Times New Roman;font-size:24px;font-weight:bold;'>Registered Office:</span><br />
                <span style='font-family:Times New Roman;font-size:16px;display:block;'>{2}</span><br />
            </p>
            <p style='text-align:center;color:#0f243e'>
                <span style='font-family:Cambria;font-size:15px;margin-top:30px;display:block'>This is to certify that</span>
            </p>

            <p style='text-align:center;font-weight:bold'>{3}</p>
            <table class='tablerow' align='center'>
                <tr>
                    <td align='center' width='50%' style='height:0px;' class='bordertop'></td>
                </tr>
            </table>

            <p style='text-align:center;font-weight:bold'>{4}</p>
            <table class='tablerow' align='center' style='width:500px'>
                <tr>
                    <td align='center' width='50%' class='bordertop'></td>
                </tr>
            </table>
            <p style='text-align:center;color:#0f243e;font-family:sans-serif;font-size:15px;margin-top:30px;display:block'>
                is the registered holder of
            </p>
            <p style='text-align:center;color:#0f243e;'>
                <span style='font-family:sans-serif;font-size:16px;display:block;font-weight:bold;'>
                    {5}<br />
                    ${6}.00 ORDINARY SHARES
                </span><br />
            </p>
            <p style='text-align:center;color:#0f243e;font-family:sans-serif;font-size:15px;display:block;line-height:1.3em;'>
                in the above-mentioned company<br />
                being the share(s) having the distinctive serial numbers<br />
                <strong>{7} </strong>
            </p>
            <p style='text-align:center;color:#0f243e;font-family:sans-serif;font-size:15px;display:block;'>

                <span style='font-size:16px'>Dated: {8}</span><br />
                <span style='font-size:16px'>Executed on behalf of the company            </span>
            </p>
            <table class='tablerow' align='center' style='width:500px'>
                <tr>
                    <td align='center' width='50%' class='borderbottom'></td>
                </tr>
            </table>
            <p style='text-align:center;color:#0f243e;font-family:sans-serif;font-size:15px;display:block;margin-top:0;'>
                (Director(s)/ Secretary(s) to sign above as appropriate
            </p>
            <p style='padding:20px 50px 0;text-align:left;color:#0f243e;font-family:Calibri;line-height:1.1em;display:block;letter-spacing:initial'>
                <small>
                    If the company has only one director, then this share certificate may be signed merely by that sole director – see sections 124(1),
                    127(1), 127(4),198E(1) and 204A(1) of the Corporations Act 2001.
                    If the company has two directors or a director and a secretary, then any two of them may sign this share certificate - sections
                    127(1)(a) of the Corporations Act 2001.
                </small>
            </p>

        </div>
    </div>",
    companyname.ToUpper(),
    acn,
    address,
    directorname,
    directoradd,
    noofshare,
    shareamount,
    inclof,
    companydate
    );

            return str;
        }

        private static string getshareswithF(string companyname, string acn, string address, string directorname,
           string directoradd, string companydate, int noofshare, double shareamount, string inclof, string shareType)
        {
            string str = string.Format(@"<span style='display:block;page-break-after:always'></span>
    <div class='page'>
        <div style='padding-top:200px'>
            <p style='text-align:center;font-size:24px;color:#0f243e'>
                <strong style='font-family:Times New Roman'> {0}</strong><br />
                <strong style='font-family:Times New Roman'> A.C.N {1} </strong><br />
                <span style='font-family:Arial;font-size:12px'> (Registered under the Corporations Act 2001)</span>
            </p>

            <p style='text-align:center;color:#0f243e'>
                <span style='font-family:Times New Roman;font-size:24px;font-weight:bold;'>Registered Office:</span><br />
                <span style='font-family:Times New Roman;font-size:16px;display:block;'>{2}</span><br />
            </p>
            <p style='text-align:center;color:#0f243e'>
                <span style='font-family:Cambria;font-size:15px;margin-top:30px;display:block'>This is to certify that</span>
            </p>

            <p style='text-align:center;font-weight:bold'>{3}</p>
            <table class='tablerow' align='center'>
                <tr>
                    <td align='center' width='50%' style='height:0px;' class='bordertop'></td>
                </tr>
            </table>

            <p style='text-align:center;font-weight:bold'>{4}</p>
            <table class='tablerow' align='center' style='width:500px'>
                <tr>
                    <td align='center' width='50%' class='bordertop'></td>
                </tr>
            </table>
            <p style='text-align:center;color:#0f243e;font-family:sans-serif;font-size:15px;margin-top:30px;display:block'>
                is the registered holder of
            </p>
            <p style='text-align:center;color:#0f243e;'>
                <span style='font-family:sans-serif;font-size:16px;display:block;font-weight:bold;'>
                    {5}<br />
                    ${6} {9} SHARES
                </span><br />
            </p>
            <p style='text-align:center;color:#0f243e;font-family:sans-serif;font-size:15px;display:block;line-height:1.3em;'>
                in the above-mentioned company<br />
                being the share(s) having the distinctive serial numbers<br />
                <strong>{8} </strong>
            </p>
            <p style='text-align:center;color:#0f243e;font-family:sans-serif;font-size:15px;display:block;'>

                <span style='font-size:16px'>Dated: {7}</span><br />
                <span style='font-size:16px'>Executed on behalf of the company            </span>
            </p>
            <table class='tablerow' align='center' style='width:500px'>
                <tr>
                    <td align='center' width='50%' class='borderbottom'></td>
                </tr>
            </table>
            <p style='text-align:center;color:#0f243e;font-family:sans-serif;font-size:15px;display:block;margin-top:0;'>
                (Director(s)/ Secretary(s) to sign above as appropriate
            </p>
            <p style='padding:20px 50px 0;text-align:left;color:#0f243e;font-family:Calibri;line-height:1.1em;display:block;letter-spacing:initial'>
                <small>
                    If the company has only one director, then this share certificate may be signed merely by that sole director – see sections 124(1),
                    127(1), 127(4),198E(1) and 204A(1) of the Corporations Act 2001.
                    If the company has two directors or a director and a secretary, then any two of them may sign this share certificate - sections
                    127(1)(a) of the Corporations Act 2001.
                </small>
            </p>

        </div>
    </div>",
    companyname.ToUpper(),
    acn,
    address,
    directorname,
    directoradd,
    noofshare,
    shareamount,
    inclof,
    companydate,
    getFullShareType(shareType)
    );

            return str;
        }

        private static string getShareType(string sharename)
        {
            if (sharename.Trim() == "ORDINARY SHARE")
            {
                return "ORD";
            }
            else if (sharename.Trim() == "Redeemable Preference[REDP]")
            {
                return "REDP";
            }
            else if (sharename.Trim() == "Founders")
            {
                return "FOU";
            }
            else if (sharename.Trim() == "Special")
            {
                return "SPE";
            }
            else if (sharename.Trim() == "Cumulative Preference")
            {
                return "CUMP";
            }
            else if (sharename.Trim() == "Preference")
            {
                return "PRF";
            }
            else if (sharename.Trim() == "Employees")
            {
                return "EMP";
            }
            else
            {
                return sharename;
            }
        }

        private static string getShareTypeS(string sharename)
        {
            if (sharename.Trim() == "A")
            {
                return "CLASS A";
            }
            else if (sharename.Trim() == "B")
            {
                return "CLASS B";
            }
            else if (sharename.Trim() == "C")
            {
                return "CLASS C";
            }
            else if (sharename.Trim() == "D")
            {
                return "CLASS D";
            }
            else
            {
                return sharename;
            }
        }

        private static string getFullShareType(string sharename)
        {
            if (sharename.ToUpper().Trim() == "ORD")
            {
                return "ORDINARY SHARE";
            }
            else if (sharename.ToUpper().Trim() == "REDP")
            {
                return "Redeemable Preference".ToUpper();
            }
            else if (sharename.ToUpper().Trim() == "FOU")
            {
                return "Founders".ToUpper();
            }
            else if (sharename.ToUpper().Trim() == "SPE")
            {
                return "Special".ToUpper();
            }
            else if (sharename.ToUpper().Trim() == "CUMP")
            {
                return "Cumulative Preference".ToUpper();
            }
            else if (sharename.Trim() == "PRF")
            {
                return "Preference".ToUpper();
            }
            else if (sharename.ToUpper().Trim() == "EMP")
            {
                return "Employees".ToUpper();
            }
            else if (sharename.ToUpper().Trim() == "A")
            {
                return "CLASS A".ToUpper();
            }
            else if (sharename.ToUpper().Trim() == "B")
            {
                return "CLASS B".ToUpper();
            }
            else if (sharename.ToUpper().Trim() == "C")
            {
                return "CLASS C".ToUpper();
            }
            else if (sharename.ToUpper().Trim() == "D")
            {
                return "CLASS D".ToUpper();
            }
            else
            {
                return sharename;
            }
        }

        private static string getconsent(string companyname, string acn, string directorname,
            string dirDob, string directoradd, string dirPlaceofbirth, string companydate)
        {
            string imgpath = Helper.GetBaseURL() + "/Content/deedhtml/dashed.png";
            string str = string.Format(@"<span style='display:block;page-break-after:always'></span>
    <div class='page'>
        <div style='padding:100px 50px 0'>
            <p style='text-align:center;font-size:20px;'>
                <strong style='font-family:sans-serif'>{0}</strong><br />
                <strong style='font-family:sans-serif'> A.C.N {1} </strong><br />
            </p>
            <br />
            <p style='text-align:center;font-size:20px;line-height:16px;'>
                <strong style='font-family:sans-serif;display:block'> SECRETARY'S CONSENT TO ACT</strong><br />
                <span style='font-family:Arial;font-size:12px'> Pursuant to sections 201D and 204C of the Corporations act 2001</span>
            </p>
            <br />
            <p style='font-family:sans-serif;font-size:15px'>
                I hereby consent to act as Secretary of the Company and provide the following information:
            </p>
            <div style='padding:50px 0'>
                <table align='center' cellspacing='20'>
                    <tr>
                        <td width='200'>Full Name : </td>
                        <td><strong>{2}</strong></td>
                    </tr>
                    <tr>
                        <td>Registered Address:</td>
                        <td>{3}</td>
                    </tr>
                    <tr>
                        <td>Date of Birth:</td>
                        <td>{4}</td>
                    </tr>
                    <tr>
                        <td>Place of Birth:</td>
                        <td>{5}</td>
                    </tr>
                </table>
            </div>

            <br /><br />
            <img src='{6}' width='300' />
            <br />
            <p style='display:inline-block;margin-top:0;font-family:sans-serif'>
                {7}
            </p>
            <br /><br /><br /><br /><br /><br />
            <p style='font-family:sans-serif'>
                {8}
            </p>

        </div>
    </div>",
    companyname.ToUpper(),
    acn,
    directorname,
    directoradd,
    dirDob,
    dirPlaceofbirth,
    imgpath,
    directorname,
    companydate
    );

            return str;
        }

        private static string getconsent_d(string companyname, string acn, string directorname,
          string dirDob, string directoradd, string dirPlaceofbirth, string companydate)
        {
            string imgpath = Helper.GetBaseURL() + "/Content/deedhtml/dashed.png";
            string str = string.Format(@"<span style='display:block;page-break-after:always'></span>
    <div class='page'>
        <div style='padding:100px 50px 0'>
            <p style='text-align:center;font-size:20px;'>
                <strong style='font-family:sans-serif'>{0}</strong><br />
                <strong style='font-family:sans-serif'> A.C.N {1} </strong><br />
            </p>
            <br />
            <p style='text-align:center;font-size:20px;line-height:16px;'>
                <strong style='font-family:sans-serif;display:block'> INDIVIDUAL'S CONSENT TO ACT</strong><br />
                <span style='font-family:Arial;font-size:12px'> Pursuant to sections 201D and 204C of the Corporations act 2001</span>
            </p>
            <br />
            <p style='font-family:sans-serif;font-size:15px'>
                I hereby consent to act as Director of the Company and provide the following information:
            </p>
            <div style='padding:50px 0'>
                <table align='center' cellspacing='20'>
                    <tr>
                        <td width='200'>Full Name : </td>
                        <td><strong>{2}</strong></td>
                    </tr>
                    <tr>
                        <td>Registered Address:</td>
                        <td>{3}</td>
                    </tr>
                    <tr>
                        <td>Date of Birth:</td>
                        <td>{4}</td>
                    </tr>
                    <tr>
                        <td>Place of Birth:</td>
                        <td>{5}</td>
                    </tr>
                </table>
            </div>

            <br /><br />
            <img src='{6}' width='300' />
            <br />
            <p style='display:inline-block;margin-top:0;font-family:sans-serif'>
                {7}
            </p>
            <br /><br /><br /><br /><br /><br />
            <p style='font-family:sans-serif'>
                {8}
            </p>

        </div>
    </div>",
    companyname.ToUpper(),
    acn,
    directorname,
    directoradd,
    dirDob,
    dirPlaceofbirth,
    imgpath,
    directorname,
    companydate
    );

            return str;
        }

        private static string getconsent_c(string companyname, string acn, string directorname,
         string abn, string directoradd, string dirPlaceofbirth, string companydate)
        {
            string imgpath = Helper.GetBaseURL() + "/Content/deedhtml/dashed.png";
            string str = string.Format(@"<span style='display:block;page-break-after:always'></span>
    <div class='page'>
        <div style='padding:100px 50px 0'>
            <p style='text-align:center;font-size:20px;'>
                <strong style='font-family:sans-serif'>{0}</strong><br />
                <strong style='font-family:sans-serif'> A.C.N {1} </strong><br />
            </p>
            <br />
            <p style='text-align:center;font-size:20px;line-height:16px;'>
                <strong style='font-family:sans-serif;display:block'> COMPANY'S CONSENT TO ACT</strong><br />
                <span style='font-family:Arial;font-size:12px'> Pursuant to sections 201D and 204C of the Corporations act 2001</span>
            </p>
            <br />
            <p style='font-family:sans-serif;font-size:15px'>
                I hereby consent to act as Secretary of the Company and provide the following information:
            </p>
            <div style='padding:50px 0'>
                <table align='center' cellspacing='20'>
                    <tr>
                        <td width='200'>Company Name : </td>
                        <td><strong>{2}</strong></td>
                    </tr>
                    <tr>
                        <td>ABN :</td>
                        <td>{3}</td>
                    </tr>
                    <tr>
                        <td>Residential Address :</td>
                        <td>{4}</td>
                    </tr>

                </table>
            </div>

            <br /><br />
            <img src='{5}' width='300' />
            <br />
            <p style='display:inline-block;margin-top:0;font-family:sans-serif'>
                {6}
            </p>
            <br /><br /><br /><br /><br /><br />
            <p style='font-family:sans-serif'>
                {7}
            </p>

        </div>
    </div>",
    companyname.ToUpper(),
    acn,
    directorname,
    abn,
    directoradd,
    imgpath,
    directorname,
    companydate
    );

            return str;
        }

        private static string getshareapplication(string companyname, string acn, string address, string directorname,
            string directoradd, string companydate, string noofshare, string shareamount, string sharetype, string isbeneficial_s)
        {
            string imgpath = Helper.GetBaseURL() + "/Content/deedhtml/dashed.png";
            string str = string.Format(@"
    <span style='display:block;page-break-after:always'></span>
    <div class='page'>
        <div style='padding:100px 50px 0'>
            <p style='text-align:center;font-size:20px;'>
                <strong style='font-family:sans-serif'> {0}</strong><br />
                <strong style='font-family:sans-serif'> A.C.N {1} </strong><br />
            </p>
            <br />
            <p style='text-align:center;font-size:20px;line-height:16px;'>
                <strong style='font-family:sans-serif;display:block'> APPLICATION FOR SHARES</strong><br />
            </p>
            <br />
            <p style='font-family:sans-serif;font-size:15px'>
                I hereby apply for an allotment of shares in {2}, agree to be bound by the
                Constitution of the company and provide the following information:

            </p>
            <div style='padding:50px 0'>
                <table align='center' cellspacing='20'>
                    <tr>
                        <td width='200'>Full Name : </td>
                        <td><strong>{3}</strong></td>
                    </tr>
                    <tr>
                        <td>Registered Address:</td>
                        <td>{4}</td>
                    </tr>
                </table>
            </div>

            <div style='padding:50px 0'>
                <p><strong>Share Allotment Details</strong></p>
                <table class='tblborder'>
                    <tr class='headfill'>
                        <th>Type</th>
                        <th>Number Of Share</th>
                        <th>Paid Per Share</th>
                        <th>Unpaid Per Share</th>
                        <th>Beneficially Held</th>
                    </tr>
                    <tr>
                        <td>{5}</td>
                        <td>{6}</td>
                        <td>{7}</td>
                        <td>{8}</td>
                        <td>{9}</td>
                    </tr>
                </table>
            </div>

            <br /><br />
            <img src='{10}' width='300' />
            <br />
            <p style='display:inline-block;margin-top:0;font-family:sans-serif'>
                {11}
            </p>
            <br /><br /><br /><br />
            <p style='font-family:sans-serif'>
                {12}
            </p>
        </div>
    </div>
            ",
            companyname.ToUpper(),
            acn,
            companyname.ToUpper(),
            directorname,
            directoradd,
            getShareTypeS(sharetype),
            noofshare,
            shareamount,
            "$0",
            isbeneficial_s,
            imgpath,
            directorname,
            companydate
            );
            return str;
        }

        #endregion Certification
    }
}