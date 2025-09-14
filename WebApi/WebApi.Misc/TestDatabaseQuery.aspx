<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html>
<html>
<head>
    <title>Test Database Columns</title>
    <style>
        table { border-collapse: collapse; margin: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        .error { color: red; font-weight: bold; }
        .success { color: green; font-weight: bold; }
    </style>
</head>
<body>
    <h1>Test Database Columns</h1>
    
    <%
    string connectionString = WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];
    
    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            Response.Write("<p class='success'>Connessione al database riuscita!</p>");
            
            // Test 1: Get column names from ordini.Ordini
            Response.Write("<h2>Colonne in ordini.Ordini:</h2>");
            string query1 = @"
                SELECT COLUMN_NAME, DATA_TYPE 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = 'ordini' AND TABLE_NAME = 'Ordini'
                ORDER BY ORDINAL_POSITION";
            
            using (SqlCommand cmd = new SqlCommand(query1, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    Response.Write("<tr><th>Nome Colonna</th><th>Tipo Dati</th></tr>");
                    while (reader.Read())
                    {
                        Response.Write($"<tr><td>{reader["COLUMN_NAME"]}</td><td>{reader["DATA_TYPE"]}</td></tr>");
                    }
                    Response.Write("</table>");
                }
            }
            
            // Test 2: Get column names from anagrafica.Stampi
            Response.Write("<h2>Colonne in anagrafica.Stampi:</h2>");
            string query2 = @"
                SELECT COLUMN_NAME, DATA_TYPE 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = 'anagrafica' AND TABLE_NAME = 'Stampi'
                ORDER BY ORDINAL_POSITION";
            
            using (SqlCommand cmd = new SqlCommand(query2, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    Response.Write("<tr><th>Nome Colonna</th><th>Tipo Dati</th></tr>");
                    while (reader.Read())
                    {
                        Response.Write($"<tr><td>{reader["COLUMN_NAME"]}</td><td>{reader["DATA_TYPE"]}</td></tr>");
                    }
                    Response.Write("</table>");
                }
            }
            
            // Test 3: Try to get one row to see actual data
            Response.Write("<h2>Test query su ordini.Ordini (TOP 1):</h2>");
            string query3 = "SELECT TOP 1 * FROM ordini.Ordini WHERE Barcode IS NOT NULL";
            
            using (SqlCommand cmd = new SqlCommand(query3, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    
                    // Get column names
                    Response.Write("<tr>");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Response.Write($"<th>{reader.GetName(i)}</th>");
                    }
                    Response.Write("</tr>");
                    
                    // Get data
                    if (reader.Read())
                    {
                        Response.Write("<tr>");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader[i];
                            Response.Write($"<td>{(value == DBNull.Value ? "NULL" : value.ToString())}</td>");
                        }
                        Response.Write("</tr>");
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