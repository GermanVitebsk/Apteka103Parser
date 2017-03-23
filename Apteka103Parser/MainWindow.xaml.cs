using System.Windows;
using Apteka103Parser.ViewModels;

namespace Apteka103Parser
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closed += MainWindow_Closed;

            DataContext = new MedecineViewModel();
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            var vm = DataContext as MedecineViewModel;

            vm?.Dispose();
        }
    }
}