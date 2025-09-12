using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.CLIENTI.ImportFiles.Disenia
{
    public class DTO : RowBaseDTO
    {
        public string Cod_Articolo { get; set; }
        public string Descrizione { get; set; }
        public double Qta_UM { get; set; }
        public DateTime Cons_Rich {  get; set; }


    }
}
