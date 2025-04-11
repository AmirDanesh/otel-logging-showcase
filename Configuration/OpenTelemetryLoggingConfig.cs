using OpenTelemetry.Logs;

namespace OTELSample.Configuration;

public static class OpenTelemetryLoggingConfig
{
    public static void AddCustomOpenTelemetryLogger(this ILoggingBuilder loggingBuilder, IConfiguration configuration, IHostEnvironment env)
    {
        loggingBuilder.ClearProviders();

        loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
        loggingBuilder.AddFilter("System", LogLevel.Warning);
        loggingBuilder.AddFilter("OTELSample", LogLevel.Information);

        loggingBuilder.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;

            if (env.IsDevelopment())
            {
                options.AddConsoleExporter();
            }
            options.AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri(configuration["OpenTelemetry:OtlpEndpoint"]!);
            });
        });
    }
}
