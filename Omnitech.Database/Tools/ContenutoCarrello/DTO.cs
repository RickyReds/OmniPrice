using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Database.Tools.ContenutoCarrello
{
    public class DTO
    {
        public class Item
        {
            public string BarCode { get; set; }
            public string Cliente { get; set; }
            public bool? RIL { get; set; }
            public string Stampo { get; set; }
            public string Dim { get; set; }

            public string Ral { get; set; }
            public string Descrizione { get; set; }
            public DateTime? DataConsegnaPianificata { get; set; }
            public string Carrello { get; set; }                    //se c'è un titolo è inutile stamparlo
            public float? QtyVern_kg { get; set; }
        }
    }
}
