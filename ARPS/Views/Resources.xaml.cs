using ARPS.ViewModels;
using Prism.Events;
using System.Windows.Controls;

namespace ARPS.Views
{
    /// <summary>
    /// Interaction logic for Resources.xaml
    /// </summary>
    public partial class Resources : UserControl
    {
        private IEventAggregator eventAggregator;

        public Resources()
        {
            InitializeComponent();

            // Erstelle eine neue Instanz eines EventAggregator
            eventAggregator = new EventAggregator();

            this.DataContext = new ResourcesViewModel(eventAggregator);
        }

        private void ListView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            eventAggregator.GetEvent<PubSubEvent<DirectoryItemViewModel>>().Publish(treeView_Directorys.SelectedItem as DirectoryItemViewModel);
        }
    }
}
