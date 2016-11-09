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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Parsing_resume_avito_ru.Infrastructure;

namespace WPF_Parsing_resume_avito_ru
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new Presenter(this);
        }

        public event EventHandler page_CV = null;
        public event EventHandler search_CV = null;

        private void button_search_CV_Click(object sender, RoutedEventArgs e)
        {
            search_CV.Invoke(sender, e);
        }

        private void button_page_CV_Click(object sender, RoutedEventArgs e)
        {
            page_CV.Invoke(sender, e);
        }
    }
}
