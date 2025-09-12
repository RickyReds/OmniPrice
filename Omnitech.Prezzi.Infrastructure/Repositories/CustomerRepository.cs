using Dapper;
using Omnitech.Prezzi.Core.Enums;
using Omnitech.Prezzi.Core.Models;
using Omnitech.Prezzi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Omnitech.Prezzi.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public CustomerInfo GetCustomerInfo(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        c.IdCliente as CustomerId,
                        c.RagioneSociale as CompanyName,
                        c.ListinoML as PriceListType,
                        c.MaggiorazioneColore as ColorSurcharge,
                        c.MaggiorazioneColore_Catini as ColorSurchargeCatini,
                        c.MaggiorazioneNero as BlackSurcharge,
                        c.MaggiorazioneNero_Catini as BlackSurchargeCatini,
                        c.LimiteProfonditaCalcoloPrezzoTVI as DepthLimitForTVI,
                        c.MaggiorazioneProfonditaMassima as MaxDepthSurcharge,
                        c.ScontoGenerale as GeneralDiscount,
                        c.DataAumenti as PriceIncreaseDate,
                        c.Aumenti2022Flag as Increases2022Flag,
                        CASE WHEN lp.IdCliente IS NOT NULL THEN 1 ELSE 0 END as HasCustomPriceList
                    FROM anagrafica.Clienti c
                    LEFT JOIN anagrafica.ListiniPersonalizzati lp 
                        ON c.IdCliente = lp.IdCliente AND lp.Attivo = 1
                    WHERE c.IdCliente = @customerId";

                var customer = connection.QueryFirstOrDefault<CustomerInfo>(query, new { customerId });
                
                if (customer != null)
                {
                    // Converti il valore booleano per PriceListType
                    customer.PriceListType = customer.PriceListType == PriceListType.MetroLineare 
                        ? PriceListType.MetroLineare 
                        : PriceListType.Standard;
                }

                return customer;
            }
        }

        public CustomerInfo GetCustomerByCode(string customerCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        c.IdCliente as CustomerId,
                        c.RagioneSociale as CompanyName,
                        c.ListinoML as PriceListType,
                        c.MaggiorazioneColore as ColorSurcharge,
                        c.MaggiorazioneColore_Catini as ColorSurchargeCatini,
                        c.MaggiorazioneNero as BlackSurcharge,
                        c.MaggiorazioneNero_Catini as BlackSurchargeCatini,
                        c.LimiteProfonditaCalcoloPrezzoTVI as DepthLimitForTVI,
                        c.MaggiorazioneProfonditaMassima as MaxDepthSurcharge,
                        c.ScontoGenerale as GeneralDiscount,
                        c.DataAumenti as PriceIncreaseDate,
                        c.Aumenti2022Flag as Increases2022Flag,
                        CASE WHEN lp.IdCliente IS NOT NULL THEN 1 ELSE 0 END as HasCustomPriceList
                    FROM anagrafica.Clienti c
                    LEFT JOIN anagrafica.ListiniPersonalizzati lp 
                        ON c.IdCliente = lp.IdCliente AND lp.Attivo = 1
                    WHERE c.CodiceCliente = @customerCode";

                var customer = connection.QueryFirstOrDefault<CustomerInfo>(query, new { customerCode });
                
                if (customer != null)
                {
                    customer.PriceListType = customer.PriceListType == PriceListType.MetroLineare 
                        ? PriceListType.MetroLineare 
                        : PriceListType.Standard;
                }

                return customer;
            }
        }

        public bool HasCustomPriceList(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(1) 
                    FROM anagrafica.ListiniPersonalizzati 
                    WHERE IdCliente = @customerId AND Attivo = 1";

                return connection.QuerySingle<int>(query, new { customerId }) > 0;
            }
        }

        public List<CustomerInfo> GetActiveCustomers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        c.IdCliente as CustomerId,
                        c.RagioneSociale as CompanyName,
                        c.ListinoML as PriceListType,
                        c.MaggiorazioneColore as ColorSurcharge,
                        c.MaggiorazioneColore_Catini as ColorSurchargeCatini,
                        c.MaggiorazioneNero as BlackSurcharge,
                        c.MaggiorazioneNero_Catini as BlackSurchargeCatini,
                        c.LimiteProfonditaCalcoloPrezzoTVI as DepthLimitForTVI,
                        c.MaggiorazioneProfonditaMassima as MaxDepthSurcharge,
                        c.ScontoGenerale as GeneralDiscount,
                        c.DataAumenti as PriceIncreaseDate,
                        c.Aumenti2022Flag as Increases2022Flag,
                        CASE WHEN lp.IdCliente IS NOT NULL THEN 1 ELSE 0 END as HasCustomPriceList
                    FROM anagrafica.Clienti c
                    LEFT JOIN anagrafica.ListiniPersonalizzati lp 
                        ON c.IdCliente = lp.IdCliente AND lp.Attivo = 1
                    WHERE c.Attivo = 1
                    ORDER BY c.RagioneSociale";

                var customers = connection.Query<CustomerInfo>(query).ToList();
                
                foreach (var customer in customers)
                {
                    customer.PriceListType = customer.PriceListType == PriceListType.MetroLineare 
                        ? PriceListType.MetroLineare 
                        : PriceListType.Standard;
                }

                return customers;
            }
        }

        public CustomerSurcharges GetCustomerSurcharges(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        MaggiorazioneColore as ColorSurcharge,
                        MaggiorazioneColore_Catini as ColorSurchargeCatini,
                        MaggiorazioneNero as BlackSurcharge,
                        MaggiorazioneNero_Catini as BlackSurchargeCatini,
                        MaggiorazioneProfonditaMassima as MaxDepthSurcharge,
                        LimiteProfonditaCalcoloPrezzoTVI as DepthLimitForTVI
                    FROM anagrafica.Clienti
                    WHERE IdCliente = @customerId";

                return connection.QueryFirstOrDefault<CustomerSurcharges>(query, new { customerId });
            }
        }

        public bool UpdateCustomerInfo(CustomerInfo customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE anagrafica.Clienti 
                    SET 
                        RagioneSociale = @CompanyName,
                        ListinoML = @PriceListType,
                        MaggiorazioneColore = @ColorSurcharge,
                        MaggiorazioneColore_Catini = @ColorSurchargeCatini,
                        MaggiorazioneNero = @BlackSurcharge,
                        MaggiorazioneNero_Catini = @BlackSurchargeCatini,
                        LimiteProfonditaCalcoloPrezzoTVI = @DepthLimitForTVI,
                        MaggiorazioneProfonditaMassima = @MaxDepthSurcharge,
                        ScontoGenerale = @GeneralDiscount,
                        DataAumenti = @PriceIncreaseDate,
                        Aumenti2022Flag = @Increases2022Flag,
                        DataUltimaModifica = GETDATE()
                    WHERE IdCliente = @CustomerId";

                var rowsAffected = connection.Execute(query, customer);
                return rowsAffected > 0;
            }
        }
    }
}