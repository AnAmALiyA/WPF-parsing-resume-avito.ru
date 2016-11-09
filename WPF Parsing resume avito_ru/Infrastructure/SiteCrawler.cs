using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPF_Parsing_resume_avito_ru.Concrete;
using WPF_Parsing_resume_avito_ru.Entitties;
using WPF_Parsing_resume_avito_ru.Infrastructure.Services;
using WPF_Parsing_resume_avito_ru.Interfaces;

namespace WPF_Parsing_resume_avito_ru.Infrastructure
{
    public class SiteCrawler
    {
        public int MaxPage { get; set; }
        private IRepository _repository;
        public SiteCrawler()
        {
            MaxPage = 5;
            _repository = Repository.Inctance;
        }

        public async Task<List<string>> CrawlAsync(string url)
        {
            //список куда я буду складывать найденные URL
            List<string> siteList = new List<string>();
            string host = new HelperService().GetHostFromUrl(url);

            siteList = await CrawlPage(siteList, host, url);

            return siteList; // возврат найденых адрессов

        }

        private async Task<List<string>> CrawlPage(List<string> siteList, string host, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL can not be null");
            }

            //ищу похожий или список больше MaxPage
            if (siteList.Find(x=>x == url) !=null || siteList.Count == MaxPage)
            {
                return siteList;
            }

            string resultHttpClient = "";
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {

                    //if http status not 200 - stop executing method Если удачно, то false и я не зайду в if
                    if (!response.IsSuccessStatusCode)
                    {
                        return siteList;
                    }
                        resultHttpClient = await content.ReadAsStringAsync();                
                }
            }
            catch
            {
                return siteList;
            }

            //получаю контен и в нём ищу url
            List<string> pagesCrawledList = new List<string>(); //list of parsed links
            
            //Regex regex = new Regex("<a[^>]*? href=\"(?<url>[^\"]+)\"[^>]*?>(?<text>.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase); //reg to find links on html page                                      
            Regex regex = new Regex("<a[^>]*? class=\"pagination-page\"[^>]*? href=\"(?<url>[^\"]+)\"[^>]*?>(?<text>.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase); //reg to find links on html page                                      
            //< a class="pagination-page" href="/moskovskaya_oblast/rezume?p=2&amp;s=101">2</a>
            //(Клар.*?)\s     между последовательностью “К”, ”л”, “а”, “р” и пробелом \s могут встретиться любые символы.*, также их может не быть вовсе ?.

            MatchCollection matches;
            matches = regex.Matches(resultHttpClient);
            
            bool isAdded = siteList.Find((x) => x == url) == null;
            // если в списке нет этого url, то я его добовляю
            if (isAdded)
            {
                siteList.Add(url);
            }

            if (!isAdded || matches.Count == 0) //if page already crawled or hasn't links
            {
                return siteList;
            }

            //get parsed links
            string link = string.Empty;
            foreach (Match match in matches)
            {
                link = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(link))
                {
                    continue;
                }

                if (!link.StartsWith("http") && !link.StartsWith("www") && !link.StartsWith(host)) //transform absolute path to absolure url
                {
                    if (!link.StartsWith("/"))
                    {
                        link = "/" + link;
                    }
                    link = host + link;
                }

                if (link.StartsWith(host)) //get only internal links
                {
                    link = CorrectUrl(link);

                    if ((siteList.Find((x) => x == link)) == null && !pagesCrawledList.Contains(link))
                    {
                        pagesCrawledList.Add(link);
                    }
                }
            }

            if (pagesCrawledList.Count == 0)//if html page hasn't internal links
            {
                return siteList;
            }

            //check MaxPageToCrawling. Тут условия длины проверки.
            int length = pagesCrawledList.Count;
            if (pagesCrawledList.Count + siteList.Count > MaxPage)
            {
                length = MaxPage - siteList.Count;
            }

            for (int i = 0; i < length; i++)
            {
                await CrawlPage(siteList, host, pagesCrawledList[i]);
            }
            return siteList;
        }

        private string CorrectUrl(string url)
        {
            string lastSymbol = string.Empty;

            if (url.Length > 1)
            {
                lastSymbol = url.Substring(url.Length - 2, 2);

                if (lastSymbol == "/#")
                {
                    return url.Remove(url.Length - 2);
                }
            }

            lastSymbol = url.Substring(url.Length - 1, 1);
            if (lastSymbol == "/" || lastSymbol == "#") //delete '/' or '#' from the end of url if it exist
            {
                return url.Remove(url.Length - 1);
            }

            return url;
        }        
    }
}
