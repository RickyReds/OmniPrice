using Omnitech.Prezzi.Core.Services;
using System;
using System.Collections.Generic;

namespace Omnitech.Prezzi.Core.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati degli sconti
    /// </summary>
    public interface IDiscountRepository
    {
        /// <summary>
        /// Ottiene gli sconti applicabili per un cliente
        /// </summary>
        List<CustomerDiscount> GetCustomerDiscounts(int customerId);

        /// <summary>
        /// Ottiene gli sconti per un cliente e una data specifica
        /// </summary>
        List<CustomerDiscount> GetCustomerDiscounts(int customerId, DateTime orderDate);

        /// <summary>
        /// Ottiene gli sconti generali applicabili
        /// </summary>
        List<GeneralDiscount> GetGeneralDiscounts();

        /// <summary>
        /// Ottiene gli sconti per quantità
        /// </summary>
        QuantityDiscount GetQuantityDiscount(int customerId, int quantity);

        /// <summary>
        /// Verifica se uno sconto è attivo per un cliente
        /// </summary>
        bool IsDiscountActive(int customerId, string discountCode);

        /// <summary>
        /// Salva un nuovo sconto cliente
        /// </summary>
        bool SaveCustomerDiscount(int customerId, CustomerDiscount discount);

        /// <summary>
        /// Rimuove uno sconto cliente
        /// </summary>
        bool RemoveCustomerDiscount(int customerId, int discountId);
    }

    /// <summary>
    /// Sconto generale applicabile a tutti
    /// </summary>
    public class GeneralDiscount
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Percentage { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Sconto basato sulla quantità
    /// </summary>
    public class QuantityDiscount
    {
        public int MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Description { get; set; }
    }
}