# PowerShell script to test the API
$baseUrl = "http://localhost:54340"
$barcode = "TEST123456"

try {
    Write-Host "Testing API endpoint: $baseUrl/api/v2/price/calculate/$barcode" -ForegroundColor Yellow
    
    $response = Invoke-RestMethod -Uri "$baseUrl/api/v2/price/calculate/$barcode" -Method Get -ErrorAction Stop
    
    Write-Host "Success! Response:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10
}
catch {
    Write-Host "Error occurred:" -ForegroundColor Red
    Write-Host $_.Exception.Message
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response body:" -ForegroundColor Red
        Write-Host $responseBody
    }
}

Write-Host "`nChecking log file..." -ForegroundColor Yellow
if (Test-Path "C:\WebApiLog\Price\LogWebApiPrice.txt") {
    Write-Host "Last 50 lines of log:" -ForegroundColor Cyan
    Get-Content "C:\WebApiLog\Price\LogWebApiPrice.txt" -Tail 50
}
else {
    Write-Host "Log file not found" -ForegroundColor Red
}