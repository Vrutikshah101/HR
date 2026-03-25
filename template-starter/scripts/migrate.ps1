param(
  [string]$ApiProject = "backend/src/Api/Api.csproj"
)

Write-Host "[migrate] Applying migrations..."
if (-not (Test-Path $ApiProject)) {
  Write-Error "API project not found: $ApiProject"
  exit 1
}

dotnet ef database update --project $ApiProject
