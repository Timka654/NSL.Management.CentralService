
using Microsoft.AspNetCore.Mvc;
using NSL.Management.CentralService.ExternalClient.ASPNET;

namespace NSL.Management.CentralService.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddCentralServiceLogger();

            //builder.Services.AddCentalServiceClient(() => new ExternalClient.CentralServiceClient()
            //{
            //    ServerId = Guid.Parse("34c2db00-feb0-4d1f-8c2f-fda5941799ba"),
            //    ServerToken = "11beaa3e-6849-4a81-8080-c48edc27c31232c877b7-0e5b-4728-86bf-e6740d35df10",
            //    BaseUrl = "http://localhost:5055/"
            //});

            builder.Services.AddCentralServiceClient(() => new ExternalClient.CentralServiceClient()
            {
                BaseUrl = "https://devcc.arrible.com/",
                ServerId = Guid.Parse("aa31fa1b-f543-4747-ae17-b4fb49761d21"),
                ServerToken = "65499a25-b97c-4493-892a-4d34a26787fd2e5b956f-e680-4428-9d7d-f61fe3519adb",
            });

            builder.Services.AddCentralServiceMetricsProvider();

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/debugLog", (HttpContext httpContext, [FromServices] ILogger logger) =>
            {
                logger.LogDebug("debugLog test!!");
            });

            app.AddCentralServiceReportOnClose();
            app.AddCentralServiceReportOnUnhandledException();

            DevMetrics(app.Services);

            DevThrow();

            app.Run();
        }

        private static async void DevMetrics(IServiceProvider serviceProvider)
        {
            var metrics = serviceProvider.GetRequiredService<CentralServiceMetricsProvider>();

            while (true)
            {
                metrics.EnqueueIncrementMetric("inc_splited_5s", 1, TimeSpan.FromSeconds(5));
                metrics.EnqueueIncrementMetric("inc_full_1", 1);
                metrics.EnqueueIsolateMetric("new_1", 1);

                await Task.Delay(200);
            }
        }

        private static async void DevThrow()
        {
            await Task.Delay(10_000);

            throw new Exception();
        }
    }
}
