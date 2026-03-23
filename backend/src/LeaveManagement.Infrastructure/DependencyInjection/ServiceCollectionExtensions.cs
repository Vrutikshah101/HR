using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Leaves;
using LeaveManagement.Infrastructure.Auth;
using LeaveManagement.Infrastructure.Persistence;
using LeaveManagement.Infrastructure.Security;
using LeaveManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        var serverVersion = new MySqlServerVersion(new Version(8, 4, 0));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        var jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);
        services.AddSingleton(Options.Create(jwtOptions));

        var leaveOptions = new LeaveOptions();
        configuration.GetSection(LeaveOptions.SectionName).Bind(leaveOptions);
        services.AddSingleton(Options.Create(leaveOptions));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IHierarchyService, HierarchyService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        services.AddScoped<ILeaveWorkflowService, LeaveWorkflowService>();
        services.AddScoped<INotificationService, LoggingNotificationService>();
        services.AddScoped<DevelopmentDataSeeder>();

        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
