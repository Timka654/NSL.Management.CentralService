using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace NSL.Management.CentralService.ExternalClient.ASPNET
{
    public static partial class StartupExtensions
    {
        public static IServiceCollection AddCentralServiceClient(
            this IServiceCollection collection, Func<CentralServiceClient> build)
        {
            collection.AddSingleton<CentralServiceClient>(s => build());

            return collection;
        }
    }

    public static partial class StartupExtensions
    {
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
    }

    public static partial class StartupExtensions
    {
        public static IServiceCollection AddCentralServiceMetricsProvider(
            this IServiceCollection services)
        {
            services.AddSingleton<CentralServiceMetricsProvider>();

            return services;
        }

        public static IServiceCollection AddCentralServiceMetricsProvider(
            this IServiceCollection services
            , TimeSpan delayReport)
        {
            services.AddSingleton<CentralServiceMetricsProvider>();

            return services;
        }
    }

    public static partial class StartupExtensions
    {
        public static void AddCentralServiceReportOnUnhandledException(this IHost s, int maxTryToReport = 2)
        {
            s.Services.AddCentralServiceReportOnUnhandledException(maxTryToReport);
        }

        public static void AddCentralServiceReportOnClose(this IHost s, int maxTryToReport = 2)
        {
            s.Services.AddCentralServiceReportOnClose(maxTryToReport);
        }

        /// <summary>
        /// Add unhandled exception handle for all available centralService providers can report before shutdown
        /// </summary>
        /// <param name="s"></param>
        /// <param name="requireReport"></param>
        public static void AddCentralServiceReportOnUnhandledException(this IServiceProvider s, int maxTryToReport = 2)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var lps = s.GetRequiredService<IEnumerable<ILoggerProvider>>();

                var lp = lps?.FirstOrDefault(x => x is CentralServiceLogProvider);
                if (lp != null)
                {
                    var ex = (e.ExceptionObject as Exception).ToString();

                    var createTime = DateTime.UtcNow;

                    (lp as CentralServiceLogProvider).EnqueueLog(new Data.Models.RequestModels.SyncReportLogDataModel() { Content = $"[-1]     APPLICATION - {ex}", CreateTime = DateTime.UtcNow, LogLevel = Data.Enums.LogLevelEnum.Critical });
                }

                TryReport(() => WaitCentralServiceLoggerReport(s), maxTryToReport);
                TryReport(() => WaitCentralServiceMetricsReport(s), maxTryToReport);
            };
        }

        /// <summary>
        /// Add process close handle for all available centralService providers can report before shutdown
        /// </summary>
        /// <param name="s"></param>
        /// <param name="requireReport"></param>
        public static void AddCentralServiceReportOnClose(this IServiceProvider s, int maxTryToReport = 2)
        {
            var lps = s.GetRequiredService<IEnumerable<ILoggerProvider>>();

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                TryReport(() => WaitCentralServiceLoggerReport(s), maxTryToReport);
                TryReport(() => WaitCentralServiceMetricsReport(s), maxTryToReport);
            };
        }
    }

    public static partial class StartupExtensions
    {
        public static bool WaitCentralServiceMetricsReport(this IServiceProvider s)
        {
            var lp = s.GetService<CentralServiceMetricsProvider>();

            if (lp == null)
                return true;

            return lp.WaitForReport();
        }

        public static async Task<bool> WaitCentralServiceMetricsReportAsync(this IServiceProvider s)
        {
            var lp = s.GetService<CentralServiceMetricsProvider>();

            if (lp == null)
                return true;

            return await (lp).WaitForReportAsync();
        }
    }

    public static partial class StartupExtensions
    {
        private static bool TryReport(Func<bool> report, int maxTry)
        {
            bool success = false;

            for (int i = 0; i < maxTry && !success; i++)
            {
                success = report();
            }

            return success;
        }

    }
    public static partial class StartupExtensions
    {
        public static bool WaitCentralServiceLoggerReport(this IServiceProvider s)
        {
            var lps = s.GetRequiredService<IEnumerable<ILoggerProvider>>();

            var lp = lps.FirstOrDefault(x => x is CentralServiceLogProvider);

            if (lp == null)
                return true;

            return (lp as CentralServiceLogProvider).WaitForReport();
        }

        public static async Task<bool> WaitCentralServiceLoggerReportAsync(this IServiceProvider s)
        {
            var lps = s.GetRequiredService<IEnumerable<ILoggerProvider>>();

            var lp = lps.FirstOrDefault(x => x is CentralServiceLogProvider);

            if (lp == null)
                return true;

            return await (lp as CentralServiceLogProvider).WaitForReportAsync();
        }
    }
}