using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyfisUtilities.Models
{
    class SysServerConnectionSettings
    {
        public String Server { get; set; }
        public String Authentication { get; set; }
        public String User { get; set; }
        public String Password { get; set; }
        public Boolean RememberPassword { get; set; }
    }
}
