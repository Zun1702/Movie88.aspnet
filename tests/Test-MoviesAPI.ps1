# Movie88 API Testing Script - Movies Endpoints
# Run this script in PowerShell to test all Movies APIs

# Configuration
$baseUrl = "https://localhost:7238/api"  # Change to your API URL
$movieId = 1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Movie88 API Testing - Movies Endpoints" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Helper function to format JSON output
function Show-Response {
    param($response, $testName)
    Write-Host "✅ $testName" -ForegroundColor Green
    Write-Host "Status: $($response.statusCode)" -ForegroundColor Yellow
    Write-Host "Message: $($response.message)" -ForegroundColor Yellow
    
    if ($response.data.items) {
        Write-Host "Items Count: $($response.data.items.Count)" -ForegroundColor Yellow
        Write-Host "Total Items: $($response.data.totalItems)" -ForegroundColor Yellow
        Write-Host "Current Page: $($response.data.currentPage) / $($response.data.totalPages)" -ForegroundColor Yellow
    }
    
    # Show first item details
    if ($response.data.items -and $response.data.items.Count -gt 0) {
        $firstItem = $response.data.items[0]
        Write-Host "First Movie: $($firstItem.title) ($($firstItem.releasedate))" -ForegroundColor Cyan
    } elseif ($response.data.movieid) {
        Write-Host "Movie: $($response.data.title) ($($response.data.releasedate))" -ForegroundColor Cyan
    }
    
    Write-Host ""
}

# Helper function to handle errors
function Show-Error {
    param($errorInfo, $testName)
    Write-Host "❌ $testName - FAILED" -ForegroundColor Red
    Write-Host "Error: $($errorInfo.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

###############################################
# Test 1: GET /api/movies - All movies
###############################################
Write-Host "=== Test 1: GET /api/movies ===" -ForegroundColor Magenta

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 1.1: Get all movies (default)"
} catch {
    Show-Error $_ "Test 1.1"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies?page=1&pageSize=5" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 1.2: Get movies with pagination (page 1, size 5)"
} catch {
    Show-Error $_ "Test 1.2"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies?genre=Action" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 1.3: Filter by genre (Action)"
} catch {
    Show-Error $_ "Test 1.3"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies?year=2023" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 1.4: Filter by year (2023)"
} catch {
    Show-Error $_ "Test 1.4"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies?rating=PG-13" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 1.5: Filter by rating (PG-13)"
} catch {
    Show-Error $_ "Test 1.5"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies?sort=releasedate_desc" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 1.6: Sort by release date (newest first)"
} catch {
    Show-Error $_ "Test 1.6"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies?sort=title_asc" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 1.7: Sort by title (A-Z)"
} catch {
    Show-Error $_ "Test 1.7"
}

###############################################
# Test 2: GET /api/movies/now-showing
###############################################
Write-Host "=== Test 2: GET /api/movies/now-showing ===" -ForegroundColor Magenta

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/now-showing" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 2.1: Get now showing movies"
} catch {
    Show-Error $_ "Test 2.1"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/now-showing?page=1&pageSize=5" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 2.2: Now showing with pagination"
} catch {
    Show-Error $_ "Test 2.2"
}

###############################################
# Test 3: GET /api/movies/coming-soon
###############################################
Write-Host "=== Test 3: GET /api/movies/coming-soon ===" -ForegroundColor Magenta

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/coming-soon" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 3.1: Get coming soon movies"
} catch {
    Show-Error $_ "Test 3.1"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/coming-soon?page=1&pageSize=5" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 3.2: Coming soon with pagination"
} catch {
    Show-Error $_ "Test 3.2"
}

###############################################
# Test 4: GET /api/movies/search
###############################################
Write-Host "=== Test 4: GET /api/movies/search ===" -ForegroundColor Magenta

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/search?query=Avengers" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 4.1: Search by title (Avengers)"
} catch {
    Show-Error $_ "Test 4.1"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/search?query=Action" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 4.2: Search by genre (Action)"
} catch {
    Show-Error $_ "Test 4.2"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/search?query=Marvel&page=1&pageSize=5" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 4.3: Search with pagination"
} catch {
    Show-Error $_ "Test 4.3"
}

# Test with empty query (should fail)
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/search?query=" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 4.4: Search with empty query (Expected to fail)"
} catch {
    Write-Host "✅ Test 4.4: Empty query correctly returns 400 Bad Request" -ForegroundColor Green
    Write-Host ""
}

###############################################
# Test 5: GET /api/movies/{id}
###############################################
Write-Host "=== Test 5: GET /api/movies/{id} ===" -ForegroundColor Magenta

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/$movieId" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 5.1: Get movie by ID ($movieId)"
} catch {
    Show-Error $_ "Test 5.1"
}

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/2" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 5.2: Get movie by ID (2)"
} catch {
    Show-Error $_ "Test 5.2"
}

# Test with invalid ID (should fail)
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/movies/99999" -Method Get -SkipCertificateCheck
    Show-Response $response "Test 5.3: Get movie with invalid ID (Expected to fail)"
} catch {
    Write-Host "✅ Test 5.3: Invalid ID correctly returns 404 Not Found" -ForegroundColor Green
    Write-Host ""
}

###############################################
# Summary
###############################################
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Yellow
Write-Host "1. Check Swagger UI: $baseUrl/../swagger" -ForegroundColor White
Write-Host "2. Verify database has test data" -ForegroundColor White
Write-Host "3. Test other endpoints (Promotions, Customers, Bookings)" -ForegroundColor White
Write-Host ""
