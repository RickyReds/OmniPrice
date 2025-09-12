using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.CLIENTI.ImportFiles.Arteba
{
    public class DTO : RowBaseDTO
    {
        public string Numero_Ordine {  get; set; }              //tipicamente complilato
        public DateTime? Data_Ordine { get; set; }
        public string Codice_Articolo { get; set; }
        public string Descrizione_Articolo { get; set; }
        public int? N_Dim_1 { get; set; }
        public int? N_Dim_2 { get; set; }
        public int? N_Dim_3 { get; set; }

        public int? Qta_Ordinata { get; set; }
        public DateTime? Data_Consegna { get; set; }
        public string Numero_Ordine_Cliente { get; set; }       //tipicamente compilato
        public string BarCode { get; set; }                     //tipicamente compilato

    }
}
