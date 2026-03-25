param(
  [string]$WorkingDir = "."
)

Write-Host "[bootstrap] Starting project bootstrap..."
Push-Location $WorkingDir

if (Test-Path ".env.example" -and -not (Test-Path ".env")) {
  Copy-Item ".env.example" ".env"
  Write-Host "[bootstrap] Created .env from .env.example"
}

Write-Host "[bootstrap] Next steps:"
Write-Host "1) Update .env values"
Write-Host "2) Start containers: docker compose up -d"
Write-Host "3) Run API migrations"

Pop-Location
