namespace Omnitech.Prezzi.Core.Enums
{
    /// <summary>
    /// Tipologia articolo (estratto da eTipologiaArticolo in PsR)
    /// </summary>
    public enum ArticleType
    {
        Undefined = -1,
        TopVascaIntegrata = 1,
        TopVascaIntegrataSemincasso = 2,
        Lavanderia = 3,
        LastraPiuVascaSaldata = 4,
        PiattoDoccia = 5,
        VascaDaBagno = 6,
        Lastra = 7,
        Catino = 8,
        Contract = 9,
        Scatolatura = 10,
        Fresata = 11,
        FresataLED = 12
    }

    /// <summary>
    /// Tipo di imballo
    /// </summary>
    public enum PackagingType
    {
        Normal = 0,
        Robusto = 1  // IR - Imballo Robusto
    }
}