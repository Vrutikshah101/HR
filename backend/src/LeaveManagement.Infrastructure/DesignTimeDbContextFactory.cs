using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LeaveManagement.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "LeaveManagement.API"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is required for design-time DbContext creation.");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var serverVersion = new MySqlServerVersion(new Version(8, 4, 0));
        optionsBuilder.UseMySql(connectionString, serverVersion);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
