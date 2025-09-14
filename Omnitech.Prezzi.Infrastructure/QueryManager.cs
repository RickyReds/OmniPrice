using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using Dapper;

namespace Omnitech.Prezzi.Infrastructure
{
    public class QueryManager
    {
        private string _connectionString;
        private int _idQuery;
        private DataTable _dt;
        private DataRow _dr;
        private List<string> _args;
        private Dictionary<string, string> _replace;
        private string _executedQuery;

        // Thread-safe logging
        private static readonly object _logLock = new object();
        private static readonly string _logPath = @"C:\WebApiLog\QueryManagerDebug.log";
        private static readonly string _queryOutputPath = @"C:\WebApiLog\query516_our_output.sql";

        public QueryManager()
        {
            _connectionString = WebConfigurationManager.AppSettings["ConnectionStringAvanzamentoProduzione"] 
                ?? WebConfigurationManager.AppSettings["ConnectionStringPrezzi"]?.Replace("Initial Catalog=prezzi", "Initial Catalog=AvanzamentoProduzione")
                ?? throw new InvalidOperationException("Connection string not configured");
            
            _args = new List<string>();
            _replace = new Dictionary<string, string>();
        }

        public int IdQuery
        {
            get { return _idQuery; }
            set { _idQuery = value; }
        }

        public DataTable DT
        {
            get { return _dt; }
        }

        public DataRow DR
        {
            get { return _dr; }
        }

        public List<string> Args
        {
            get { return _args; }
            set { _args = value; }
        }

        public Dictionary<string, string> Replace
        {
            get { return _replace; }
            set { _replace = value; }
        }

        public string ExecutedQuery
        {
            get { return _executedQuery; }
        }

        // Proprietà QueryWithParam identica a PsR
        public string QueryWithParam
        {
            get
            {
                try
                {
                    string queryText = GetQueryText(_idQuery);
                    if (string.IsNullOrEmpty(queryText))
                        return string.Empty;

                    // Prima fase: String.Format come PsR
                    string qr = string.Format(queryText, _args.ToArray());

                    // Seconda fase: Dictionary-based replacements (2020.09.18)
                    foreach (var r in _replace)
                    {
                        qr = qr.Replace(r.Key, r.Value);
                    }

                    return qr;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error processing QueryWithParam: {ex.Message}", ex);
                }
            }
        }

        public bool GetQuery(string barcode = null)
        {
            try
            {
                // Thread-safe logging
                SafeLog($"GetQuery started for barcode: {barcode}, idQuery: {_idQuery}");

                // Setup Args collection like PsR
                _args.Clear();
                if (!string.IsNullOrEmpty(barcode))
                {
                    _args.Add(barcode);
                }

                // Get processed query using QueryWithParam property (like PsR)
                string processedQuery = QueryWithParam;

                SafeLog($"QueryWithParam returned {processedQuery?.Length ?? 0} characters");

                if (string.IsNullOrEmpty(processedQuery))
                {
                    SafeLog("ProcessedQuery is empty, returning false");
                    return false;
                }

                // Write processed query to file for comparison with PsR (thread-safe)
                try
                {
                    SafeWriteQueryFile(processedQuery);
                    SafeLog($"Query written to timestamped file, length: {processedQuery.Length}");
                }
                catch (Exception debugEx)
                {
                    SafeLog($"Failed to write query to file: {debugEx.Message}");
                }

                // Store the executed query for API response
                _executedQuery = processedQuery;
                
                // Execute the query with retry logic
                int maxRetries = 3;
                int retryDelayMs = 1000; // Start with 1 second
                
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();
                            
                            using (var command = new SqlCommand(processedQuery, connection))
                            {
                                command.CommandTimeout = 90; // Increased timeout to 90 seconds
                                
                                // Execute query and fill DataTable
                                using (var adapter = new SqlDataAdapter(command))
                                {
                                    _dt = new DataTable();
                                    adapter.Fill(_dt);
                                    
                                    if (_dt.Rows.Count > 0)
                                    {
                                        _dr = _dt.Rows[0];
                                        SafeLog($"Query executed successfully on attempt {attempt}, {_dt.Rows.Count} rows returned");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException ex) when (ex.Number == -2 && attempt < maxRetries) // Timeout error
                    {
                        SafeLog($"Query timeout on attempt {attempt}, retrying in {retryDelayMs}ms...");

                        Thread.Sleep(retryDelayMs);
                        retryDelayMs *= 2; // Exponential backoff
                        continue;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                SafeLog($"Error executing query {_idQuery}: {ex.Message}");
                throw new Exception($"Error executing query {_idQuery}: {ex.Message}", ex);
            }
        }

        private string GetQueryText(int idQuery)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    string sql = @"
                        SELECT TOP 1 query 
                        FROM dev.query WITH (NOLOCK)
                        WHERE idQry = @idQuery 
                        ORDER BY ver DESC";
                    
                    string queryText = connection.QuerySingleOrDefault<string>(sql, new { idQuery });
                    
                    // Add NOLOCK hints to prevent blocking for query 516
                    if (idQuery == 516 && !string.IsNullOrEmpty(queryText))
                    {
                        queryText = AddNolockHints(queryText);
                    }
                    
                    return queryText;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving query {idQuery}: {ex.Message}", ex);
            }
        }
        
        private string AddNolockHints(string query)
        {
            // Add NOLOCK hints to all table references in the FROM and JOIN clauses
            var replacements = new Dictionary<string, string>
            {
                { "FROM ordini.Ordini oo", "FROM ordini.Ordini oo WITH (NOLOCK)" },
                { "LEFT JOIN anagrafica.LeanTypes lt ON", "LEFT JOIN anagrafica.LeanTypes lt WITH (NOLOCK) ON" },
                { "LEFT JOIN anagrafica.ScalaRAL ral ON", "LEFT JOIN anagrafica.ScalaRAL ral WITH (NOLOCK) ON" },
                { "LEFT JOIN anagrafica.CodiciForm cf ON", "LEFT JOIN anagrafica.CodiciForm cf WITH (NOLOCK) ON" },
                { "LEFT JOIN anagrafica.StampiDimensioni sd ON", "LEFT JOIN anagrafica.StampiDimensioni sd WITH (NOLOCK) ON" },
                { "LEFT JOIN anagrafica.StampiCategorieArticoli sca", "LEFT JOIN anagrafica.StampiCategorieArticoli sca WITH (NOLOCK)" },
                { "LEFT JOIN anagrafica.CategorieArticoli ca", "LEFT JOIN anagrafica.CategorieArticoli ca WITH (NOLOCK)" },
                { "LEFT JOIN clienti.DescrizioneArticoli da ON", "LEFT JOIN clienti.DescrizioneArticoli da WITH (NOLOCK) ON" },
                { "LEFT JOIN clienti.DescrizioneArticoli da2", "LEFT JOIN clienti.DescrizioneArticoli da2 WITH (NOLOCK)" },
                { "LEFT JOIN ordini.OrdiniLavorazioni ol ON", "LEFT JOIN ordini.OrdiniLavorazioni ol WITH (NOLOCK) ON" },
                { "FROM warehouse.Orders", "FROM warehouse.Orders WITH (NOLOCK)" },
                { "FROM Ordini.Locazioni", "FROM Ordini.Locazioni WITH (NOLOCK)" }
            };
            
            foreach (var replacement in replacements)
            {
                query = query.Replace(replacement.Key, replacement.Value);
            }
            
            return query;
        }

        // Thread-safe logging methods
        private static void SafeLog(string message)
        {
            try
            {
                lock (_logLock)
                {
                    var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";

                    // Ensure directory exists
                    var logDir = Path.GetDirectoryName(_logPath);
                    if (!Directory.Exists(logDir))
                    {
                        Directory.CreateDirectory(logDir);
                    }

                    // Use FileStream with FileShare.ReadWrite for thread-safe access
                    using (var fileStream = new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(logMessage);
                        writer.Flush();
                    }
                }
            }
            catch
            {
                // Silent fail - logging errors shouldn't break the main flow
            }
        }

        private static void SafeWriteQueryFile(string queryContent)
        {
            try
            {
                lock (_logLock)
                {
                    // Ensure directory exists
                    var queryDir = Path.GetDirectoryName(_queryOutputPath);
                    if (!Directory.Exists(queryDir))
                    {
                        Directory.CreateDirectory(queryDir);
                    }

                    // Write with timestamp to make it unique per execution
                    var timestampedPath = _queryOutputPath.Replace(".sql", $"_{DateTime.Now:yyyyMMdd_HHmmss}.sql");

                    using (var fileStream = new FileStream(timestampedPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(queryContent);
                        writer.Flush();
                    }
                }
            }
            catch
            {
                // Silent fail - query file writing errors shouldn't break the main flow
            }
        }
    }
}