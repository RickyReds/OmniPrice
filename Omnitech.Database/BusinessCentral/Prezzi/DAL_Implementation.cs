using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Omnitech.DTO.BusinessCentral.Alexide;

namespace Omnitech.Database.BusinessCentral.Prezzi
{
    public class DAL_Implementation
    {
        public static decimal GetBasePrice(string connectionString, string customerNo, string productCategory = null)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CustomerNo", customerNo);
                parameters.Add("@ProductCategory", productCategory);
                parameters.Add("@BasePrice", dbType: System.Data.DbType.Decimal, direction: System.Data.ParameterDirection.Output, precision: 10, scale: 2);

                db.Execute("sp_CalculatePrice", parameters, commandType: CommandType.StoredProcedure);
                
                return parameters.Get<decimal>("@BasePrice");
            }
        }

        public static List<PriceRule> GetPriceRules(string connectionString, string customerNo)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT Id, CustomerNo, ProductCategory, BasePrice, DiscountPercentage, 
                           ValidFrom, ValidTo, IsActive
                    FROM PriceRules
                    WHERE CustomerNo = @CustomerNo AND IsActive = 1
                    ORDER BY ProductCategory";

                return db.Query<PriceRule>(sql, new { CustomerNo = customerNo }).ToList();
            }
        }

        public static List<PriceFactor> GetPriceFactors(string connectionString)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT Id, FactorName, FactorType, MultiplierValue, AdditiveValue
                    FROM PriceFactors
                    WHERE IsActive = 1";

                return db.Query<PriceFactor>(sql).ToList();
            }
        }

        public static void SavePriceHistory(string connectionString, string customerNo, decimal calculatedPrice, ConfigItem item, string ipAddress = null)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO PriceHistory (CustomerNo, ItemCode, CalculatedPrice, RequestData, IPAddress)
                    VALUES (@CustomerNo, @ItemCode, @CalculatedPrice, @RequestData, @IPAddress)";

                var itemJson = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                
                db.Execute(sql, new
                {
                    CustomerNo = customerNo,
                    ItemCode = item?.FamigliaProdotto ?? "UNKNOWN",
                    CalculatedPrice = calculatedPrice,
                    RequestData = itemJson,
                    IPAddress = ipAddress
                });
            }
        }

        public static bool TestConnection(string connectionString)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    var result = db.QuerySingle<int>("SELECT 1");
                    return result == 1;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public class PriceRule
    {
        public int Id { get; set; }
        public string CustomerNo { get; set; }
        public string ProductCategory { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
    }

    public class PriceFactor
    {
        public int Id { get; set; }
        public string FactorName { get; set; }
        public string FactorType { get; set; }
        public decimal MultiplierValue { get; set; }
        public decimal AdditiveValue { get; set; }
    }
}