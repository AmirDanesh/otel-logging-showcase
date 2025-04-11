using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace OTELSample.Configuration;

public static class OpenTelemetryConfig
{
    public static void AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService("OTELSample.WebAPI"))
            .WithTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(configuration["OpenTelemetry:OtlpEndpoint"]!);
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    //.AddAspNetCoreInstrumentation() //Requests Metric (Duration, Active, TotalCount, Size, TotalDuration)
                    //.AddHttpClientInstrumentation() //HTTPClient Metric (Duration, Active, TotalCount, Size, TotalDuration)
                    //.AddRuntimeInstrumentation() // (GC, Heap, JIT, Threds and ThreadPool)
                    .AddMeter("OTELSample.Metrics")
                    .AddConsoleExporter();
            });
    }
}

