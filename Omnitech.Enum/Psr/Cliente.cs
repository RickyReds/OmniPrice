using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.Psr
{
    public enum Cliente
    {
        [Description("Non definito")]
        Undefined = -1,

        Agorà = 1,
        [Description("Alpa Bagno")]
        AlpaBagno = 2,
        Aqua = 3,
        Arbi = 4,
        Arblu = 5,
        Arca = 6,
        Archeda = 7,
        Arcom = 8,
        AreaBagno = 9,
        ArlexItalia = 10,
        Arteba = 12,                    //è usato? C'è un Arteba Srl (che sembra usato)
        Artesi = 13,
        Azzurra = 14,
        BMArredamenti = 15,
        BMT = 18,
        Buratti = 20,
        GeD = 21,
        Cerasa = 23,
        Disenia = 26,
        GBGroup = 34,
        Idea = 35,
        [Description("Ideal Bagni")]
        IdealBagni = 36,
        Karol = 39,
        Kios = 40,
        LasaIdea = 42,
        MobilSRL = 43,
        LegnoBagno = 45,
        Lagnox = 46,
        MagiAgostino = 47,
        Mastella = 48,
        Milldue = 50,
        Mobilcrab = 51,
        Mobilduenne = 52,
        Modulnova = 53,
        Pozzebon = 55,
        NovelBath = 56,
        Novello = 57,
        Pirovano = 59,                                  // 2021.09.07
        Puntotre = 60,
        AdrianoRizzetto = 64,                           // 2021.08.31
        Synergie = 66,                                  // 2020.07.13
        Oasis = 75,
        Omnitech = 86,
        RAB = 61,
        Rexa = 90,
        Agape = 122,                                    // 2018.11.16
        Soema = 124,
        TreMC = 136,
        DallAgnese = 141,                               // 2023.07.24
        Makro = 150,                                    // 2022.04.11
        Blubleu = 156,
        Branchetti = 165,                               // 2019.11.27
        Pi4 = 204,                                      // 2020.07.13
        GruppoGeromin = 215,
        Blob = 246,
        Relax = 259,
        GruppoIDEA = 266,
        Imab = 271,
        BTGroup = 276,                                  // 2019.07.08
        GranTour = 277,                                 // 2019.05.23
        Albatros = 282,                                 // 2019.02.15
        Rainbox = 290,                                  // 2019.04.30
        RapidInterventionSolution = 304,                // 2019.10.15
        Glass = 314,                                    // 2020.03.05
        Stocco = 318,                                   // 2022.04.01
        ArtebaSrl = 357,
        LaDocciaSrl = 414                               //2025.06.23        --ha sostituito Albatros (che aveva sostituito Raibox)

    }


}
