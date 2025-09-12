using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.BusinessCentral.COMMON
{
    public static class BcTipoPilettaSifone
    {
        public const string Nessuno = "0";
        
        public const string ScaricoLibero = "1";                            //blocco-tipologie tipicamente applicate a: TOP VASCA INTEGRATA / LASTRA + VASCA / LAMINATO / CATINI / MONOBLOCCHI/ COLONNA / CONSOLLE
        public const string ScaricoLiberoCromato = "10";
        public const string ScaricoLiberoInTinta = "11";
        public const string ScaricoLiberoInTintaConLogo = "12";
        public const string ScaricoLiberoH70InTinta = "13";
        public const string ScaricoLiberoH70InTintaConLogo = "14";
        public const string ScaricoLiberoCliente = "15";
        public const string ScaricoLiberoConPilettoneInTinta = "16";

        public const string ClickerCromato = "2";
        public const string ClickerInTinta = "20";
        public const string ClickerInTintaConLogo = "21";
        public const string ClickerH90InTinta = "22";
        public const string ClickerH90InTintaConLogo = "23";
        public const string ClickerCliente = "24";
        public const string ClickerConPilettoneInTinta = "25";

        public const string Invisibile = "3";

        public const string CuffiaIspezionabile = "4";

        public const string SifoneConScaricoLibero = "5";
        public const string SifoneConScaricoLiberoInTinta = "50";
        public const string SifoneConScaricoLiberoInTintaConLogo = "51";
        public const string SifoneConClickerInTinta = "52";
        public const string SifoneConClickerInTintaConLogo = "53";
        public const string SifoneConScaricoLiberoConPilettoneInTinta = "54";
        public const string SifoneConClickerConPilettoneInTinta = "55";

        public const string GamboPortaTappoPiuCover = "6";

        public const string SifoneOmpConDentelliConCover = "90";            //blocco-tipologie tipicamente applicate a: PIATTI DOCCIA
        public const string SifoneOmpSenzaDentelliConCover = "91";
        public const string SifoneOmpSenzaDentelliConCoverInox = "95";
        public const string SifoneOmpSenzaDentelliConCoverInTinta = "96";
        public const string SifoneOmpAlbireoConCover = "92";
        public const string SifoneSacithConCover = "93";
        public const string SifoneRibassatoConCover = "94";

        public const string SifoneOmpRibassatoConCover = "40";          //blocco-tipologie tipicamente applicate a: VASCHE DA BAGNO
        public const string SifoneOmpRibassatoConCoverConLogo = "401";      
        public const string SifoneOmpLgaConCover = "41";                
        public const string SifoneOmpLgaConCoverConLogo = "411";            
        public const string SifoneSilfraConCover = "42";                    
        public const string SifoneSilfraConCoverConLogo = "421";            

    }
}
