using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NSL.Database.EntityFramework.Filter;
using NSL.Database.EntityFramework.Filter.Enums;
using NSL.Database.EntityFramework.Filter.Models;
using NSL.Management.CentralService.Client.Services;
using NSL.Management.CentralService.ExternalClient.Data.Enums;
using NSL.Management.CentralService.Shared.Models;

namespace NSL.Management.CentralService.Client.Pages.Server
{
    public partial class DetailsPage : ComponentBase, IDisposable
    {
        [Parameter] public Guid DetailId { get; set; }

        [Inject] public ServersService ServersService { get; set; }
        [Inject] public ToastService ToastService { get; set; }


        ServerModel Details { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var response = await ServersService.ServerGetDetailsPostRequest(DetailId);

            if (!response.IsSuccess)
                return;

            Details = response.Data;

        }

        #region Log

        List<ServerLogModel> Logs { get; set; }

        long logsCount;

        long newLogsCount;

        bool haveNextLogs => logsCount != newLogsCount;

        bool havePrevLogs => logsCount > Logs.Count;

        DateTime? filterLogsFrom = null;

        DateTime? filterLogsTo = null;

        LogLevelEnum? filterLogsLevel = null;

        string? searchLogsText = null;
        CompareType? searchLogsTextType = CompareType.ContainsIgnoreCase;

        bool HaveLogsFilter => filterLogsFrom != null || filterLogsTo != null || filterLogsLevel != null || !string.IsNullOrWhiteSpace(searchLogsText);

        async Task clearLogsFilter()
        {
            filterLogsFrom = null;
            filterLogsTo = null;
            filterLogsLevel = null;
            searchLogsText = null;

            await _loadLogs();
        }

        async Task LoadLogs()
        {
            if (Logs != null)
                return;

            await _loadLogs();

            logsNewCountGetting();
        }

        private const int loadLogsCount = 100;

        async Task _loadLogs()
        {
            var filter = EntityFilterBuilder
                .Create()
                .SetOffset(0)
                .SetCount(loadLogsCount)
                .CreateFilterBlock(x => SetLogsFilters(x).AddFilter(nameof(ServerLogModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerLogModel.CreateTime), false);

            var response = await ServersService.LogGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            newLogsCount = logsCount = response.Data.Count;

            Logs = response.Data.Data.Reverse().ToList();
        }

        async Task clearLogItems()
        {
            var filter = EntityFilterBuilder
                .Create()
                .CreateFilterBlock(x => SetLogsFilters(x).AddFilter(nameof(ServerLogModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerLogModel.CreateTime), false);

            var response = await ServersService.LogClearPostRequest(filter);

            if (!response.IsSuccess)
                return;

            ToastService.Notify(new ToastMessage() { Message = $"Success removed {response.Data}", Type = ToastType.Success, Title = "Clear action", AutoHide = true, IconName = IconName.Info });
        }


        private async Task logsLoadPrev()
        {
            var newOffset = logsCount - Logs.Count - loadLogsCount;

            var count = (long)loadLogsCount;

            if (newOffset < 0)
            {
                count += newOffset;
                newOffset = 0;
            }

            var filter = EntityFilterBuilder
                .Create()
                .SetOffset((int)newOffset)
                .SetCount((int)count)
                .CreateFilterBlock(x => SetLogsFilters(x).AddFilter(nameof(ServerLogModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerLogModel.CreateTime));

            var response = await ServersService.LogGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            newLogsCount = response.Data.Count;

            Logs = response.Data.Data.Concat(Logs).ToList();
        }

        private async Task logsLoadNext()
        {
            var nlc = newLogsCount;

            var filter = EntityFilterBuilder
                .Create()
                .SetOffset((int)logsCount)
                .SetCount((int)(nlc - logsCount))
                .CreateFilterBlock(x => SetLogsFilters(x).AddFilter(nameof(ServerLogModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerLogModel.CreateTime));

            var response = await ServersService.LogGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            logsCount = nlc;

            newLogsCount = response.Data.Count;

            Logs = Logs.Concat(response.Data.Data).ToList();
        }

        private EntityFilterBlockModel SetLogsFilters(EntityFilterBlockModel block)
        {
            if (HaveLogsFilter)
            {
                if (filterLogsFrom.HasValue)
                    block.AddFilter(nameof(ServerLogModel.CreateTime), Database.EntityFramework.Filter.Enums.CompareType.More, filterLogsFrom.Value.ToUniversalTime());
                if (filterLogsTo.HasValue)
                    block.AddFilter(nameof(ServerLogModel.CreateTime), Database.EntityFramework.Filter.Enums.CompareType.Less, filterLogsTo.Value.ToUniversalTime());
                if (filterLogsLevel.HasValue)
                    block.AddFilter(nameof(ServerLogModel.LogLevel), Database.EntityFramework.Filter.Enums.CompareType.Equals, filterLogsLevel.Value);
                if (!string.IsNullOrWhiteSpace(searchLogsText))
                    block.AddFilter(nameof(ServerLogModel.Content), searchLogsTextType ?? CompareType.ContainsIgnoreCase, searchLogsText);
            }

            return block;
        }

        private async void logsNewCountGetting()
        {
            try
            {
                logsCTS = new CancellationTokenSource();
                var token = logsCTS.Token;
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(5_000, token);

                    var response = await ServersService.LogGetCountPostRequest(DetailId);

                    if (response.IsSuccess)
                    {
                        newLogsCount = response.Data;

                        if (haveNextLogs)
                        {
                            StateHasChanged();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private CancellationTokenSource? logsCTS;

        #endregion

        #region Metric

        List<ServerMetricsModel> Metrics { get; set; }

        long metricsCount;

        long newMetricsCount;

        bool haveNextMetrics => metricsCount != newMetricsCount;

        bool havePrevMetrics => metricsCount > Metrics.Count;

        DateTime? filterMetricsFrom = null;

        DateTime? filterMetricsTo = null;

        string? filterMetricsName = null;
        CompareType? filterMetricsNameType = CompareType.ContainsIgnoreCase;
        long metricsCalcDisplayNumber = 0;
        string metricsCalcDisplayTitle = "";
        Modal? metricsCalcModalRef = null;

        bool HaveMetricsFilter => filterMetricsFrom != null || filterMetricsTo != null || !string.IsNullOrWhiteSpace(filterMetricsName);

        async Task clearMetricsFilter()
        {
            filterMetricsFrom = null;
            filterMetricsTo = null;
            filterMetricsName = null;
            await _loadMetrics();
        }

        async Task LoadMetrics()
        {
            if (Metrics != null)
                return;

            await _loadMetrics();

            metricsNewCountGetting();
        }

        private const int loadMetricsCount = 100;

        async Task _loadMetrics()
        {
            var filter = EntityFilterBuilder
                .Create()
                .SetOffset(0)
                .SetCount(loadMetricsCount)
                .CreateFilterBlock(x => SetMetricsFilters(x).AddFilter(nameof(ServerMetricsModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerMetricsModel.CreateTime), false);

            var response = await ServersService.MetricGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            newMetricsCount = metricsCount = response.Data.Count;

            Metrics = response.Data.Data.Reverse().ToList();
        }

        async Task calculateMetricsMin()
        {
            var filter = EntityFilterBuilder
                .Create()
                .CreateFilterBlock(x => SetMetricsFilters(x).AddFilter(nameof(ServerMetricsModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId));

            var response = await ServersService.MetricCalculateMinPostRequest(filter);

            if (!response.IsSuccess)
                return;

            string[] titleParts = [
                "Min",
                string.IsNullOrWhiteSpace(filterMetricsName) ? null : $"Name contains {filterMetricsName}",
                filterMetricsFrom == null ? null :  $"from {filterMetricsFrom}",
                filterMetricsTo == null ? null :  $"to {filterMetricsTo}",
                ];


            metricsCalcDisplayTitle = string.Join(" ", titleParts.Where(x => x != null));
            metricsCalcDisplayNumber = response.Data;

            await metricsCalcModalRef.ShowAsync();
        }

        async Task calculateMetricsAvg()
        {
            var filter = EntityFilterBuilder
                .Create()
                .CreateFilterBlock(x => SetMetricsFilters(x).AddFilter(nameof(ServerMetricsModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId));

            var response = await ServersService.MetricCalculateAvgPostRequest(filter);

            if (!response.IsSuccess)
                return;

            string[] titleParts = [
                "Avg",
                string.IsNullOrWhiteSpace(filterMetricsName) ? null : $"Name contains {filterMetricsName}",
                filterMetricsFrom == null ? null :  $"from {filterMetricsFrom}",
                filterMetricsTo == null ? null :  $"to {filterMetricsTo}",
                ];


            metricsCalcDisplayTitle = string.Join(" ", titleParts.Where(x => x != null));
            metricsCalcDisplayNumber = response.Data;

            await metricsCalcModalRef.ShowAsync();
        }

        async Task calculateMetricsMax()
        {
            var filter = EntityFilterBuilder
                .Create()
                .CreateFilterBlock(x => SetMetricsFilters(x).AddFilter(nameof(ServerMetricsModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId));

            var response = await ServersService.MetricCalculateMaxPostRequest(filter);

            if (!response.IsSuccess)
                return;

            string[] titleParts = [
                "Max",
                string.IsNullOrWhiteSpace(filterMetricsName) ? null : $"Name contains {filterMetricsName}",
                filterMetricsFrom == null ? null :  $"from {filterMetricsFrom}",
                filterMetricsTo == null ? null :  $"to {filterMetricsTo}",
                ];


            metricsCalcDisplayTitle = string.Join(" ", titleParts.Where(x => x != null));
            metricsCalcDisplayNumber = response.Data;

            await metricsCalcModalRef.ShowAsync();
        }

        async Task clearMetricItems()
        {
            var filter = EntityFilterBuilder
                .Create()
                .CreateFilterBlock(x => SetMetricsFilters(x).AddFilter(nameof(ServerMetricsModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId));

            var response = await ServersService.MetricClearPostRequest(filter);

            if (!response.IsSuccess)
                return;

            ToastService.Notify(new ToastMessage() { Message = $"Success removed {response.Data}", Type = ToastType.Success, Title = "Clear action", AutoHide = true, IconName = IconName.Info });
        }


        private async Task metricsLoadPrev()
        {
            var newOffset = metricsCount - Metrics.Count - loadMetricsCount;

            var count = (long)loadMetricsCount;

            if (newOffset < 0)
            {
                count += newOffset;
                newOffset = 0;
            }

            var filter = EntityFilterBuilder
                .Create()
                .SetOffset((int)newOffset)
                .SetCount((int)count)
                .CreateFilterBlock(x => SetMetricsFilters(x).AddFilter(nameof(ServerMetricsModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerMetricsModel.CreateTime));

            var response = await ServersService.MetricGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            newMetricsCount = response.Data.Count;

            Metrics = response.Data.Data.Concat(Metrics).ToList();
        }

        private async Task metricsLoadNext()
        {
            var nlc = newMetricsCount;

            var filter = EntityFilterBuilder
                .Create()
                .SetOffset((int)metricsCount)
                .SetCount((int)(nlc - metricsCount))
                .CreateFilterBlock(x => SetMetricsFilters(x).AddFilter(nameof(ServerMetricsModel.ServerId), Database.EntityFramework.Filter.Enums.CompareType.Equals, DetailId))
                .AddOrder(nameof(ServerMetricsModel.CreateTime));

            var response = await ServersService.MetricGetPostRequest(filter);

            if (!response.IsSuccess)
                return;

            metricsCount = nlc;

            newMetricsCount = response.Data.Count;

            Metrics = Metrics.Concat(response.Data.Data).ToList();
        }

        private EntityFilterBlockModel SetMetricsFilters(EntityFilterBlockModel block)
        {
            if (HaveMetricsFilter)
            {
                if (filterMetricsFrom.HasValue)
                    block.AddFilter(nameof(ServerMetricsModel.CreateTime), Database.EntityFramework.Filter.Enums.CompareType.More, filterMetricsFrom.Value.ToUniversalTime());
                if (filterMetricsTo.HasValue)
                    block.AddFilter(nameof(ServerMetricsModel.CreateTime), Database.EntityFramework.Filter.Enums.CompareType.Less, filterMetricsTo.Value.ToUniversalTime());
                if (!string.IsNullOrWhiteSpace(filterMetricsName))
                    block.AddFilter(nameof(ServerMetricsModel.Name), filterMetricsNameType ?? CompareType.ContainsIgnoreCase, filterMetricsName);
            }

            return block;
        }

        private async void metricsNewCountGetting()
        {
            try
            {
                metricsCTS = new CancellationTokenSource();
                var token = metricsCTS.Token;
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(5_000, token);

                    var response = await ServersService.MetricGetCountPostRequest(DetailId);

                    if (response.IsSuccess)
                    {
                        newMetricsCount = response.Data;

                        if (haveNextMetrics)
                        {
                            StateHasChanged();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private CancellationTokenSource? metricsCTS;

        #endregion


        public void Dispose()
        {
            logsCTS?.Cancel();
        }
    }
}
