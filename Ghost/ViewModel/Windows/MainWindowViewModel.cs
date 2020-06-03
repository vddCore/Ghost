using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Ghost.Model.Messaging;
using System;

namespace Ghost.ViewModel.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        [Command]
        public void EndOrStartNewChat()
            => Messenger.Default.Send(new EndOrStartNewChatMessage { Force = false });

        [Command]
        public void ForceStartNewChat()
            => Messenger.Default.Send(new EndOrStartNewChatMessage { Force = true });

        [Command]
        public void QuitApplication()
            => Environment.Exit(0);
    }
}