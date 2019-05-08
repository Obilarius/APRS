namespace ARPS
{
    public class ADUser
    {
        public ADUser(string sID, string name, string samAccountName, string distinguishedName, string pricipalName, bool enabled)
        {
            SID = sID;
            Name = name;
            SamAccountName = samAccountName;
            DistinguishedName = distinguishedName;
            PricipalName = pricipalName;
            Enabled = enabled;
        }

        public string SID { get; set; }
        public string Name { get; set; }
        public string SamAccountName { get; set; }
        public string DistinguishedName { get; set; }
        public string PricipalName { get; set; }
        public bool Enabled { get; set; }

    }
}