<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html>
<html>
<head>
    <title>Test RAL Database Structure</title>
    <style>
        table { border-collapse: collapse; margin: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        .error { color: red; font-weight: bold; }
        .success { color: green; font-weight: bold; }
        .info { color: blue; }
    </style>
</head>
<body>
    <h1>Test RAL Database Structure</h1>
    
    <%
    string connectionString = WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];
    
    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            Response.Write("<p class='success'>Connessione al database riuscita!</p>");
            
            // Test 1: Verifica esistenza tabella ScalaRAL
            Response.Write("<h2>Test 1: Verifica esistenza tabella anagrafica.ScalaRAL:</h2>");
            string queryCheckTable = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'anagrafica' 
                AND TABLE_NAME = 'ScalaRAL'";
            
            using (SqlCommand cmd = new SqlCommand(queryCheckTable, conn))
            {
                int tableExists = (int)cmd.ExecuteScalar();
                if (tableExists > 0)
                {
                    Response.Write("<p class='success'>Tabella anagrafica.ScalaRAL esiste!</p>");
                    
                    // Test 2: Mostra colonne della tabella ScalaRAL
                    Response.Write("<h2>Test 2: Colonne in anagrafica.ScalaRAL:</h2>");
                    string queryColumns = @"
                        SELECT 
                            COLUMN_NAME as 'Nome Colonna',
                            DATA_TYPE as 'Tipo Dati'
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_SCHEMA = 'anagrafica' 
                        AND TABLE_NAME = 'ScalaRAL'
                        ORDER BY ORDINAL_POSITION";
                    
                    using (SqlCommand cmdColumns = new SqlCommand(queryColumns, conn))
                    {
                        using (SqlDataReader reader = cmdColumns.ExecuteReader())
                        {
                            Response.Write("<table>");
                            Response.Write("<tr><th>Nome Colonna</th><th>Tipo Dati</th></tr>");
                            while (reader.Read())
                            {
                                Response.Write($"<tr>");
                                Response.Write($"<td>{reader["Nome Colonna"]}</td>");
                                Response.Write($"<td>{reader["Tipo Dati"]}</td>");
                                Response.Write($"</tr>");
                            }
                            Response.Write("</table>");
                        }
                    }
                    
                    // Test 3: Mostra alcuni dati dalla tabella ScalaRAL
                    Response.Write("<h2>Test 3: Dati di esempio da anagrafica.ScalaRAL:</h2>");
                    string queryData = "SELECT TOP 10 * FROM anagrafica.ScalaRAL ORDER BY 1";
                    
                    using (SqlCommand cmdData = new SqlCommand(queryData, conn))
                    {
                        using (SqlDataReader reader = cmdData.ExecuteReader())
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
                                    Response.Write($"<td>{reader[i] ?? "NULL"}</td>");
                                }
                                Response.Write("</tr>");
                            }
                            Response.Write("</table>");
                        }
                    }
                }
                else
                {
                    Response.Write("<p class='error'>Tabella anagrafica.ScalaRAL NON esiste!</p>");
                    
                    // Test 4: Cerca tabelle simili
                    Response.Write("<h2>Test 4: Ricerca tabelle con 'RAL' nel nome:</h2>");
                    string queryRALTables = @"
                        SELECT 
                            TABLE_SCHEMA,
                            TABLE_NAME
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME LIKE '%RAL%'
                        ORDER BY TABLE_SCHEMA, TABLE_NAME";
                    
                    using (SqlCommand cmdRAL = new SqlCommand(queryRALTables, conn))
                    {
                        using (SqlDataReader reader = cmdRAL.ExecuteReader())
                        {
                            Response.Write("<table>");
                            Response.Write("<tr><th>Schema</th><th>Nome Tabella</th></tr>");
                            while (reader.Read())
                            {
                                Response.Write($"<tr>");
                                Response.Write($"<td>{reader["TABLE_SCHEMA"]}</td>");
                                Response.Write($"<td>{reader["TABLE_NAME"]}</td>");
                                Response.Write($"</tr>");
                            }
                            Response.Write("</table>");
                        }
                    }
                }
            }
            
            // Test 5: Query di esempio con JOIN se la tabella esiste
            conn.Close();
            conn.Open();
            
            Response.Write("<h2>Test 5: Test query con JOIN (se possibile):</h2>");
            string queryTestJoin = @"
                SELECT TOP 3 
                    o.BarCode,
                    o.idRAL,
                    CASE 
                        WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'anagrafica' AND TABLE_NAME = 'ScalaRAL')
                        THEN 'JOIN possibile'
                        ELSE 'JOIN non possibile - tabella mancante'
                    END as StatusJoin
                FROM ordini.Ordini o
                WHERE o.idRAL IS NOT NULL 
                AND o.idRAL > 0
                ORDER BY o.idOrdine DESC";
            
            using (SqlCommand cmd = new SqlCommand(queryTestJoin, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    Response.Write("<tr><th>BarCode</th><th>idRAL</th><th>Status JOIN</th></tr>");
                    while (reader.Read())
                    {
                        Response.Write($"<tr>");
                        Response.Write($"<td>{reader["BarCode"]}</td>");
                        Response.Write($"<td>{reader["idRAL"]}</td>");
                        Response.Write($"<td>{reader["StatusJoin"]}</td>");
                        Response.Write($"</tr>");
                    }
                    Response.Write("</table>");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Response.Write($"<p class='error'>Errore: {ex.Message}</p>");
        Response.Write($"<pre>{ex.StackTrace}</pre>");
    }
    %>
</body>
</html>