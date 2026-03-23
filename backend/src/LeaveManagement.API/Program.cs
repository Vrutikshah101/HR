using System.Text;
using LeaveManagement.API.Services;
using LeaveManagement.Application.Abstractions;
using LeaveManagement.Infrastructure.Auth;
using LeaveManagement.Infrastructure.DependencyInjection;
using LeaveManagement.Infrastructure.Persistence;
using LeaveManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:5001",
                "https://localhost:7001")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt configuration is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("LocalDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    if (app.Environment.IsDevelopment())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>();
        await seeder.SeedAsync(CancellationToken.None);
    }
}

app.Run();
