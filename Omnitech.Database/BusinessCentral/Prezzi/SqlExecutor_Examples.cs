using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Omnitech.Database.BusinessCentral.Prezzi
{
    /// <summary>
    /// Esempi di utilizzo di SqlExecutor
    /// </summary>
    public static class SqlExecutor_Examples
    {
        /// <summary>
        /// Esempi di query SELECT
        /// </summary>
        public static class SelectExamples
        {
            // Esempio 0;
            public static string GetBC(string connectionString)
            {
                string sql = "SELECT TOP(1) Barcode FROM ordini.ordini ORDER BY DataOrdine DESC";
                    return SqlExecutor.ExecuteScalar<string>(connectionString, sql);
            }

            // Esempio 1: Query semplice senza parametri
            public static decimal GetMaxPrice(string connectionString)
            {
                string sql = "SELECT MAX(BasePrice) FROM PriceRules";
                return SqlExecutor.ExecuteScalar<decimal>(connectionString, sql);
            }

            // Esempio 2: Query con parametri
            public static List<dynamic> GetCustomerPrices(string connectionString, string customerNo)
            {
                string sql = @"
                    SELECT CustomerNo, ProductCategory, BasePrice, DiscountPercentage
                    FROM PriceRules 
                    WHERE CustomerNo = @CustomerNo AND IsActive = 1";
                
                return SqlExecutor.ExecuteQuery(connectionString, sql, new { CustomerNo = customerNo });
            }

            // Esempio 3: Query tipizzata
            public static List<PriceInfo> GetPricesByCategory(string connectionString, string category)
            {
                string sql = @"
                    SELECT CustomerNo, BasePrice, DiscountPercentage
                    FROM PriceRules 
                    WHERE ProductCategory = @Category";
                
                return SqlExecutor.ExecuteQuery<PriceInfo>(connectionString, sql, new { Category = category });
            }

            // Esempio 4: Query con JOIN
            public static List<dynamic> GetPricesWithFactors(string connectionString)
            {
                string sql = @"
                    SELECT 
                        pr.CustomerNo,
                        pr.BasePrice,
                        pf.FactorName,
                        pf.MultiplierValue
                    FROM PriceRules pr
                    CROSS JOIN PriceFactors pf
                    WHERE pr.IsActive = 1 AND pf.IsActive = 1";
                
                return SqlExecutor.ExecuteQuery(connectionString, sql);
            }

            // Esempio 5: Query con aggregazione
            public static dynamic GetPriceStatistics(string connectionString, string customerNo)
            {
                string sql = @"
                    SELECT 
                        COUNT(*) as TotalRules,
                        AVG(BasePrice) as AveragePrice,
                        MIN(BasePrice) as MinPrice,
                        MAX(BasePrice) as MaxPrice
                    FROM PriceRules
                    WHERE CustomerNo = @CustomerNo";
                
                var result = SqlExecutor.ExecuteQuery(connectionString, sql, new { CustomerNo = customerNo });
                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// Esempi di query INSERT/UPDATE/DELETE
        /// </summary>
        public static class ModifyExamples
        {
            // Esempio 1: INSERT semplice
            public static int InsertPriceRule(string connectionString, string customerNo, decimal basePrice)
            {
                string sql = @"
                    INSERT INTO PriceRules (CustomerNo, BasePrice, CreatedDate)
                    VALUES (@CustomerNo, @BasePrice, GETDATE())";
                
                return SqlExecutor.ExecuteNonQuery(connectionString, sql, 
                    new { CustomerNo = customerNo, BasePrice = basePrice });
            }

            // Esempio 2: INSERT con ritorno ID
            public static int InsertPriceRuleWithId(string connectionString, string customerNo, decimal basePrice)
            {
                string sql = @"
                    INSERT INTO PriceRules (CustomerNo, BasePrice, CreatedDate)
                    VALUES (@CustomerNo, @BasePrice, GETDATE())";
                
                return SqlExecutor.ExecuteInsertWithIdentity(connectionString, sql,
                    new { CustomerNo = customerNo, BasePrice = basePrice });
            }

            // Esempio 3: UPDATE
            public static int UpdateCustomerDiscount(string connectionString, string customerNo, decimal discount)
            {
                string sql = @"
                    UPDATE PriceRules 
                    SET DiscountPercentage = @Discount, ModifiedDate = GETDATE()
                    WHERE CustomerNo = @CustomerNo";
                
                return SqlExecutor.ExecuteNonQuery(connectionString, sql,
                    new { CustomerNo = customerNo, Discount = discount });
            }

            // Esempio 4: DELETE
            public static int DeleteInactivePrices(string connectionString)
            {
                string sql = "DELETE FROM PriceRules WHERE IsActive = 0";
                return SqlExecutor.ExecuteNonQuery(connectionString, sql);
            }

            // Esempio 5: MERGE (UPSERT)
            public static int UpsertPriceRule(string connectionString, string customerNo, string category, decimal price)
            {
                string sql = @"
                    MERGE PriceRules AS target
                    USING (SELECT @CustomerNo as CustomerNo, @Category as ProductCategory) AS source
                    ON target.CustomerNo = source.CustomerNo AND target.ProductCategory = source.ProductCategory
                    WHEN MATCHED THEN
                        UPDATE SET BasePrice = @Price, ModifiedDate = GETDATE()
                    WHEN NOT MATCHED THEN
                        INSERT (CustomerNo, ProductCategory, BasePrice, CreatedDate)
                        VALUES (@CustomerNo, @Category, @Price, GETDATE());";
                
                return SqlExecutor.ExecuteNonQuery(connectionString, sql,
                    new { CustomerNo = customerNo, Category = category, Price = price });
            }
        }

        /// <summary>
        /// Esempi con transazioni
        /// </summary>
        public static class TransactionExamples
        {
            // Esempio: Aggiornamento multiplo in transazione
            public static bool UpdateMultiplePrices(string connectionString, string customerNo, decimal increasePercent)
            {
                var commands = new List<SqlCommand>
                {
                    new SqlCommand 
                    { 
                        Query = "UPDATE PriceRules SET BasePrice = BasePrice * @Factor WHERE CustomerNo = @CustomerNo",
                        Parameters = new { CustomerNo = customerNo, Factor = 1 + increasePercent/100 }
                    },
                    new SqlCommand
                    {
                        Query = "INSERT INTO PriceHistory (CustomerNo, CalculatedPrice, CalculationDate) VALUES (@CustomerNo, 0, GETDATE())",
                        Parameters = new { CustomerNo = customerNo }
                    }
                };

                return SqlExecutor.ExecuteTransaction(connectionString, commands);
            }
        }

        /// <summary>
        /// Esempi con Stored Procedures
        /// </summary>
        public static class StoredProcedureExamples
        {
            // Esempio 1: Chiamata stored procedure con parametro output
            public static decimal CalculatePriceWithSP(string connectionString, string customerNo, string category)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CustomerNo", customerNo);
                parameters.Add("@ProductCategory", category);
                parameters.Add("@BasePrice", dbType: System.Data.DbType.Decimal, direction: System.Data.ParameterDirection.Output, precision: 10, scale: 2);

                SqlExecutor.ExecuteStoredProcedureWithOutput(connectionString, "sp_CalculatePrice", parameters);
                
                return parameters.Get<decimal>("@BasePrice");
            }

            // Esempio 2: Stored procedure che ritorna risultati
            public static List<dynamic> GetPriceHistoryWithSP(string connectionString, int days)
            {
                return SqlExecutor.ExecuteStoredProcedure<dynamic>(connectionString, "sp_GetRecentPrices",
                    new { DaysBack = days });
            }
        }

        /// <summary>
        /// Esempi di utility
        /// </summary>
        public static class UtilityExamples
        {
            // Verifica esistenza tabella
            public static bool CheckTableExists(string connectionString, string tableName)
            {
                return SqlExecutor.TableExists(connectionString, tableName);
            }

            // Ottieni schema tabella
            public static void PrintTableSchema(string connectionString, string tableName)
            {
                var schema = SqlExecutor.GetTableSchema(connectionString, tableName);
                foreach (var col in schema)
                {
                    Console.WriteLine($"{col.ColumnName} - {col.DataType} - Nullable: {col.IsNullable}");
                }
            }
        }

        // Classe di supporto per esempi tipizzati
        public class PriceInfo
        {
            public string CustomerNo { get; set; }
            public decimal BasePrice { get; set; }
            public decimal DiscountPercentage { get; set; }
        }
    }
}