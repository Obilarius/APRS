
using System.Collections.ObjectModel;

namespace ARPS
{
    public class DirectoryItem
    {
        /// <summary>
        /// Die ID (aus der MSSQL Datenbank) des Items
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Der absolute Pfad zum diesem Item
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Das ist der Name des Items
        /// </summary>
        public string Name { get { return DirectoryStructure.GetFolderName(this.FullPath); } }

        /// <summary>
        /// Der Typ des Items
        /// </summary>
        public DirectoryItemType Type { get; set; }

        /// <summary>
        /// Dei SID des Besitzers
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Die ID (aus der MSSQL Datenbank) des Elternelements
        /// </summary>
        public int ParentID { get; set; }


    }
}
