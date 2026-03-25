param(
  [Parameter(Mandatory = $true)]
  [string]$ModuleName,

  [string]$Root = ".",

  [switch]$IncludeFrontend,

  [switch]$IncludeController
)

function To-PascalCase([string]$value) {
  if ([string]::IsNullOrWhiteSpace($value)) { return $value }
  $parts = $value -split "[^a-zA-Z0-9]" | Where-Object { $_ -ne "" }
  if ($parts.Count -eq 0) { return $value }
  return ($parts | ForEach-Object { $_.Substring(0,1).ToUpper() + $_.Substring(1).ToLower() }) -join ""
}

function To-KebabCase([string]$value) {
  if ([string]::IsNullOrWhiteSpace($value)) { return $value }
  $normalized = [regex]::Replace($value, "([a-z0-9])([A-Z])", '$1-$2')
  $normalized = $normalized -replace "[^a-zA-Z0-9]+", "-"
  $normalized = $normalized.Trim('-').ToLower()
  return $normalized
}

$modulePascal = To-PascalCase $ModuleName
$moduleKebab = To-KebabCase $ModuleName
$moduleLower = $modulePascal.Substring(0,1).ToLower() + $modulePascal.Substring(1)

$backendRoot = Join-Path $Root "backend/src"
$frontendRoot = Join-Path $Root "frontend/src"

$paths = @(
  Join-Path $backendRoot "Domain/Entities",
  Join-Path $backendRoot "Application/$modulePascal",
  Join-Path $backendRoot "Application/Abstractions",
  Join-Path $backendRoot "Infrastructure/Services",
  Join-Path $backendRoot "Infrastructure/Persistence/Configurations"
)

if ($IncludeController) {
  $paths += Join-Path $backendRoot "Api/Controllers"
}

if ($IncludeFrontend) {
  $paths += Join-Path $frontendRoot "features/$moduleKebab/pages"
}

$paths | ForEach-Object { New-Item -Path $_ -ItemType Directory -Force | Out-Null }

$entityPath = Join-Path $backendRoot "Domain/Entities/$modulePascal.cs"
$dtoPath = Join-Path $backendRoot "Application/$modulePascal/$($modulePascal)Dto.cs"
$serviceInterfacePath = Join-Path $backendRoot "Application/Abstractions/I$($modulePascal)Service.cs"
$servicePath = Join-Path $backendRoot "Infrastructure/Services/$($modulePascal)Service.cs"
$configPath = Join-Path $backendRoot "Infrastructure/Persistence/Configurations/$($modulePascal)Configuration.cs"

@"
namespace Domain.Entities;

public class $modulePascal
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
"@ | Set-Content $entityPath

@"
namespace Application.$modulePascal;

public sealed record $($modulePascal)Dto(
    Guid Id,
    string Name,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);
"@ | Set-Content $dtoPath

@"
using Application.$modulePascal;

namespace Application.Abstractions;

public interface I$($modulePascal)Service
{
    Task<IReadOnlyCollection<$($modulePascal)Dto>> GetAllAsync(CancellationToken cancellationToken);
    Task<$($modulePascal)Dto> CreateAsync(string name, CancellationToken cancellationToken);
}
"@ | Set-Content $serviceInterfacePath

@"
using Application.Abstractions;
using Application.$modulePascal;

namespace Infrastructure.Services;

public class $($modulePascal)Service : I$($modulePascal)Service
{
    public Task<IReadOnlyCollection<$($modulePascal)Dto>> GetAllAsync(CancellationToken cancellationToken)
    {
        // TODO: wire EF query
        IReadOnlyCollection<$($modulePascal)Dto> result = Array.Empty<$($modulePascal)Dto>();
        return Task.FromResult(result);
    }

    public Task<$($modulePascal)Dto> CreateAsync(string name, CancellationToken cancellationToken)
    {
        // TODO: wire EF create + save
        var created = new $($modulePascal)Dto(Guid.NewGuid(), name, true, DateTimeOffset.UtcNow);
        return Task.FromResult(created);
    }
}
"@ | Set-Content $servicePath

@"
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class $($modulePascal)Configuration : IEntityTypeConfiguration<$modulePascal>
{
    public void Configure(EntityTypeBuilder<$modulePascal> builder)
    {
        builder.ToTable("$($moduleKebab)s");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("char(36)");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();
    }
}
"@ | Set-Content $configPath

if ($IncludeController) {
  $controllerPath = Join-Path $backendRoot "Api/Controllers/$($modulePascal)Controller.cs"
  @"
using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/$moduleLower")]
public class $($modulePascal)Controller : ControllerBase
{
    private readonly I$($modulePascal)Service _service;

    public $($modulePascal)Controller(I$($modulePascal)Service service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var rows = await _service.GetAllAsync(cancellationToken);
        return Ok(rows);
    }
}
"@ | Set-Content $controllerPath
}

if ($IncludeFrontend) {
  $pagePath = Join-Path $frontendRoot "features/$moduleKebab/pages/$($modulePascal)Page.jsx"
  @"
export default function $($modulePascal)Page() {
  return (
    <section>
      <h2>$modulePascal</h2>
      <p>TODO: implement $modulePascal UI.</p>
    </section>
  );
}
"@ | Set-Content $pagePath
}

Write-Host "[new-module] Created module scaffold: $modulePascal"
Write-Host "[new-module] Backend entity: $entityPath"
Write-Host "[new-module] Application DTO: $dtoPath"
Write-Host "[new-module] Service interface: $serviceInterfacePath"
Write-Host "[new-module] Service implementation: $servicePath"
Write-Host "[new-module] EF configuration: $configPath"
if ($IncludeController) { Write-Host "[new-module] Controller created." }
if ($IncludeFrontend) { Write-Host "[new-module] Frontend page created." }
Write-Host "[new-module] Next: register DbSet, service DI, route, and migration."
