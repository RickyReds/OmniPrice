using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                // Debug: Log start
                System.IO.File.AppendAllText(@"C:\WebApiLog\QueryManagerDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] GetQuery started for barcode: {barcode}, idQuery: {_idQuery}\n");

                // Setup Args collection like PsR
                _args.Clear();
                if (!string.IsNullOrEmpty(barcode))
                {
                    _args.Add(barcode);
                }
                
                // Get processed query using QueryWithParam property (like PsR)
                string processedQuery = QueryWithParam;
                
                System.IO.File.AppendAllText(@"C:\WebApiLog\QueryManagerDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] QueryWithParam returned {processedQuery?.Length ?? 0} characters\n");
                
                if (string.IsNullOrEmpty(processedQuery))
                {
                    System.IO.File.AppendAllText(@"C:\WebApiLog\QueryManagerDebug.log", 
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ProcessedQuery is empty, returning false\n");
                    return false;
                }

                // Write processed query to file for comparison with PsR
                try
                {
                    System.IO.File.WriteAllText(@"C:\WebApiLog\query516_our_output.sql", processedQuery);
                    System.IO.File.AppendAllText(@"C:\WebApiLog\QueryManagerDebug.log", 
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Query written to query516_our_output.sql, length: {processedQuery.Length}\n");
                }
                catch (Exception debugEx)
                {
                    System.IO.File.AppendAllText(@"C:\WebApiLog\QueryManagerDebug.log", 
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Failed to write query to file: {debugEx.Message}\n");
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
                                command.CommandTimeout = 15; // Reduced timeout to 15 seconds
                                
                                // Execute query and fill DataTable
                                using (var adapter = new SqlDataAdapter(command))
                                {
                                    _dt = new DataTable();
                                    adapter.Fill(_dt);
                                    
                                    if (_dt.Rows.Count > 0)
                                    {
                                        _dr = _dt.Rows[0];
                                        System.IO.File.AppendAllText(@"C:\WebApiLog\QueryManagerDebug.log", 
                                            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Query executed successfully on attempt {attempt}, {_dt.Rows.Count} rows returned\n");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException ex) when (ex.Number == -2 && attempt < maxRetries) // Timeout error
                    {
                        System.IO.File.AppendAllText(@"C:\WebApiLog\QueryManagerDebug.log", 
                            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Query timeout on attempt {attempt}, retrying in {retryDelayMs}ms...\n");
                        
                        System.Threading.Thread.Sleep(retryDelayMs);
                        retryDelayMs *= 2; // Exponential backoff
                        continue;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\WebApiLog\QueryManagerDebug.log", 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error executing query {_idQuery}: {ex.Message}\n");
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
    }
}