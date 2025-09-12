using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.BusinessCentral.COMMON
{
    public static class BcTipologiaImballo
    {
        public const string Nessuno = "0";                                  //generici (che si applicano a tutti i sottogruppi)
        public const string Industriale = "1";

        public const string CartoneSenzaRinforzo = "10";                    //piatti doccia
        public const string CartoneConGabbiaNormale = "11";                 //(come sinonimo sarebbe anche "CartoneConRinforzo")
        public const string CartoneConGabbiaFumigata = "12";
        public const string CartoneConCassa6LatiNormale = "13";
        public const string CartoneConCassa6LatiFumigata = "14";
        public const string Cassa6LatiNormale = "15";
        public const string Cassa6LatiFumigata = "16";

        public const string TipoASenzaRinforzo = "20";
        public const string TipoAConGabbiaNormale = "21";
        public const string TipoAConGabbiaFumigata = "22";
        public const string TipoAConCassa6LatiNormale = "23";
        public const string TipoAConCassa6LatiFumigata = "24";

        public const string TipoBSenzaRinforzo = "30";
        public const string TipoBConGabbiaNormale = "31";
        public const string TipoBConGabbiaFumigata = "32";
        public const string TipoBConCassa6LatiNormale = "33";
        public const string TipoBConCassa6LatiFumigata = "34";

        public const string TipoBRibSenzaRinforzo = "40";
        public const string TipoBRibConGabbiaNormale = "41";
        public const string TipoBRibConGabbiaFumigata = "42";
        public const string TipoBRibConCassa6LatiNormale = "43";
        public const string TipoBRibConCassa6LatiFumigata = "44";

        public const string TipoCSenzaRinforzo = "50";
        public const string TipoCConGabbiaNormale = "51";
        public const string TipoCConGabbiaFumigata = "52";
        public const string TipoCConCassa6LatiNormale = "53";
        public const string TipoCConCassa6LatiFumigata = "54";

        public const string TipoA = "60";                                   //vasche da bagno
        public const string TipoBOsbNormale = "61";
        public const string TipoBOsbFumigato = "62";
        public const string TipoCOsbNormale = "63";
        public const string TipoCOsbFumigato = "64";
        public const string IdealStandard = "65";
        public const string Mastella = "66";

        //                                                                    top (valori gia presenti)

        //                                                                    boiserie (valori gia presenti)

        public const string ImballoNovello = "67";                          //catini e monoblocchi (+altri valori gia presenti)

        public const string ImballoFashion = "70";                          //colonne (+altri valori gia presenti)
        public const string ImballoHilton = "71";
        public const string ImballoRoman = "72";

    }
}
