using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS.ViewModels
{
    public class ResourcesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string FolderName { get; set; } = "Name";
        public string FolderPath { get; set; } = "\\\\Path"; 
        public string Owner { get; set; } = "Kein Besitzer gefunden";

        public ResourcesViewModel()
        {
            Task.Run(async () =>
            {
                int i = 0;

                while (true)
                {
                    await Task.Delay(200);
                    FolderName = (i++).ToString();
                }
            });
        }
    }
}
