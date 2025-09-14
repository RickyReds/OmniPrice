using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web.Configuration;

namespace Omnitech.Prezzi.Infrastructure
{
    public static class ConnectionManager
    {
        private static readonly object _lock = new object();
        private static readonly string _logPath = @"C:\WebApiLog\ConnectionManager.log";

        // Predefined connections
        private static readonly Dictionary<string, string> _connections = new Dictionary<string, string>
        {
            { "local", null }, // Will be loaded from config
            { "dual", "Data Source=192.168.1.190;Initial Catalog=AvanzamentoProduzione;User ID=rickyreds;Password=Omni!2k25@rReds!;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30" },
        };

        private static string _currentConnection = "local";

        static ConnectionManager()
        {
            // Load local connection from config
            _connections["local"] = WebConfigurationManager.AppSettings["ConnectionStringAvanzamentoProduzione"]
                ?? WebConfigurationManager.AppSettings["ConnectionStringPrezzi"]?.Replace("Initial Catalog=prezzi", "Initial Catalog=AvanzamentoProduzione")
                ?? "Data Source=localhost\\SQLEXPRESS;Initial Catalog=TestDB;Integrated Security=True;";
        }

        public static string CurrentConnectionName => _currentConnection;

        public static string CurrentConnectionString => GetConnectionString(_currentConnection);

        public static Dictionary<string, string> AvailableConnections => new Dictionary<string, string>(_connections);

        public static string GetConnectionString(string connectionName)
        {
            if (_connections.ContainsKey(connectionName))
            {
                return _connections[connectionName];
            }
            throw new ArgumentException($"Connection '{connectionName}' not found");
        }

        public static bool SetCurrentConnection(string connectionName)
        {
            lock (_lock)
            {
                if (!_connections.ContainsKey(connectionName))
                {
                    LogMessage($"ERROR: Connection '{connectionName}' not found");
                    return false;
                }

                var oldConnection = _currentConnection;
                _currentConnection = connectionName;
                LogMessage($"Connection switched from '{oldConnection}' to '{connectionName}'");
                return true;
            }
        }

        public static bool TestConnection(string connectionName)
        {
            try
            {
                var connectionString = GetConnectionString(connectionName);
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT 1", connection))
                    {
                        command.ExecuteScalar();
                    }
                }
                LogMessage($"Connection test successful for '{connectionName}'");
                return true;
            }
            catch (Exception ex)
            {
                LogMessage($"Connection test failed for '{connectionName}': {ex.Message}");
                return false;
            }
        }

        public static void AddCustomConnection(string name, string connectionString)
        {
            lock (_lock)
            {
                _connections[name] = connectionString;
                LogMessage($"Custom connection '{name}' added");
            }
        }

        public static bool RemoveConnection(string name)
        {
            lock (_lock)
            {
                if (name == "local" || name == "dual")
                {
                    LogMessage($"ERROR: Cannot remove predefined connection '{name}'");
                    return false;
                }

                if (_connections.ContainsKey(name))
                {
                    _connections.Remove(name);
                    if (_currentConnection == name)
                    {
                        _currentConnection = "local";
                        LogMessage($"Current connection reset to 'local' after removing '{name}'");
                    }
                    LogMessage($"Custom connection '{name}' removed");
                    return true;
                }
                return false;
            }
        }

        public static ConnectionInfo GetConnectionInfo(string connectionName = null)
        {
            connectionName = connectionName ?? _currentConnection;
            var connectionString = GetConnectionString(connectionName);

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                return new ConnectionInfo
                {
                    Name = connectionName,
                    DataSource = builder.DataSource,
                    InitialCatalog = builder.InitialCatalog,
                    IntegratedSecurity = builder.IntegratedSecurity,
                    UserID = builder.UserID,
                    IsActive = connectionName == _currentConnection,
                    LastTested = DateTime.MinValue // Will be updated when tested
                };
            }
            catch (Exception ex)
            {
                LogMessage($"Error parsing connection string for '{connectionName}': {ex.Message}");
                return new ConnectionInfo
                {
                    Name = connectionName,
                    DataSource = "Unknown",
                    InitialCatalog = "Unknown",
                    IsActive = connectionName == _currentConnection,
                    LastTested = DateTime.MinValue
                };
            }
        }

        private static void LogMessage(string message)
        {
            try
            {
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
                var logDir = Path.GetDirectoryName(_logPath);

                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                using (var fileStream = new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.Write(logMessage);
                    writer.Flush();
                }
            }
            catch
            {
                // Silent fail - logging errors shouldn't break the main flow
            }
        }
    }

    public class ConnectionInfo
    {
        public string Name { get; set; }
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string UserID { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastTested { get; set; }
        public bool TestResult { get; set; }
    }
}