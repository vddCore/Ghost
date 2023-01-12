using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Ghost.Model.Messaging;
using Ghost.Xenus;

namespace Ghost.ViewModel.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
        }

        [Command]
        public void StartNewChat()
            => Messenger.Default.Send(new StartNewChatMessage());
    }
}