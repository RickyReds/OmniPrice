using Dapper;
using Omnitech.Prezzi.Core.Enums;
using Omnitech.Prezzi.Core.Models;
using Omnitech.Prezzi.Core.Repositories;
using Omnitech.Prezzi.Core.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Omnitech.Prezzi.Infrastructure.Repositories
{
    public class PriceRepository : IPriceRepository
    {
        private readonly string _connectionString;

        public PriceRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public string GetCustomPriceListTable(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT TabellaPrezzi 
                    FROM anagrafica.ListiniPersonalizzati 
                    WHERE IdCliente = @customerId AND Attivo = 1";

                return connection.QueryFirstOrDefault<string>(query, new { customerId });
            }
        }

        public PriceData GetIntegratedTopPrice(string tableName, string stampCode, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Query basata sulla logica di cListini.vb
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        ISNULL(Sconto, 0) as Discount
                    FROM {tableName}
                    WHERE Stampo = @stampCode
                        AND Materiale = @material
                        AND Lunghezza >= @length - @tolerance
                        AND Lunghezza <= @length + @tolerance
                        AND Profondita >= @depth - @tolerance
                        AND Profondita <= @depth + @tolerance
                        AND TipoArticolo = 'TVI'";

                var parameters = new
                {
                    stampCode,
                    material = (int)material,
                    length = dimensions.Length,
                    depth = dimensions.Depth,
                    tolerance = 5 // tolleranza standard di 5mm
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetShowerTrayPrice(string tableName, string stampCode, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        ISNULL(Sconto, 0) as Discount
                    FROM {tableName}
                    WHERE Stampo = @stampCode
                        AND Materiale = @material
                        AND Lunghezza >= @length - @tolerance
                        AND Lunghezza <= @length + @tolerance
                        AND Profondita >= @depth - @tolerance
                        AND Profondita <= @depth + @tolerance
                        AND TipoArticolo = 'PD'";

                var parameters = new
                {
                    stampCode,
                    material = (int)material,
                    length = dimensions.Length,
                    depth = dimensions.Depth,
                    tolerance = 5
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetBathtubPrice(string tableName, string stampCode, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        ISNULL(Sconto, 0) as Discount
                    FROM {tableName}
                    WHERE Stampo = @stampCode
                        AND Materiale = @material
                        AND Lunghezza >= @length - @tolerance
                        AND Lunghezza <= @length + @tolerance
                        AND Profondita >= @depth - @tolerance
                        AND Profondita <= @depth + @tolerance
                        AND TipoArticolo = 'VB'";

                var parameters = new
                {
                    stampCode,
                    material = (int)material,
                    length = dimensions.Length,
                    depth = dimensions.Depth,
                    tolerance = 5
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetSheetPrice(string tableName, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        ISNULL(Sconto, 0) as Discount
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND Lunghezza >= @length - @tolerance
                        AND Lunghezza <= @length + @tolerance
                        AND Profondita >= @depth - @tolerance
                        AND Profondita <= @depth + @tolerance
                        AND TipoArticolo = 'LASTRA'";

                var parameters = new
                {
                    material = (int)material,
                    length = dimensions.Length,
                    depth = dimensions.Depth,
                    tolerance = 5
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public double GetSheetPricePerSqm(string tableName, Material material)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT PrezzoMQ 
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND TipoArticolo = 'LASTRA_MQ'";

                return connection.QueryFirstOrDefault<double>(query, new { material = (int)material });
            }
        }

        public PriceData GetBoxingPrice(string tableName, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Calcolo scatolatura basato su perimetro
                var perimeter = 2 * (dimensions.Length + dimensions.Depth);
                
                var query = $@"
                    SELECT 
                        PrezzoML * @perimeter / 1000 as Price,
                        ISNULL(Sconto, 0) as Discount
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND TipoArticolo = 'SCATOLATURA'";

                var parameters = new
                {
                    material = (int)material,
                    perimeter
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetMillingPrice(string tableName, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var perimeter = 2 * (dimensions.Length + dimensions.Depth);
                
                var query = $@"
                    SELECT 
                        PrezzoML * @perimeter / 1000 as Price,
                        ISNULL(Sconto, 0) as Discount
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND TipoArticolo = 'FRESATA'";

                var parameters = new
                {
                    material = (int)material,
                    perimeter
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetLEDMillingPrice(string tableName, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var perimeter = 2 * (dimensions.Length + dimensions.Depth);
                
                var query = $@"
                    SELECT 
                        PrezzoML * @perimeter / 1000 as Price,
                        0 as Discount
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND TipoArticolo = 'FRESATA_LED'";

                var parameters = new
                {
                    material = (int)material,
                    perimeter
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetBasinPrice(string tableName, string stampCode, Material material)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        0 as Discount
                    FROM {tableName}
                    WHERE Stampo = @stampCode
                        AND Materiale = @material
                        AND TipoArticolo = 'CATINO'";

                var parameters = new
                {
                    stampCode,
                    material = (int)material
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetConsolePrice(string tableName, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        0 as Discount
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND Lunghezza >= @length - @tolerance
                        AND Lunghezza <= @length + @tolerance
                        AND TipoArticolo = 'CONSOLLE'";

                var parameters = new
                {
                    material = (int)material,
                    length = dimensions.Length,
                    tolerance = 5
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetColumnPrice(string tableName, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        0 as Discount
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND Altezza >= @height - @tolerance
                        AND Altezza <= @height + @tolerance
                        AND TipoArticolo = 'COLONNA'";

                var parameters = new
                {
                    material = (int)material,
                    height = dimensions.Height,
                    tolerance = 5
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetWoodenBoxPrice(string tableName, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        ISNULL(Sconto, 0) as Discount
                    FROM {tableName}
                    WHERE Lunghezza >= @length - @tolerance
                        AND Lunghezza <= @length + @tolerance
                        AND Profondita >= @depth - @tolerance
                        AND Profondita <= @depth + @tolerance
                        AND TipoArticolo = 'CASSA_LEGNO'";

                var parameters = new
                {
                    length = dimensions.Length,
                    depth = dimensions.Depth,
                    tolerance = 10
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }

        public PriceData GetTemplatePrice(string tableName, string stampCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        ISNULL(Sconto, 0) as Discount
                    FROM {tableName}
                    WHERE Stampo = @stampCode
                        AND TipoArticolo = 'DIMA'";

                return connection.QueryFirstOrDefault<PriceData>(query, new { stampCode });
            }
        }

        public PriceData GetDripSaverPrice(string tableName, Material material)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT 
                        Prezzo as Price,
                        0 as Discount
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND TipoArticolo = 'SALVAGOCCIA'";

                return connection.QueryFirstOrDefault<PriceData>(query, new { material = (int)material });
            }
        }

        public PriceData GetContainmentEdgePrice(string tableName, Material material, Dimensions dimensions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var perimeter = 2 * (dimensions.Length + dimensions.Depth);
                
                var query = $@"
                    SELECT 
                        PrezzoML * @perimeter / 1000 as Price,
                        0 as Discount
                    FROM {tableName}
                    WHERE Materiale = @material
                        AND TipoArticolo = 'BORDO_CONTENITIVO'";

                var parameters = new
                {
                    material = (int)material,
                    perimeter
                };

                return connection.QueryFirstOrDefault<PriceData>(query, parameters);
            }
        }
    }
}