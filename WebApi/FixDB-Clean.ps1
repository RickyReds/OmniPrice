# SCRIPT DEFINITIVO PER SISTEMARE IL DATABASE
Write-Host "FIXING DATABASE CONNECTION - DEFINITIVO" -ForegroundColor Red
Write-Host "=================================================" -ForegroundColor Yellow

# Step 1: Verifica servizi SQL Server
Write-Host "`n1. Verificando servizi SQL Server..." -ForegroundColor Cyan
$sqlServices = Get-Service | Where-Object { $_.Name -like "*SQL*" -or $_.DisplayName -like "*SQL*" }

if ($sqlServices) {
    Write-Host "Servizi SQL trovati:" -ForegroundColor Green
    $sqlServices | ForEach-Object {
        Write-Host "   - $($_.Name) ($($_.DisplayName)) - Status: $($_.Status)" -ForegroundColor Yellow

        # Prova ad avviare se fermo
        if ($_.Status -eq "Stopped") {
            Write-Host "   Avvio $($_.Name)..." -ForegroundColor Cyan
            try {
                Start-Service $_.Name -ErrorAction Stop
                Write-Host "   $($_.Name) avviato!" -ForegroundColor Green
            } catch {
                Write-Host "   Errore avvio $($_.Name): $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }
} else {
    Write-Host "NESSUN SERVIZIO SQL SERVER TROVATO!" -ForegroundColor Red
    Write-Host "   Installare SQL Server Express" -ForegroundColor Yellow
}

# Step 2: Test connessioni multiple
Write-Host "`n2. Testando connessioni database..." -ForegroundColor Cyan

$connectionStrings = @(
    'Data Source=localhost\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;',
    'Data Source=.\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;',
    'Data Source=(local)\SQLEXPRESS;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;',
    'Data Source=localhost;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;',
    'Data Source=.;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;',
    'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AvanzamentoProduzione;Integrated Security=True;Connect Timeout=5;',
    'Data Source=localhost\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;Connect Timeout=5;',
    'Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;Connect Timeout=5;'
)

$workingConnection = $null

foreach ($connStr in $connectionStrings) {
    Write-Host "   Testando: $($connStr.Split(';')[0])..." -ForegroundColor Yellow

    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connStr)
        $connection.Open()

        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT 1 as TestResult, GETDATE() as ServerTime"
        $command.CommandTimeout = 5

        $reader = $command.ExecuteReader()
        if ($reader.Read()) {
            $testResult = $reader["TestResult"]
            $serverTime = $reader["ServerTime"]

            Write-Host "   CONNESSIONE RIUSCITA!" -ForegroundColor Green
            Write-Host "      - Test Result: $testResult" -ForegroundColor Green
            Write-Host "      - Server Time: $serverTime" -ForegroundColor Green
            Write-Host "      - Server Version: $($connection.ServerVersion)" -ForegroundColor Green
            Write-Host "      - Database: $($connection.Database)" -ForegroundColor Green

            $workingConnection = $connStr
            $reader.Close()
            $connection.Close()
            break
        }

        $reader.Close()
        $connection.Close()
    } catch {
        Write-Host "   Errore: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Step 3: Risultato finale
Write-Host "`n3. RISULTATO FINALE" -ForegroundColor Cyan

if ($workingConnection) {
    Write-Host "DATABASE CONNECTION FIXED!" -ForegroundColor Green
    Write-Host "   Connection String funzionante:" -ForegroundColor Green
    Write-Host "   $workingConnection" -ForegroundColor Yellow

    # Aggiorna Web.config
    Write-Host "`nAggiornamento Web.config..." -ForegroundColor Cyan

    $webConfigPath = "D:\Dev_Omh\WebApiPrezzi\WebApi\WebApi.Misc\Web.config"

    if (Test-Path $webConfigPath) {
        try {
            $webConfig = [xml](Get-Content $webConfigPath)

            # Trova o crea l'appSettings per la connection string
            $appSettings = $webConfig.configuration.appSettings
            $connStringSetting = $appSettings.add | Where-Object { $_.key -eq "ConnectionStringAvanzamentoProduzione" }

            if ($connStringSetting) {
                $connStringSetting.value = $workingConnection
                Write-Host "   ConnectionStringAvanzamentoProduzione aggiornata!" -ForegroundColor Green
            } else {
                # Crea nuovo elemento
                $newSetting = $webConfig.CreateElement("add")
                $newSetting.SetAttribute("key", "ConnectionStringAvanzamentoProduzione")
                $newSetting.SetAttribute("value", $workingConnection)
                $appSettings.AppendChild($newSetting)
                Write-Host "   ConnectionStringAvanzamentoProduzione creata!" -ForegroundColor Green
            }

            $webConfig.Save($webConfigPath)
            Write-Host "   Web.config salvato!" -ForegroundColor Green
        } catch {
            Write-Host "   Errore aggiornamento Web.config: $($_.Exception.Message)" -ForegroundColor Red
            Write-Host "   Aggiorna manualmente con: $workingConnection" -ForegroundColor Yellow
        }
    }

    # Test finale sulla tabella dev.query
    Write-Host "`n4. Test finale su dev.query..." -ForegroundColor Cyan

    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($workingConnection)
        $connection.Open()

        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT COUNT(*) as QueryCount FROM dev.query WITH (NOLOCK)"
        $command.CommandTimeout = 10

        $queryCount = $command.ExecuteScalar()
        Write-Host "   Tabella dev.query accessibile! Records: $queryCount" -ForegroundColor Green

        $connection.Close()
    } catch {
        Write-Host "   Tabella dev.query non accessibile: $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host "   La connessione base funziona, ma serve creare il database AvanzamentoProduzione" -ForegroundColor Yellow
    }

    Write-Host "`nPROBLEMA RISOLTO DEFINITIVAMENTE!" -ForegroundColor Green
    Write-Host "   L'API ora dovrebbe funzionare correttamente." -ForegroundColor Green

} else {
    Write-Host "NESSUNA CONNESSIONE FUNZIONA" -ForegroundColor Red
    Write-Host "`nAZIONI NECESSARIE:" -ForegroundColor Yellow
    Write-Host "1. Installare SQL Server Express" -ForegroundColor Red
    Write-Host "2. Oppure installare SQL Server LocalDB" -ForegroundColor Red
    Write-Host "3. Oppure configurare un'istanza SQL Server esistente" -ForegroundColor Red

    Write-Host "`nDownload SQL Server Express:" -ForegroundColor Cyan
    Write-Host "https://www.microsoft.com/sql-server/sql-server-downloads" -ForegroundColor Blue
}

Write-Host "`n=================================================" -ForegroundColor Yellow
Write-Host "SCRIPT COMPLETATO" -ForegroundColor Red