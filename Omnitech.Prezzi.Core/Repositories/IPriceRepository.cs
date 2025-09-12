using Omnitech.Prezzi.Core.Enums;
using Omnitech.Prezzi.Core.Models;
using Omnitech.Prezzi.Core.Services;

namespace Omnitech.Prezzi.Core.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati dei prezzi
    /// </summary>
    public interface IPriceRepository
    {
        /// <summary>
        /// Ottiene la tabella del listino personalizzato per un cliente
        /// </summary>
        string GetCustomPriceListTable(int customerId);

        /// <summary>
        /// Ottiene il prezzo per un top vasca integrata
        /// </summary>
        PriceData GetIntegratedTopPrice(string tableName, string stampCode, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per un piatto doccia
        /// </summary>
        PriceData GetShowerTrayPrice(string tableName, string stampCode, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per una vasca da bagno
        /// </summary>
        PriceData GetBathtubPrice(string tableName, string stampCode, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per una lastra
        /// </summary>
        PriceData GetSheetPrice(string tableName, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo al metro quadro per una lastra
        /// </summary>
        double GetSheetPricePerSqm(string tableName, Material material);

        /// <summary>
        /// Ottiene il prezzo per la scatolatura
        /// </summary>
        PriceData GetBoxingPrice(string tableName, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per la fresata
        /// </summary>
        PriceData GetMillingPrice(string tableName, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per la fresata LED
        /// </summary>
        PriceData GetLEDMillingPrice(string tableName, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per un catino
        /// </summary>
        PriceData GetBasinPrice(string tableName, string stampCode, Material material);

        /// <summary>
        /// Ottiene il prezzo per una consolle
        /// </summary>
        PriceData GetConsolePrice(string tableName, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per una colonna
        /// </summary>
        PriceData GetColumnPrice(string tableName, Material material, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per la cassa in legno
        /// </summary>
        PriceData GetWoodenBoxPrice(string tableName, Dimensions dimensions);

        /// <summary>
        /// Ottiene il prezzo per la dima
        /// </summary>
        PriceData GetTemplatePrice(string tableName, string stampCode);

        /// <summary>
        /// Ottiene il prezzo per il salva goccia
        /// </summary>
        PriceData GetDripSaverPrice(string tableName, Material material);

        /// <summary>
        /// Ottiene il prezzo per il bordo contenitivo
        /// </summary>
        PriceData GetContainmentEdgePrice(string tableName, Material material, Dimensions dimensions);
    }
}