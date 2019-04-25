
namespace ARPS
{
    /// <summary>
    /// Jedes Infopaar über den User
    /// </summary>
    public class UserInfoEntry
    {
        public string Name { get; set; }
        public string Prop { get; set; }

        public UserInfoEntry(string Name, string Prop)
        {
            this.Name = Name;
            this.Prop = Prop;
        }
    }
}
