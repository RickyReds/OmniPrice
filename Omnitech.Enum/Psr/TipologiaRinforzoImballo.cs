using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Omnitech.Enum.Psr
{
    public enum TipologiaRinforzoImballo
    {
        Undefined = -1,
        Inapplicable = 0,

        [Description("1|2|6|7|8|14|16|34")]
        Nessuno = 1,
        [Description("7")]
        Osb_1top = 2,               // 'top
        [Description("7")]
        Osb_5lati = 3,              //'top + 4 laterali
        [Description("1|2|6")]
        Osb_6Lati = 4,              // 'top + 4 laterali + bottom
        [Description("1|2|6|8|14|16|34")]
        Gabbia = 5
    }
}
