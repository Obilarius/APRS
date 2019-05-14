using System;
using System.DirectoryServices.AccountManagement;

namespace ARPS
{
    public class ADElement
    {
        /// <summary>
        /// Konstruktor für DummyElement
        /// </summary>
        /// <param name="name"></param>
        public ADElement(string name)
        {
            Name = name;
        }


        /// <summary>
        /// Konstruktor für User
        /// </summary>
        /// <param name="sID"></param>
        /// <param name="name"></param>
        /// <param name="samAccountName"></param>
        /// <param name="distinguishedName"></param>
        /// <param name="pricipalName"></param>
        /// <param name="enabled"></param>
        public ADElement(string sid, string name, string samAccountName, string distinguishedName, string pricipalName, bool enabled, ADElementType type = ADElementType.User)
        {
            SID = sid;
            Name = name;
            SamAccountName = samAccountName;
            DistinguishedName = distinguishedName;
            PricipalName = pricipalName;
            Enabled = enabled;
            Type = type;
        }

        /// <summary>
        /// Konstruktor für Gruppen
        /// </summary>
        /// <param name="sID"></param>
        /// <param name="name"></param>
        /// <param name="samAccountName"></param>
        /// <param name="distinguishedName"></param>
        /// <param name="description"></param>
        /// <param name="isSecurityGroup"></param>
        /// <param name="groupScope"></param>
        public ADElement(string sid, string name, string samAccountName, string distinguishedName, string description, bool isSecurityGroup, GroupScope groupScope)
        {
            SID = sid;
            Name = name;
            SamAccountName = samAccountName;
            DistinguishedName = distinguishedName;
            Description = description;
            IsSecurityGroup = isSecurityGroup;
            GroupScope = groupScope;
            Type = ADElementType.Group;
        }

        /// <summary>
        /// Konstruktor für Computer
        /// </summary>
        /// <param name="sID"></param>
        /// <param name="name"></param>
        /// <param name="samAccountName"></param>
        /// <param name="distinguishedName"></param>
        /// <param name="description"></param>
        /// <param name="enabled"></param>
        /// <param name="lastLogon"></param>
        /// <param name="lastPasswordSet"></param>
        public ADElement(string sID, string name, string samAccountName, string distinguishedName, 
            string description, bool enabled, DateTime lastLogon, DateTime lastPasswordSet)
        {
            Type = ADElementType.Computer;
            SID = sID;
            Name = name;
            SamAccountName = samAccountName;
            DistinguishedName = distinguishedName;
            Description = description;
            Enabled = enabled;
            LastLogon = lastLogon;
            LastPasswordSet = lastPasswordSet;
        }

        public ADElementType Type { get; set; }
        public string SID { get; set; }
        public string Name { get; set; }
        public string SamAccountName { get; set; }
        public string DistinguishedName { get; set; }


        public string Description { get; set; }
        public bool Enabled { get; set; }


        #region Computer spezifische Propertys
        public DateTime LastLogon { get; set; }
        public DateTime LastPasswordSet { get; set; }
        #endregion

        #region Gruppen spezifische Propertys
        public bool IsSecurityGroup { get; set; }
        public GroupScope GroupScope { get; set; }
        #endregion

        #region User spezifische Propertys
        public string PricipalName { get; set; }
        #endregion


        public override string ToString()
        {
            return (String.IsNullOrEmpty(PricipalName)) ? Name : Name + " (" + PricipalName + ")";
        }
    }


}