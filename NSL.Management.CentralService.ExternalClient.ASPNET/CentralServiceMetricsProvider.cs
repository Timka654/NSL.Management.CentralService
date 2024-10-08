using NSL.Management.CentralService.ExternalClient.Data.Models.RequestModels;
using System.Collections.Concurrent;

namespace NSL.Management.CentralService.ExternalClient.ASPNET
{
    public class CentralServiceMetricsProvider
    {
        private readonly ConcurrentDictionary<string, DateTime> _increment =
            new(StringComparer.OrdinalIgnoreCase);

        internal static TimeSpan DefaultReportDelay = TimeSpan.FromSeconds(30);

        private readonly IServiceProvider serviceProvider;
        public TimeSpan delayTime { get; } = DefaultReportDelay;

        public CentralServiceMetricsProvider(IServiceProvider serviceProvider, TimeSpan? delayTime = null)
        {
            this.serviceProvider = serviceProvider;
            this.delayTime = delayTime ?? DefaultReportDelay;
            reportCycle();
        }

        private async void reportCycle()
        {
            while (true)
            {
                await Task.Delay(delayTime);

                metricReport();
            }
        }

        private async void metricReport()
        {
            var l = metrics;

            metrics = new ConcurrentBag<SyncReportMetricDataModel>();

            var result = await serviceProvider.GetRequiredService<CentralServiceClient>().MetricsReportAsync(new SyncReportMetricsRequestModel()
            {
                Metrics = l.ToArray()
            });

            if (!result)
            {
                foreach (var item in l)
                {
                    metrics.Add(item);
                }
            }
            else
            {
                latestReportResult = true;
            }

            waitToken?.Cancel();
            waitToken = null;
        }


        private ConcurrentBag<SyncReportMetricDataModel> metrics = new();

        private AutoResetEvent _clearLocker = new AutoResetEvent(true);

        internal void EnqueueMetric(SyncReportMetricDataModel record)
        {
            if (record.OperationType == MetricsOperationType.Increment)
            {
                if (_increment.TryGetValue(record.Name, out var old_time))
                {
                    if (record.ValidInterval.HasValue)
                    {
                        while (record.CreateTime - old_time < record.ValidInterval)
                        {
                            old_time = old_time + record.ValidInterval.Value;
                        }
                    }

                    record.CreateTime = old_time;
                }

                _increment.AddOrUpdate(record.Name, record.CreateTime, (n, o) => record.CreateTime);
            }

            metrics.Add(record);
        }

        public void Dispose()
            => metrics.Clear();

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

                metricReport();

                await Task.Delay(Timeout.Infinite, ow.Token);
            }
            catch (Exception)
            {
            }

            return latestReportResult;
        }
    }
}
