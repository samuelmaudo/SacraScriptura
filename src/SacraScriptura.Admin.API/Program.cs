using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SacraScriptura.Admin.Application;
using SacraScriptura.Admin.Domain;
using SacraScriptura.Admin.Infrastructure;
using SacraScriptura.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(); // https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAdminApplication();
builder.Services.AddAdminDomain();
builder.Services.AddAdminInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks()
       .AddNpgSql(
           builder.Configuration.GetConnectionString("DefaultConnection"),
           name: "database",
           failureStatus: HealthStatus.Unhealthy,
           tags: ["db", "postgresql"]
       );

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// Health check endpoint with detailed response
app.MapHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        ResponseWriter = async (
            context,
            report
        ) =>
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Status = report.Status.ToString(),
                Duration = report.TotalDuration,
                Info = report.Entries.Select(
                    e => new
                    {
                        Key = e.Key,
                        Status = e.Value.Status.ToString(),
                        Description = e.Value.Description,
                        Duration = e.Value.Duration
                    }
                )
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
).WithName("health").WithOpenApi();

app.Run();