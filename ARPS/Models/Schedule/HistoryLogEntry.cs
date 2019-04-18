using System;
using System.Windows.Media;

namespace ARPS
{
    public class HistoryLogEntry
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string UserSid { get; set; }
        public string Groupname { get; set; }
        public string GroupSid { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
        public string Creator { get; set; }
        public string Comment { get; set; }
    }
}
