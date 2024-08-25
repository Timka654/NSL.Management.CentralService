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
            builder.Services.AddSingleton<ILoggerProvider>((s) => new CentralServiceLogProvider(s));

            return builder;
        }

        public static ILoggingBuilder AddCentralServiceLogger(
            this ILoggingBuilder builder
            , TimeSpan delayReport)
        {
            builder.Services.AddSingleton<ILoggerProvider>((s) => new CentralServiceLogProvider(s, delayReport));

            return builder;
        }

        public static void WaitCentralServiceLoggerReport(this IServiceProvider s)
        {
            var lps = s.GetRequiredService<IEnumerable<ILoggerProvider>>();

            var lp = lps.FirstOrDefault(x => x is CentralServiceLogProvider);

            if (lp == null)
                throw new Exception($"CentralServiceLogProvider not registered!!");

            (lp as CentralServiceLogProvider).WaitForReport();
        }

        public static void AddCentralServiceLoggerReportOnClose(this IHost s)
        {
            var lps = s.Services.GetRequiredService<IEnumerable<ILoggerProvider>>();

            var lp = lps.FirstOrDefault(x => x is CentralServiceLogProvider);

            if (lp == null)
                throw new Exception($"CentralServiceLogProvider not registered!!");

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = (e.ExceptionObject as Exception).ToString();

                var createTime = DateTime.UtcNow;

                (lp as CentralServiceLogProvider).EnqueueLog(new Data.Models.RequestModels.SyncReportLogDataModel() { Content = $"[-1]     APPLICATION - {ex}", CreateTime = DateTime.UtcNow, LogLevel = Data.Enums.LogLevelEnum.Critical});

                WaitCentralServiceLoggerReport(s.Services);
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                (lp as CentralServiceLogProvider).WaitForReport();
            };
        }

        public static async void WaitCentralServiceLoggerReportAsync(this IServiceProvider s)
        {
            var lps = s.GetRequiredService<IEnumerable<ILoggerProvider>>();

            var lp = lps.FirstOrDefault(x => x is CentralServiceLogProvider);

            if (lp == null)
                throw new Exception($"CentralServiceLogProvider not registered!!");

            await (lp as CentralServiceLogProvider).WaitForReportAsync();
        }
    }
}