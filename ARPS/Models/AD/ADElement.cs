using System.DirectoryServices.AccountManagement;

namespace ARPS
{
    public class ADElement
    {
        /// <summary>
        /// Konstruktor für User
        /// </summary>
        /// <param name="sID"></param>
        /// <param name="name"></param>
        /// <param name="samAccountName"></param>
        /// <param name="distinguishedName"></param>
        /// <param name="pricipalName"></param>
        /// <param name="enabled"></param>
        public ADElement(string sid, string name, string samAccountName, string distinguishedName, string pricipalName, bool enabled, UserType type = UserType.User)
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
            Type = UserType.Group;
        }

        public UserType Type { get; set; }
        public string SID { get; set; }
        public string Name { get; set; }
        public string SamAccountName { get; set; }
        public string DistinguishedName { get; set; }


        #region Gruppen spezifische Propertys
        public string Description { get; set; }
        public bool IsSecurityGroup { get; set; }
        public GroupScope GroupScope { get; set; }
        #endregion

        #region User spezifische Propertys
        public string PricipalName { get; set; }
        public bool Enabled { get; set; }
        #endregion

    }


}