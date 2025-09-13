using Omnitech.Prezzi.Core.Models;
using Omnitech.Prezzi.Core.Repositories;
using Omnitech.Prezzi.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace Omnitech.Prezzi.Infrastructure.Repositories
{
    public class QueryManagerOrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private QueryManager _queryManager;
        public string LastExecutedQuery { get; private set; }

        public QueryManagerOrderRepository(string connectionString)
        {
            _connectionString = connectionString;
            _queryManager = new QueryManager();
        }

        public Order GetOrderByBarcode(string barcode)
        {
            try
            {
                // Debug: Log entry
                System.IO.File.AppendAllText(@"C:\WebApiLog\OrderRepositoryDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] GetOrderByBarcode called for: {barcode}\n");

                _queryManager.IdQuery = 516;
                bool success = _queryManager.GetQuery(barcode);
                
                // Store the actual executed query for API response
                LastExecutedQuery = _queryManager.ExecutedQuery;
                
                if (!success || _queryManager.DR == null)
                {
                    return null;
                }

                return MapQueryResultToOrder(_queryManager.DR, barcode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving order by barcode {barcode}: {ex.Message}", ex);
            }
        }

        private Order MapQueryResultToOrder(DataRow row, string barcode)
        {
            try
            {
                var order = new Order
                {
                    Barcode = barcode,
                    CustomerId = GetSafeValue<int>(row, "idCliente"),
                    
                    // Dimensioni - use the actual column names from query 516
                    Dimensions = new Dimensions
                    {
                        Length = GetSafeValue<double>(row, "L"),
                        Height = GetSafeValue<double>(row, "H"),
                        Depth = GetSafeValue<double>(row, "P")
                    },

                    // Materiale e Texture - map from IDs
                    Material = MapMaterial(GetSafeValue<int>(row, "idMateriale").ToString()),
                    Texture = MapTexture(GetSafeValue<int>(row, "idTexture").ToString()),

                    // Stampo
                    Stamp = new Stamp
                    {
                        StampId = GetSafeValue<int>(row, "idStampo"),
                        StampCode = GetSafeValue<string>(row, "CodiceStampo"),
                        ArticleCategory = GetSafeValue<string>(row, "CategoriaArticolo")
                    },

                    // Prezzi esistenti
                    ExistingPrice = GetSafeValue<decimal>(row, "Prezzo"),
                    AutomaticPrice = GetSafeValue<decimal>(row, "PrezzoAutomatico"),

                    // Codice RAL from idRAL
                    RALCode = GetSafeValue<int>(row, "idRAL").ToString(),

                    // Date - use actual column names
                    OrderDate = GetSafeValue<DateTime>(row, "DataOrdine"),

                    // Modello
                    Model = GetSafeValue<string>(row, "Modello"),

                    // Customer Article Code
                    CustomerArticleCode = GetSafeValue<string>(row, "CodiceCliente")
                };

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error mapping query result to Order: {ex.Message}", ex);
            }
        }

        private T GetSafeValue<T>(DataRow row, string columnName)
        {
            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                {
                    return (T)Convert.ChangeType(row[columnName], typeof(T));
                }
                return default(T);
            }
            catch
            {
                return default(T);
            }
        }

        private Material MapMaterial(string materialString)
        {
            if (string.IsNullOrEmpty(materialString))
                return Material.Undefined;

            // Mappa i valori stringa ai valori enum (basato su ID materiale)
            if (int.TryParse(materialString, out int materialId))
            {
                return (Material)materialId;
            }

            // Fallback su stringa se non è un numero
            switch (materialString.ToUpper())
            {
                case "TECNORIL":
                    return Material.Tecnoril;
                case "GEACRIL":
                    return Material.Geacril;
                case "ECOMALTA":
                    return Material.Ecomalta;
                default:
                    return Material.Undefined;
            }
        }

        private Texture MapTexture(string textureString)
        {
            if (string.IsNullOrEmpty(textureString))
                return Texture.Undefined;

            // Mappa i valori stringa ai valori enum (basato su ID texture)
            if (int.TryParse(textureString, out int textureId))
            {
                return (Texture)textureId;
            }

            // Fallback su stringa se non è un numero
            switch (textureString.ToUpper())
            {
                case "LISCIA":
                    return Texture.Liscia;
                case "RIGATA":
                    return Texture.Rigata;
                case "ANTISCIVOLO":
                    return Texture.Antiscivolo;
                case "SLATE":
                    return Texture.Slate;
                case "STONE":
                    return Texture.Stone;
                default:
                    return Texture.Undefined;
            }
        }

        public bool UpdateOrderPrice(string barcode, decimal newPrice)
        {
            // Per ora implementazione vuota - il prezzo viene aggiornato dal sistema PsR
            // Se necessario, implementare logica di aggiornamento
            return true;
        }

        public Stamp GetStampInfo(int stampId)
        {
            // Le informazioni del stampo dovrebbero già essere incluse nella query 516
            // Se necessario, implementare query specifica per stampo
            return new Stamp { StampId = stampId };
        }

        public List<Order> GetCustomerOrders(int customerId)
        {
            // Implementazione base - restituisce lista vuota
            return new List<Order>();
        }

        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            // Implementazione base - restituisce lista vuota
            return new List<Order>();
        }

        public Stamp GetStampByCode(string stampCode)
        {
            // Implementazione base - restituisce null
            return null;
        }

        public bool SaveOrder(Order order)
        {
            // Implementazione base - restituisce true
            return true;
        }

        public bool OrderExists(string barcode)
        {
            // Implementazione base usando GetOrderByBarcode
            return GetOrderByBarcode(barcode) != null;
        }

        public int GetCustomerOrderCount(int customerId)
        {
            // Implementazione base - restituisce 0
            return 0;
        }

        public List<Order> GetOrdersToSyncWithNAV()
        {
            // Implementazione base - restituisce lista vuota
            return new List<Order>();
        }

        public bool MarkOrderAsSynced(string barcode, string navOrderNo, string syncDateTime)
        {
            // Implementazione base - restituisce true
            return true;
        }
    }
}