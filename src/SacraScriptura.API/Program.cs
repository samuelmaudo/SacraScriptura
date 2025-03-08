using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using SacraScriptura.Application;
using SacraScriptura.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(); // https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks()
       .AddNpgSql(
           builder.Configuration.GetConnectionString("DefaultConnection"),
           name: "database",
           failureStatus: HealthStatus.Unhealthy,
           tags: ["db", "postgresql"]
       );

// Configure Kestrel to listen on all interfaces
builder.WebHost.ConfigureKestrel(
    serverOptions =>
    {
        // HTTP
        serverOptions.ListenAnyIP(8080);

        // HTTPS
        serverOptions.ListenAnyIP(
            8443,
            listenOptions =>
            {
                listenOptions.UseHttps(
                    "/Users/samuelmaudo/.aspnet/https/localhost.pfx",
                    "local-development"
                );
            }
        );
    }
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