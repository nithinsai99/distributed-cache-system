using Cache.Core.Interfaces;
using Cache.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console();

// If Seq URL is provided via configuration or env var, add Seq sink
var seqUrl = builder.Configuration.GetValue<string>("Seq:Url") ?? Environment.GetEnvironmentVariable("Seq__Url");
if (!string.IsNullOrWhiteSpace(seqUrl))
{
    loggerConfig = loggerConfig.WriteTo.Seq(seqUrl);
}

Log.Logger = loggerConfig.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// Configure Redis via env or appsettings
var redisConfig = builder.Configuration.GetValue<string>("Redis:Configuration") ?? Environment.GetEnvironmentVariable("Redis__Configuration") ?? "localhost:6379";
builder.Services.AddSingleton<ICacheService>(sp => new RedisCacheService(redisConfig));

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<RequestThrottlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
