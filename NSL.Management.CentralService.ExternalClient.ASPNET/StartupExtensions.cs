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
        public static ILoggingBuilder AddCentralServiceMetricsProvider(
            this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<CentralServiceMetricsProvider>();

            return builder;
        }

        public static ILoggingBuilder AddCentralServiceMetricsProvider(
            this ILoggingBuilder builder
            , TimeSpan delayReport)
        {
            builder.Services.AddSingleton<CentralServiceMetricsProvider>();

            return builder;
        }
    }

    public static partial class StartupExtensions
    {
        public static void AddCentralServiceReportOnUnhandledException(this IHost s, bool requireReport = true)
        {
            var lps = s.Services.GetRequiredService<IEnumerable<ILoggerProvider>>();

            var lp = lps.FirstOrDefault(x => x is CentralServiceLogProvider);

            if (lp == null)
                throw new Exception($"CentralServiceLogProvider not registered!!");

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = (e.ExceptionObject as Exception).ToString();

                var createTime = DateTime.UtcNow;

                (lp as CentralServiceLogProvider).EnqueueLog(new Data.Models.RequestModels.SyncReportLogDataModel() { Content = $"[-1]     APPLICATION - {ex}", CreateTime = DateTime.UtcNow, LogLevel = Data.Enums.LogLevelEnum.Critical });

                bool finished = false;

                do
                {
                    finished = WaitCentralServiceLoggerReport(s.Services);
                } while (!finished && requireReport);

                finished = false;

                do
                {
                    finished = WaitCentralServiceMetricsReport(s.Services);
                } while (!finished && requireReport);
            };
        }

        public static void AddCentralServiceReportOnClose(this IHost s, bool requireReport = true)
        {
            var lps = s.Services.GetRequiredService<IEnumerable<ILoggerProvider>>();

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                bool finished = false;

                do
                {
                    finished = WaitCentralServiceLoggerReport(s.Services);
                } while (!finished && requireReport);

                finished = false;

                do
                {
                    finished = WaitCentralServiceMetricsReport(s.Services);
                } while (!finished && requireReport);
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