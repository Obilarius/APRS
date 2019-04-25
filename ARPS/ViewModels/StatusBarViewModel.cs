using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS.ViewModels
{
    public class StatusBarViewModel : BaseViewModel
    {
        public string StatusBarText { get; set; } = "Keine Meldungen Vorhanden";

        public StatusBarViewModel()
        {

        }
    }
}
