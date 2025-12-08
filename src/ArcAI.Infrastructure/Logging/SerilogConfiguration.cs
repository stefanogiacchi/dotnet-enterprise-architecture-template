using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ArcAI.Infrastructure.Logging;

/// <summary>
/// Serilog configuration for structured logging.
/// Provides methods to configure Serilog for console, file, and Application Insights sinks.
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Creates a bootstrap logger for early application startup logging.
    /// Use this before the full configuration is loaded.
    /// </summary>
    public static void CreateBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        Log.Information("Starting up application...");
    }

    /// <summary>
    /// Configures Serilog with full settings from appsettings.json.
    /// Call this in Program.cs: builder.Host.UseSerilog(SerilogConfiguration.ConfigureLogger);
    /// </summary>
    public static void ConfigureLogger(HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration)
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "ArcAI")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/arcai-.log",
                rollingInterval: Serilog.RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10_000_000, // 10MB
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

        // Optional: Add Application Insights if configured and package is installed
        // Uncomment these lines if you have Serilog.Sinks.ApplicationInsights installed:
        /*
        var appInsightsKey = context.Configuration["ApplicationInsights:InstrumentationKey"];
        if (!string.IsNullOrWhiteSpace(appInsightsKey))
        {
            configuration.WriteTo.ApplicationInsights(
                appInsightsKey,
                TelemetryConverter.Traces);
        }
        */
    }

    /// <summary>
    /// Configures Serilog request logging middleware.
    /// Call this in Program.cs: app.UseSerilogRequestLogging(SerilogConfiguration.ConfigureRequestLogging);
    /// </summary>
    public static void ConfigureRequestLogging(Serilog.AspNetCore.RequestLoggingOptions options)
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : elapsed > 1000
                ? LogEventLevel.Warning
                : LogEventLevel.Information;

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());

            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value
                    ?? httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
            }

            if (httpContext.Request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", httpContext.Request.QueryString.Value);
            }
        };
    }

    /// <summary>
    /// Closes and flushes the Serilog logger.
    /// Call this at application shutdown: Log.CloseAndFlush();
    /// </summary>
    public static void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}