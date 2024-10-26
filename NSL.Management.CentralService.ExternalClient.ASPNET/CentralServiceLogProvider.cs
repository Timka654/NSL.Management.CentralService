using NSL.Management.CentralService.ExternalClient.Data.Models.RequestModels;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace NSL.Management.CentralService.ExternalClient.ASPNET
{
    public class CentralServiceLogProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, CentralServiceLogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);
        internal static TimeSpan DefaultReportDelay = TimeSpan.FromSeconds(30);

        private readonly IServiceProvider serviceProvider;
        public TimeSpan delayTime { get; } = DefaultReportDelay;

        long logId = 0;

        Timer reportTimer;

        public CentralServiceLogProvider(IServiceProvider serviceProvider, TimeSpan? delayTime = null)
        {
            this.serviceProvider = serviceProvider;
            this.delayTime = delayTime ?? DefaultReportDelay;
            reportTimer = new Timer((e) => logReport(), null, this.delayTime, this.delayTime);
        }

        private async void logReport()
        {
            var l = logs;

            logs = new ConcurrentBag<SyncReportLogDataModel>();

            var result = await serviceProvider.GetRequiredService<CentralServiceClient>().LogReportAsync(new SyncReportLogsRequestModel()
            {
                Logs = l.ToArray()
            });

            if (!result)
            {
                foreach (var item in l)
                {
                    logs.Add(item);
                }
            }
            else
            {
                latestReportResult = true;
            }

            waitToken?.Cancel();
            waitToken = null;
        }


        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new CentralServiceLogger(name, this));

        private ConcurrentBag<SyncReportLogDataModel> logs = new();

        private AutoResetEvent _clearLocker = new AutoResetEvent(true);

        internal void EnqueueLog(SyncReportLogDataModel record)
        {
            logs.Add(record);
        }

        public void Dispose()
            => _loggers.Clear();

        public bool WaitForReport()
        {
            var t = WaitForReportAsync();

            t.Wait();

            return t.Result;
        }

        private CancellationTokenSource? waitToken;

        private bool latestReportResult = false;

        public async Task<bool> WaitForReportAsync()
        {
            latestReportResult = false;
            try
            {
                var ow = waitToken;

                if (ow == null)
                    ow = waitToken = new CancellationTokenSource();

                logReport();

                await Task.Delay(Timeout.Infinite, ow.Token);
            }
            catch (Exception)
            {
            }

            return latestReportResult;
        }
    }
}