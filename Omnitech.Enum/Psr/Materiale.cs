using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.Psr
{
    public enum Materiale
    {
        Undefined = 0,
        
        [Description("Tecnoril")]
        Tecnoril = 1,
        
        [Description("Geacril")]
        Geacril = 2,
        
        [Description("Geacril MATT")]
        GeacrilMATT = 3,
        
        [Description("Deimos")]
        Deimos = 4,
        
        [Description("Tecnoril BASIC")]
        Tecnoril_Basic = 5,
       
        [Description("Geacril BASIC")]
        Geacril_Basic = 6,
        
        [Description("Deimos Stone")]
        DeimosStone = 7,
        
        [Description("Ecomalta")]
        Ecomalta = 8,                    // 2018.11.30

        [Description("Deimos Bianco")]
        DeimosBianco = 9,                // 2018.07.26

        [Description("Deimos Mineral")]
        DeimosMineral = 10,                   // 2024-10-01 

        [Description("Sheer Crystal")]
        SheerCrystal = 11,                   // 2024-10-01 

        [Description("SheerCrystal Matt")]
        SheerCrystalMatt = 12                   // 2024-10-01 

    }
}
