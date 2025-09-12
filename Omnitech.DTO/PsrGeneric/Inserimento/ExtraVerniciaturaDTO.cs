using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.PsrGeneric.Inserimento
{
    public class ExtraVerniciaturaArticolo
    {
        public string Barcode { get; set; }
        public string Stampo { get; set; }
        public string CategoriaArticolo { get; set; }

        public DateTime? DataConsegna { get; set; }
        public DateTime? DataConsegnaDefinitiva { get; set; }       
        public DateTime? DataDisponibilitaVernice { get; set; }             //per ora 2 settimane (10 gg lavorativi prima)


        public string RalVernice { get; set; }
        public double QtaVernKg { get; set; }

        //public string StatoOrdine { get; set; }
    }

    public class ExtraVerniciaturaArticoloOLD
    {
        public string Barcode { get; set; }
        public string Stampo { get; set; }
        public string CategoriaArticolo { get; set; }
        public string RalVernice { get; set; }
        public string StatoOrdine { get; set; }
    }
}
