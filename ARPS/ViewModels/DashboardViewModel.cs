using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        /// <summary>
        /// Die Liste enthält alle Zeilen der Kategorie "Benutzer und andere Accounts"
        /// </summary>
        public List<CountEnrty> UserAndOtherAccounts { get; set; }

        /// <summary>
        /// Die Liste enthält alle Zeilen der Kategorie "Gruppen"
        /// </summary>
        public List<CountEnrty> Groups { get; set; }

        /// <summary>
        /// Die Liste enthält alle Zeilen der Kategorie "OU / Kontakte / Mehr"
        /// </summary>
        public List<CountEnrty> OU { get; set; }



        public DashboardViewModel()
        {
            UserAndOtherAccounts = new UserAndOtherAccounts().Entrys;
            Groups = new Groups().Entrys;
            OU = new OU().Entrys;
        }


    }

    
}
