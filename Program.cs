using OTELSample.Configuration;

namespace OTELSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCustomOpenTelemetry(builder.Configuration);
            builder.Logging.AddCustomOpenTelemetryLogger(builder.Configuration, builder.Environment);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.MapControllers();
            app.Run();
        }
    }
}
