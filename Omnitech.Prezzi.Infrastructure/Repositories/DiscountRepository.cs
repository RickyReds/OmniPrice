using Dapper;
using Omnitech.Prezzi.Core.Repositories;
using Omnitech.Prezzi.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Omnitech.Prezzi.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly string _connectionString;

        public DiscountRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<CustomerDiscount> GetCustomerDiscounts(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        sc.Descrizione as Description,
                        sc.Percentuale as Percentage,
                        sc.IdMateriale as Material,
                        sc.IdTipologiaArticolo as ArticleType,
                        sc.DimensioneMinima as MinDimension
                    FROM sconti.ScontiCliente sc
                    WHERE sc.IdCliente = @customerId 
                        AND sc.Attivo = 1
                        AND (sc.DataInizio IS NULL OR sc.DataInizio <= GETDATE())
                        AND (sc.DataFine IS NULL OR sc.DataFine >= GETDATE())";

                return connection.Query<CustomerDiscount>(query, new { customerId }).ToList();
            }
        }

        public List<CustomerDiscount> GetCustomerDiscounts(int customerId, DateTime orderDate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        sc.Descrizione as Description,
                        sc.Percentuale as Percentage,
                        sc.IdMateriale as Material,
                        sc.IdTipologiaArticolo as ArticleType,
                        sc.DimensioneMinima as MinDimension
                    FROM sconti.ScontiCliente sc
                    WHERE sc.IdCliente = @customerId 
                        AND sc.Attivo = 1
                        AND (sc.DataInizio IS NULL OR sc.DataInizio <= @orderDate)
                        AND (sc.DataFine IS NULL OR sc.DataFine >= @orderDate)";

                return connection.Query<CustomerDiscount>(query, new { customerId, orderDate }).ToList();
            }
        }

        public List<GeneralDiscount> GetGeneralDiscounts()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        IdSconto as Id,
                        Codice as Code,
                        Descrizione as Description,
                        Percentuale as Percentage,
                        DataInizio as ValidFrom,
                        DataFine as ValidTo,
                        Attivo as IsActive
                    FROM sconti.ScontiGenerali
                    WHERE Attivo = 1
                        AND (DataInizio IS NULL OR DataInizio <= GETDATE())
                        AND (DataFine IS NULL OR DataFine >= GETDATE())";

                return connection.Query<GeneralDiscount>(query).ToList();
            }
        }

        public QuantityDiscount GetQuantityDiscount(int customerId, int quantity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT TOP 1
                        sq.QuantitaMinima as MinQuantity,
                        sq.QuantitaMassima as MaxQuantity,
                        sq.PercentualeSconto as DiscountPercentage,
                        sq.Descrizione as Description
                    FROM sconti.ScontiQuantita sq
                    WHERE sq.IdCliente = @customerId
                        AND sq.QuantitaMinima <= @quantity
                        AND (sq.QuantitaMassima IS NULL OR sq.QuantitaMassima >= @quantity)
                        AND sq.Attivo = 1
                    ORDER BY sq.PercentualeSconto DESC";

                return connection.QueryFirstOrDefault<QuantityDiscount>(query, new { customerId, quantity });
            }
        }

        public bool IsDiscountActive(int customerId, string discountCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(1)
                    FROM sconti.ScontiCliente sc
                    WHERE sc.IdCliente = @customerId 
                        AND sc.CodiceSconto = @discountCode
                        AND sc.Attivo = 1
                        AND (sc.DataInizio IS NULL OR sc.DataInizio <= GETDATE())
                        AND (sc.DataFine IS NULL OR sc.DataFine >= GETDATE())";

                return connection.QuerySingle<int>(query, new { customerId, discountCode }) > 0;
            }
        }

        public bool SaveCustomerDiscount(int customerId, CustomerDiscount discount)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO sconti.ScontiCliente (
                        IdCliente, Descrizione, Percentuale, 
                        IdMateriale, IdTipologiaArticolo, DimensioneMinima,
                        DataInizio, Attivo, DataInserimento
                    ) VALUES (
                        @customerId, @Description, @Percentage,
                        @Material, @ArticleType, @MinDimension,
                        GETDATE(), 1, GETDATE()
                    )";

                var parameters = new
                {
                    customerId,
                    discount.Description,
                    discount.Percentage,
                    Material = discount.Material.HasValue ? (int?)discount.Material.Value : null,
                    ArticleType = discount.ArticleType.HasValue ? (int?)discount.ArticleType.Value : null,
                    discount.MinDimension
                };

                var rowsAffected = connection.Execute(query, parameters);
                return rowsAffected > 0;
            }
        }

        public bool RemoveCustomerDiscount(int customerId, int discountId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Soft delete - marca come non attivo invece di eliminare fisicamente
                var query = @"
                    UPDATE sconti.ScontiCliente 
                    SET 
                        Attivo = 0,
                        DataFine = GETDATE(),
                        DataUltimaModifica = GETDATE()
                    WHERE IdCliente = @customerId AND IdSconto = @discountId";

                var rowsAffected = connection.Execute(query, new { customerId, discountId });
                return rowsAffected > 0;
            }
        }
    }
}