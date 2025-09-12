using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.BusinessCentral.COMMON
{
    public enum ReportType
    {
        EtichettaStandard = 100,               //Nuove coordinate
        EtichettaStandardPilette = 100,        //NOTA BENE: Solo GRUPPO IDEA (Idea, Disenia, Aqua, Blob). E' un "custom ridotto" dell'etichetta standard, usata per stampare delle etichette aggiuntive (all'ordine) per le eventuali pilette.       

        EtichettaStandard_PSR = 6,                   //vecchie coordinate
        EtichettaStandardPilette_PSR = 6,            //NOTA BENE: Solo GRUPPO IDEA (Idea, Disenia, Aqua, Blob). E' un "custom ridotto" dell'etichetta standard, usata per stampare delle etichette aggiuntive (all'ordine) per le eventuali pilette.
        
        

        EtichettaText_SoemaNonScatolati2150 = 1000,              //Per NON scatolati e L>=2150 e Cliente <> Soema --> cPrint.PrintLabelDoppiaVasca2150()
        
        EtichettaCorian150x100 = 2000,
        EtichettaCorian175x60 = 2001

    }
}
