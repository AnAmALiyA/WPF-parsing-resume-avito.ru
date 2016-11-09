using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_Parsing_resume_avito_ru.Infrastructure;

namespace WPF_Parsing_resume_avito_ru.Interfaces
{
    /// <summary>
    /// Логика взаимодействия для WindowFoUrl.xaml
    /// </summary>
    public partial class WindowFoUrl : Window
    {
        public WindowFoUrl()
        {
            InitializeComponent();
        }
        
        public event EventDelegate Url = null;

        private void button_url_Click(object sender, RoutedEventArgs e)
        {
            Url.Invoke(this);
        }
    }
}
