param(
  [string]$ApiProject = "backend/src/Api/Api.csproj"
)

Write-Host "[seed] Run the API in Development to execute seeders."
if (-not (Test-Path $ApiProject)) {
  Write-Error "API project not found: $ApiProject"
  exit 1
}

dotnet run --project $ApiProject
