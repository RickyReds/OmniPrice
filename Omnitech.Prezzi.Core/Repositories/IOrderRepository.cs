using Omnitech.Prezzi.Core.Models;
using System;
using System.Collections.Generic;

namespace Omnitech.Prezzi.Core.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati degli ordini
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Ottiene un ordine tramite barcode
        /// </summary>
        Order GetOrderByBarcode(string barcode);

        /// <summary>
        /// Ottiene gli ordini di un cliente
        /// </summary>
        List<Order> GetCustomerOrders(int customerId);

        /// <summary>
        /// Ottiene gli ordini in un intervallo di date
        /// </summary>
        List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Ottiene le informazioni dello stampo
        /// </summary>
        Stamp GetStampInfo(int stampId);

        /// <summary>
        /// Ottiene le informazioni dello stampo tramite codice
        /// </summary>
        Stamp GetStampByCode(string stampCode);

        /// <summary>
        /// Salva un ordine nel database
        /// </summary>
        bool SaveOrder(Order order);

        /// <summary>
        /// Aggiorna il prezzo di un ordine
        /// </summary>
        bool UpdateOrderPrice(string barcode, decimal price);

        /// <summary>
        /// Verifica se un ordine esiste
        /// </summary>
        bool OrderExists(string barcode);

        /// <summary>
        /// Ottiene il conteggio degli ordini per cliente
        /// </summary>
        int GetCustomerOrderCount(int customerId);

        /// <summary>
        /// Ottiene gli ordini NAV da sincronizzare
        /// </summary>
        List<Order> GetOrdersToSyncWithNAV();

        /// <summary>
        /// Marca un ordine come sincronizzato con NAV
        /// </summary>
        bool MarkOrderAsSynced(string barcode, string navOrderNumber, string navRowNumber);
    }
}