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
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        public string LastExecutedQuery { get; private set; }

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Order GetOrderByBarcode(string barcode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        o.BarCode as Barcode,
                        o.idCliente as CustomerId,
                        o.idMateriale as Material,
                        o.idTexture as Texture,
                        r.RAL as RALCode,
                        o.L as Length,
                        o.P as Depth,
                        o.H as Height,
                        o.idStampo as StampId,
                        ISNULL(s.Tipologia, 1) as MainArticleType,
                        o.NumeroVasche as NumberOfBaths,
                        o.ImballoRobusto as RobustPackaging,
                        o.Contract as IsContract,
                        o.Modello as Model,
                        o.DataOrdine as OrderDate,
                        o.Prezzo as ExistingPrice,
                        o.PrezzoAutomatico as AutomaticPrice,
                        o.Ecomalta as IsEcomalta,
                        o.Ecomalta as EcomaltaCost,
                        o.LastraPiuVascaSaldata as IsSheetPlusBath,
                        o.CambioProfondità as DepthChange,
                        o.CodiceCliente as CustomerArticleCode,
                        o.No_NAV as NAVOrderNumber,
                        NULL as NAVRowNumber,
                        o.toNAV as ToNAV,
                        o.Magazzino as IsWarehouse,
                        s.Stampo as StampCode,
                        s.parentMouldID as ParentStampId
                    FROM ordini.Ordini o
                    LEFT JOIN anagrafica.Stampi s ON o.idStampo = s.idStampo
                    LEFT JOIN anagrafica.ScalaRAL r ON o.idRAL = r.idRAL
                    WHERE o.BarCode = @barcode";

                // Salva la query per debug
                LastExecutedQuery = query.Replace("@barcode", $"'{barcode}'");

                var order = connection.QueryFirstOrDefault<dynamic>(query, new { barcode });
                
                if (order == null) return null;

                return MapToOrder(order);
            }
        }

        public List<Order> GetCustomerOrders(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        o.BarCode as Barcode,
                        o.idCliente as CustomerId,
                        o.idMateriale as Material,
                        o.idTexture as Texture,
                        r.RAL as RALCode,
                        o.L as Length,
                        o.P as Depth,
                        o.H as Height,
                        o.idStampo as StampId,
                        ISNULL(s.Tipologia, 1) as MainArticleType,
                        o.NumeroVasche as NumberOfBaths,
                        o.ImballoRobusto as RobustPackaging,
                        o.Contract as IsContract,
                        o.Modello as Model,
                        o.DataOrdine as OrderDate,
                        o.Prezzo as ExistingPrice,
                        o.PrezzoAutomatico as AutomaticPrice,
                        o.Ecomalta as IsEcomalta,
                        o.Ecomalta as EcomaltaCost,
                        o.LastraPiuVascaSaldata as IsSheetPlusBath,
                        o.CambioProfondità as DepthChange,
                        o.CodiceCliente as CustomerArticleCode,
                        o.No_NAV as NAVOrderNumber,
                        NULL as NAVRowNumber,
                        o.toNAV as ToNAV,
                        o.Magazzino as IsWarehouse,
                        s.Stampo as StampCode,
                        s.parentMouldID as ParentStampId
                    FROM ordini.Ordini o
                    LEFT JOIN anagrafica.Stampi s ON o.idStampo = s.idStampo
                    LEFT JOIN anagrafica.ScalaRAL r ON o.idRAL = r.idRAL
                    WHERE o.idCliente = @customerId
                    ORDER BY o.DataOrdine DESC";

                var orders = connection.Query<dynamic>(query, new { customerId });
                
                return orders.Select(MapToOrder).ToList();
            }
        }

        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        o.BarCode as Barcode,
                        o.idCliente as CustomerId,
                        o.idMateriale as Material,
                        o.idTexture as Texture,
                        r.RAL as RALCode,
                        o.L as Length,
                        o.P as Depth,
                        o.H as Height,
                        o.idStampo as StampId,
                        ISNULL(s.Tipologia, 1) as MainArticleType,
                        o.NumeroVasche as NumberOfBaths,
                        o.ImballoRobusto as RobustPackaging,
                        o.Contract as IsContract,
                        o.Modello as Model,
                        o.DataOrdine as OrderDate,
                        o.Prezzo as ExistingPrice,
                        o.PrezzoAutomatico as AutomaticPrice,
                        o.Ecomalta as IsEcomalta,
                        o.Ecomalta as EcomaltaCost,
                        o.LastraPiuVascaSaldata as IsSheetPlusBath,
                        o.CambioProfondità as DepthChange,
                        o.CodiceCliente as CustomerArticleCode,
                        o.No_NAV as NAVOrderNumber,
                        NULL as NAVRowNumber,
                        o.toNAV as ToNAV,
                        o.Magazzino as IsWarehouse,
                        s.Stampo as StampCode,
                        s.parentMouldID as ParentStampId
                    FROM ordini.Ordini o
                    LEFT JOIN anagrafica.Stampi s ON o.idStampo = s.idStampo
                    LEFT JOIN anagrafica.ScalaRAL r ON o.idRAL = r.idRAL
                    WHERE o.DataOrdine >= @startDate AND o.DataOrdine <= @endDate
                    ORDER BY o.DataOrdine DESC";

                var orders = connection.Query<dynamic>(query, new { startDate, endDate });
                
                return orders.Select(MapToOrder).ToList();
            }
        }

        public Stamp GetStampInfo(int stampId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        idStampo as StampId,
                        Stampo as StampCode,
                        parentMouldID as ParentStampId,
                        NULL as MaxLength,
                        NULL as MaxDepth,
                        NULL as MaxHeight,
                        Tipologia as ArticleCategory
                    FROM anagrafica.Stampi
                    WHERE idStampo = @stampId";

                var stamp = connection.QueryFirstOrDefault<dynamic>(query, new { stampId });
                
                if (stamp == null) return null;

                return new Stamp
                {
                    StampId = stamp.StampId,
                    StampCode = stamp.StampCode,
                    ParentStampId = stamp.ParentStampId,
                    ArticleCategory = stamp.ArticleCategory,
                    MaxDimensions = new Dimensions
                    {
                        Length = stamp.MaxLength ?? 0,
                        Depth = stamp.MaxDepth ?? 0,
                        Height = stamp.MaxHeight ?? 0
                    }
                };
            }
        }

        public Stamp GetStampByCode(string stampCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        idStampo as StampId,
                        Stampo as StampCode,
                        parentMouldID as ParentStampId,
                        NULL as MaxLength,
                        NULL as MaxDepth,
                        NULL as MaxHeight,
                        Tipologia as ArticleCategory
                    FROM anagrafica.Stampi
                    WHERE Stampo = @stampCode";

                var stamp = connection.QueryFirstOrDefault<dynamic>(query, new { stampCode });
                
                if (stamp == null) return null;

                return new Stamp
                {
                    StampId = stamp.StampId,
                    StampCode = stamp.StampCode,
                    ParentStampId = stamp.ParentStampId,
                    ArticleCategory = stamp.ArticleCategory,
                    MaxDimensions = new Dimensions
                    {
                        Length = stamp.MaxLength ?? 0,
                        Depth = stamp.MaxDepth ?? 0,
                        Height = stamp.MaxHeight ?? 0
                    }
                };
            }
        }

        public bool SaveOrder(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO ordini.Ordini (
                        BarCode, idCliente, idMateriale, idTexture, idRAL,
                        L, P, H, idStampo,
                        NumeroVasche, ImballoRobusto,
                        Contract, Modello, DataOrdine, Prezzo, PrezzoAutomatico,
                        Ecomalta, LastraPiuVascaSaldata, CambioProfondità,
                        CodiceCliente, No_NAV,
                        toNAV, Magazzino, DataInserimento
                    ) VALUES (
                        @Barcode, @CustomerId, @Material, @Texture, @RALCode,
                        @Length, @Depth, @Height, @StampId,
                        @NumberOfBaths, @RobustPackaging,
                        @IsContract, @Model, @OrderDate, @ExistingPrice, @AutomaticPrice,
                        @IsEcomalta, @IsSheetPlusBath, @DepthChange,
                        @CustomerArticleCode, @NAVOrderNumber,
                        @ToNAV, @IsWarehouse, GETDATE()
                    )";

                var parameters = new
                {
                    order.Barcode,
                    order.CustomerId,
                    Material = (int)order.Material,
                    Texture = order.Texture.HasValue ? (int?)order.Texture.Value : null,
                    order.RALCode,
                    order.Dimensions.Length,
                    order.Dimensions.Depth,
                    order.Dimensions.Height,
                    StampId = order.Stamp?.StampId,
                    MainArticleType = (int)order.MainArticleType,
                    order.NumberOfBaths,
                    order.RobustPackaging,
                    order.IsContract,
                    order.Model,
                    order.OrderDate,
                    order.ExistingPrice,
                    order.AutomaticPrice,
                    order.IsEcomalta,
                    order.EcomaltaCost,
                    order.IsSheetPlusBath,
                    order.DepthChange,
                    order.CustomerArticleCode,
                    order.NAVOrderNumber,
                    order.NAVRowNumber,
                    order.ToNAV,
                    order.IsWarehouse
                };

                var rowsAffected = connection.Execute(query, parameters);
                return rowsAffected > 0;
            }
        }

        public bool UpdateOrderPrice(string barcode, decimal price)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE ordini.Ordini 
                    SET 
                        PrezzoAutomatico = @price,
                        DataModifica = GETDATE()
                    WHERE Barcode = @barcode";

                var rowsAffected = connection.Execute(query, new { barcode, price });
                return rowsAffected > 0;
            }
        }

        public bool OrderExists(string barcode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT COUNT(1) FROM ordini.Ordini WHERE BarCode = @barcode";
                return connection.QuerySingle<int>(query, new { barcode }) > 0;
            }
        }

        public int GetCustomerOrderCount(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT COUNT(1) FROM ordini.Ordini WHERE IdCliente = @customerId";
                return connection.QuerySingle<int>(query, new { customerId });
            }
        }

        public List<Order> GetOrdersToSyncWithNAV()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        o.BarCode as Barcode,
                        o.idCliente as CustomerId,
                        o.idMateriale as Material,
                        o.idTexture as Texture,
                        r.RAL as RALCode,
                        o.L as Length,
                        o.P as Depth,
                        o.H as Height,
                        o.idStampo as StampId,
                        ISNULL(s.Tipologia, 1) as MainArticleType,
                        o.NumeroVasche as NumberOfBaths,
                        o.ImballoRobusto as RobustPackaging,
                        o.Contract as IsContract,
                        o.Modello as Model,
                        o.DataOrdine as OrderDate,
                        o.Prezzo as ExistingPrice,
                        o.PrezzoAutomatico as AutomaticPrice,
                        o.Ecomalta as IsEcomalta,
                        o.Ecomalta as EcomaltaCost,
                        o.LastraPiuVascaSaldata as IsSheetPlusBath,
                        o.CambioProfondità as DepthChange,
                        o.CodiceCliente as CustomerArticleCode,
                        o.No_NAV as NAVOrderNumber,
                        NULL as NAVRowNumber,
                        o.toNAV as ToNAV,
                        o.Magazzino as IsWarehouse,
                        s.Stampo as StampCode,
                        s.parentMouldID as ParentStampId
                    FROM ordini.Ordini o
                    LEFT JOIN anagrafica.Stampi s ON o.idStampo = s.idStampo
                    LEFT JOIN anagrafica.ScalaRAL r ON o.idRAL = r.idRAL
                    WHERE o.toNAV = 1 AND o.No_NAV IS NULL
                    ORDER BY o.DataOrdine";

                var orders = connection.Query<dynamic>(query);
                
                return orders.Select(MapToOrder).ToList();
            }
        }

        public bool MarkOrderAsSynced(string barcode, string navOrderNumber, string navRowNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE ordini.Ordini 
                    SET 
                        No_NAV = @navOrderNumber,
                        toNAVdata = GETDATE()
                    WHERE BarCode = @barcode";

                var rowsAffected = connection.Execute(query, new { barcode, navOrderNumber, navRowNumber });
                return rowsAffected > 0;
            }
        }

        private Order MapToOrder(dynamic data)
        {
            if (data == null) return null;
            
            var order = new Order
            {
                Barcode = data.Barcode?.ToString()?.Trim(),
                CustomerId = data.CustomerId ?? 0,
                Material = data.Material != null ? (Material)(int)data.Material : Material.Deimos,
                Texture = data.Texture != null ? (Texture?)(int)data.Texture : null,
                RALCode = data.RALCode,
                MainArticleType = data.MainArticleType != null ? (ArticleType)(int)data.MainArticleType : ArticleType.TopVascaIntegrata,
                NumberOfBaths = data.NumberOfBaths ?? 1,
                RobustPackaging = data.RobustPackaging ?? false,
                IsContract = data.IsContract ?? false,
                Model = data.Model?.ToString()?.Trim(),
                OrderDate = data.OrderDate ?? DateTime.Now,
                ExistingPrice = data.ExistingPrice ?? 0m,
                AutomaticPrice = data.AutomaticPrice ?? 0m,
                IsEcomalta = data.IsEcomalta ?? false,
                EcomaltaCost = data.EcomaltaCost ?? 0m,
                IsSheetPlusBath = data.IsSheetPlusBath ?? false,
                DepthChange = data.DepthChange ?? false,
                CustomerArticleCode = data.CustomerArticleCode?.ToString()?.Trim(),
                NAVOrderNumber = data.NAVOrderNumber?.ToString()?.Trim(),
                NAVRowNumber = data.NAVRowNumber?.ToString()?.Trim(),
                ToNAV = data.ToNAV ?? false,
                IsWarehouse = data.IsWarehouse ?? false
            };

            // Imposta dimensioni
            order.Dimensions = new Dimensions
            {
                Length = data.Length ?? 0m,
                Depth = data.Depth ?? 0m,
                Height = data.Height ?? 0m
            };

            // Imposta stampo se presente
            if (data.StampId != null && (int)data.StampId > 0)
            {
                order.Stamp = new Stamp
                {
                    StampId = (int)data.StampId,
                    StampCode = data.StampCode?.ToString()?.Trim(),
                    ParentStampId = data.ParentStampId != null ? (int?)data.ParentStampId : null
                };
            }

            // Inizializza lista tipologie articolo
            order.ArticleTypes = new List<ArticleType> { order.MainArticleType };

            return order;
        }
    }
}