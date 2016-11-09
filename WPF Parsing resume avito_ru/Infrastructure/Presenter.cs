using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Parsing_resume_avito_ru.Concrete;
using WPF_Parsing_resume_avito_ru.Entitties;
using WPF_Parsing_resume_avito_ru.Infrastructure.Services;
using WPF_Parsing_resume_avito_ru.Interfaces;

namespace WPF_Parsing_resume_avito_ru.Infrastructure
{
    public delegate void EventDelegate(WindowFoUrl windowURL);

    class Presenter
    {
        MainWindow mainWindow = null;
        string url = null;
        HelperService _helperService;
        SiteCrawler _crawler;
        private IRepository _repository;

        public Presenter(MainWindow mainWindow)
        {
            _helperService = new HelperService();
            _crawler = new SiteCrawler();
            _repository = Repository.Inctance;
            this.mainWindow = mainWindow;
            this.mainWindow.search_CV += new EventHandler(mainWindow_search_CV);
            this.mainWindow.page_CV += new EventHandler(mainWindow_page_CV);            
        }
        
        private void fill()
        {
            this.mainWindow.listView.ItemsSource = null;            
            this.mainWindow.listView.ItemsSource = _helperService.ViewDate(_repository.GetAllCV());
                
        }

        private async void mainWindow_search_CV(object sender, System.EventArgs e)
        {
            string tempUrl = null;

            if (url == null)
            {                
                List<URL> urlList = new List<URL>(_repository.GetURL());
                int item = urlList.Count-1;
                tempUrl = urlList[item].StrURL;
            }
            else
            {
                tempUrl = url;
            }
            
            List<string> siteList = await _crawler.CrawlAsync(tempUrl);

            foreach (var url in siteList)
            {
                List<CV> parseDateList = await _helperService.GetParseDateList(url);
                                
                for (int i = 0; i < parseDateList.Count; i++)
                {  
                    _repository.AddCV(parseDateList[i]);
                }             
            }

            fill();
        }
        
        private async void mainWindow_page_CV(object sender, System.EventArgs e)
        {
            WindowFoUrl windowURL = new WindowFoUrl();
            windowURL.Url += new EventDelegate(set_URL);
            windowURL.Show();           
        }

        private async void set_URL(WindowFoUrl windowURL)
        {      
            string tempUrl = windowURL.textBox.Text;

            bool isUrlExist = await _helperService.IsUrlExistAsync(url);
            if (isUrlExist)
            {
                url = tempUrl;
            }
                     
            windowURL.Close();
        }
    }
}
