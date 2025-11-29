<#
.SYNOPSIS
    Initializes the full folder structure for the .NET Enterprise Architecture Template.

.DESCRIPTION
    This script creates:
    - src structure (Api, Application, Domain, Infrastructure, Shared)
    - tests structure
    - documentation structure
    - assets/images folder
    It is safe to run multiple times and supports -Force to recreate missing folders.

.PARAMETER Force
    If specified, the script will recreate missing folders without prompts.

.EXAMPLE
    .\init-structure.ps1

.EXAMPLE
    .\init-structure.ps1 -Force
#>

param(
    [switch]$Force
)

function Write-Info($msg) {
    Write-Host "[INFO] $msg" -ForegroundColor Cyan
}

function Write-Success($msg) {
    Write-Host "[OK]   $msg" -ForegroundColor Green
}

function Write-ErrorMsg($msg) {
    Write-Host "[ERROR] $msg" -ForegroundColor Red
}

function New-SafeDirectory($path) {
    if (Test-Path $path) {
        Write-Info "Folder already exists: $path"
    } else {
        New-Item -ItemType Directory -Force -Path $path | Out-Null
        Write-Success "Created: $path"
    }
}

Write-Host "===============================================" -ForegroundColor Yellow
Write-Host " .NET Enterprise Architecture Template - Setup" -ForegroundColor Yellow
Write-Host "===============================================" -ForegroundColor Yellow

# Project Folders
$folders = @(
    "src",
    "src\Api",
    "src\Application",
    "src\Application\Behaviors",
    "src\Application\Commands",
    "src\Application\Queries",
    "src\Application\Common",
    "src\Domain",
    "src\Domain\Entities",
    "src\Domain\ValueObjects",
    "src\Domain\Events",
    "src\Domain\Specifications",
    "src\Infrastructure",
    "src\Infrastructure\Persistence",
    "src\Infrastructure\Services",
    "src\Infrastructure\Repositories",
    "src\Infrastructure\Migrations",
    "src\Shared",
    "src\Shared\Utils",
    "src\Shared\Extensions",
    "src\Shared\Constants",
    "tests",
    "tests\UnitTests",
    "tests\IntegrationTests",
    "docs",
    "docs\architecture",
    "docs\domain",
    "docs\api",
    "assets",
    "assets\images"
)

foreach ($folder in $folders) {
    New-SafeDirectory $folder
}

# Create docs/README.md if missing
$docsReadme = "docs\README.md"
if (-not (Test-Path $docsReadme) -or $Force) {
    "# Documentation" | Out-File $docsReadme -Encoding utf8
    Write-Success "Created: docs\README.md"
} else {
    Write-Info "docs/README.md already exists, skipping"
}

Write-Host ""
Write-Success "Folder structure initialized successfully."
Write-Host ""
Write-Host "You can now start adding your .NET projects." -ForegroundColor Cyan
Write-Host ""
