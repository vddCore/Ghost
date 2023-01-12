using System;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Ghost.Xenus;

namespace Ghost.ViewModel.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Client ObcyClient { get; set; }
        public ObservableCollection<string> LogItems { get; set; }

        public MainWindowViewModel()
        {
            LogItems = new ObservableCollection<string>();
            ObcyClient = new Client();

            try
            {
                ObcyClient.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}