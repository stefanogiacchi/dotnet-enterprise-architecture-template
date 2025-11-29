@echo off
REM ======================================================
REM  Initialize folder structure for .NET Enterprise Architecture Template
REM  Run this from the ROOT of the repository
REM ======================================================

echo Creating src structure...

mkdir src
mkdir src\Api

mkdir src\Application
mkdir src\Application\Behaviors
mkdir src\Application\Commands
mkdir src\Application\Queries
mkdir src\Application\Common

mkdir src\Domain
mkdir src\Domain\Entities
mkdir src\Domain\ValueObjects
mkdir src\Domain\Events
mkdir src\Domain\Specifications

mkdir src\Infrastructure
mkdir src\Infrastructure\Persistence
mkdir src\Infrastructure\Services
mkdir src\Infrastructure\Repositories
mkdir src\Infrastructure\Migrations

mkdir src\Shared
mkdir src\Shared\Utils
mkdir src\Shared\Extensions
mkdir src\Shared\Constants

echo Creating tests structure...

mkdir tests
mkdir tests\UnitTests
mkdir tests\IntegrationTests

echo Creating docs structure...

mkdir docs
mkdir docs\architecture
mkdir docs\domain
mkdir docs\api

echo # Documentation>docs\README.md

echo Creating assets/images for banners & diagrams...

mkdir assets
mkdir assets\images

echo Done.
pause
