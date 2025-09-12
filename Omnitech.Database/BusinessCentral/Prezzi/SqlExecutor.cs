using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using Dapper;

namespace Omnitech.Database.BusinessCentral.Prezzi
{
    /// <summary>
    /// Classe per eseguire query SQL personalizzate
    /// </summary>
    public static class SqlExecutor
    {
        #region Query che ritornano dati (SELECT)

        /// <summary>
        /// Esegue una query SQL e ritorna i risultati come lista dinamica
        /// </summary>
        public static List<dynamic> ExecuteQuery(string connectionString, string sqlQuery, object parameters = null)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query(sqlQuery, parameters).ToList();
            }
        }

        /// <summary>
        /// Esegue una query SQL e ritorna i risultati come lista tipizzata
        /// </summary>
        public static List<T> ExecuteQuery<T>(string connectionString, string sqlQuery, object parameters = null)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<T>(sqlQuery, parameters).ToList();
            }
        }

        /// <summary>
        /// Esegue una query SQL e ritorna il primo risultato
        /// </summary>
        public static T ExecuteQuerySingle<T>(string connectionString, string sqlQuery, object parameters = null)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.QuerySingleOrDefault<T>(sqlQuery, parameters);
            }
        }

        /// <summary>
        /// Esegue una query SQL e ritorna un valore scalare
        /// </summary>
        public static T ExecuteScalar<T>(string connectionString, string sqlQuery, object parameters = null)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.ExecuteScalar<T>(sqlQuery, parameters);
            }
        }

        #endregion

        #region Query che modificano dati (INSERT, UPDATE, DELETE)

        /// <summary>
        /// Esegue una query SQL di modifica (INSERT, UPDATE, DELETE) e ritorna il numero di righe interessate
        /// </summary>
        public static int ExecuteNonQuery(string connectionString, string sqlQuery, object parameters = null)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Execute(sqlQuery, parameters);
            }
        }

        /// <summary>
        /// Esegue una query INSERT e ritorna l'ID generato
        /// </summary>
        public static int ExecuteInsertWithIdentity(string connectionString, string sqlQuery, object parameters = null)
        {
            sqlQuery = sqlQuery.TrimEnd(';') + "; SELECT CAST(SCOPE_IDENTITY() as int)";
            
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.QuerySingle<int>(sqlQuery, parameters);
            }
        }

        #endregion

        #region Query con transazioni

        /// <summary>
        /// Esegue multiple query in una transazione
        /// </summary>
        public static bool ExecuteTransaction(string connectionString, List<SqlCommand> commands)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var cmd in commands)
                        {
                            connection.Execute(cmd.Query, cmd.Parameters, transaction);
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Stored Procedures

        /// <summary>
        /// Esegue una stored procedure
        /// </summary>
        public static List<T> ExecuteStoredProcedure<T>(string connectionString, string procedureName, object parameters = null)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        /// <summary>
        /// Esegue una stored procedure con parametri di output
        /// </summary>
        public static DynamicParameters ExecuteStoredProcedureWithOutput(string connectionString, string procedureName, DynamicParameters parameters)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return parameters;
            }
        }

        #endregion

        #region Utility

        /// <summary>
        /// Verifica se una tabella esiste
        /// </summary>
        public static bool TableExists(string connectionString, string tableName)
        {
            string sql = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = @TableName";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.ExecuteScalar<int>(sql, new { TableName = tableName }) > 0;
            }
        }

        /// <summary>
        /// Ottiene lo schema di una tabella
        /// </summary>
        public static List<ColumnInfo> GetTableSchema(string connectionString, string tableName)
        {
            string sql = @"
                SELECT 
                    COLUMN_NAME as ColumnName,
                    DATA_TYPE as DataType,
                    IS_NULLABLE as IsNullable,
                    CHARACTER_MAXIMUM_LENGTH as MaxLength
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @TableName
                ORDER BY ORDINAL_POSITION";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<ColumnInfo>(sql, new { TableName = tableName }).ToList();
            }
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Classe per rappresentare un comando SQL
    /// </summary>
    public class SqlCommand
    {
        public string Query { get; set; }
        public object Parameters { get; set; }
    }

    /// <summary>
    /// Classe per informazioni colonna
    /// </summary>
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public int? MaxLength { get; set; }
    }

    #endregion
}