using System.Security.AccessControl;

namespace ARPS
{
    public class NTFSEntry
    {
        public NTFSEntry(DirectoryACE ace)
        {

        }

        /// <summary>
        /// Die SID des Elements
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Der Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ob diese Regel eine Vererbe Regel ist oder nicht
        /// </summary>
        public bool IsInheritance { get; set; }

        /// <summary>
        /// Ob dieser Eintrag eine Gruppe ist oder User
        /// </summary>
        public bool IsGroup { get; set; }

        public FileSystemRights Right { get; set; }


        #region PropagationFlags
        /// <summary>
        /// Das Element wir nur auf den aktuellen Ordner angewendet
        /// </summary>
        public bool PropagationNone { get; set; }

        /// <summary>
        /// Das Element wir nur auf die Untergeordneten Objecte angewendet
        /// </summary>
        public bool PropagationInheritOnly { get; set; }
        #endregion

        #region InheritanceFlags
        /// <summary>
        /// Gilt das Element auf den Objekttyp Ordner
        /// </summary>
        public bool ContainerInherit { get; set; }
        /// <summary>
        /// Gilt das Element auf den Objekttyp Dateien
        /// </summary>
        public bool ObjectInherit { get; set; }
        #endregion

        #region NTFS Rechte
        /// <summary>
        /// Vollzugriff
        /// </summary>
        public bool FullControl { get; set; }

        /// <summary>
        /// Ändern
        /// </summary>
        public bool Modify { get; set; }

        /// <summary>
        /// Lesen, Ausführen
        /// </summary>
        public bool ReadAndExexute { get; set; }

        /// <summary>
        /// Ordnerinhalt anzeigen
        /// </summary>
        public bool ListFolder { get; set; }

        /// <summary>
        /// Lesen
        /// </summary>
        public bool Read { get; set; }

        /// <summary>
        /// Schreiben
        /// </summary>
        public bool Write { get; set; }
        #endregion
    }
}