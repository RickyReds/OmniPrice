using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.CLIENTI.ImportFiles.Arblu
{
    public class DTO : RowBaseDTO
    {
        public int ANNO { get; set; }
        public string NUMERATORE { get; set; }
        public string ORDINE { get; set; }
        public DateTime? DT_ORDINE_FORNITORE { get; set; }
        public DateTime? dt_ordine_acq { get; set; }
        public int RIGA { get; set; }
        public string barcode { get; set; }                 //importante (es:  2025ACQ001726-11001)
        public string c_commessa { get; set; }
        public string c_fornitore { get; set; }
        public string ARTICOLO { get; set; }
        public string Cod_Scheda { get; set; }
        public double? PREZZO { get; set; }
        public double? SOVRAPPREZZO { get; set; }
        public double? prezzi { get; set; }
        public string DS_ESTESA { get; set; }
        public string DS_AGGIUNTIVA { get; set; }
        public int qta_attesa { get; set; }
        public string sg_um_dim1 { get; set; }
        public string sg_um_dim2 { get; set; }
        public string sg_um_dim3 { get; set; }
        public double n_dim_1 { get; set; }
        public double n_dim_2 { get; set; }
        public double n_dim_3 { get; set; }
        public string c_gruppo_ricerca { get; set; }
        public string ANNO_OC { get; set; }
        public string NUMERATORE_OC { get; set; }
        public string ORDINE_OC { get; set; }
        public DateTime? dt_consegna_richiesta { get; set; }
        public string fg_classe_importanza { get; set; }
        public string COD_MAGAZZINO { get; set; }
        public string DS_ESTESA_MAG { get; set; }
        public string DS_AGGIUNTIVA_MAG { get; set; }
        public string c_collo { get; set; }
        public string Finitura { get; set; }
        public string Bordi_tagli { get; set; }
        public string Tipo_lavabo { get; set; }
        public string Doppio_lavabo { get; set; }
        public string Stampo_lavabo { get; set; }
        public string Fori_lavabi { get; set; }
        public string Doppia_vasca { get; set; }
        public string Tipo_piatto { get; set; }
        public string Verso { get; set; }
        public string cod_finale { get; set; }



        public int BLL_L 
        {  
            get
            {
                return (int)Math.Ceiling(n_dim_1);
            }
        }
        public int BLL_P
        {
            get
            {
                return (int)Math.Ceiling(n_dim_3);
            }
        }

        public int BLL_H
        {
            get
            {
                return (int)Math.Ceiling(n_dim_2);
            }
        }

        public string BLL_Collo
        {
            get
            {
                switch(NUMERATORE)
                {
                    case "ACQ":
                        return COD_MAGAZZINO;

                    case "ACF":
                        return c_collo;

                    default:
                        return null;
                }
            }
        }
    }
}
