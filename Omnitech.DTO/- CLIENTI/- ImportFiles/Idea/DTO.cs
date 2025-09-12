using Omnitech.Enum.BusinessCentral.COMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.CLIENTI.ImportFiles.Idea
{
    public class DTO : RowBaseDTO
    {
        public DateTime DataOrdine { get; set; }
        public DateTime DataConsegna { get; set; }
        public string ComN { get; set; }


        public string Mis { get; set; }
        public string ArtForm { get; set; }
        public char? Mix { get; set; }
        public string Finitura { get; set; }
        public string Rif { get; set; }
        public string Piletta { get; set; }
        public string Rif_Piletta { get; set; }
        public string TP { get; set; }


        public string BLL_PosizioneVasca
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Mis))
                    return null;

                else if (Mis.EndsWith("sx"))
                    return "SINISTRA";
                else if (Mis.EndsWith("cx"))
                    return "CENTRALE";
                else if (Mis.EndsWith("dx"))
                    return "DESTRA";
                else
                    return null;
            }
        }

        public string BLL_Foro                              //poco utile ma....
        {
            get
            {
                if (!Mix.HasValue)
                    return null;

                switch(Mix)
                {
                    case 'A':
                        return "NO_FORO";

                    case 'B':
                        return "CENTRALE";

                    case 'E':
                        return "3_FORI";

                    default:
                        return null;
                }

            }
        }

        public string BLL_Materiale
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Finitura))
                    return null;

                else if (Finitura.ToUpperInvariant().Trim().StartsWith("M.LUX"))
                    return BcMateriale.Geacril;

                else if (Finitura.ToUpperInvariant().Trim().StartsWith("M.SOLID"))
                    return BcMateriale.GeacrilMatt;

                else
                    return null;
            }
        }

        public string BLL_DescrizioneColore
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Finitura))
                    return null;

                int col_index = Finitura.IndexOf(' ') + 1;

                return Finitura.Substring(col_index);
            }
        }

        public string BLL_TipoPiletta
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Piletta))
                    return BcTipoPilettaSifone.Nessuno;

                else if (Piletta.ToUpperInvariant().Contains("LIBERO"))
                    return BcTipoPilettaSifone.ScaricoLiberoInTinta;

                else if (Piletta.ToUpperInvariant().Contains("-CLA") || Piletta.ToUpperInvariant().Contains("CLAC"))
                    return BcTipoPilettaSifone.ClickerInTinta;

                else
                    return BcTipoPilettaSifone.Nessuno;
            }
        }

        public bool BLL_TroppoPieno
        {
            get 
            {
                return (!string.IsNullOrWhiteSpace(TP) && TP.ToUpperInvariant().Trim() == "T/P");
            }
        }

    }
}
