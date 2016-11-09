using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Parsing_resume_avito_ru.Entitties;

namespace WPF_Parsing_resume_avito_ru.Interfaces
{
    public interface IRepository
    {       
        IEnumerable<CV> GetAllCV();
        void AddCV(CV resume);
        CV GetCV(int id);
        void DeleteCV(int id);

        void AddURL(string strUrl);
        IEnumerable<URL> GetURL();
    }
}
