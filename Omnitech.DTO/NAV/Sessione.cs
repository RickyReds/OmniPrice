using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.NAV
{
    public class Sessione
    {
        public int SessionID { get; set; }
        public string ServerInstanceName { get; set; }

        public string UserID { get; set; }
        public string Alias { get; set; }
        public string ClientComputerName { get; set; }
        public DateTime LoginDateTime { get; set; }

    }
}
