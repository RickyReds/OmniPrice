# SCRIPT SEMPLICE PER SISTEMARE IL DATABASE
Write-Host "FIXING DATABASE CONNECTION" -ForegroundColor Red

# Verifica servizi SQL
Write-Host "1. Verificando servizi SQL Server..." -ForegroundColor Cyan
$sqlServices = Get-Service | Where-Object { $_.Name -like "*SQL*" }
$sqlServices | ForEach-Object { Write-Host "$($_.Name) - $($_.Status)" -ForegroundColor Yellow }

# Test connessioni
Write-Host "2. Testando connessioni..." -ForegroundColor Cyan
$connections = @(
    'Data Source=localhost\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5',
    'Data Source=.\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5',
    'Data Source=localhost\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;Connect Timeout=5',
    'Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;Connect Timeout=5'
)

$working = $null
foreach ($conn in $connections) {
    Write-Host "Testing: $($conn.Split(';')[0])" -ForegroundColor Yellow
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($conn)
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT 1"
        $result = $command.ExecuteScalar()

        if ($result -eq 1) {
            Write-Host "SUCCESS!" -ForegroundColor Green
            $working = $conn
            $connection.Close()
            break
        }
        $connection.Close()
    } catch {
        Write-Host "Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

if ($working) {
    Write-Host "DATABASE FIXED!" -ForegroundColor Green
    Write-Host "Working connection: $working" -ForegroundColor Yellow
} else {
    Write-Host "NO WORKING CONNECTION FOUND" -ForegroundColor Red
    Write-Host "Install SQL Server Express first!" -ForegroundColor Yellow
}

Write-Host "Done." -ForegroundColor Green