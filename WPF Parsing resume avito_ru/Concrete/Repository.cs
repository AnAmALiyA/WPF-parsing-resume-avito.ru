using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Parsing_resume_avito_ru.Entitties;
using WPF_Parsing_resume_avito_ru.Interfaces;

namespace WPF_Parsing_resume_avito_ru.Concrete
{
    public class Repository : IRepository
    {
        private bool _disposed;
        private ApplicationContext _dbApp;       
        private static Repository instance;

        private Repository()
        {
            _dbApp = new ApplicationContext();
            _dbApp.CreateDB();            
        }

        public static Repository Inctance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Repository();
                }
                return instance;
            }
        }
       
        public IEnumerable<CV> GetAllCV()
        {
            return _dbApp.GetAllCV();
           
        }

        public void AddCV(CV resume)
        {
            _dbApp.AddCV(resume);
        }

        public void AddCVList(IEnumerable<CV> resume)
        {
            _dbApp.AddCVList(resume);
        }

        public void AddURL(string strUrl)
        {
            _dbApp.AddURL(strUrl);
        }

        public IEnumerable<URL> GetURL()
        {
            return _dbApp.GetURL();           
        }

        public CV GetCV(int id)
        {
            return _dbApp.GetCV(id);
        }

        public void DeleteCV(int id)
        {
            _dbApp.DeleteCV(id);
        }        
    }
}
