
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
                ServerId = Guid.Parse("5fd5459e-bde2-45de-8b80-2b8670b0a3f4"),
                ServerToken = "33d65a1e-2e7e-4194-8b98-17e3d8f2665de1964618-cd2c-4dc9-9f83-a70b81628468",
            });

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

            app.AddCentralServiceLoggerReportOnClose();

            DevThrow();

            app.Run();
        }

        private static async void DevThrow()
        {
            await Task.Delay(3_000);

            throw new Exception(); 
        }
    }
}
