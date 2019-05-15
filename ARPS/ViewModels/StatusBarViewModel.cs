using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Deployment.Application;

namespace ARPS.ViewModels
{
    public class StatusBarViewModel : BindableBase
    {
        public string StatusBarText { get; set; } = "Keine Meldungen Vorhanden";

        public string AppVersion { get; set; }

        public StatusBarViewModel()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                AppVersion = string.Format("ARPS - v{0}", ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4));
            }
            else
            {
                AppVersion = "Fehler beim auslesen der Version";
            }
        }
    }
}
