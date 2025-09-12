using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.CLIENTI.ImportFiles
{
    public class RowBaseDTO
    {
        public int __FileId { get; set; }       //si potrebbe usare un semplice FileId (int)    -- Foreign Key
        public int __RowId {  get; set; }       //Primary Key (della tabella)                           
        public Guid __RowGuid { get; set; }     //Campo univoco (trasversalmente a tutte le tabelle)    --per recuperare la riga da Business Central
        
    }
}
