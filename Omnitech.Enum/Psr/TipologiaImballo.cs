using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.Psr
{
    public enum TipologiaImballo
    {
        undefined = -1,
        inapplicable = 0,

        nessuna = 1,
        
        industriale = 2,
        
        cartone = 10,
        
        [Description("Top Vasca Integrata")]
        tviTipoA = 20,
        
        [Description("Piatti Doccia con tasca")]
        pdcTipoA = 30,
        pdcTipoB = 31,                                  // Piatti Doccia con legni in testa
        pdcTipoB_Rib = 32,                              // Piatti Doccia con legni in testa
        pdcTipoC = 33,                                  // Piatti Doccia Con angolari in cartone, polistiroli e cartone sui 2 lati lunghi
        
        vdbTipoA = 40,                                  // Vasche da Bagno cartone
        vdbTipoB = 41,                                  // Vasche da Bagno
        vdbTipoC = 42,                                  // Vasche da Bagno OSB
        
        vdbTipoB_Spec = 43                              // Vasche da Bagno IdealStandard
    }
}
