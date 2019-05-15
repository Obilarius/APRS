using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    public class PermissionItem : DirectoryACE
    {
        public int PathID { get; set; }
        public string UncPath { get; set; }
        public string OwnerSid { get; set; }
        public bool HasChildren { get; set; }
        public long Size { get; set; }
        public string LocalPath { get; set; }
        public bool IsHidden { get; set; }
        public int ParentId { get; set; }
        public int ScanDeepth { get; set; }

        public PermissionItem(string sid, FileSystemRights rights, bool type, string fileSystemRight, bool isInherited, int inheritanceFlags, int propagationFlags) : 
            base(sid, rights, type, fileSystemRight, isInherited, inheritanceFlags, propagationFlags)
        {

        }
    }
}
