using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Serilog basic setup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add MediatR (scans Application assembly)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("Application"));
});

// Controllers / Minimal API
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.MapControllers();

// Minimal health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
