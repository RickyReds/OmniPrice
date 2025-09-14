<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html>
<html>
<head>
    <title>Test Simple Query - Just Execute Query 516</title>
    <style>
        .error { color: red; font-weight: bold; }
        .success { color: green; font-weight: bold; }
        .info { color: blue; }
    </style>
</head>
<body>
    <h1>Test Simple Query - Just Execute Query 516</h1>
    
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
            
            // Get query 516
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
                
                // Replace DECLARE statement with actual barcode
                string processedQuery = query516.Replace("DECLARE @BarCode AS dbo.BarCode = '{0}'", "DECLARE @BarCode AS nvarchar(50) = '011378'");
                
                try
                {
                    using (SqlCommand cmdExec = new SqlCommand(processedQuery, conn))
                    {
                        cmdExec.CommandTimeout = 120; // 2 minutes timeout
                        
                        using (SqlDataReader reader = cmdExec.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Response.Write("<p class='success'>Query eseguita con successo!</p>");
                                Response.Write($"<p><strong>Numero di colonne:</strong> {reader.FieldCount}</p>");
                                
                                // Show first 10 column names and values
                                Response.Write("<h3>Prime 10 colonne:</h3>");
                                for (int i = 0; i < Math.Min(10, reader.FieldCount); i++)
                                {
                                    string columnName = reader.GetName(i);
                                    object value = reader.IsDBNull(i) ? "NULL" : reader[i];
                                    Type fieldType = reader.GetFieldType(i);
                                    
                                    Response.Write($"<p><strong>{columnName}</strong> ({fieldType.Name}): {value}</p>");
                                }
                                
                                // Look for specific columns we're interested in
                                Response.Write("<h3>Colonne specifiche per Order mapping:</h3>");
                                string[] targetColumns = {"BarCode", "idCliente", "L", "P", "H", "Quantità", "Prezzo", "PrezzoAutomatico", "idStampo", "idMateriale", "idRAL"};
                                
                                foreach (string colName in targetColumns)
                                {
                                    try
                                    {
                                        object value = reader[colName];
                                        Type fieldType = reader.GetFieldType(reader.GetOrdinal(colName));
                                        Response.Write($"<p><strong>{colName}</strong> ({fieldType.Name}): {value ?? "NULL"}</p>");
                                    }
                                    catch (Exception colEx)
                                    {
                                        Response.Write($"<p class='error'><strong>{colName}</strong>: Colonna non trovata - {colEx.Message}</p>");
                                    }
                                }
                            }
                            else
                            {
                                Response.Write("<p class='error'>Nessun risultato per barcode 011378</p>");
                            }
                        }
                    }
                }
                catch (Exception execEx)
                {
                    Response.Write($"<p class='error'>Errore esecuzione query: {execEx.Message}</p>");
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
    }
    %>
</body>
</html>