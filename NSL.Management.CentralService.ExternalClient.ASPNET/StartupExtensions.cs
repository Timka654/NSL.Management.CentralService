using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace NSL.Management.CentralService.ExternalClient.ASPNET
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCentalServiceClient(
            this IServiceCollection collection, Func<CentralServiceClient> build)
        {
            collection.AddSingleton<CentralServiceClient>(s => build());

            return collection;
        }

        public static ILoggingBuilder AddCentralServiceLogger(
            this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, CentralServiceLogProvider>((s) => new CentralServiceLogProvider(s)));

            return builder;
        }

        public static ILoggingBuilder AddCentralServiceLogger(
            this ILoggingBuilder builder
            , TimeSpan delayReport)
        {
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, CentralServiceLogProvider>((s) => new CentralServiceLogProvider(s, delayReport)));

            return builder;
        }

        public static void WaitCentralServiceLoggerReport(this IServiceProvider s)
        {
            var lp = s.GetRequiredService<ILoggerProvider>();

            (lp as CentralServiceLogProvider).WaitForReport();
        }

        public static void AddCentralServiceLoggerReportOnClose(this IHost s)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var logger = s.Services.GetRequiredService<ILoggerProvider>();

                var ex = (e.ExceptionObject as Exception).ToString();

                var createTime = DateTime.UtcNow;

                (logger as CentralServiceLogProvider).EnqueueLog(new Data.Models.RequestModels.SyncReportLogDataModel() { Content = $"[-1]     APPLICATION - {ex}", CreateTime = DateTime.UtcNow, LogLevel = Data.Enums.LogLevelEnum.Critical});

                WaitCentralServiceLoggerReport(s.Services);
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                WaitCentralServiceLoggerReport(s.Services);
            };
        }

        public static async void WaitCentralServiceLoggerReportAsync(this IServiceProvider s)
        {
            var lp = s.GetRequiredService<ILoggerProvider>();

            await (lp as CentralServiceLogProvider).WaitForReportAsync();
        }
    }
}