using Atlas.UI.Extensions;
using Ghost.Model;
using System.Windows;
using System.Windows.Controls;

namespace Ghost.TemplateSelectors
{
    public class ChatItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemsControl = container.FindParentOfType<ItemsControl>();

            return item switch
            {
                Message _ => itemsControl.FindResource("MessageItemTemplate") as DataTemplate,
                Notification _ => itemsControl.FindResource("NotificationItemTemplate") as DataTemplate,
                _ => null
            };
        }
    }
}
