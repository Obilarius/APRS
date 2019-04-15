using ARPS.ViewModels;
using System.Windows.Controls;

namespace ARPS.Views
{
    /// <summary>
    /// Interaction logic for Resources.xaml
    /// </summary>
    public partial class Resources : UserControl
    {
        public Resources()
        {
            InitializeComponent();
            this.DataContext = new ResourcesViewModel();
        }
    }
}
