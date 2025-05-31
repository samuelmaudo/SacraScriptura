using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SacraScriptura.Shared.Infrastructure;
using SacraScriptura.Web.Application;
using SacraScriptura.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(); // https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddWebApplication();
builder.Services.AddWebInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);

// Add health checks
builder
    .Services.AddHealthChecks()
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

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

// Health check endpoint with detailed response
app.MapHealthChecks(
        "/health",
        new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Status = report.Status.ToString(),
                    Duration = report.TotalDuration,
                    Info = report.Entries.Select(e => new
                    {
                        Key = e.Key,
                        Status = e.Value.Status.ToString(),
                        Description = e.Value.Description,
                        Duration = e.Value.Duration,
                    }),
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            },
        }
    )
    .WithName("health")
    .WithOpenApi();

app.Run();
