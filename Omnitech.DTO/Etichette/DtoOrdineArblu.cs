using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.Etichette
{
    public class DtoOrdineArblu
    {
        public string Numeratore { get; set; }
        public int Ordine { get; set; }
        public int Riga { get; set; }
        public string BarcodeCliente { get; set; }
        public string Articolo { get; set; }
        public string Descrizione { get; set; }

        public int L { get; set; }
        public int P { get; set; }
        public int H { get; set; }

        public decimal Prezzo { get; set; }
        public DateTime DataOrdine { get; set; }
        public DateTime DateConsegna { get; set; }
        public string Collo { get; set; }
        public string Finitura { get; set; }
    }
}
