using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.CLIENTI.ImportFiles.Arcom
{
    public class DTO : RowBaseDTO
    {
        public string IDENTI { get; set; }
        public string ORDINE { get; set; }
        public string RIGA { get; set; }
        public string MATRICOLA { get; set; }
        public string BARCODE { get; set; }
        public string IDARTICOLI { get; set; }
        public string DESCRIZIONE { get; set; }
        public string CODICECOMMERCIALE { get; set; }
        public int DIMX { get; set; }
        public int DIMY { get; set; }
        public int DIMZ { get; set; }
        public int QUANTITA { get; set; }
        public string IDUM { get; set; }
        public DateTime DATACONSEGNA { get; set; }
        public string PRIORITA { get; set; }
        public string RIFERIMENTO { get; set; }
        public string RIFERIMENTOFORNITORE { get; set; }
        public string COLLO { get; set; }
        public string VARIANTI { get; set; }
        public string ATTRIBUTI { get; set; }
        public string NOTE { get; set; }




        public string BLL_Ordine
        {
            get
            {
                return (ORDINE != null && ORDINE.StartsWith("F")) ? ORDINE.Substring(1) : ORDINE;
            }
        }

        public string BLL_Riferimento
        {
            get
            {
                return (RIFERIMENTO != null && RIFERIMENTO.StartsWith("V")) ? RIFERIMENTO.Substring(1) : RIFERIMENTO;
            }
        }

        public string BLL_RiferimentoFornitore
        {
            get
            {
                return (RIFERIMENTOFORNITORE != null && RIFERIMENTOFORNITORE.StartsWith("V")) ? RIFERIMENTOFORNITORE.Substring(1) : RIFERIMENTOFORNITORE;
            }
        }


        public int BLL_L
        {
            get
            {
                return DIMX;
            }
        }
        public int BLL_P
        {
            get
            {
                return DIMY;
            }
        }
        public int BLL_H
        {
            get
            {
                return DIMZ;
            }
        }

    }
}

