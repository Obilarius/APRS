namespace ARPS
{
    public class AccountWithPermissions
    {
        public ADElementType UserType { get; private set; }
        public string IdentityName { get; private set; }
        public int Count { get; private set; }
        public int InheritedCount { get; private set; }
        public string SID { get; private set; }

        public AccountWithPermissions(ADElementType userType, string identityName, int count, int inheritedCount, string sid)
        {
            this.UserType = userType;
            this.IdentityName = identityName;
            this.Count = count;
            this.InheritedCount = inheritedCount;
            this.SID = sid;
        }
    }
}