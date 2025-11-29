@echo off
REM ======================================================
REM   Initialize folder structure for .NET Enterprise Architecture Template
REM   This version includes .gitkeep files for Git tracking.
REM   Run this from the ROOT of the repository.
REM ======================================================

echo [SRC] Creating source code structure...

mkdir src
echo. > src\.gitkeep

mkdir src\Api
echo. > src\Api\.gitkeep

mkdir src\Application
echo. > src\Application\.gitkeep
mkdir src\Application\Behaviors
echo. > src\Application\Behaviors\.gitkeep
mkdir src\Application\Commands
echo. > src\Application\Commands\.gitkeep
mkdir src\Application\Queries
echo. > src\Application\Queries\.gitkeep
mkdir src\Application\Common
echo. > src\Application\Common\.gitkeep

mkdir src\Domain
echo. > src\Domain\.gitkeep
mkdir src\Domain\Entities
echo. > src\Domain\Entities\.gitkeep
mkdir src\Domain\ValueObjects
echo. > src\Domain\ValueObjects\.gitkeep
mkdir src\Domain\Events
echo. > src\Domain\Events\.gitkeep
mkdir src\Domain\Specifications
echo. > src\Domain\Specifications\.gitkeep

mkdir src\Infrastructure
echo. > src\Infrastructure\.gitkeep
mkdir src\Infrastructure\Persistence
echo. > src\Infrastructure\Persistence\.gitkeep
mkdir src\Infrastructure\Services
echo. > src\Infrastructure\Services\.gitkeep
mkdir src\Infrastructure\Repositories
echo. > src\Infrastructure\Repositories\.gitkeep
mkdir src\Infrastructure\Migrations
echo. > src\Infrastructure\Migrations\.gitkeep

mkdir src\Shared
echo. > src\Shared\.gitkeep
mkdir src\Shared\Utils
echo. > src\Shared\Utils\.gitkeep
mkdir src\Shared\Extensions
echo. > src\Shared\Extensions\.gitkeep
mkdir src\Shared\Constants
echo. > src\Shared\Constants\.gitkeep

echo [TESTS] Creating tests structure...

mkdir tests
echo. > tests\.gitkeep
mkdir tests\UnitTests
echo. > tests\UnitTests\.gitkeep
mkdir tests\IntegrationTests
echo. > tests\IntegrationTests\.gitkeep

echo [DOCS] Creating documentation structure...

mkdir docs
mkdir docs\architecture
echo. > docs\architecture\.gitkeep
mkdir docs\domain
echo. > docs\domain\.gitkeep
mkdir docs\api
echo. > docs\api\.gitkeep

REM Creazione del file README.md nella cartella docs
echo # Documentation>docs\README.md

echo [ASSETS] Creating assets/images for banners and diagrams...

mkdir assets
echo. > assets\.gitkeep
mkdir assets\images
echo. > assets\images\.gitkeep

echo.
echo Done. Folder structure for .NET Enterprise Architecture Template has been created and populated with .gitkeep files.
echo.
pause