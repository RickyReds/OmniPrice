using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.Psr
{
    //[Revision("2018.05.14", "range[90-99]")]
    public enum TipologiaSifone
    {
        Undefined = -1,

        Inapplicable = 0,

        Nessuno = 90,
        OMP = 91,
        Bonomini = 92,                      // Piatti doccia, ora si chiama Lyonnaise
        Silfra = 93,
        OMP_LGA = 94,                       // 2021.05.20
        SACITH = 98                         // 2021.06.01
    }
}
