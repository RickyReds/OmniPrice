using Omnitech.Prezzi.Core.Models;
using System.Collections.Generic;

namespace Omnitech.Prezzi.Core.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati dei clienti
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Ottiene le informazioni complete di un cliente
        /// </summary>
        CustomerInfo GetCustomerInfo(int customerId);

        /// <summary>
        /// Ottiene le informazioni di un cliente tramite codice
        /// </summary>
        CustomerInfo GetCustomerByCode(string customerCode);

        /// <summary>
        /// Verifica se un cliente ha un listino personalizzato
        /// </summary>
        bool HasCustomPriceList(int customerId);

        /// <summary>
        /// Ottiene la lista dei clienti attivi
        /// </summary>
        List<CustomerInfo> GetActiveCustomers();

        /// <summary>
        /// Ottiene le maggiorazioni specifiche per un cliente
        /// </summary>
        CustomerSurcharges GetCustomerSurcharges(int customerId);

        /// <summary>
        /// Aggiorna le informazioni di un cliente
        /// </summary>
        bool UpdateCustomerInfo(CustomerInfo customer);
    }

    /// <summary>
    /// Maggiorazioni specifiche del cliente
    /// </summary>
    public class CustomerSurcharges
    {
        public decimal ColorSurcharge { get; set; }
        public decimal ColorSurchargeCatini { get; set; }
        public decimal BlackSurcharge { get; set; }
        public decimal BlackSurchargeCatini { get; set; }
        public decimal MaxDepthSurcharge { get; set; }
        public double DepthLimitForTVI { get; set; }
    }
}