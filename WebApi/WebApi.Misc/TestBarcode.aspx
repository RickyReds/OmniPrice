<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html>
<html>
<head>
    <title>Test Barcode Query</title>
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
    <h1>Test Barcode Query</h1>
    
    <%
    string connectionString = WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];
    string barcodeToTest = "011378";
    
    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            Response.Write("<p class='success'>Connessione al database riuscita!</p>");
            
            // Test 1: Cerca barcode esatti
            Response.Write($"<h2>Test 1: Cerca barcode '{barcodeToTest}':</h2>");
            string query1 = $@"
                SELECT TOP 5 
                    BarCode,
                    LEN(BarCode) as Lunghezza,
                    LTRIM(RTRIM(BarCode)) as BarcodeTrimmed,
                    idOrdine,
                    Modello
                FROM ordini.Ordini 
                WHERE BarCode = '{barcodeToTest}'";
            
            using (SqlCommand cmd = new SqlCommand(query1, conn))
            {
                Response.Write($"<p class='info'>Query: {query1}</p>");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    Response.Write("<tr><th>BarCode</th><th>Lunghezza</th><th>BarcodeTrimmed</th><th>idOrdine</th><th>Modello</th></tr>");
                    int count = 0;
                    while (reader.Read())
                    {
                        Response.Write($"<tr>");
                        Response.Write($"<td>'{reader["BarCode"]}'</td>");
                        Response.Write($"<td>{reader["Lunghezza"]}</td>");
                        Response.Write($"<td>'{reader["BarcodeTrimmed"]}'</td>");
                        Response.Write($"<td>{reader["idOrdine"]}</td>");
                        Response.Write($"<td>{reader["Modello"]}</td>");
                        Response.Write($"</tr>");
                        count++;
                    }
                    Response.Write("</table>");
                    Response.Write($"<p>Trovati {count} record con barcode esatto '{barcodeToTest}'</p>");
                }
            }
            
            // Test 2: Cerca barcode che contengono la stringa
            Response.Write($"<h2>Test 2: Cerca barcode che contengono '11378':</h2>");
            string query2 = @"
                SELECT TOP 10 
                    BarCode,
                    LEN(BarCode) as Lunghezza,
                    LTRIM(RTRIM(BarCode)) as BarcodeTrimmed,
                    idOrdine,
                    Modello
                FROM ordini.Ordini 
                WHERE BarCode LIKE '%11378%'
                ORDER BY idOrdine DESC";
            
            using (SqlCommand cmd = new SqlCommand(query2, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    Response.Write("<tr><th>BarCode</th><th>Lunghezza</th><th>BarcodeTrimmed</th><th>idOrdine</th><th>Modello</th></tr>");
                    while (reader.Read())
                    {
                        Response.Write($"<tr>");
                        Response.Write($"<td>'{reader["BarCode"]}'</td>");
                        Response.Write($"<td>{reader["Lunghezza"]}</td>");
                        Response.Write($"<td>'{reader["BarcodeTrimmed"]}'</td>");
                        Response.Write($"<td>{reader["idOrdine"]}</td>");
                        Response.Write($"<td>{reader["Modello"]}</td>");
                        Response.Write($"</tr>");
                    }
                    Response.Write("</table>");
                }
            }
            
            // Test 3: Mostra alcuni barcode validi
            Response.Write("<h2>Test 3: Alcuni barcode validi nel database:</h2>");
            string query3 = @"
                SELECT TOP 10 
                    BarCode,
                    LEN(BarCode) as Lunghezza,
                    LTRIM(RTRIM(BarCode)) as BarcodeTrimmed,
                    idOrdine,
                    Modello
                FROM ordini.Ordini 
                WHERE BarCode IS NOT NULL 
                    AND BarCode != ''
                    AND BarCode != '0'
                ORDER BY idOrdine DESC";
            
            using (SqlCommand cmd = new SqlCommand(query3, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Response.Write("<table>");
                    Response.Write("<tr><th>BarCode</th><th>Lunghezza</th><th>BarcodeTrimmed</th><th>idOrdine</th><th>Modello</th></tr>");
                    while (reader.Read())
                    {
                        Response.Write($"<tr>");
                        Response.Write($"<td>'{reader["BarCode"]}'</td>");
                        Response.Write($"<td>{reader["Lunghezza"]}</td>");
                        Response.Write($"<td>'{reader["BarcodeTrimmed"]}'</td>");
                        Response.Write($"<td>{reader["idOrdine"]}</td>");
                        Response.Write($"<td>{reader["Modello"]}</td>");
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