using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.CLIENTI.ImportFiles.Arbi
{
    public class DTO : RowBaseDTO
    {
        public string ORDINE {  get; set; }
        public string RIFERIMENTO { get; set; }
        public DateTime DATA_CONSEGNA { get; set; }
        public string CODICE { get; set; }
        public float QTA { get; set; }
        public string DESCRIZIONE { get; set; }
        public string VS_CODICE { get; set; }
        public string COD_BARRE { get; set; }
        public string LINEA { get; set; }
        public string UM { get; set; }
        public DateTime? CONSEGNA { get; set; }
        public string COLORE { get; set; }
        public string CARICO { get; set; }

        public string COLORE_COPRIPILETTA { get; set; }         //Se 2 righe hanno lo stesso "COD_BARRE", la 2^ si assume essere il copripiletta
                                                                //--> se ne legge il "COLORE" e lo si salva nella riga del piatto doccia.
                                                                //(LA riga del copripiletta viene poi ignorata)

        
        
        public bool BLL_FuoriMisura                             //Non sono importati ma gestiti manualmente. (Perchè?)    (Sarà eventualmente escluso anche il relativo copripiletta)
        {
            get
            {
                //I codici dei "fuori misura" sembrano essere del tipo: PSDFMLET
                //Assumiamo che contenga "FM" ma NON all'inizio o alla fine.

                string c = CODICE.ToUpperInvariant();

                return (!string.IsNullOrWhiteSpace(c) && c.Contains("FM") && !c.StartsWith("FM") && !c.EndsWith("FM"));       //fin troppo generica e sucettibile a falsi positivi.... :-(

            
            }
        }

        public bool BLL_HasCopripiletta
        {
            get
            {
                return !string.IsNullOrWhiteSpace(COLORE_COPRIPILETTA);
            }
        }

    }
}
