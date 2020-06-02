using Atlas.UI.Extensions;
using System;
using System.Windows;

namespace Ghost.View.Controls
{
    public partial class ChatMessageView
    {
        public static DependencyProperty BodyProperty 
            = Dependency.Register<string>(nameof(Body));

        public static DependencyProperty SenderProperty
            = Dependency.Register<string>(nameof(Sender));

        public static DependencyProperty CreatedAtProperty
            = Dependency.Register<DateTime>(nameof(CreatedAt));

        public string Body
        {
            get => (string)GetValue(BodyProperty);
            set => SetValue(BodyProperty, value);
        }

        public string Sender
        {
            get => (string)GetValue(SenderProperty);
            set => SetValue(SenderProperty, value);
        }

        public DateTime CreatedAt
        {
            get => (DateTime)GetValue(CreatedAtProperty);
            set => SetValue(CreatedAtProperty, value);
        }

        public ChatMessageView()
            => InitializeComponent();
    }
}
