
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
        /// Dei SID des Besitzers
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Die ID (aus der MSSQL Datenbank) des Elternelements
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// Ob der Ordner ein Rootordner ist (also eine Freigabe)
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        /// Ob der Ordner Unterordner (Kinder) besitzt
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// Die Ebene des Ordners oder auch Tiefe
        /// </summary>
        public int ScanDeepth { get; set; }

        /// <summary>
        /// Die Größe des Ordners in Bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Der Typ des Items
        /// </summary>
        public DirectoryItemType Type { get; set; }

        /// <summary>
        /// Das ist der Name des Items
        /// </summary>
        public string Name { get { return DirectoryStructure.GetFolderName(this.FullPath); } }

        ///// <summary>
        ///// Konstruktor mit allen Propertys
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="fullPath"></param>
        ///// <param name="owner"></param>
        ///// <param name="parentID"></param>
        ///// <param name="isRoot"></param>
        ///// <param name="hasChildren"></param>
        ///// <param name="scanDeepth"></param>
        ///// <param name="size"></param>
        ///// <param name="type"></param>
        //public DirectoryItem(int id, string fullPath, string owner, int parentID, bool isRoot, bool hasChildren, int scanDeepth, long size, DirectoryItemType type)
        //{
        //    Id = id;
        //    FullPath = fullPath;
        //    Owner = owner;
        //    ParentID = parentID;
        //    IsRoot = isRoot;
        //    HasChildren = hasChildren;
        //    ScanDeepth = scanDeepth;
        //    Size = size;
        //    Type = type;
        //}

        ///// <summary>
        ///// kurzer Konstruktor
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="fullPath"></param>
        ///// <param name="owner"></param>
        ///// <param name="type"></param>
        //public DirectoryItem(int id, string fullPath, string owner, DirectoryItemType type)
        //{
        //    Id = id;
        //    FullPath = fullPath;
        //    Owner = owner;
        //    Type = type;
        //}
    }
}
