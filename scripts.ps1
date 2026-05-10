param (
  [Parameter(Mandatory=$true, Position=0)]
  [string]$Command,

  [Parameter(Mandatory=$false)]
  [Alias("n", "name")]
  [string]$MigrationName = "Init"
)

$InfrastructureProj = ".\src\L3.Infrastructure\"
$PresentationProj = ".\src\L4.Presentation\"
$DbContextName = "AppDbContext"

function Ensure-DotnetEf {
  $installed = dotnet tool list -g | Select-String "dotnet-ef"
  if (-not $installed) {
    Write-Host "=> Đang cài đặt dotnet-ef global..." -ForegroundColor Cyan
    dotnet tool install --global dotnet-ef
  }
}

function Ensure-ReportGenerator {
  $installed = dotnet tool list -g | Select-String "dotnet-reportgenerator-globaltool"
  if (-not $installed) {
    Write-Host "=> Đang cài đặt reportgenerator global..." -ForegroundColor Cyan
    dotnet tool install --global dotnet-reportgenerator-globaltool
  }
}

switch ($Command) {
  "build" {
    Write-Host "=> Building project..." -ForegroundColor Green
    dotnet build
  }
  "migrate" {
    Ensure-DotnetEf
    Write-Host "=> Adding migration: $MigrationName..." -ForegroundColor Green
    dotnet ef migrations add $MigrationName -p $InfrastructureProj -s $PresentationProj -c $DbContextName
  }
  "update" {
    Ensure-DotnetEf
    Write-Host "=> Updating database..." -ForegroundColor Green
    dotnet ef database update -p $InfrastructureProj -s $PresentationProj -c $DbContextName
  }
  "drop" {
    Ensure-DotnetEf
    Write-Host "=> Dropping database..." -ForegroundColor Red
    dotnet ef database drop -p $InfrastructureProj -s $PresentationProj -c $DbContextName -f
  }
  "seed" {
    Write-Host "=> Seeding data..." -ForegroundColor Green
    dotnet run --project $PresentationProj --seeding
  }
  "docker" {
    Write-Host "=> Starting Docker containers..." -ForegroundColor Green
    docker compose up -d
  }
  "docker-down" {
    Write-Host "=> Stopping Docker containers..." -ForegroundColor Yellow
    docker compose down
  }
  "reset" {
    Write-Host "=> Resetting database and migrations..." -ForegroundColor Magenta
    Ensure-DotnetEf

    $MigrationsDir = Join-Path $InfrastructureProj "Migrations"

    dotnet ef database drop -p $InfrastructureProj -s $PresentationProj -c $DbContextName -f

    if (Test-Path $MigrationsDir) {
      Write-Host "=> Removed old Migrations folder." -ForegroundColor Yellow
      Remove-Item -Recurse -Force $MigrationsDir
    }

    dotnet ef migrations add "Init" -p $InfrastructureProj -s $PresentationProj -c $DbContextName
    dotnet ef database update -p $InfrastructureProj -s $PresentationProj -c $DbContextName
    dotnet run --project $PresentationProj --seeding
  }
  "test" {
    Write-Host "=> Running tests & generating coverage report..." -ForegroundColor Green
    Ensure-ReportGenerator

    if (Test-Path "TestResults") { Remove-Item -Recurse -Force "TestResults" }
    if (Test-Path "coveragereport") { Remove-Item -Recurse -Force "coveragereport" }

    dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory ./TestResults
    reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
    Start-Process ".\coveragereport\index.html"
  }
  "dev" {
    Write-Host "=> Watching and running project..." -ForegroundColor Green
    dotnet watch run --project $PresentationProj
  }
  default {
    Write-Host "Lệnh không hợp lệ: $Command" -ForegroundColor Red
    Write-Host "Các lệnh hỗ trợ: build, migrate, update, drop, seed, docker, docker-down, reset, test, dev" -ForegroundColor Yellow
  }
}
