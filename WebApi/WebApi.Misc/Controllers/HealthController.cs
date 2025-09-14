using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Diagnostics;
using System.Web.Configuration;
using Omnitech.Prezzi.Infrastructure;

namespace WebApi.Misc.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        private readonly string _connectionString;
        private readonly string _logPath = @"C:\WebApiLog\";

        public HealthController()
        {
            _connectionString = WebConfigurationManager.AppSettings["ConnectionStringAvanzamentoProduzione"] 
                ?? "Data Source=localhost\\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;";
        }

        /// <summary>
        /// Verifica lo stato del servizio SQL Server
        /// </summary>
        [HttpGet]
        [Route("sql")]
        public IHttpActionResult GetSqlServerStatus()
        {
            try
            {
                // Test connessione database
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT 1", connection);
                    var result = command.ExecuteScalar();
                    
                    if (result != null)
                    {
                        return Ok(new
                        {
                            status = "online",
                            message = "SQL Server operativo",
                            connectionString = HidePassword(_connectionString),
                            timestamp = DateTime.Now
                        });
                    }
                }
                
                return Ok(new
                {
                    status = "offline",
                    message = "SQL Server non raggiungibile",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    message = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Controllo stato generale dell'API
        /// </summary>
        [HttpGet]
        [Route("api")]
        public IHttpActionResult GetApiStatus()
        {
            try
            {
                var stats = GetStatistics();
                
                return Ok(new
                {
                    status = "online",
                    message = "Web API operativa",
                    version = "2.0",
                    uptime = GetUptime(),
                    statistics = stats,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Avvia servizio SQL Server (richiede privilegi amministratore)
        /// </summary>
        [HttpPost]
        [Route("sql/start")]
        public IHttpActionResult StartSqlService()
        {
            return ManageSqlService("start");
        }

        /// <summary>
        /// Ferma servizio SQL Server (richiede privilegi amministratore)
        /// </summary>
        [HttpPost]
        [Route("sql/stop")]
        public IHttpActionResult StopSqlService()
        {
            return ManageSqlService("stop");
        }

        /// <summary>
        /// Riavvia servizio SQL Server (richiede privilegi amministratore)
        /// </summary>
        [HttpPost]
        [Route("sql/restart")]
        public IHttpActionResult RestartSqlService()
        {
            return ManageSqlService("restart");
        }

        /// <summary>
        /// Gestione servizio SQL Server (richiede privilegi amministratore)
        /// </summary>
        private IHttpActionResult ManageSqlService(string action)
        {
            try
            {
                if (action != "start" && action != "stop" && action != "restart")
                {
                    return BadRequest("Azione non valida. Usare: start, stop, restart");
                }

                var serviceName = "MSSQL$SQLEXPRESS"; // Nome servizio SQL Server Express
                var processInfo = new ProcessStartInfo();
                
                if (action == "start")
                {
                    processInfo.FileName = "net";
                    processInfo.Arguments = $"start \"{serviceName}\"";
                }
                else if (action == "stop")
                {
                    processInfo.FileName = "net";
                    processInfo.Arguments = $"stop \"{serviceName}\"";
                }
                else if (action == "restart")
                {
                    // Prima ferma, poi avvia
                    ExecuteServiceCommand("stop", serviceName);
                    System.Threading.Thread.Sleep(2000);
                    ExecuteServiceCommand("start", serviceName);
                    
                    return Ok(new
                    {
                        success = true,
                        message = $"Servizio {serviceName} riavviato",
                        timestamp = DateTime.Now
                    });
                }

                processInfo.UseShellExecute = false;
                processInfo.RedirectStandardOutput = true;
                processInfo.RedirectStandardError = true;
                processInfo.CreateNoWindow = true;
                processInfo.Verb = "runas"; // Richiede privilegi amministratore

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit(10000); // Timeout 10 secondi
                    
                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    
                    if (process.ExitCode == 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = $"Servizio {serviceName} {action} completato",
                            output = output,
                            timestamp = DateTime.Now
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = $"Errore durante {action} del servizio",
                            error = error,
                            exitCode = process.ExitCode,
                            timestamp = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Ottieni statistiche dettagliate dai log
        /// </summary>
        [HttpGet]
        [Route("statistics")]
        public IHttpActionResult GetDetailedStatistics()
        {
            try
            {
                var stats = GetStatistics();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Pulisci tutti i file di log
        /// </summary>
        [HttpPost]
        [Route("clearlogs")]
        public IHttpActionResult ClearLogs()
        {
            try
            {
                var logFiles = Directory.GetFiles(_logPath, "*.log", SearchOption.AllDirectories);
                var clearedFiles = new List<string>();
                
                foreach (var file in logFiles)
                {
                    try
                    {
                        File.Delete(file);
                        clearedFiles.Add(Path.GetFileName(file));
                    }
                    catch (Exception ex)
                    {
                        // Log file potrebbe essere in uso
                        continue;
                    }
                }
                
                return Ok(new
                {
                    success = true,
                    message = $"Puliti {clearedFiles.Count} file di log",
                    files = clearedFiles,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private void ExecuteServiceCommand(string action, string serviceName)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "net",
                Arguments = $"{action} \"{serviceName}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                Verb = "runas"
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit(10000);
            }
        }

        private object GetStatistics()
        {
            try
            {
                var today = DateTime.Today;
                var stats = new
                {
                    TotalRequests = GetTotalRequestsToday(),
                    SuccessfulQueries = GetSuccessfulQueries(),
                    TimeoutQueries = GetTimeoutQueries(),
                    LastRequest = GetLastRequestTime(),
                    AverageResponseTime = GetAverageResponseTime(),
                    ErrorRate = GetErrorRate(),
                    TopBarcodes = GetTopBarcodes(),
                    ServerUptime = GetUptime()
                };

                return stats;
            }
            catch (Exception)
            {
                return new
                {
                    TotalRequests = 0,
                    SuccessfulQueries = 0,
                    TimeoutQueries = 0,
                    LastRequest = "N/A",
                    AverageResponseTime = "N/A",
                    ErrorRate = "N/A",
                    TopBarcodes = new List<object>(),
                    ServerUptime = GetUptime()
                };
            }
        }

        private int GetTotalRequestsToday()
        {
            try
            {
                var controllerLogPath = Path.Combine(_logPath, "ControllerDebug.log");
                if (!File.Exists(controllerLogPath)) return 0;

                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var lines = File.ReadAllLines(controllerLogPath);
                
                return lines.Count(line => line.StartsWith($"[{today}") && line.Contains("About to call GetOrderByBarcode"));
            }
            catch
            {
                return 0;
            }
        }

        private int GetSuccessfulQueries()
        {
            try
            {
                var queryLogPath = Path.Combine(_logPath, "QueryManagerDebug.log");
                if (!File.Exists(queryLogPath)) return 0;

                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var lines = File.ReadAllLines(queryLogPath);
                
                return lines.Count(line => line.StartsWith($"[{today}") && line.Contains("Query executed successfully"));
            }
            catch
            {
                return 0;
            }
        }

        private int GetTimeoutQueries()
        {
            try
            {
                var queryLogPath = Path.Combine(_logPath, "QueryManagerDebug.log");
                if (!File.Exists(queryLogPath)) return 0;

                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var lines = File.ReadAllLines(queryLogPath);
                
                return lines.Count(line => line.StartsWith($"[{today}") && 
                    (line.Contains("timeout") || line.Contains("Timeout") || line.Contains("scaduto")));
            }
            catch
            {
                return 0;
            }
        }

        private string GetLastRequestTime()
        {
            try
            {
                var controllerLogPath = Path.Combine(_logPath, "ControllerDebug.log");
                if (!File.Exists(controllerLogPath)) return "N/A";

                var lines = File.ReadAllLines(controllerLogPath);
                var lastLine = lines.LastOrDefault(line => line.Contains("About to call GetOrderByBarcode"));
                
                if (lastLine != null)
                {
                    var timeStart = lastLine.IndexOf('[') + 1;
                    var timeEnd = lastLine.IndexOf(']');
                    if (timeStart > 0 && timeEnd > timeStart)
                    {
                        return lastLine.Substring(timeStart, timeEnd - timeStart);
                    }
                }
                
                return "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        private string GetAverageResponseTime()
        {
            // Per ora restituisce un valore simulato
            // In una implementazione completa, si potrebbero analizzare i timestamp nei log
            return "2.3s";
        }

        private string GetErrorRate()
        {
            var total = GetTotalRequestsToday();
            var timeouts = GetTimeoutQueries();
            
            if (total == 0) return "0%";
            
            var errorRate = (double)timeouts / total * 100;
            return $"{errorRate:F1}%";
        }

        private List<object> GetTopBarcodes()
        {
            try
            {
                var controllerLogPath = Path.Combine(_logPath, "ControllerDebug.log");
                if (!File.Exists(controllerLogPath)) return new List<object>();

                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var lines = File.ReadAllLines(controllerLogPath);
                
                var barcodes = lines
                    .Where(line => line.StartsWith($"[{today}") && line.Contains("About to call GetOrderByBarcode for:"))
                    .Select(line => {
                        var parts = line.Split(':');
                        return parts.Length > 1 ? parts.Last().Trim() : "";
                    })
                    .Where(barcode => !string.IsNullOrEmpty(barcode))
                    .GroupBy(barcode => barcode)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new { Barcode = g.Key, Count = g.Count() })
                    .ToList<object>();

                return barcodes;
            }
            catch
            {
                return new List<object>();
            }
        }

        private string GetUptime()
        {
            try
            {
                var startTime = Process.GetCurrentProcess().StartTime;
                var uptime = DateTime.Now - startTime;
                
                if (uptime.TotalDays >= 1)
                    return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";
                else if (uptime.TotalHours >= 1)
                    return $"{uptime.Hours}h {uptime.Minutes}m";
                else
                    return $"{uptime.Minutes}m {uptime.Seconds}s";
            }
            catch
            {
                return "N/A";
            }
        }

        /// <summary>
        /// Esegue una query SQL personalizzata
        /// </summary>
        [HttpPost]
        [Route("sql/execute")]
        public IHttpActionResult ExecuteCustomQuery([FromBody] dynamic request)
        {
            try
            {
                string query = request?.query?.ToString();
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest("Query SQL richiesta");
                }

                // Validazione sicurezza - solo SELECT e alcune operazioni sicure
                var queryLower = query.ToLower().Trim();
                if (!queryLower.StartsWith("select") &&
                    !queryLower.StartsWith("show") &&
                    !queryLower.StartsWith("describe") &&
                    !queryLower.StartsWith("explain"))
                {
                    return Ok(new
                    {
                        success = false,
                        error = "Solo query SELECT, SHOW, DESCRIBE, EXPLAIN sono consentite per sicurezza",
                        timestamp = DateTime.Now
                    });
                }

                var startTime = DateTime.Now;

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 30;

                        var results = new List<Dictionary<string, object>>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var fieldName = reader.GetName(i);
                                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    row[fieldName] = value?.ToString() ?? "";
                                }
                                results.Add(row);
                            }
                        }

                        var endTime = DateTime.Now;
                        var duration = (endTime - startTime).TotalSeconds;

                        return Ok(new
                        {
                            success = true,
                            results = results,
                            rowCount = results.Count,
                            duration = $"{duration:F2}s",
                            executionTime = duration,
                            message = results.Count == 0 ? "Nessun risultato trovato" : $"{results.Count} righe restituite",
                            timestamp = DateTime.Now
                        });
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                return Ok(new
                {
                    success = false,
                    error = $"Errore SQL: {sqlEx.Message}",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = $"Errore: {ex.Message}",
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Esegue una query per numero ID specificato
        /// </summary>
        [HttpPost]
        [Route("query")]
        public IHttpActionResult ExecuteQueryById([FromBody] dynamic request)
        {
            try
            {
                string queryIdStr = request?.queryId?.ToString();
                if (string.IsNullOrEmpty(queryIdStr) || !int.TryParse(queryIdStr, out int queryId))
                {
                    return BadRequest("Query ID numerico richiesto");
                }

                var queryManager = new QueryManager();
                queryManager.IdQuery = queryId;

                var startTime = DateTime.Now;
                bool success = false;

                // Handle structured parameters (new way)
                if (request?.parameters != null)
                {
                    var parameters = request.parameters;

                    // Populate Args collection manually AFTER setting IdQuery
                    queryManager.Args.Clear();
                    for (int i = 0; i < 10; i++)
                    {
                        var paramValue = parameters[i.ToString()]?.ToString();
                        if (!string.IsNullOrEmpty(paramValue))
                        {
                            queryManager.Args.Add(paramValue);
                        }
                    }

                    // Handle dictionary parameters
                    if (request?.dictionaryParameters != null)
                    {
                        var dictParams = request.dictionaryParameters;
                        foreach (var prop in dictParams)
                        {
                            var key = "{" + prop.Name.ToUpper() + "}";
                            var value = prop.Value?.ToString();
                            if (!string.IsNullOrEmpty(value))
                            {
                                queryManager.Replace[key] = value;
                            }
                        }
                    }

                    // Execute query without clearing Args (pass null to preserve current Args)
                    success = queryManager.GetQuery(null);
                }
                // Fallback to legacy method
                else if (request?.barcode != null)
                {
                    success = queryManager.GetQuery(request.barcode.ToString());
                }
                else
                {
                    success = queryManager.GetQuery("");
                }

                var endTime = DateTime.Now;
                var duration = (endTime - startTime).TotalSeconds;

                if (success && queryManager.DR != null)
                {
                    // Estrae i dati dalla DataRow
                    var result = new Dictionary<string, object>();
                    for (int i = 0; i < queryManager.DR.Table.Columns.Count; i++)
                    {
                        var columnName = queryManager.DR.Table.Columns[i].ColumnName;
                        var value = queryManager.DR.IsNull(i) ? null : queryManager.DR[i];
                        result[columnName] = value?.ToString() ?? "";
                    }

                    return Ok(new
                    {
                        success = true,
                        queryId = queryId,
                        duration = $"{duration:F2}s",
                        executedQuery = queryManager.ExecutedQuery,
                        rowCount = queryManager.DT?.Rows.Count ?? 0,
                        data = result,
                        message = $"Query {queryId} eseguita con successo in {duration:F2} secondi",
                        timestamp = DateTime.Now
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        queryId = queryId,
                        duration = $"{duration:F2}s",
                        executedQuery = queryManager.ExecutedQuery,
                        message = $"Query {queryId} eseguita ma nessun risultato trovato",
                        timestamp = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = $"Errore Query: {ex.Message}",
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Recupera il template di una query e identifica i parametri richiesti (Step 1)
        /// </summary>
        [HttpGet]
        [Route("query/{queryId}/template")]
        public IHttpActionResult GetQueryTemplate(int queryId)
        {
            try
            {
                var queryManager = new QueryManager();
                queryManager.IdQuery = queryId;

                // Ottiene il testo raw della query
                string rawQuery = queryManager.GetQueryText(queryId);

                if (string.IsNullOrEmpty(rawQuery))
                {
                    return Ok(new
                    {
                        success = false,
                        error = $"Query {queryId} non trovata nel database",
                        timestamp = DateTime.Now
                    });
                }

                // Analizza i parametri richiesti dalla query
                var parameters = new List<object>();
                var parameterNumbers = new List<int>();

                // Cerca parametri {0}, {1}, {2}, etc.
                for (int i = 0; i < 10; i++)
                {
                    if (rawQuery.Contains("{" + i + "}"))
                    {
                        parameterNumbers.Add(i);
                        parameters.Add(new {
                            index = i,
                            placeholder = "{" + i + "}",
                            name = i == 0 ? "Barcode" : $"Parametro {i}",
                            required = true,
                            defaultValue = i == 0 ? "012027" : ""
                        });
                    }
                }

                // Cerca anche parametri con nomi specifici (per dictionary replacements)
                var dictionaryParams = new List<object>();
                var commonReplacements = new[] { "{BARCODE}", "{CLIENT}", "{DATE}", "{USER}" };

                foreach (var param in commonReplacements)
                {
                    if (rawQuery.Contains(param))
                    {
                        dictionaryParams.Add(new {
                            placeholder = param,
                            name = param.Replace("{", "").Replace("}", ""),
                            required = true,
                            defaultValue = param == "{BARCODE}" ? "012027" : ""
                        });
                    }
                }

                return Ok(new
                {
                    success = true,
                    queryId = queryId,
                    rawQuery = rawQuery,
                    parameters = parameters,
                    dictionaryParameters = dictionaryParams,
                    timeout = queryId == 516 ? 180 : 90,
                    message = $"Query {queryId} trovata con {parameters.Count} parametri indicizzati e {dictionaryParams.Count} parametri named",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = $"Errore recupero template Query {queryId}: {ex.Message}",
                    timestamp = DateTime.Now
                });
            }
        }

        private string HidePassword(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return "";

            // Nasconde password nella connection string per sicurezza
            return connectionString.Contains("Password=") ?
                connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***" :
                connectionString;
        }
    }
}