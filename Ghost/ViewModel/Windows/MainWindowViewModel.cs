using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Ghost.Model.Messaging;
using Ghost.Xenus;
using System;

namespace Ghost.ViewModel.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Location CurrentLocation { get; set; } = Location.EntirePoland;

        [Command]
        public void EndOrStartNewChat()
            => Messenger.Default.Send(new EndOrStartNewChatMessage { Force = false, Location = CurrentLocation });

        [Command]
        public void ForceStartNewChat()
            => Messenger.Default.Send(new EndOrStartNewChatMessage { Force = true, Location = CurrentLocation });

        [Command]
        public void QuitApplication()
            => Environment.Exit(0);
    }
}