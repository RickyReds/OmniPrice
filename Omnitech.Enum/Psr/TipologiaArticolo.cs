using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Omnitech.Enum.Psr
{
    /*
    public enum enum0
    {
        [Description("AAA")]
        a = 0,
        b = 1
    }*/

    public enum TipologiaArticolo
    {
        Undefined = 0,

        [Description("Top Vasca Integrata")]
        TopVascaIntegrata = 1,
        [Description("Top Vasca Integrata Semincasso")]
        TopVascaIntegrataSemincasso = 2,
        [Description("Catino Sottopiano")]
        CatinoSottopiano = 3,
        [Description("Catino Semincasso")]
        CatinoSemincasso = 4,
        [Description("Catino Soprapiano")]
        CatinoSoprapiano = 5,
        Lavanderia = 6,
        [Description("Vasca da Bagno")]
        VascaBagno = 7,
        [Description("Piatto Doccia")]
        PiattoDoccia = 8,
        Monoblocco = 9,
        Colonna = 10,
        Consolle = 11,
        Tavolo = 12,
        Accessorio = 13,
        Alzatina = 14,
        Lastra = 15,
        Lavamani = 16,
        Campioncini = 17,
        Copripiletta = 18,
        Mensola = 19,
        Tavoletta = 20,
        [Description("Top Vasca Integrata Scatolata")]
        TopVascaIntegrataScatolato = 21,
        [Description("Top Doppia Vasca Integrata")]
        TopDoppiaVascaIntegrata = 22,
        [Description("Top Doppia Vasca Integrata Scatolato")]
        TopDoppiaVascaIntegrataScatolato = 23,
        TopDoppiaVascaIntegrataSemincassoScatolato = 24,
        Varie = 25,
        LastraPiùVascaSaldata = 26,
        LastraPiùVascaSaldataDoppia = 27,
        LastraPiùVascaSaldataScatolata = 28,
        LastraPiùVascaSaldataDoppiaScatolata = 29,
        [Description("Piatto Doccia AZ")]
        PiattoDocciaAZ = 30,
        [Description("Piatto Doccia ONDA")]
        PiattoDocciaONDA = 31,
        [Description("Piatto Doccia CATINO")]
        PiattoDocciaCATINO = 32,
        [Description("Piatto Doccia DUNA")]
        PiattoDocciaDUNA = 33,
        Catino = 34,
        [Description("Vasca da Bagno FreeStanding")]
        VascaBagnoFreeStanding = 35,
        [Description("Vasca da Bagno Pannellata")]
        VascaBagnoPannellata = 36,
        ConsolleDoppiaVasca = 37,
        LavanderiaDoppiaVasca = 38,
        MonobloccoDoppiaVasca = 39,
        LastraCatinoSoprapiano = 40,
        LastraCatinoSoprapianoDoppio = 41,
        Laminato = 42,
        LaminatoDoppiaVasca = 43,
                                                //VascaBagnoPannellataScatolata = 44
        CatinoDoppiaVasca = 45,
        CatinoDoppiaVascaScatolato = 46,
        LastraScatolata = 47,
        LavaboCostruito = 48,
        LavaboCostruitoScatolato = 49,
        LavaboCostruitoScatolatoLPV = 50,
        LavaboCostruitoDoppiaVasca = 51,
        [Description("Piatto Doccia Line")]
        PiattoDocciaLINE = 52,                             // 61
        MonobloccoScatolato = 53,
        AlzatinaScatolata = 54,
        CatinoSottopianoScatolato = 55,
        TopDoppiaVascaIntegrataSemincasso = 56,
        LavanderiaScatolata = 57,
        LavaboCostruitoLastraPiùVasca = 58,
        MonobloccoDoppiaVascaScatolato = 59,
        ConsolleScatolata = 60,                                         //2018.07.27
        PiattoDocciaAZScatolato = 61,                                         //2019.03.20
        [Description("Piatto Doccia ELLE")]
        PiattoDocciaELLE = 62,                                         //2019.01.24
        VascaBagnoPannellataScatolata = 63,                                         //2019.03.01
        Cassettiera = 64,                                         //2019.03.01
        LastraPiùVascaSaldataScatolataLaminata = 65,                                         //2019.03.01
        Boiserie = 66,                                         //2019.03.06
        PiattoDocciaSTREET = 67,                                         //il LINE per disenia
        PiattoDocciaONDAScatolato = 68,                                         //2019.06.26
        [Description("Mini Piscina")]
        MiniPiscina = 75,                                         //2019.06.26
        Pensile = 76,                                         //2019.09.30
        Folding = 77,                                         //2019.09.30
        Sgabello = 78,                                         //2019.09.30
        PiattoDocciaLINEAR = 79,                                         //2021.03.09
        PiattoDocciaPARALLEL = 80,                                         //2021.03.09
        PiattoDocciaPIETRA = 81,                                         //2021.03.09
        VascaBagnoGuscio = 82,                                         //2021.03.10
        VascaBagnoGuscioScatolato = 83,                                         //2021.03.24
        PiattoDocciaDuoLine = 84,                                         //2022.03.02
        Bacinella = 85,                                         //2022.02.15
        PedanaPiattoDoccia = 86,                                         //2022.02.15
        Soffione = 87,                                         //2022.02.15
        PortaSapone = 88,                                         //2022.02.15
        FrontaleCassetto = 89,
        BaseTavolo = 90,                                         //2022.02.15
        Paralume = 91,
        Ted_B391 = 92,
        PortaOggetti = 93,
        PannelloCapitone = 94,
        Anello = 95,
        PortaRubinetto = 96,
        Vassoio = 97,
        Pavimentazione = 98,
        Supporto = 99,
        Carter = 100,
        Maniglia = 101,
        PortaBicchiere = 102,
        Termoarredo = 103,
        Barra = 104,
        Tappo = 105,
        LastraDiBase = 106,
        Top = 107,
        TopScatolato = 108,                                         //2022.02.23
        VascaBagnoMuretto = 109,                                         //2022.02.23
        TavolettaFondoLavabo = 110,                                         //2022.03.07
        Copripiletta_LAV = 111,                                         //2022.03.07
        Copripiletta_PDC = 112,                                         //2022.03.07
        Ted_B392 = 113,                                         //2022.03.07
        PiattoDocciaEASYSTONE = 114,                                         //2022.03.11
        ConsolleMultivascaScatolata = 115,                                         //2022.04.22
        PannelloVasca = 116,                                         //2022.05.10
        Erogatore = 117,                                         //2022.05.10
        LavanderiaMultivasca = 118,                                         //2022.05.10
        LavanderiaMultivascaScatolata = 119,                                         //2022.05.10
        TopVascaIntegrataMuretto = 120,                                         //2022.05.13
        
        Test = 99999

    }


}
