namespace Omnitech.Prezzi.Core.Enums
{
    /// <summary>
    /// Enum dei clienti (estratto da eCliente in PsR)
    /// I valori più comuni trovati nel codice PsR
    /// </summary>
    public enum Customer
    {
        Undefined = -1,
        Omnitech = 1,
        Idea = 2,
        Aqua = 3,
        Blob = 4,
        RAB = 5,
        Mobilcrab = 6,
        Milldue = 7,
        Rexa = 8,
        Puntotre = 9,
        GBGroup = 10,
        Cerasa = 11,
        AdrianoRizzetto = 12,
        Arbi = 13,
        Azzurra = 14,
        Archeda = 15,
        Agora = 16,
        NoraDesign = 17
    }

    /// <summary>
    /// Tipo di listino utilizzato
    /// </summary>
    public enum PriceListType
    {
        Undefined = -1,
        Standard = 0,     // Listino standard
        MetroLineare = 1  // ML - Metro Lineare
    }
}