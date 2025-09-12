using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.BusinessCentral.COMMON
{
    public static class BcCustomer
    {
        public static bool IsGruppoIdea(string CustomerCode)
        {
            switch(CustomerCode)
            {
                case Idea:
                case Aqua:
                case Disenia:
                case Blob:
                    return true;

                default:
                    return false;
            } 
        }
        public static bool IsGruppoAzzuraAreaBagno(string CustomerCode)
        {
            switch (CustomerCode)
            {
                case Azzurra:
                case AreaBagno:
                    return true;

                default:
                    return false;
            }
        }



        //public const string Blubleu = 156;
        //public const string GruppoIDEA = 266;
        //public const string NovelBath = 56;
        //public const string RapidInterventionSolution = 304;
        //public const string Legnox = "CI000168";                              //Vecchio
        //public const string LegnoBagno = "CI000167";                          //Vecchio        
        //public const string KarolItalia = "CI000421";                         //(Vecchi, non attivi "CI000157", "CI000323")
        //public const string Arteba = "CI000049";                            //Vecchio (Sembra che in PSR non fosse mai usato)

        public const string Omnitech = "CI000411";

        public const string AAMSrl = "CI000335";
        public const string AdrianoRizzetto = "CI000027";
        public const string AeT = "CI000286";
        public const string Agape = "CI000028";
        public const string Agorà = "CI000029";
        public const string Albatros = "CI000030";                          //è diventato "La Doccia Srl" ?!
        public const string AlessiaViolante = "CI000365";
        public const string AlMoAlberghi = "CI000063";
        public const string AlpaBagno = "CI000031";
        public const string Aqua = "CI000033";
        public const string Arbi = "CI000037";
        public const string Arblu = "CI000038";
        public const string Arca = "CI000036";
        public const string Archeda = "CI000039";
        public const string Arcom = "CI000040";
        public const string AreaBagno = "CI000042";
        public const string ArlexItalia = "CI000044";
        public const string ArredamentiIvanTuani = "CI000061";
        public const string ArtebaSrl = "CI000415";
        public const string Artesi = "CI000050";
        public const string Azzurra = "CI000055";
        public const string BMArredamenti = "CI000277";
        public const string BMT = "CI000064";
        public const string BTGroup = "CI000018";
        public const string Blob = "CI000380";
        public const string Branchetti = "CI000351";
        public const string Brini = "CI000290";
        public const string Buratti = "CI000066";
        public const string CapoDOpera = "CI000069";
        public const string Carmenta = "CI000071";
        public const string Cerasa = "CI000079";
        public const string CeramicheCalzavaraSas = "CI000425";
        public const string Cesaro = "CI000327";
        public const string DallAgneseGroup = "CI000308";         //DA Group
        public const string DaRoldArreda = "CI000434";
        public const string DeZottiDesign = "CI000294";
        public const string Disenia = "CI000098";
        public const string DueBBagni = "CI000304";
        public const string Edmo = "CI000104";
        public const string Floema = "CI000340";
        public const string GBGroup = "CI000135";
        public const string GeDArredamenti = "CI000297";
        public const string Glass = "CI000067";
        public const string GranTour = "CI000019";
        public const string GruppoGeromin = "CI000360";
        public const string GZdiGianniZandona = "CI000318";
        public const string Idea = "CI000145";
        public const string IdealBagni = "CI000146";
        public const string IdiStudioSrl = "CI000354";
        public const string Imab = "CI000015";
        public const string Inda = "CI000151";
        public const string Irdide = "CI000008";
        //public const string IrdideOld = "CI000329";
        public const string Kios = "CI000158";
        public const string LaDocciaSrl = "CI000459";
        public const string LasaIdea = "CI000165";
        public const string LicorDesign = "CI000078";
        public const string LucaViscovi = "CI000363";
        public const string MagiAgostino = "CI000178";
        public const string Makro = "CI000319";
        public const string Mastella = "CI000185";
        public const string MasterBath = "CI000362";
        public const string MastroFiore = "CI000389";
        public const string MaxMeroni = "CI000381";
        public const string Milldue = "CI000192";
        public const string MobilSRL = "CI000194";
        public const string Mobilcrab = "CI000195";
        public const string Mobilduenne = "CI000196";
        public const string Modulnova = "CI000197";
        public const string ModusOperandi = "CI000364";
        public const string Nilo = "CI000379";
        public const string NorahDesignSrl = "CI000453";
        public const string Novellini = "CI000396";
        public const string Novello = "CI000205";
        public const string Oasis = "CI000208";
        public const string Orlando = "CI000344";
        public const string Padoan = "CI000213";
        public const string PadoanSistemi = "CI000338";
        public const string PietraNera = "CI000321";
        public const string PiQuattro = "CI000352";
        public const string Pirovano = "CI000219";
        public const string Pozzebon = "CI000271";
        public const string ProgettoBagno = "CI000225";
        public const string Puntotre = "CI000227";
        public const string RABArredobagno = "CI000231";
        public const string Rainbox = "CI000132";
        public const string RCR = "CI000229";
        public const string Relax = "CI000007";
        public const string Rexa = "CI000234";
        public const string RonalBathroomsDivKarol = "CI000455";
        public const string SachGroupSrl = "CI000075";
        public const string SimoneBiscontin = "CI000451";
        public const string SocietaAgricolaBellavista = "CI000454";
        public const string Soema = "CI000278";
        public const string SpagnolMobili = "CI000420";
        public const string Stocco = "CI000062";
        public const string Synergie = "CI000252";
        public const string TDA = "CI000356";
        public const string TipsSrl = "CI000400";
        public const string TreMC = "CI000021";
        public const string VettorettiCeramiche = "CI000285";
        
    }
}
