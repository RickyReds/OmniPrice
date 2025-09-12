using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Database.Tools.ProductionMonitor
{
    public class DTO
    {
        public class Postazione
        {
            public int CodiceCentroLavoro { get; set; }

            public int CodiceOperatore { get; set; }
            public string Operatore { get; set; }
            
            
            public string OdP {  get; set; }                //public int OdP { get; set; }

            public int Timpiegato { get; set; }             //in secondi
            public int Tassegnato { get; set; }             //in secondi        POTREBBE NON ESSERE VALORIZZATO PER ORDINI NUOVI!
            
            public string Affidabilita { get; set; }        //TBD               POTREBBE NON ESSERE VALORIZZATO PER ORDINI NUOVI!



            public int CompletatiOperatoreToday { get; set; }       // NOTA BENE: N° di ordini completati (oggi) dall'operatore attualmente nella postazione
        }
    }
}
