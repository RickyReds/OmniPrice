using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.Psr
{
    public enum TipoFaP
    {
        Undefined = -1,

        Normale = 0,
        Contract = 1,
        MyTime = 2,
        PromozioneQualità = 3,
        IdeaForm = 4,                    // 2018.11.23
        Bent = 5,                        // 2019.01.17
        PiattiDoccia = 6,                // 2021.03.30
        Varie_NAV_NonUsare = 7,             // 2023.03.02 : Aggiunto (per precauzione) perchè in NAV è definito.
        Promozioni_NonUsare = 8             // 2023.03.02 : TEST Per una possibile aggiunta preannunciata da Elena
    }
}
