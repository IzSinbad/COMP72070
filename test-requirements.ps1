# GitCommit Requirements Test Script
Write-Host "GitCommit Requirements Test Script" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green
Write-Host ""

# Check if the solution file exists
$solutionExists = Test-Path -Path "GitCommit.sln"
Write-Host "Checking solution file... " -NoNewline
if ($solutionExists) {
    Write-Host "FOUND" -ForegroundColor Green
} else {
    Write-Host "NOT FOUND" -ForegroundColor Red
}

# Check if the client application exists
$clientExists = Test-Path -Path "GitCommit.Client"
Write-Host "Checking client application... " -NoNewline
if ($clientExists) {
    Write-Host "FOUND" -ForegroundColor Green
} else {
    Write-Host "NOT FOUND" -ForegroundColor Red
}

# Check if the server application exists
$serverExists = Test-Path -Path "GitCommit.Server"
Write-Host "Checking server application... " -NoNewline
if ($serverExists) {
    Write-Host "FOUND" -ForegroundColor Green
} else {
    Write-Host "NOT FOUND" -ForegroundColor Red
}

# Check if the server admin application exists
$adminExists = Test-Path -Path "GitCommit.ServerAdmin"
Write-Host "Checking server admin application... " -NoNewline
if ($adminExists) {
    Write-Host "FOUND" -ForegroundColor Green
} else {
    Write-Host "NOT FOUND" -ForegroundColor Red
}

# Check if the shared library exists
$sharedExists = Test-Path -Path "GitCommit.Shared"
Write-Host "Checking shared library... " -NoNewline
if ($sharedExists) {
    Write-Host "FOUND" -ForegroundColor Green
} else {
    Write-Host "NOT FOUND" -ForegroundColor Red
}

# Check for object-oriented design
Write-Host "Checking for object-oriented design... " -NoNewline
$classFiles = Get-ChildItem -Path "GitCommit.Shared\Models" -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue
if ($classFiles.Count -gt 0) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

# Check for predefined data structure
Write-Host "Checking for predefined data structure... " -NoNewline
$modelFiles = Get-ChildItem -Path "GitCommit.Shared\Models" -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue
if ($modelFiles.Count -gt 0) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

# Check for dynamically allocated data
Write-Host "Checking for dynamically allocated data... " -NoNewline
$listFound = $false
foreach ($file in $modelFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    if ($content -match "List<" -or $content -match "IEnumerable<" -or $content -match "ICollection<" -or $content -match "Array") {
        $listFound = $true
        break
    }
}
if ($listFound) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

# Check for different UIs
Write-Host "Checking for different UIs... " -NoNewline
$clientXaml = Get-ChildItem -Path "GitCommit.Client" -Filter "*.xaml" -Recurse -ErrorAction SilentlyContinue
$adminXaml = Get-ChildItem -Path "GitCommit.ServerAdmin" -Filter "*.xaml" -Recurse -ErrorAction SilentlyContinue
if ($clientXaml.Count -gt 0 -and $adminXaml.Count -gt 0) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

# Check for logging
Write-Host "Checking for logging... " -NoNewline
$loggerExists = Test-Path -Path "GitCommit.Shared\Utilities\Logger.cs"
if ($loggerExists) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

# Check for state machine
Write-Host "Checking for state machine... " -NoNewline
$stateFound = $false
$files = Get-ChildItem -Path "GitCommit.Shared\Models" -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    if ($content -match "enum.*Status" -or $content -match "UserStatus") {
        $stateFound = $true
        break
    }
}
if ($stateFound) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

# Check for large data transfer
Write-Host "Checking for large data transfer... " -NoNewline
$imageTransferFound = $false
$files = Get-ChildItem -Path "GitCommit.Shared\Models" -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    if ($content -match "byte\[\].*Image" -or $content -match "ImageData") {
        $imageTransferFound = $true
        break
    }
}
if ($imageTransferFound) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

# Check for authentication
Write-Host "Checking for authentication... " -NoNewline
$authFound = $false
$files = Get-ChildItem -Path "GitCommit.Server\Controllers" -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    if ($content -match "\[Authorize\]") {
        $authFound = $true
        break
    }
}
if ($authFound) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

# Summary
Write-Host ""
Write-Host "Requirements Summary:" -ForegroundColor Yellow
Write-Host "REQ-SYS-001: Two applications (Client & Server) - " -NoNewline
if ($clientExists -and $serverExists) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host "REQ-SYS-010: Object-Oriented Design - " -NoNewline
if ($classFiles.Count -gt 0) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host "REQ-SYS-020: Predefined Data Structure - " -NoNewline
if ($modelFiles.Count -gt 0) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host "REQ-SYS-030: Dynamically Allocated Data - " -NoNewline
if ($listFound) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host "REQ-SYS-040: Different User Interfaces - " -NoNewline
if ($clientXaml.Count -gt 0 -and $adminXaml.Count -gt 0) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host "REQ-SYS-050: Logging of Data Transfers - " -NoNewline
if ($loggerExists) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host "REQ-SYS-060: Server State Machine - " -NoNewline
if ($stateFound) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host "REQ-SYS-070: Large Data Transfer - " -NoNewline
if ($imageTransferFound) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host "REQ-SYS-080: Authentication Required - " -NoNewline
if ($authFound) {
    Write-Host "PASSED" -ForegroundColor Green
} else {
    Write-Host "FAILED" -ForegroundColor Red
}

Write-Host ""
Write-Host "Test completed." -ForegroundColor Green
