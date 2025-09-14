<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html>
<html>
<head>
    <title>Explore dev.query Table Structure</title>
    <style>
        table { border-collapse: collapse; margin: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        .error { color: red; font-weight: bold; }
        .success { color: green; font-weight: bold; }
        .info { color: blue; }
        .query-text { font-family: Consolas, monospace; font-size: 10px; white-space: pre-wrap; max-width: 800px; overflow-wrap: break-word; }
    </style>
</head>
<body>
    <h1>Explore dev.query Table Structure</h1>
    
    <%
    string connectionString = WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];
    
    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            Response.Write("<p class='success'>Connessione al database AvanzamentoProduzione riuscita!</p>");
            
            // Test 1: Mostra struttura della tabella dev.query
            Response.Write("<h2>Test 1: Struttura tabella dev.query:</h2>");
            string queryColumns = @"
                SELECT 
                    COLUMN_NAME as 'Nome Colonna',
                    DATA_TYPE as 'Tipo Dati',
                    IS_NULLABLE as 'Nullable',
                    CHARACTER_MAXIMUM_LENGTH as 'Max Length'
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = 'dev' 
                AND TABLE_NAME = 'query'
                ORDER BY ORDINAL_POSITION";
            
            using (SqlCommand cmd = new SqlCommand(queryColumns, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    Response.Write("<tr><th>Nome Colonna</th><th>Tipo Dati</th><th>Nullable</th><th>Max Length</th></tr>");
                    while (reader.Read())
                    {
                        Response.Write("<tr>");
                        Response.Write($"<td><strong>{reader["Nome Colonna"]}</strong></td>");
                        Response.Write($"<td>{reader["Tipo Dati"]}</td>");
                        Response.Write($"<td>{reader["Nullable"]}</td>");
                        Response.Write($"<td>{reader["Max Length"] ?? "NULL"}</td>");
                        Response.Write("</tr>");
                    }
                    Response.Write("</table>");
                }
            }
            
            // Test 2: Mostra tutti i record per vedere la struttura
            Response.Write("<h2>Test 2: Primi 10 record della tabella dev.query:</h2>");
            string queryTop10 = "SELECT TOP 10 * FROM dev.query ORDER BY 1";
            
            using (SqlCommand cmd = new SqlCommand(queryTop10, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    
                    // Header
                    Response.Write("<tr>");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Response.Write($"<th>{reader.GetName(i)}</th>");
                    }
                    Response.Write("</tr>");
                    
                    // Data rows
                    while (reader.Read())
                    {
                        Response.Write("<tr>");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader[i] ?? "NULL";
                            if (reader.GetName(i).ToLower().Contains("query") && value.ToString().Length > 100)
                            {
                                // Accorcia le query lunghe
                                value = value.ToString().Substring(0, 100) + "...";
                            }
                            Response.Write($"<td>{value}</td>");
                        }
                        Response.Write("</tr>");
                    }
                    Response.Write("</table>");
                }
            }
            
            // Test 3: Cerca specificamente record con valore 516 in qualsiasi colonna
            Response.Write("<h2>Test 3: Cerca record che contengono '516':</h2>");
            string querySearch516 = @"
                SELECT * 
                FROM dev.query 
                WHERE CAST(idQuery as varchar) LIKE '%516%' 
                   OR (query IS NOT NULL AND query LIKE '%516%')";
            
            try
            {
                using (SqlCommand cmd = new SqlCommand(querySearch516, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Response.Write("<table>");
                        
                        // Header
                        Response.Write("<tr>");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Response.Write($"<th>{reader.GetName(i)}</th>");
                        }
                        Response.Write("</tr>");
                        
                        int count = 0;
                        while (reader.Read() && count < 20) // Limita a 20 risultati
                        {
                            Response.Write("<tr>");
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader[i] ?? "NULL";
                                if (reader.GetName(i).ToLower().Contains("query") && value.ToString().Length > 100)
                                {
                                    Response.Write($"<td class='query-text'>{value.ToString().Substring(0, 200)}...</td>");
                                }
                                else
                                {
                                    Response.Write($"<td>{value}</td>");
                                }
                            }
                            Response.Write("</tr>");
                            count++;
                        }
                        Response.Write("</table>");
                    }
                }
            }
            catch (Exception ex3)
            {
                Response.Write($"<p class='error'>Errore ricerca 516: {ex3.Message}</p>");
                
                // Prova senza CAST
                Response.Write("<h3>Prova senza CAST:</h3>");
                string querySimple = "SELECT TOP 5 * FROM dev.query WHERE idQuery = 516";
                try
                {
                    using (SqlCommand cmd = new SqlCommand(querySimple, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int count = 0;
                            while (reader.Read())
                            {
                                count++;
                                Response.Write($"<p>Record {count} trovato!</p>");
                            }
                            if (count == 0)
                            {
                                Response.Write("<p>Nessun record con idQuery = 516</p>");
                            }
                        }
                    }
                }
                catch (Exception ex4)
                {
                    Response.Write($"<p class='error'>Errore anche con query semplice: {ex4.Message}</p>");
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