<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html>
<html>
<head>
    <title>Test Query Manager - Execute Query 516</title>
    <style>
        table { border-collapse: collapse; margin: 20px; width: 100%; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        .error { color: red; font-weight: bold; }
        .success { color: green; font-weight: bold; }
        .info { color: blue; }
        .query-text { 
            font-family: Consolas, monospace; 
            font-size: 10px; 
            white-space: pre-wrap; 
            max-width: 100%; 
            overflow-wrap: break-word;
            background-color: #f8f8f8;
            padding: 10px;
            border: 1px solid #ddd;
        }
    </style>
</head>
<body>
    <h1>Test Query Manager - Execute Query 516 with Barcode 011378</h1>
    
    <%
    string connectionString = WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];
    string connectionStringAP = connectionString?.Replace("Initial Catalog=prezzi", "Initial Catalog=AvanzamentoProduzione")
                                               .Replace("Database=prezzi", "Database=AvanzamentoProduzione");
    
    try
    {
        using (SqlConnection conn = new SqlConnection(connectionStringAP ?? connectionString))
        {
            conn.Open();
            Response.Write("<p class='success'>Connessione al database AvanzamentoProduzione riuscita!</p>");
            
            // Step 1: Get query 516 with highest version
            Response.Write("<h2>Step 1: Get Query 516 (Latest Version):</h2>");
            string getQuerySql = @"
                SELECT TOP 1 query 
                FROM dev.query 
                WHERE idQry = 516 
                ORDER BY ver DESC";
            
            string query516 = null;
            using (SqlCommand cmd = new SqlCommand(getQuerySql, conn))
            {
                query516 = cmd.ExecuteScalar()?.ToString();
            }
            
            if (!string.IsNullOrEmpty(query516))
            {
                Response.Write("<p class='success'>Query 516 trovata!</p>");
                Response.Write($"<p><strong>Query Length:</strong> {query516.Length} characters</p>");
                Response.Write("<h3>Query SQL (primi 500 caratteri):</h3>");
                string previewQuery = query516.Length > 500 ? query516.Substring(0, 500) + "..." : query516;
                Response.Write($"<div class='query-text'>{Server.HtmlEncode(previewQuery)}</div>");
                
                // Step 2: Execute query with barcode 011378
                Response.Write("<h2>Step 2: Execute Query with Barcode 011378:</h2>");
                
                // Replace the DECLARE statement with the actual barcode value
                string processedQuery = query516.Replace("DECLARE @BarCode AS dbo.BarCode = '{0}'", "DECLARE @BarCode AS nvarchar(50) = '011378'")
                                               .Replace("DECLARE @BarCode AS dbo.BarCode", "DECLARE @BarCode AS nvarchar(50) = '011378'")
                                               .Replace("DECLARE @BarCode dbo.BarCode", "DECLARE @BarCode AS nvarchar(50) = '011378'");
                
                try
                {
                    using (SqlCommand cmdExec = new SqlCommand(processedQuery, conn))
                    {
                        cmdExec.CommandTimeout = 60; // 1 minute timeout
                        
                        using (SqlDataReader reader = cmdExec.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Response.Write("<p class='success'>Query eseguita con successo! Risultati trovati.</p>");
                                Response.Write("<h3>Column Names and Data Types:</h3>");
                                Response.Write("<table>");
                                Response.Write("<tr><th>Column Name</th><th>Data Type</th><th>Value</th><th>.NET Type</th></tr>");
                                
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    Type fieldType = reader.GetFieldType(i);
                                    object value = reader.IsDBNull(i) ? "NULL" : reader[i];
                                    string sqlType = reader.GetDataTypeName(i);
                                    
                                    Response.Write("<tr>");
                                    Response.Write($"<td><strong>{columnName}</strong></td>");
                                    Response.Write($"<td>{sqlType}</td>");
                                    Response.Write($"<td>{value}</td>");
                                    Response.Write($"<td>{fieldType.Name}</td>");
                                    Response.Write("</tr>");
                                }
                                Response.Write("</table>");
                            }
                            else
                            {
                                Response.Write("<p class='error'>Nessun risultato trovato per barcode 011378</p>");
                            }
                        }
                    }
                }
                catch (Exception execEx)
                {
                    Response.Write($"<p class='error'>Errore esecuzione query: {execEx.Message}</p>");
                    Response.Write($"<pre>{execEx.StackTrace}</pre>");
                }
            }
            else
            {
                Response.Write("<p class='error'>Query 516 non trovata!</p>");
            }
        }
    }
    catch (Exception ex)
    {
        Response.Write($"<p class='error'>Errore connessione: {ex.Message}</p>");
        Response.Write($"<pre>{ex.StackTrace}</pre>");
    }
    %>
</body>
</html>