using Omnitech.Prezzi.Core.Models;
using Omnitech.Prezzi.Core.Repositories;
using Omnitech.Prezzi.Core.Services;
using Omnitech.Prezzi.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using HI.Libs.Utility;
using System.Data.SqlClient;

namespace WebApi.Misc.Controllers
{
    /// <summary>
    /// Controller per il calcolo prezzi usando il nuovo sistema Core
    /// </summary>
    [RoutePrefix("api/v2/price")]
    public class PriceCalculationController : ApiController
    {
        private readonly IPriceRepository _priceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly PriceCalculator _priceCalculator;
        
        private readonly string _connectionString;
        private readonly bool _debugMode;
        private readonly string _logFileName;

        public PriceCalculationController()
        {
            try 
            {
                System.IO.File.AppendAllText(@"C:\WebApiLog\ConstructorDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Constructor started\n");

                // Configurazione da web.config
                _connectionString = WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];
                _debugMode = WebConfigurationManager.AppSettings["LogModeDebug"] == "true";
                _logFileName = WebConfigurationManager.AppSettings["LogFilename"] ?? @"C:\WebApiLog\Price\LogWebApiPrice.txt";

                System.IO.File.AppendAllText(@"C:\WebApiLog\ConstructorDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Configuration loaded, connection string length: {_connectionString?.Length ?? 0}\n");

                // Inizializza repository
                _priceRepository = new PriceRepository(_connectionString);
                _customerRepository = new CustomerRepository(_connectionString);
                _discountRepository = new DiscountRepository(_connectionString);
                
                System.IO.File.AppendAllText(@"C:\WebApiLog\ConstructorDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] About to create QueryManagerOrderRepository\n");
                
                _orderRepository = new QueryManagerOrderRepository(_connectionString);

                System.IO.File.AppendAllText(@"C:\WebApiLog\ConstructorDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] About to create PriceCalculator\n");

                // Inizializza calculator
                _priceCalculator = new PriceCalculator(
                    _priceRepository,
                    _customerRepository,
                    _discountRepository
                );

                System.IO.File.AppendAllText(@"C:\WebApiLog\ConstructorDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Constructor completed successfully\n");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\WebApiLog\ConstructorDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Constructor failed: {ex.Message}\n{ex.StackTrace}\n");
                throw;
            }
        }

        /// <summary>
        /// Calcola il prezzo per un articolo tramite barcode
        /// </summary>
        /// <param name="barcode">Barcode univoco dell'articolo</param>
        /// <returns>Risultato del calcolo prezzo</returns>
        [Route("calculate/{barcode}")]
        [HttpGet]
        public async Task<IHttpActionResult> CalculateByBarcode(string barcode)
        {
            try
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[CalculateByBarcode] Richiesta calcolo per barcode: {barcode}", 
                    _debugMode);

                // Recupera ordine dal database
                Order order = null;
                string sqlQuery = null;
                
                try
                {
                    System.IO.File.AppendAllText(@"C:\WebApiLog\ControllerDebug.log", 
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] About to call GetOrderByBarcode for: {barcode}\n");
                    
                    order = _orderRepository.GetOrderByBarcode(barcode);
                    sqlQuery = (_orderRepository as OrderRepository)?.LastExecutedQuery;
                }
                catch (Exception dbEx)
                {
                    FileLogger.LogDebugWithTimestamp(_logFileName, 
                        $"[CalculateByBarcode] Errore database: {dbEx.Message}\nStackTrace: {dbEx.StackTrace}", 
                        _debugMode);
                    
                    return InternalServerError(new Exception($"Errore database: {dbEx.Message}", dbEx));
                }
                
                if (order == null)
                {
                    FileLogger.LogDebugWithTimestamp(_logFileName, 
                        $"[CalculateByBarcode] Ordine non trovato per barcode: {barcode}", 
                        _debugMode);
                    
                    // Restituisce anche la query per debug
                    var notFoundResult = new 
                    {
                        Error = "Ordine non trovato",
                        Barcode = barcode,
                        SqlQuery = sqlQuery
                    };
                    
                    return Content(System.Net.HttpStatusCode.NotFound, notFoundResult);
                }

                // Calcola prezzo
                var result = await Task.Run(() => _priceCalculator.CalculatePrice(order));
                
                // Aggiungi la query SQL al risultato per debug
                if (_orderRepository is QueryManagerOrderRepository queryManagerRepo)
                {
                    result.SqlQuery = queryManagerRepo.LastExecutedQuery;
                }
                else if (_orderRepository is OrderRepository orderRepo)
                {
                    result.SqlQuery = orderRepo.LastExecutedQuery;
                }

                if (!result.IsSuccess)
                {
                    FileLogger.LogDebugWithTimestamp(_logFileName, 
                        $"[CalculateByBarcode] Errore calcolo: {string.Join(", ", result.Errors)}", 
                        _debugMode);
                    
                    // Restituisce l'oggetto completo con errori e query
                    return Content(System.Net.HttpStatusCode.BadRequest, result);
                }

                // Aggiorna prezzo nel database se richiesto
                if (order.AutomaticPrice != result.FinalPrice)
                {
                    _orderRepository.UpdateOrderPrice(barcode, result.FinalPrice);
                    
                    FileLogger.LogDebugWithTimestamp(_logFileName, 
                        $"[CalculateByBarcode] Prezzo aggiornato: {order.AutomaticPrice} -> {result.FinalPrice}", 
                        _debugMode);
                }

                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[CalculateByBarcode] Calcolo completato. Prezzo finale: {result.FinalPrice}", 
                    _debugMode);

                return Ok(result);
            }
            catch (Exception ex)
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[CalculateByBarcode] Errore: {ex.Message}", 
                    _debugMode);
                
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Calcola il prezzo per un ordine fornito
        /// </summary>
        /// <param name="order">Dati dell'ordine</param>
        /// <returns>Risultato del calcolo prezzo</returns>
        [Route("calculate")]
        [HttpPost]
        public async Task<IHttpActionResult> Calculate([FromBody] Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[Calculate] Richiesta calcolo per ordine: {order.Barcode}, Cliente: {order.CustomerId}", 
                    _debugMode);

                // Carica informazioni cliente se non presenti
                if (order.Customer == null)
                {
                    order.Customer = _customerRepository.GetCustomerInfo(order.CustomerId);
                    
                    if (order.Customer == null)
                    {
                        FileLogger.LogDebugWithTimestamp(_logFileName, 
                            $"[Calculate] Cliente non trovato: {order.CustomerId}", 
                            _debugMode);
                        
                        return BadRequest($"Cliente {order.CustomerId} non trovato");
                    }
                }

                // Carica informazioni stampo se non presenti
                if (order.Stamp == null && order.Stamp?.StampId > 0)
                {
                    order.Stamp = _orderRepository.GetStampInfo(order.Stamp.StampId);
                }

                // Calcola prezzo
                var result = await Task.Run(() => _priceCalculator.CalculatePrice(order));

                if (!result.IsSuccess)
                {
                    FileLogger.LogDebugWithTimestamp(_logFileName, 
                        $"[Calculate] Errore calcolo: {string.Join(", ", result.Errors)}", 
                        _debugMode);
                    
                    return BadRequest(string.Join(", ", result.Errors));
                }

                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[Calculate] Calcolo completato. Prezzo finale: {result.FinalPrice}", 
                    _debugMode);

                return Ok(result);
            }
            catch (Exception ex)
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[Calculate] Errore: {ex.Message}", 
                    _debugMode);
                
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Ottiene i dettagli di prezzo per un barcode senza ricalcolare
        /// </summary>
        /// <param name="barcode">Barcode dell'articolo</param>
        /// <returns>Ordine con prezzo esistente</returns>
        [Route("details/{barcode}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPriceDetails(string barcode)
        {
            try
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[GetPriceDetails] Richiesta dettagli per barcode: {barcode}", 
                    _debugMode);

                var order = await Task.Run(() => _orderRepository.GetOrderByBarcode(barcode));
                
                if (order == null)
                {
                    FileLogger.LogDebugWithTimestamp(_logFileName, 
                        $"[GetPriceDetails] Ordine non trovato per barcode: {barcode}", 
                        _debugMode);
                    
                    return NotFound();
                }

                // Carica informazioni aggiuntive
                order.Customer = _customerRepository.GetCustomerInfo(order.CustomerId);
                
                if (order.Stamp?.StampId > 0)
                {
                    order.Stamp = _orderRepository.GetStampInfo(order.Stamp.StampId);
                }

                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[GetPriceDetails] Dettagli recuperati. Prezzo esistente: {order.ExistingPrice}", 
                    _debugMode);

                return Ok(order);
            }
            catch (Exception ex)
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[GetPriceDetails] Errore: {ex.Message}", 
                    _debugMode);
                
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Calcola prezzi in batch per più barcode
        /// </summary>
        /// <param name="barcodes">Lista di barcode</param>
        /// <returns>Lista di risultati calcolo prezzo</returns>
        [Route("calculate/batch")]
        [HttpPost]
        public async Task<IHttpActionResult> CalculateBatch([FromBody] string[] barcodes)
        {
            try
            {
                if (barcodes == null || barcodes.Length == 0)
                {
                    return BadRequest("Nessun barcode fornito");
                }

                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[CalculateBatch] Richiesta calcolo batch per {barcodes.Length} barcode", 
                    _debugMode);

                var results = new System.Collections.Generic.List<PriceResult>();

                foreach (var barcode in barcodes)
                {
                    try
                    {
                        var order = _orderRepository.GetOrderByBarcode(barcode);
                        
                        if (order != null)
                        {
                            var result = await Task.Run(() => _priceCalculator.CalculatePrice(order));
                            results.Add(result);

                            // Aggiorna prezzo se diverso
                            if (result.IsSuccess && order.AutomaticPrice != result.FinalPrice)
                            {
                                _orderRepository.UpdateOrderPrice(barcode, result.FinalPrice);
                            }
                        }
                        else
                        {
                            results.Add(new PriceResult
                            {
                                Barcode = barcode,
                                IsSuccess = false,
                                Errors = new System.Collections.Generic.List<string> { "Ordine non trovato" }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new PriceResult
                        {
                            Barcode = barcode,
                            IsSuccess = false,
                            Errors = new System.Collections.Generic.List<string> { ex.Message }
                        });
                    }
                }

                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[CalculateBatch] Batch completato. Successi: {results.Count(r => r.IsSuccess)}/{results.Count}", 
                    _debugMode);

                return Ok(results);
            }
            catch (Exception ex)
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[CalculateBatch] Errore: {ex.Message}", 
                    _debugMode);
                
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Endpoint di test per verificare la connessione database
        /// </summary>
        [Route("test/connection")]
        [HttpGet]
        public IHttpActionResult TestConnection()
        {
            try
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    "[TestConnection] Testing database connection", 
                    _debugMode);

                // Test connessione semplice
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT 1", connection))
                    {
                        var result = (int)command.ExecuteScalar();
                        
                        return Ok(new { 
                            Status = "Connected", 
                            ConnectionString = _connectionString?.Substring(0, Math.Min(50, _connectionString.Length)) + "...",
                            TestResult = result 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[TestConnection] Error: {ex.Message}", 
                    _debugMode);
                
                return InternalServerError(new Exception($"Connection test failed: {ex.Message}", ex));
            }
        }

        /// <summary>
        /// Test query 516 senza QueryManager - restituisce query completa
        /// </summary>
        [Route("test/query516/{barcode}")]
        [HttpGet]
        public IHttpActionResult TestQuery516(string barcode)
        {
            try
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[TestQuery516] Testing query 516 for barcode: {barcode}", 
                    _debugMode);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Prima verifica se esiste dev.query
                    using (var command = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_SCHEMA = 'dev' AND TABLE_NAME = 'query'", connection))
                    {
                        var queryExists = (int)command.ExecuteScalar();
                        
                        if (queryExists == 0)
                        {
                            return Ok(new { 
                                Error = "Table dev.query does not exist",
                                Barcode = barcode 
                            });
                        }
                    }
                    
                    // Recupera la query 516
                    using (var command = new SqlCommand(@"
                        SELECT TOP 1 query 
                        FROM dev.query 
                        WHERE idQry = 516 
                        ORDER BY ver DESC", connection))
                    {
                        var queryText = command.ExecuteScalar() as string;
                        
                        if (string.IsNullOrEmpty(queryText))
                        {
                            return Ok(new { 
                                Error = "Query 516 not found in dev.query",
                                Barcode = barcode 
                            });
                        }
                        
                        // Simula il processing e salva la query finale
                        string processedQuery = queryText;
                        try
                        {
                            // Usa String.Format come fa il QueryManager
                            processedQuery = string.Format(queryText, barcode);
                        }
                        catch (System.FormatException)
                        {
                            // Fallback: sostituzione diretta
                            processedQuery = queryText.Replace("{0}", barcode);
                        }
                        
                        // Salva la query finale
                        System.IO.File.WriteAllText(@"C:\WebApiLog\query516.sql", processedQuery);
                        
                        return Ok(new { 
                            Status = "Found", 
                            Barcode = barcode,
                            OriginalQueryLength = queryText.Length,
                            ProcessedQueryLength = processedQuery.Length,
                            QueryChanged = queryText != processedQuery,
                            OriginalQuery = queryText,  // Query completa originale
                            ProcessedQuery = processedQuery,  // Query completa dopo processing
                            SavedToFile = "C:\\WebApiLog\\query516.sql"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogDebugWithTimestamp(_logFileName, 
                    $"[TestQuery516] Error: {ex.Message}", 
                    _debugMode);
                
                return InternalServerError(new Exception($"Query 516 test failed: {ex.Message}", ex));
            }
        }

        /// <summary>
        /// Debug per vedere la query 516 dopo il processing
        /// </summary>
        [Route("debug/query516processed/{barcode}")]
        [HttpGet]
        public IHttpActionResult DebugQuery516Processed(string barcode)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Recupera la query 516 originale
                    using (var command = new SqlCommand(@"
                        SELECT TOP 1 query 
                        FROM dev.query 
                        WHERE idQry = 516 
                        ORDER BY ver DESC", connection))
                    {
                        var originalQuery = command.ExecuteScalar() as string;
                        
                        if (string.IsNullOrEmpty(originalQuery))
                        {
                            return Ok(new { Error = "Query 516 not found" });
                        }
                        
                        // Simula il processing del QueryManager
                        string processedQuery = originalQuery;
                        if (!string.IsNullOrEmpty(barcode))
                        {
                            // Pattern dal QueryManager
                            var original1 = processedQuery;
                            processedQuery = processedQuery.Replace("DECLARE @BarCode AS dbo.BarCode = '{0}'", $"DECLARE @BarCode AS nvarchar(50) = '{barcode}'");
                            var after1 = processedQuery;
                            
                            processedQuery = processedQuery.Replace("DECLARE @BarCode AS dbo.BarCode", $"DECLARE @BarCode AS nvarchar(50) = '{barcode}'");
                            var after2 = processedQuery;
                            
                            processedQuery = processedQuery.Replace("DECLARE @BarCode dbo.BarCode", $"DECLARE @BarCode AS nvarchar(50) = '{barcode}'");
                            var after3 = processedQuery;
                            
                            return Ok(new {
                                Barcode = barcode,
                                OriginalLength = originalQuery.Length,
                                ProcessedLength = processedQuery.Length,
                                OriginalFirst200 = originalQuery.Substring(0, Math.Min(200, originalQuery.Length)),
                                ProcessedFirst200 = processedQuery.Substring(0, Math.Min(200, processedQuery.Length)),
                                ReplacementsMade = new {
                                    Pattern1Changed = original1 != after1,
                                    Pattern2Changed = after1 != after2, 
                                    Pattern3Changed = after2 != after3,
                                    FinalChanged = originalQuery != processedQuery
                                }
                            });
                        }
                        
                        return Ok(new { 
                            Error = "No barcode provided",
                            OriginalQuery = originalQuery.Substring(0, Math.Min(500, originalQuery.Length))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Debug failed: {ex.Message}", ex));
            }
        }

        /// <summary>
        /// Mostra la query finale che viene eseguita dal QueryManager
        /// </summary>
        [Route("debug/finalquery/{barcode}")]
        [HttpGet]
        public IHttpActionResult DebugFinalQuery(string barcode)
        {
            try
            {
                var queryManager = new Omnitech.Prezzi.Infrastructure.QueryManager();
                queryManager.IdQuery = 516;
                
                // Recupera la query originale
                string originalQuery = queryManager.GetType()
                    .GetMethod("GetQueryText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(queryManager, new object[] { 516 }) as string;

                if (string.IsNullOrEmpty(originalQuery))
                {
                    return Ok(new { Error = "Cannot retrieve original query 516" });
                }

                // Simula il processing del QueryManager
                string processedQuery = originalQuery;
                if (!string.IsNullOrEmpty(barcode))
                {
                    try
                    {
                        // Prima prova String.Format (approccio PsR)
                        processedQuery = string.Format(originalQuery, barcode);
                    }
                    catch (System.FormatException ex)
                    {
                        // Fallback: sostituzione diretta
                        processedQuery = originalQuery.Replace("{0}", barcode);
                    }
                }

                return Ok(new
                {
                    Barcode = barcode,
                    OriginalQueryLength = originalQuery.Length,
                    ProcessedQueryLength = processedQuery.Length,
                    QueryChanged = originalQuery != processedQuery,
                    OriginalQueryFirst500 = originalQuery.Substring(0, Math.Min(500, originalQuery.Length)),
                    ProcessedQueryFirst500 = processedQuery.Substring(0, Math.Min(500, processedQuery.Length)),
                    ProcessedQueryFull = processedQuery // Attenzione: questa potrebbe essere molto lunga!
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Debug query failed: {ex.Message}", ex));
            }
        }

        /// <summary>
        /// Salva la query 516 processata come file query516.sql
        /// </summary>
        [Route("save/query516/{barcode}")]
        [HttpGet]
        public IHttpActionResult SaveQuery516(string barcode)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Recupera la query 516
                    using (var command = new SqlCommand(@"
                        SELECT TOP 1 query 
                        FROM dev.query 
                        WHERE idQry = 516 
                        ORDER BY ver DESC", connection))
                    {
                        var originalQuery = command.ExecuteScalar() as string;
                        
                        if (string.IsNullOrEmpty(originalQuery))
                        {
                            return Ok(new { Error = "Query 516 not found" });
                        }
                        
                        // Processing della query
                        string processedQuery = originalQuery;
                        try
                        {
                            // String.Format approach
                            processedQuery = string.Format(originalQuery, barcode);
                        }
                        catch (System.FormatException)
                        {
                            // Fallback: direct replacement
                            processedQuery = originalQuery.Replace("{0}", barcode);
                        }
                        
                        // Salva nel file
                        System.IO.File.WriteAllText(@"C:\WebApiLog\query516.sql", processedQuery);
                        
                        return Ok(new {
                            Status = "Saved",
                            Barcode = barcode,
                            FilePath = @"C:\WebApiLog\query516.sql",
                            OriginalLength = originalQuery.Length,
                            ProcessedLength = processedQuery.Length,
                            QueryChanged = originalQuery != processedQuery
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Save query failed: {ex.Message}", ex));
            }
        }

        /// <summary>
        /// Test nuovo QueryManager con proprietà QueryWithParam identica a PsR
        /// </summary>
        [Route("test/querymanager/{barcode}")]
        [HttpGet]
        public IHttpActionResult TestNewQueryManager(string barcode)
        {
            try
            {
                var queryManager = new Omnitech.Prezzi.Infrastructure.QueryManager();
                queryManager.IdQuery = 516;
                
                // Simula l'approccio PsR: aggiungi barcode agli Args
                queryManager.Args.Add(barcode);
                
                // Usa la proprietà QueryWithParam (come PsR)
                string processedQuery = queryManager.QueryWithParam;
                
                // Salva per confronto
                System.IO.File.WriteAllText(@"C:\WebApiLog\query516_test_output.sql", processedQuery);
                
                return Ok(new
                {
                    Status = "Success",
                    Barcode = barcode,
                    QueryLength = processedQuery.Length,
                    SavedToFile = "C:\\WebApiLog\\query516_test_output.sql",
                    QueryPreview = processedQuery.Substring(0, Math.Min(500, processedQuery.Length)) + "..."
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"QueryManager test failed: {ex.Message}", ex));
            }
        }
    }
}