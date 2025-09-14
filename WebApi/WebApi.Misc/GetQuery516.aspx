<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html>
<html>
<head>
    <title>Get Query 516 - Latest Version</title>
    <style>
        table { border-collapse: collapse; margin: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        .error { color: red; font-weight: bold; }
        .success { color: green; font-weight: bold; }
        .info { color: blue; }
        .query-text { 
            font-family: Consolas, monospace; 
            font-size: 11px; 
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
    <h1>Get Query 516 - Latest Version</h1>
    
    <%
    string connectionString = WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];
    
    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            Response.Write("<p class='success'>Connessione al database AvanzamentoProduzione riuscita!</p>");
            
            // Test 1: Trova tutte le versioni di idQry 516
            Response.Write("<h2>Test 1: Tutte le versioni di idQry 516:</h2>");
            string queryVersions = @"
                SELECT 
                    id,
                    idQry,
                    ver,
                    active,
                    description,
                    LEN(query) as QueryLength,
                    DataCreazione,
                    DataModifica
                FROM dev.query 
                WHERE idQry = 516
                ORDER BY ver DESC";
            
            using (SqlCommand cmd = new SqlCommand(queryVersions, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    Response.Write("<tr><th>ID</th><th>idQry</th><th>Versione</th><th>Active</th><th>Descrizione</th><th>Query Length</th><th>Data Creazione</th><th>Data Modifica</th></tr>");
                    while (reader.Read())
                    {
                        Response.Write("<tr>");
                        Response.Write($"<td>{reader["id"]}</td>");
                        Response.Write($"<td>{reader["idQry"]}</td>");
                        Response.Write($"<td><strong>{reader["ver"]}</strong></td>");
                        Response.Write($"<td>{reader["active"]}</td>");
                        Response.Write($"<td>{reader["description"] ?? "NULL"}</td>");
                        Response.Write($"<td>{reader["QueryLength"]}</td>");
                        Response.Write($"<td>{reader["DataCreazione"] ?? "NULL"}</td>");
                        Response.Write($"<td>{reader["DataModifica"] ?? "NULL"}</td>");
                        Response.Write("</tr>");
                    }
                    Response.Write("</table>");
                }
            }
            
            // Test 2: Ottieni la query con versione maggiore
            Response.Write("<h2>Test 2: Query con versione maggiore per idQry 516:</h2>");
            string queryLatest = @"
                SELECT TOP 1
                    id,
                    idQry,
                    ver,
                    active,
                    description,
                    query,
                    notes,
                    DataCreazione,
                    DataModifica
                FROM dev.query
                WHERE idQry = 516
                ORDER BY ver DESC";
            
            using (SqlCommand cmd = new SqlCommand(queryLatest, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Response.Write($"<p><strong>ID:</strong> {reader["id"]}</p>");
                        Response.Write($"<p><strong>idQry:</strong> {reader["idQry"]}</p>");
                        Response.Write($"<p><strong>Versione:</strong> {reader["ver"]}</p>");
                        Response.Write($"<p><strong>Active:</strong> {reader["active"]}</p>");
                        Response.Write($"<p><strong>Descrizione:</strong> {reader["description"] ?? "NULL"}</p>");
                        Response.Write($"<p><strong>Note:</strong> {reader["notes"] ?? "NULL"}</p>");
                        Response.Write($"<p><strong>Data Creazione:</strong> {reader["DataCreazione"] ?? "NULL"}</p>");
                        Response.Write($"<p><strong>Data Modifica:</strong> {reader["DataModifica"] ?? "NULL"}</p>");
                        
                        Response.Write("<h3>Query SQL completa:</h3>");
                        string queryText = reader["query"]?.ToString() ?? "NULL";
                        Response.Write($"<div class='query-text'>{Server.HtmlEncode(queryText)}</div>");
                        
                        // Test 3: Se troviamo la query, proviamo a eseguirla con un barcode di esempio
                        if (!string.IsNullOrEmpty(queryText) && queryText != "NULL")
                        {
                            Response.Write("<h2>Test 3: Prova esecuzione query con barcode '011378':</h2>");
                            try
                            {
                                // Sostituisci i parametri nella query
                                string testQuery = queryText;
                                
                                // La query PsR potrebbe avere parametri diversi, proviamo alcune sostituzioni comuni
                                testQuery = testQuery.Replace("@barcode", "'011378'");
                                testQuery = testQuery.Replace("@Barcode", "'011378'");
                                testQuery = testQuery.Replace("@BarCode", "'011378'");
                                testQuery = testQuery.Replace("@P_BARCODE", "'011378'");
                                testQuery = testQuery.Replace("?", "'011378'");
                                
                                // Mostra la query che stiamo per eseguire (primi 500 caratteri)
                                Response.Write("<h4>Query che verrà eseguita (primi 500 caratteri):</h4>");
                                string previewQuery = testQuery.Length > 500 ? testQuery.Substring(0, 500) + "..." : testQuery;
                                Response.Write($"<div class='query-text'>{Server.HtmlEncode(previewQuery)}</div>");
                            }
                            catch (Exception testEx)
                            {
                                Response.Write($"<p class='error'>Errore preparazione test query: {testEx.Message}</p>");
                            }
                        }
                    }
                    else
                    {
                        Response.Write("<p class='error'>Nessuna query trovata con idQry = 516</p>");
                    }
                }
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