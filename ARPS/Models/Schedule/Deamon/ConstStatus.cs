using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS.Models.Schedule.Deamon
{
    public sealed class ConstStatus
    {
        public static readonly string Planned = "planned";
        public static readonly string Set = "set";
        public static readonly string Terminate = "terminate";
        public static readonly string Deleted = "deleted";
    }
}
