using System;
using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Configuration;

namespace WebApi.Misc.Controllers
{
    [RoutePrefix("api/sqltest")]
    public class SqlTestController : ApiController
    {
        private readonly string _connectionString;

        public SqlTestController()
        {
            _connectionString = WebConfigurationManager.AppSettings["ConnectionStringAvanzamentoProduzione"]
                ?? "Data Source=localhost\\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;";
        }

        /// <summary>
        /// Test semplice SELECT 1 per verificare connessione SQL Server
        /// </summary>
        [HttpGet]
        [Route("select1")]
        public IHttpActionResult TestSelect1()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT 1 as TestResult", connection))
                    {
                        command.CommandTimeout = 5; // Timeout molto breve per test rapido

                        var result = command.ExecuteScalar();

                        return Ok(new
                        {
                            success = true,
                            message = "SQL Server connesso correttamente",
                            result = result?.ToString(),
                            connectionString = HideConnectionStringPassword(_connectionString),
                            timestamp = DateTime.Now
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                return Ok(new
                {
                    success = false,
                    error = "SQL Server Error",
                    message = ex.Message,
                    errorNumber = ex.Number,
                    severity = ex.Class,
                    state = ex.State,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = "Connection Error",
                    message = ex.Message,
                    type = ex.GetType().Name,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Test connessione semplice senza eseguire query
        /// </summary>
        [HttpGet]
        [Route("connection")]
        public IHttpActionResult TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    return Ok(new
                    {
                        success = true,
                        message = "Connessione SQL Server OK",
                        serverVersion = connection.ServerVersion,
                        database = connection.Database,
                        dataSource = connection.DataSource,
                        state = connection.State.ToString(),
                        timestamp = DateTime.Now
                    });
                }
            }
            catch (SqlException ex)
            {
                return Ok(new
                {
                    success = false,
                    error = "SQL Connection Failed",
                    message = ex.Message,
                    errorNumber = ex.Number,
                    severity = ex.Class,
                    state = ex.State,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = "Connection Failed",
                    message = ex.Message,
                    type = ex.GetType().Name,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Test query semplice su una tabella esistente
        /// </summary>
        [HttpGet]
        [Route("simplequery")]
        public IHttpActionResult TestSimpleQuery()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT TOP 1 COUNT(*) as RecordCount FROM dev.query WITH (NOLOCK)", connection))
                    {
                        command.CommandTimeout = 10;

                        var result = command.ExecuteScalar();

                        return Ok(new
                        {
                            success = true,
                            message = "Query semplice completata",
                            queryCount = result?.ToString(),
                            query = "SELECT TOP 1 COUNT(*) FROM dev.query WITH (NOLOCK)",
                            timestamp = DateTime.Now
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                return Ok(new
                {
                    success = false,
                    error = "SQL Query Error",
                    message = ex.Message,
                    errorNumber = ex.Number,
                    severity = ex.Class,
                    state = ex.State,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = "Query Error",
                    message = ex.Message,
                    type = ex.GetType().Name,
                    timestamp = DateTime.Now
                });
            }
        }

        private string HideConnectionStringPassword(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return "";

            // Nasconde password se presente
            if (connectionString.ToLower().Contains("password="))
            {
                var parts = connectionString.Split(';');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].ToLower().Trim().StartsWith("password="))
                    {
                        parts[i] = "Password=***";
                    }
                }
                return string.Join(";", parts);
            }

            return connectionString;
        }
    }
}