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
                        o.Barcode,
                        o.IdCliente as CustomerId,
                        o.IdMateriale as Material,
                        o.IdTexture as Texture,
                        o.CodiceRAL as RALCode,
                        o.DimensioniL as Length,
                        o.DimensioniP as Depth,
                        o.DimensioniH as Height,
                        o.IdStampo as StampId,
                        o.TipologiaArticoloMaster as MainArticleType,
                        o.NumeroVasche as NumberOfBaths,
                        o.ImballoRobusto as RobustPackaging,
                        o.IsContract,
                        o.Modello as Model,
                        o.DataOrdine as OrderDate,
                        o.Prezzo as ExistingPrice,
                        o.PrezzoAutomatico as AutomaticPrice,
                        o.Ecomalta as IsEcomalta,
                        o.CostoEcomalta as EcomaltaCost,
                        o.LastraPiuVasca as IsSheetPlusBath,
                        o.CambioProfondita as DepthChange,
                        o.CodiceArticoloCliente as CustomerArticleCode,
                        o.NAVOrderNumber,
                        o.NAVRowNumber,
                        o.ToNAV,
                        o.Magazzino as IsWarehouse,
                        s.Stampo as StampCode,
                        s.IdStampoPadre as ParentStampId
                    FROM ordini.Ordini o
                    LEFT JOIN anagrafica.Stampi s ON o.IdStampo = s.IdStampo
                    WHERE o.Barcode = @barcode";

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
                        o.Barcode,
                        o.IdCliente as CustomerId,
                        o.IdMateriale as Material,
                        o.IdTexture as Texture,
                        o.CodiceRAL as RALCode,
                        o.DimensioniL as Length,
                        o.DimensioniP as Depth,
                        o.DimensioniH as Height,
                        o.IdStampo as StampId,
                        o.TipologiaArticoloMaster as MainArticleType,
                        o.NumeroVasche as NumberOfBaths,
                        o.ImballoRobusto as RobustPackaging,
                        o.IsContract,
                        o.Modello as Model,
                        o.DataOrdine as OrderDate,
                        o.Prezzo as ExistingPrice,
                        o.PrezzoAutomatico as AutomaticPrice,
                        o.Ecomalta as IsEcomalta,
                        o.CostoEcomalta as EcomaltaCost,
                        o.LastraPiuVasca as IsSheetPlusBath,
                        o.CambioProfondita as DepthChange,
                        o.CodiceArticoloCliente as CustomerArticleCode,
                        o.NAVOrderNumber,
                        o.NAVRowNumber,
                        o.ToNAV,
                        o.Magazzino as IsWarehouse,
                        s.Stampo as StampCode,
                        s.IdStampoPadre as ParentStampId
                    FROM ordini.Ordini o
                    LEFT JOIN anagrafica.Stampi s ON o.IdStampo = s.IdStampo
                    WHERE o.IdCliente = @customerId
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
                        o.Barcode,
                        o.IdCliente as CustomerId,
                        o.IdMateriale as Material,
                        o.IdTexture as Texture,
                        o.CodiceRAL as RALCode,
                        o.DimensioniL as Length,
                        o.DimensioniP as Depth,
                        o.DimensioniH as Height,
                        o.IdStampo as StampId,
                        o.TipologiaArticoloMaster as MainArticleType,
                        o.NumeroVasche as NumberOfBaths,
                        o.ImballoRobusto as RobustPackaging,
                        o.IsContract,
                        o.Modello as Model,
                        o.DataOrdine as OrderDate,
                        o.Prezzo as ExistingPrice,
                        o.PrezzoAutomatico as AutomaticPrice,
                        o.Ecomalta as IsEcomalta,
                        o.CostoEcomalta as EcomaltaCost,
                        o.LastraPiuVasca as IsSheetPlusBath,
                        o.CambioProfondita as DepthChange,
                        o.CodiceArticoloCliente as CustomerArticleCode,
                        o.NAVOrderNumber,
                        o.NAVRowNumber,
                        o.ToNAV,
                        o.Magazzino as IsWarehouse,
                        s.Stampo as StampCode,
                        s.IdStampoPadre as ParentStampId
                    FROM ordini.Ordini o
                    LEFT JOIN anagrafica.Stampi s ON o.IdStampo = s.IdStampo
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
                        IdStampo as StampId,
                        Stampo as StampCode,
                        IdStampoPadre as ParentStampId,
                        LunghezzaMax as MaxLength,
                        ProfonditaMax as MaxDepth,
                        AltezzaMax as MaxHeight,
                        CategoriaArticolo as ArticleCategory
                    FROM anagrafica.Stampi
                    WHERE IdStampo = @stampId";

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
                        IdStampo as StampId,
                        Stampo as StampCode,
                        IdStampoPadre as ParentStampId,
                        LunghezzaMax as MaxLength,
                        ProfonditaMax as MaxDepth,
                        AltezzaMax as MaxHeight,
                        CategoriaArticolo as ArticleCategory
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
                        Barcode, IdCliente, IdMateriale, IdTexture, CodiceRAL,
                        DimensioniL, DimensioniP, DimensioniH, IdStampo,
                        TipologiaArticoloMaster, NumeroVasche, ImballoRobusto,
                        IsContract, Modello, DataOrdine, Prezzo, PrezzoAutomatico,
                        Ecomalta, CostoEcomalta, LastraPiuVasca, CambioProfondita,
                        CodiceArticoloCliente, NAVOrderNumber, NAVRowNumber,
                        ToNAV, Magazzino, DataInserimento
                    ) VALUES (
                        @Barcode, @CustomerId, @Material, @Texture, @RALCode,
                        @Length, @Depth, @Height, @StampId,
                        @MainArticleType, @NumberOfBaths, @RobustPackaging,
                        @IsContract, @Model, @OrderDate, @ExistingPrice, @AutomaticPrice,
                        @IsEcomalta, @EcomaltaCost, @IsSheetPlusBath, @DepthChange,
                        @CustomerArticleCode, @NAVOrderNumber, @NAVRowNumber,
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
                        DataUltimaModifica = GETDATE()
                    WHERE Barcode = @barcode";

                var rowsAffected = connection.Execute(query, new { barcode, price });
                return rowsAffected > 0;
            }
        }

        public bool OrderExists(string barcode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT COUNT(1) FROM ordini.Ordini WHERE Barcode = @barcode";
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
                        o.Barcode,
                        o.IdCliente as CustomerId,
                        o.IdMateriale as Material,
                        o.IdTexture as Texture,
                        o.CodiceRAL as RALCode,
                        o.DimensioniL as Length,
                        o.DimensioniP as Depth,
                        o.DimensioniH as Height,
                        o.IdStampo as StampId,
                        o.TipologiaArticoloMaster as MainArticleType,
                        o.NumeroVasche as NumberOfBaths,
                        o.ImballoRobusto as RobustPackaging,
                        o.IsContract,
                        o.Modello as Model,
                        o.DataOrdine as OrderDate,
                        o.Prezzo as ExistingPrice,
                        o.PrezzoAutomatico as AutomaticPrice,
                        o.Ecomalta as IsEcomalta,
                        o.CostoEcomalta as EcomaltaCost,
                        o.LastraPiuVasca as IsSheetPlusBath,
                        o.CambioProfondita as DepthChange,
                        o.CodiceArticoloCliente as CustomerArticleCode,
                        o.NAVOrderNumber,
                        o.NAVRowNumber,
                        o.ToNAV,
                        o.Magazzino as IsWarehouse,
                        s.Stampo as StampCode,
                        s.IdStampoPadre as ParentStampId
                    FROM ordini.Ordini o
                    LEFT JOIN anagrafica.Stampi s ON o.IdStampo = s.IdStampo
                    WHERE o.ToNAV = 1 AND o.NAVOrderNumber IS NULL
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
                        NAVOrderNumber = @navOrderNumber,
                        NAVRowNumber = @navRowNumber,
                        DataSincronizzazioneNAV = GETDATE()
                    WHERE Barcode = @barcode";

                var rowsAffected = connection.Execute(query, new { barcode, navOrderNumber, navRowNumber });
                return rowsAffected > 0;
            }
        }

        private Order MapToOrder(dynamic data)
        {
            var order = new Order
            {
                Barcode = data.Barcode,
                CustomerId = data.CustomerId,
                Material = (Material)data.Material,
                Texture = data.Texture != null ? (Texture?)data.Texture : null,
                RALCode = data.RALCode,
                MainArticleType = (ArticleType)data.MainArticleType,
                NumberOfBaths = data.NumberOfBaths ?? 1,
                RobustPackaging = data.RobustPackaging ?? false,
                IsContract = data.IsContract ?? false,
                Model = data.Model,
                OrderDate = data.OrderDate,
                ExistingPrice = data.ExistingPrice,
                AutomaticPrice = data.AutomaticPrice,
                IsEcomalta = data.IsEcomalta ?? false,
                EcomaltaCost = data.EcomaltaCost ?? 0,
                IsSheetPlusBath = data.IsSheetPlusBath ?? false,
                DepthChange = data.DepthChange ?? false,
                CustomerArticleCode = data.CustomerArticleCode,
                NAVOrderNumber = data.NAVOrderNumber,
                NAVRowNumber = data.NAVRowNumber,
                ToNAV = data.ToNAV ?? false,
                IsWarehouse = data.IsWarehouse ?? false
            };

            // Imposta dimensioni
            order.Dimensions = new Dimensions
            {
                Length = data.Length ?? 0,
                Depth = data.Depth ?? 0,
                Height = data.Height ?? 0
            };

            // Imposta stampo se presente
            if (data.StampId != null)
            {
                order.Stamp = new Stamp
                {
                    StampId = data.StampId,
                    StampCode = data.StampCode,
                    ParentStampId = data.ParentStampId
                };
            }

            // Inizializza lista tipologie articolo
            order.ArticleTypes = new List<ArticleType> { order.MainArticleType };

            return order;
        }
    }
}