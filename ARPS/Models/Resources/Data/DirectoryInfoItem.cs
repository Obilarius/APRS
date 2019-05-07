using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    class DirectoryInfoItem : DirectoryItem
    {
        public List<DirectoryACEs> ACEs {
            get
            {
                return DirectoryStructure.GetACEs(this.Id);
            }
        }


    }
}
