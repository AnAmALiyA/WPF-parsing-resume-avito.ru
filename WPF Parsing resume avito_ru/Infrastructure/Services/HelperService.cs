using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPF_Parsing_resume_avito_ru.Entitties;
using WPF_Parsing_resume_avito_ru.Model;

namespace WPF_Parsing_resume_avito_ru.Infrastructure.Services
{
    public class HelperService
    {
        public async Task<bool> IsUrlExistAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            bool isCorrectly = Uri.IsWellFormedUriString(url, UriKind.Absolute);
            if (!isCorrectly)
            {
                return false;
            }

            bool isExist = false;
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(url), Method = HttpMethod.Head })
                {
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        isExist = response.IsSuccessStatusCode;
                    }
                }
                return isExist;
            }
            catch
            {
                return false;
            }
        }

        public string GetHostFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL can not be null");
            }
            try
            {
                Uri uri = new Uri(url);

                return uri.Scheme + "://" + uri.Host;
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<List<CV>> GetParseDateList(string url)
        {
            List<CV> cvList = new List<CV>();
            string resultHttpClient = "";
            Regex regex = new Regex("<div[^~]*?class=\"description\"[^~]*?>[^~]*?<h3[^~]*?>[^~]*?<a[^>]*?>(?<Speciality>.*?)</a>[^~]*?</h3>[^~]*?<div[^~]*?>(?<Pay>.*?)</div>[^~]*?<div[^~]*?>[^~]*?<p[^~]*?>(?<AllDate>.*?)</p(?<AdditionalDate>.*?)<div", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            MatchCollection matches;
 #region
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {

                    //if http status not 200 - stop executing method
                    if (!response.IsSuccessStatusCode)
                    {
                        return cvList;
                    }
                    resultHttpClient = await content.ReadAsStringAsync();
                    matches = regex.Matches(resultHttpClient);
                }
            }
            catch
            {
                return cvList;
            }
            
            foreach (Match match in matches)
            {
                string tempStr = string.Empty;
                int number;
                CV temp = new CV();

                temp.Speciality = match.Groups["Speciality"].Value.Replace("\n","");

                string tempStrPay = match.Groups["Pay"].Value.Replace("\n", "");
                  
                if (tempStrPay.Contains("з/п")) 
                {
                    temp.Pay = -1;
                }
                else
                {
                    tempStr = tempStrPay.Replace(" ", "").Replace("руб.", ""); 

                    if (Int32.TryParse(tempStr, out number))
                    {
                        temp.Pay = Int32.Parse(tempStr);
                    }
                    else
                    {
                        temp.Pay = -1;
                    }

                    tempStr = string.Empty;
                    number = -1;
                }

                string allDate = match.Groups["AllDate"].Value;
                string[] tempText = match.Groups["AllDate"].Value.Split(new char[] { ',' });
                
                if (allDate.IndexOf("Мужчина") != -1)
                {
                    temp.Sex = "Мужчина";
                }
                else if(allDate.IndexOf("Женщина") != -1)
                {
                    temp.Sex = "Женщина";
                }
                else
                {
                    temp.Sex = "";
                }

                int strPosition = -1;

                if (allDate.IndexOf("года") != -1)
                {
                    strPosition = allDate.IndexOf("года");
                    temp.AgeText = "года";
                    tempStr = allDate.Substring(strPosition - 3, 2);                   
                    temp.Age = Int32.Parse(tempStr.Replace(" ", ""));
                }
                else if (allDate.IndexOf("год") != -1)
                {
                    strPosition = allDate.IndexOf("год");
                    temp.AgeText = "год";
                    tempStr = allDate.Substring(strPosition - 2, 1);
                }
                else if (allDate.IndexOf("лет") != -1)
                {
                    strPosition = allDate.IndexOf("лет");
                    temp.AgeText = "лет";
                    tempStr = allDate.Substring(strPosition - 3, 2);
                    temp.Age = Int32.Parse(tempStr.Replace(" ", ""));
                }
                else
                {
                    temp.Age = -1;
                    temp.AgeText = "";
                }

                if (allDate.IndexOf("стаж") != -1)
                {
                    for (int i = 0; i < tempText.Length; i++)
                    {
                        if (tempText[i].IndexOf("стаж") != -1)
                        {
                            string[] subTempText = tempText[i].Split(new char[] { ' ' });
                            for (int j = 0; j < subTempText.Length; j++)
                            {
                                if (subTempText[j].IndexOf("стаж") != -1)
                                {
                                    temp.Experience = Int32.Parse(subTempText[j+1]);
                                    temp.ExperienceText = subTempText[subTempText.Length - 1];
                                    break;
                                }                                
                            }
                            
                        }
                    }
                }
                else
                {
                    temp.Experience = -1;
                    temp.ExperienceText = string.Empty;
                }
                
                if (allDate.IndexOf("образование") != -1)
                {
                    for (int i = 0; i < tempText.Length; i++)
                    {
                        if (tempText[i].IndexOf("образование")!=-1)
                        {
                            temp.Education = tempText[i];
                            break;
                        }
                    }                    
                }
                else
                {
                    temp.Education = "";
                }
                
                temp.Location = tempText[tempText.Length-1];

                #endregion
                if (match.Groups["AdditionalDate"].Value.Length>10)
                {
                    regex = new Regex("<span[^~]*?>(?<DateWork>.*?)</span>[^~]*?<span[^~]*?>(?<SpecialityWork>.*?)</span>[^~]*?<span[^~]*?>(?<LocalWork>.*?)</span>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    tempStr = match.Groups["AdditionalDate"].Value.Replace("\n", "").Replace("&mdash;","-");
                    Match matchAdditional = regex.Match(tempStr);
                    temp.DateWork = matchAdditional.Groups["DateWork"].Value;

                    temp.SpecialityWork = matchAdditional.Groups["SpecialityWork"].Value;

                    temp.LocationWork = matchAdditional.Groups["LocalWork"].Value;
                }
                else
                {
                    temp.DateWork = "";

                    temp.SpecialityWork = "";

                    temp.LocationWork = "";
                }
                
                cvList.Add(temp);
            }            

            return cvList;
        }

        public IEnumerable<ViewCV> ViewDate(IEnumerable<CV> enumerable)
        {
            List<ViewCV> listViewCV = new List<ViewCV>();

            foreach (CV item in enumerable)
            {
                ViewCV viewCV = new ViewCV();

                viewCV.Speciality = item.Speciality;
                if (item.Pay == -1)
                {
                    viewCV.Pay = "з/п не указана";
                }
                else
                {
                    viewCV.Pay = item.Pay.ToString() + " руб.";
                }
                viewCV.Sex = item.Sex;

                if (item.Age==-1||item.Age==0)
                {
                    viewCV.Age = "";
                }
                else
                {
                    viewCV.Age = item.Age.ToString() + " " + item.AgeText;
                }

                if (item.Experience==-1)
                {
                    viewCV.Experience = "";
                }
                else
                {
                    viewCV.Experience = item.Experience.ToString() + " " + item.ExperienceText;
                }
                
                viewCV.Education = item.Education;
                viewCV.Location = item.Location;

                viewCV.DateWork = item.DateWork;
                viewCV.SpecialityWork = item.SpecialityWork;
                viewCV.LocationWork = item.LocationWork;

                listViewCV.Add(viewCV);
            }
            return (IEnumerable<ViewCV>)listViewCV;
        }        
    }
}
        
    

