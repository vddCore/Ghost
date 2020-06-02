using System.Windows;
using DevExpress.Mvvm;

namespace Ghost
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceContainer.Default = new ServiceContainer(this);
            ViewModelLocator.Default = new ViewModelLocator(this);
            Messenger.Default = new Messenger(false);
        }
    }
}