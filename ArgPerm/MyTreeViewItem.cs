using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArgPerm
{
    public class MyTreeViewItem : TreeViewItem
    {
        ImageSource iconSource;
        TextBlock textBlock;
        Image icon;

        public MyTreeViewItem()
        {
            StackPanel stack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            Header = stack;

            //Uncomment this code If you want to add an Image after the Node-HeaderText
            //textBlock = new TextBlock();
            //textBlock.VerticalAlignment = VerticalAlignment.Center;
            //stack.Children.Add(textBlock);

            //Add the Icon to the Header
            icon = new Image
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0),
                Source = iconSource
            };
            stack.Children.Add(icon);

            //Add the HeaderText After Adding the icon
            textBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            stack.Children.Add(textBlock);

            this.Padding = new Thickness(0, 3, 0, 3);
        }
    
        /// <summary>
        /// Gets/Sets the Selected Image for a TreeViewNode
        /// </summary>
        public ImageSource Icon
        {
            set
            {
                iconSource = value;
                icon.Source = iconSource;
            }
            get
            {
                return iconSource;
            }
        }

        /// <summary>
        /// Gets/Sets the HeaderText of TreeViewWithIcons
        /// </summary>
        public string HeaderText
        {
            set
            {
                textBlock.Text = value;
            }
            get
            {
                return textBlock.Text;
            }
        }

        public static readonly RoutedEvent CollapsingEvent =
            EventManager.RegisterRoutedEvent("Collapsing",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler),
            typeof(MyTreeViewItem));

        public static readonly RoutedEvent ExpandingEvent =
            EventManager.RegisterRoutedEvent("Expanding",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler),
            typeof(MyTreeViewItem));

        public event RoutedEventHandler Collapsing
        {
            add { AddHandler(CollapsingEvent, value); }
            remove { RemoveHandler(CollapsingEvent, value); }
        }

        public event RoutedEventHandler Expanding
        {
            add { AddHandler(ExpandingEvent, value); }
            remove { RemoveHandler(ExpandingEvent, value); }
        }

        protected override void OnExpanded(RoutedEventArgs e)
        {
            OnExpanding(new RoutedEventArgs(ExpandingEvent, this));
            base.OnExpanded(e);
        }

        protected override void OnCollapsed(RoutedEventArgs e)
        {
            OnCollapsing(new RoutedEventArgs(CollapsingEvent, this));
            base.OnCollapsed(e);
        }

        protected virtual void OnCollapsing(RoutedEventArgs e) { RaiseEvent(e); }

        protected virtual void OnExpanding(RoutedEventArgs e) { RaiseEvent(e); }
    }
}
