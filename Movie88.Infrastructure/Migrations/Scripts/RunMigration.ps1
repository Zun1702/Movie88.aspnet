# Script to execute SQL migration on Supabase PostgreSQL
# Run: .\RunMigration.ps1

$connectionString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bkhexjwlqvjwfnncnaun;Password=Yeah@17022004;SSL Mode=Require;Trust Server Certificate=true"

Write-Host "Reading SQL script..." -ForegroundColor Cyan
$sqlScript = Get-Content -Path ".\Scripts\001_AddBookingCode.sql" -Raw

Write-Host "Connecting to Supabase PostgreSQL..." -ForegroundColor Cyan

try {
    # Load Npgsql
    Add-Type -Path "$env:USERPROFILE\.nuget\packages\npgsql\8.0.11\lib\net8.0\Npgsql.dll"
    
    # Create connection
    $conn = New-Object Npgsql.NpgsqlConnection($connectionString)
    $conn.Open()
    
    Write-Host "Connected successfully!" -ForegroundColor Green
    Write-Host "Executing migration script..." -ForegroundColor Cyan
    
    # Execute SQL
    $cmd = New-Object Npgsql.NpgsqlCommand($sqlScript, $conn)
    $cmd.CommandTimeout = 60
    $cmd.ExecuteNonQuery() | Out-Null
    
    Write-Host "Migration completed successfully!" -ForegroundColor Green
    
    $conn.Close()
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
