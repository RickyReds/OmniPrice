using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Configuration;
using System.ServiceProcess;
using System.Linq;

namespace WebApi.Misc.Controllers
{
    [RoutePrefix("api/dbfix")]
    public class DbFixController : ApiController
    {
        /// <summary>
        /// ENDPOINT DEFINITIVO PER SISTEMARE LA CONNESSIONE DB
        /// Questo endpoint diagnostica e risolve automaticamente i problemi di connessione SQL Server
        /// </summary>
        [HttpPost]
        [Route("autofix")]
        public IHttpActionResult AutoFixDatabase()
        {
            var result = new
            {
                timestamp = DateTime.Now,
                steps = new System.Collections.Generic.List<object>(),
                finalStatus = "",
                connectionString = "",
                success = false
            };

            try
            {
                // Step 1: Verifica servizi SQL Server disponibili
                result.steps.Add(new { step = 1, action = "Scanning SQL Server services", status = "running" });

                var sqlServices = ServiceController.GetServices()
                    .Where(s => s.ServiceName.Contains("SQL") || s.ServiceName.Contains("MSSQL"))
                    .Select(s => new {
                        Name = s.ServiceName,
                        Status = s.Status.ToString(),
                        DisplayName = s.DisplayName
                    })
                    .ToList();

                result.steps.Add(new { step = 1, action = "SQL Services found", data = sqlServices, status = "completed" });

                // Step 2: Prova diverse configurazioni di connection string
                var connectionTests = new[]
                {
                    "Data Source=localhost\\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;",
                    "Data Source=.\\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;",
                    "Data Source=(local)\\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;",
                    "Data Source=localhost;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;",
                    "Data Source=.;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;",
                    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;"
                };

                string workingConnectionString = null;
                foreach (var connStr in connectionTests)
                {
                    result.steps.Add(new { step = 2, action = $"Testing connection", connectionString = HidePassword(connStr), status = "running" });

                    try
                    {
                        using (var conn = new SqlConnection(connStr))
                        {
                            conn.Open();
                            using (var cmd = new SqlCommand("SELECT 1", conn))
                            {
                                var testResult = cmd.ExecuteScalar();
                                if (testResult != null)
                                {
                                    workingConnectionString = connStr;
                                    result.steps.Add(new { step = 2, action = "Connection SUCCESS!", connectionString = HidePassword(connStr), status = "success" });
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.steps.Add(new { step = 2, action = "Connection failed", error = ex.Message, status = "failed" });
                    }
                }

                // Step 3: Se trovata connessione valida, aggiorna Web.config
                if (!string.IsNullOrEmpty(workingConnectionString))
                {
                    result.steps.Add(new { step = 3, action = "Updating Web.config with working connection", status = "running" });

                    // Qui normalmente aggiorneresti il Web.config, ma per sicurezza lo loggiamo solo
                    result.steps.Add(new { step = 3, action = "Connection string identified", recommendation = "Update Web.config manually", status = "completed" });

                    // Step 4: Test query complessa (dev.query)
                    try
                    {
                        using (var conn = new SqlConnection(workingConnectionString))
                        {
                            conn.Open();
                            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dev.query WITH (NOLOCK)", conn))
                            {
                                cmd.CommandTimeout = 10;
                                var queryCount = cmd.ExecuteScalar();
                                result.steps.Add(new { step = 4, action = "Testing dev.query table", queryCount = queryCount?.ToString(), status = "success" });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.steps.Add(new { step = 4, action = "dev.query table test failed", error = ex.Message, status = "warning" });
                    }

                    result.finalStatus = "SUCCESS - Working connection found";
                    result.connectionString = HidePassword(workingConnectionString);
                    result.success = true;
                }
                else
                {
                    // Step 3: Prova ad avviare servizi SQL Server
                    result.steps.Add(new { step = 3, action = "No connections worked, attempting to start SQL services", status = "running" });

                    var sqlServiceNames = new[] { "MSSQL$SQLEXPRESS", "SQLEXPRESS", "SQL Server (SQLEXPRESS)", "MSSQLServer" };

                    foreach (var serviceName in sqlServiceNames)
                    {
                        try
                        {
                            var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
                            if (service != null)
                            {
                                if (service.Status != ServiceControllerStatus.Running)
                                {
                                    result.steps.Add(new { step = 3, action = $"Starting service {serviceName}", status = "running" });
                                    service.Start();
                                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                                    result.steps.Add(new { step = 3, action = $"Service {serviceName} started", status = "success" });
                                }
                                else
                                {
                                    result.steps.Add(new { step = 3, action = $"Service {serviceName} already running", status = "info" });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result.steps.Add(new { step = 3, action = $"Failed to start {serviceName}", error = ex.Message, status = "failed" });
                        }
                    }

                    result.finalStatus = "FAILED - No working SQL Server connection found";
                    result.success = false;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.steps.Add(new { step = -1, action = "Fatal error in autofix", error = ex.Message, status = "fatal" });
                result.finalStatus = "FATAL ERROR";
                result.success = false;
                return Ok(result);
            }
        }

        /// <summary>
        /// Test rapido della connessione corrente
        /// </summary>
        [HttpGet]
        [Route("quicktest")]
        public IHttpActionResult QuickTest()
        {
            var connStr = WebConfigurationManager.AppSettings["ConnectionStringAvanzamentoProduzione"];

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT 1 as TestResult, GETDATE() as ServerTime", conn))
                    {
                        cmd.CommandTimeout = 5;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Ok(new
                                {
                                    success = true,
                                    message = "Database connection OK",
                                    testResult = reader["TestResult"].ToString(),
                                    serverTime = reader["ServerTime"].ToString(),
                                    serverVersion = conn.ServerVersion,
                                    database = conn.Database,
                                    connectionString = HidePassword(connStr),
                                    timestamp = DateTime.Now
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    connectionString = HidePassword(connStr),
                    timestamp = DateTime.Now
                });
            }

            return Ok(new { success = false, message = "No data returned" });
        }

        private string HidePassword(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return "";
            return connectionString.Contains("Password=") ?
                connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***" :
                connectionString;
        }
    }
}