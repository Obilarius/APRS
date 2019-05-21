using Prism.Mvvm;
using System.Drawing;
using System.Windows.Input;

namespace ARPS
{
    public class ContextAction : BindableBase
    {
        public string Name;
        public ICommand Action;
        public Brush Icon;
    }
}
